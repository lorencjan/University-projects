/*
 *  File: sniffer.h
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Header file for Sniffer class, which does most of the work in the packet processing
 */

#ifndef SNIFFER_H
#define SNIFFER_H

#include <iostream>
#include <string>
#include <string.h>
#include <vector>
#include <time.h>
#include <sys/time.h>
#include <pcap.h>
#define __FAVOR_BSD
#include <netinet/ip.h>
#include <netinet/tcp.h>
#include <arpa/inet.h>
#include <netinet/if_ether.h> 
#include "error.h"
#include "connection.h"

using namespace std;

/**
 * @brief Main class of the whole program. Manages the whole sniffing.
 */
class Sniffer
{
    private:
        /**
         * @brief Type of sniffing ... r/i (file/interface)
         */
        char Type;

        /**
         * @brief Name of the file/interface
         */
        string Source;

        /**
         * @brief Keeps track of currently established connections
         */
        vector<Connection> Connections;

        /**
         * @brief Size of ethernet packet header
         */
        const u_int EthHeaderSize = 14;
        /**
         * @brief Offset to the header length info in tcp header
         */
        const u_int TcpHeaderLengthOffset = 12;
        /**
         * @brief Size of ssl packet header
         */
        const u_int SslHeaderSize = 5;

        /**
         * @brief Processes packets in the given .pcapng file
         */
        void SniffFromFile();
        /**
         * @brief Listens on specified interface and sniffs for ssl packets
         */
        void SniffFromInterface();

        /**
         * @brief Sniffs from given pcap structure no matter if it's live interface listening or a file
         * @param pcap Pcap struct from which to sniff
         */
        void SniffFromPcap(pcap_t* pcap);

        /**
         * @brief Checks the existence and correct file type of the specified file, exits program if the validation fails
         */
        void CheckFileExistence();

        /**
         * @brief Finds the specified interface among available devices, outputs available interfaces and exists program if not found
         */
        void CheckInterfaceExistence();

        /**
         * @brief Finds connection based on ip addresses and source port
         * @param clientIp Ip address of the client 
         * @param serverIp Ip address of the server 
         * @param clientPort Port number of the client
         * @return Pointer to the matching collection or null if not found
         */
        Connection* FindConnection(string clientIp, string serverIp, int clientPort);

        /**
         * @brief Cretes new connection, if it doesn't exist yet
         * @param clientIp Ip address of the client 
         * @param serverIp Ip address of the server 
         * @param clientPort Port number of the client
         * @param start Timestamp of the first packet of the connection
         * @param conn Pointer to existing connection with above attribtues ... should be nullptr as it shouldn't exist, but we'll check anyway
         */
        void CreateConnection(string clientIp, string serverIp, int clientPort, struct timeval start, Connection* conn);

        /**
         * @brief Removes connection from the connection vector
         * @param conn Pointer to the connection to be deleted
         */
        void RemoveConnection(Connection *conn);

        /**
         * @brief Goes through Client Hello ssl packet and searches for the SNI value
         * @param ssl Start of the ssl/tls part of the currently processed packet
         * @return Value of SNI or empty string if not found
         */
        string FindSNI(const u_char* ssl);

        /**
         * @brief Goes through current packet, finds all ssl/tls packets (can be multiple e.g. handshake + cipher)
         * @param packet Current packet as byte array
         * @param packetLength Length of the captured packet
         * @param sslHeaderOffset Offset to the ssl header in the packet
         * @param overflowOffset Overflow offset of the current conenction
         * @return Amount of ssl encrypted bytes (the "bytes" the task requires)
         */
        int ProcessSSL(const u_char* packet, u_int packetLength, u_int sslHeaderOffset, u_int& overflowOffset);

    public:
        Sniffer(char type, string source);
        ~Sniffer();

        /**
         * @brief Processes packets from Sniffer's source (file/interface) and finds ssl/tls connections
         */
        void Sniff();
};

/**
 * @brief Helper struct for better ssl header orientation
 */
typedef struct sslHeader{
    u_char type;
    u_char version[2];
    u_char length[2];
} sslHeader_t;

#endif