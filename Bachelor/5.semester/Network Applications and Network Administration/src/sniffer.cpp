/*
 *  File: sniffer.cpp
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Source file for Sniffer class, which does most of the work in packet processing
 */

#include "sniffer.h"

Sniffer::Sniffer(char type, string source)
{
    Type = type;
    Source = source;
}

Sniffer::~Sniffer() {}

void Sniffer::SniffFromFile()
{
    CheckFileExistence();

    char errbuff[PCAP_ERRBUF_SIZE];
    pcap_t * pcap = pcap_open_offline(Source.c_str(), errbuff);

    SniffFromPcap(pcap);
}

void Sniffer::SniffFromInterface()
{
    CheckInterfaceExistence();

    char errbuff[PCAP_ERRBUF_SIZE];
    pcap_t * pcap = pcap_open_live(Source.c_str(), 65536, 1, 1000, errbuff);

    SniffFromPcap(pcap);
}

void Sniffer::SniffFromPcap(pcap_t* pcap)
{
    if(pcap == NULL)
        Error("Failed to open pcap.");
    const u_char *packet;
    struct pcap_pkthdr header;
    while((packet = pcap_next(pcap, &header)) != NULL)
    {
        struct ether_header* eth_header = (struct ether_header *) packet;
        
        if(ntohs(eth_header->ether_type) != ETHERTYPE_IP) // ignore if it's not ip packet
            continue;

        struct ip * ip = (struct ip *)(packet + EthHeaderSize);
        if(ip->ip_p != 6) // ignore if it's not tcp packet
            continue;

        u_int ipHeaderSize = ip->ip_hl * sizeof(int32_t);
        string srcIp = inet_ntoa(ip->ip_src);
        string destIp = inet_ntoa(ip->ip_dst);

        struct tcphdr *tcp = (struct tcphdr *) (packet + EthHeaderSize + ipHeaderSize);
        u_int tcpHeaderSize = (((u_char)*(packet + EthHeaderSize + ipHeaderSize + TcpHeaderLengthOffset)) >> 4) * 4;
        int srcPort = ntohs(tcp->th_sport);
        int destPort = ntohs(tcp->th_dport);

        Connection* conn = FindConnection(srcIp, destIp, srcPort);
        if(conn == nullptr)                                 // I dont't know yet from which direction the packet came ... connections are
            conn = FindConnection(destIp, srcIp, destPort); // client-server, so if the packet is server-client, might need to look again

        if (tcp->th_flags == TH_SYN) // sole SYN means new connection
        {
            CreateConnection(srcIp, destIp, srcPort, header.ts, conn);
            continue;
        }

        if(conn == nullptr) //if it is a packet for which we didn't capture connection init (handshake), ignore
            continue;

        //if the packet is not what it should be, connection is removed as not ssl/tls connection
        conn->Packets++;
        int sslBytes = 0;
        u_int sslOffset = EthHeaderSize + ipHeaderSize + tcpHeaderSize;
        switch(conn->NextExpected)
        {
            case SYN_ACK:   //right after the connection SYN should be SYN_ACK from the server
                if(tcp->th_flags & TH_SYN && tcp->th_flags & TH_ACK && srcIp == conn->ServerIp)
                    conn->NextExpected = ACK_CLIENT_HELLO;
                else
                    RemoveConnection(conn);
                break;
            case ACK_CLIENT_HELLO:  // ACKed server's SYN after server's SYN_ACK
                if(tcp->th_flags == TH_ACK && srcIp == conn->ClientIp)
                    conn->NextExpected = CLIENT_HELLO;
                else
                    RemoveConnection(conn);
                break;
            case CLIENT_HELLO:  // the packet must include ssl/tls data and if so, we need to extract SNI
                conn->Bytes = ProcessSSL(packet, header.len, sslOffset, conn->OverflowOffset);
                if(srcIp == conn->ClientIp && conn->Bytes > 0)
                {
                    conn->SNI = FindSNI(packet + sslOffset);
                    conn->NextExpected = ACK_SERVER_HELLO;
                }
                else
                {
                    RemoveConnection(conn);
                }
                break;
            case ACK_SERVER_HELLO:  // ACKed SYN after server SYN_ACK
                if(tcp->th_flags == TH_ACK && srcIp == conn->ServerIp)
                    conn->NextExpected = SERVER_HELLO;
                else
                    RemoveConnection(conn);
                break;
            case SERVER_HELLO:      // ACKed SYN after server SYN_ACK
                sslBytes = ProcessSSL(packet, header.len, sslOffset, conn->OverflowOffset);
                if(srcIp == conn->ServerIp && sslBytes > 0)
                {
                    conn->Bytes += sslBytes;
                    conn->NextExpected = WAITING_FOR_FIN;
                }
                else
                {
                    RemoveConnection(conn);
                }
                break;
            case WAITING_FOR_FIN:  // connection is established - capturing normal communication until FIN is received
                if (tcp->th_flags & TH_FIN)
                    conn->NextExpected = FIN_ACK;
                //process only if it's not ACK packet == no data, check only by flag is not enough as some reessembled packets can have just ACK but data as well
                else if (header.len != sslOffset)
                    conn->Bytes += ProcessSSL(packet, header.len, sslOffset, conn->OverflowOffset);
                break;
            case FIN_ACK:  //if FIN is received, we expect end of the connection
                if (tcp->th_flags & TH_FIN && tcp->th_flags & TH_ACK)
                    conn->NextExpected = END_CONNECTION;
                else if (header.len != sslOffset)  //other side can still send some data back before her FIN
                    conn->Bytes += ProcessSSL(packet, header.len, sslOffset, conn->OverflowOffset);
                break;
            case END_CONNECTION:  //all we need now is the ACK for the second FIN
                if (tcp->th_flags & TH_ACK)
                {                       
                    conn->End = header.ts;
                    conn->Print();
                    RemoveConnection(conn);
                }
                break;
        }
    }
}

