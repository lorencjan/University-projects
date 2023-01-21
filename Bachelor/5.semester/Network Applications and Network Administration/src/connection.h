/*
 *  File: connection.h
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Header file for Connection class
 */

#ifndef CONNECTION_H
#define CONNECTION_H

#include <iostream>
#include <iomanip>
#include <string>
#include <sys/time.h>
#include <time.h>

using namespace std;

/**
 * @brief Set of connection states
 */
enum ExpectedPacket
{ 
    SYN_ACK,
    ACK_CLIENT_HELLO,
    CLIENT_HELLO,
    ACK_SERVER_HELLO,
    SERVER_HELLO,
    WAITING_FOR_FIN,
    FIN_ACK,
    END_CONNECTION
};

/**
 * @brief Represents currently open connection with all values required from the output format
 */
class Connection
{
    public:
        /**
         * @brief Ip address of the client
         */
        string ClientIp = "";
        /**
         * @brief Port of the client
         */
        int ClientPort = 0;
        /**
         * @brief Ip address of the server
         */
        string ServerIp = "";
        /**
         * @brief Server name indication of the ssl connection
         */
        string SNI = "";
        /**
         * @brief Amount of ssl encrypted bytes transfered by the connection.
         *        Basically the sum of "length" fields from ssl headers
         */
        long Bytes = 0;
        /**
         * @brief Amount of packets of the connection from the first SYN to the ACK of second FIN
         */
        int Packets = 0;
        /**
         * @brief Timestamp of the first packet of the connection (tcp SYN)
         */
        struct timeval Start;
        /**
         * @brief Timestamp of the last packet of the connection (tcp ACK for second FIN)
         */
        struct timeval End;

        /**
         * @brief Defines the current state of the connection by specifying, what packet should come next
         */ 
        ExpectedPacket NextExpected;
        /**
         * @brief In case a packet's payload is bigger then the packet and the rest is sent afterwards,
         *        I need to know length of it, so that in search of the next SSL packet I could skip this pending payload
         */
        u_int OverflowOffset = 0;

        Connection();
        ~Connection();

        /**
         * @brief Prints the connection in the required output format
         */
        void Print();
};

#endif