void Sniffer::CheckFileExistence()
{
    //does it exist?
    FILE *f;
    if((f = fopen(Source.c_str(), "r")))
        fclose(f);
    else
        Error("File " + Source +" doesn't exist.\n");

    //does it have correct format?
    if(Source.find(".pcapng") == string::npos)
        Error("File must be of type .pcapng\n");
}

void Sniffer::CheckInterfaceExistence()
{
    pcap_if_t *alldevs, *dev;
    char errbuf[PCAP_ERRBUF_SIZE];

    //get available devices
    if (pcap_findalldevs(&alldevs, errbuf))
        Error("Failed to open input devices.\n");

    //check if specified interface exists
    bool found = false;
    for (dev = alldevs; dev != NULL; dev = dev->next)
    {
        if(strcmp(Source.c_str(), dev->name) == 0)
            found = true;
    }
    if(!found)
    {
        cerr << "Interface " << Source << " doesn't exist." << endl;
        cerr << "Available iterfaces:" << endl;
        for (dev = alldevs; dev != NULL; dev = dev->next)
            cerr << dev->name << endl;
            
        exit(1);
    }

    pcap_freealldevs(alldevs);
}

Connection* Sniffer::FindConnection(string clientIp, string serverIp, int clientPort)
{
    for(Connection& c : Connections)
    {
        if(c.ClientIp == clientIp && c.ServerIp == serverIp && c.ClientPort == clientPort)
            return &c;
    }
    return nullptr;
}

void Sniffer::CreateConnection(string clientIp, string serverIp, int clientPort, struct timeval start, Connection* conn)
{
    if(conn != nullptr) //new connection shouldn't be possible to create (from the same PC to the same server
        return;         //it goes from different port), however if an anomaly occures, I ignore it

    Connection c = Connection();
    c.ClientIp = clientIp;
    c.ClientPort = clientPort;
    c.ServerIp = serverIp;
    c.Bytes = 0;
    c.Packets = 1;
    c.Start = start;
    c.NextExpected = SYN_ACK;

    Connections.push_back(c);
}

void Sniffer::RemoveConnection(Connection *conn)
{
    int removeAt = -1;
    for (size_t i = 0; i < Connections.size(); ++i)
    {
        if(Connections[i].ClientIp == conn->ClientIp &&
           Connections[i].ServerIp == conn->ServerIp &&
           Connections[i].ClientPort == conn->ClientPort)
        {
            removeAt = i;
            break;
        }
    }
    if(removeAt != -1)
    {
        Connections.erase(Connections.begin() + removeAt);
    }
}

string Sniffer::FindSNI(const u_char* ssl)
{
    int offsetToSNI = 5; //ssl header length
    offsetToSNI += 6;    //handshake header
    offsetToSNI += 32;   //random
    //session id
    int sessionIdLength = (int)ssl[offsetToSNI];
    offsetToSNI += sessionIdLength + 1;    //+1 for the length storage
    //cipher suites
    int cipherSuitesLength = (int)ssl[offsetToSNI + 1] | (int)ssl[offsetToSNI] << 8 ; //2B value
    offsetToSNI += cipherSuitesLength + 2;
    //compression methods
    int compresionMethodsLength = ssl[offsetToSNI];
    offsetToSNI += compresionMethodsLength + 1;
    
    //extensions
    int extensionsLength = (int)ssl[offsetToSNI + 1] | (int)ssl[offsetToSNI] << 8;
    offsetToSNI += 2; //2B for extensions length storage
    
    int end = offsetToSNI + extensionsLength;
    while(offsetToSNI < end)
    {
        int extensionType = (int)ssl[offsetToSNI + 1] | (int)ssl[offsetToSNI] << 8;
        int extensionLength = (int)ssl[offsetToSNI + 3] | (int)ssl[offsetToSNI + 2] << 8;
        offsetToSNI += 4; //2B for type and 2B for length storage
        
        if(extensionType == 0) //is it server name? (server name type has value 00 00)
        {
            int serverNameLength = (int)ssl[offsetToSNI + 4] | (int)ssl[offsetToSNI + 3] << 8;
            offsetToSNI += 5; //2B list length, 1B type, 2B length
            string SNI = "";
            for(int i = 0; i < serverNameLength; i++)
            {
                SNI += (u_char)ssl[offsetToSNI + i];
            }
            return SNI;
        }
        offsetToSNI += extensionLength;
    }
    return "";
}

int Sniffer::ProcessSSL(const u_char *packet, u_int packetLength, u_int sslHeaderOffset, uint& overflowOffset)
{
    sslHeader_t* ssl = NULL;
    u_int bytes = 0, sslSize = 0;
    u_int tmpOverflowOffset = overflowOffset; 
    bool isSSL = false;
    while(true)
    {
        u_int offset = overflowOffset + sslHeaderOffset + sslSize;

        ssl = (sslHeader_t*)(packet + offset);
        if(ssl->type < 20 || ssl->type > 23) // 20 - change_cipher_spec, 21 - alert, 22 -handshake, 23 - application_data
            break;                           // it is not ssl/tls

        u_int version = ssl->version[1] | ssl->version[0] << 8;
        if(version < 768 || version > 771)  // 0x0300 == 768 - SSL 3.0, 0x03x01 == 769 - TLS 1.0, 0x03x02 == 770 - TLS 1.1, 0x0303 == 771 - TLS 1.2
            break;                          // it is not ssl/tls

        isSSL = true;

        //need to check the packet size ... 'cause pointer doesn't mind, it would point outside with pleasure
        //and if it's smaller packet than the previous one, it could read it's value again
        int remainingPacketBytes = packetLength - (offset+SslHeaderSize);
        if(remainingPacketBytes < 0)
            break;

        //add it's size to the bytes amount (according to the task, only data, not header should go there)
        u_int length = ssl->length[1] | ssl->length[0] << 8;
        bytes += length;

        //check if the size of the current packet is enough for the payload or if we should expect the rest in next packet
        int packetOverflow = length - remainingPacketBytes;
        tmpOverflowOffset = packetOverflow > 0 ? packetOverflow : 0;

        sslSize += SslHeaderSize + length;
    }

    if(isSSL)
    {
        overflowOffset = tmpOverflowOffset;
    }
    else //a pending overflowed ssl wouldn't be recognized as ssl due to different offsets
    {
        int packetData = packetLength - sslHeaderOffset;
        int reducedOverflow = overflowOffset - packetData;
        overflowOffset = reducedOverflow < 0 ? 0 : reducedOverflow; 
    }
    
    return bytes;
}

void Sniffer::Sniff()
{
    if(Type == 'i')
        SniffFromInterface();
    else
        SniffFromFile();
}