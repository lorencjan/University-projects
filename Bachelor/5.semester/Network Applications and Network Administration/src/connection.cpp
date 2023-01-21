/*
 *  File: connection.cpp
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Source file for Connection class
 */

#include "connection.h"

Connection::Connection()
{
    NextExpected = ExpectedPacket::SYN_ACK;
}

Connection::~Connection(){}

void Connection::Print()
{
    //getting current time with milliseconds
    struct timeval tv;
    gettimeofday(&tv, NULL);
    time_t t = (time_t)tv.tv_sec;
    auto tm = localtime(&t);
    char buff[20];
    strftime (buff,20,"%F %H:%M:%S",tm);

    //getting durations
    int durationSec = End.tv_sec - Start.tv_sec;
    int durationMicroseconds = End.tv_usec - Start.tv_usec;
    if(durationMicroseconds < 0)
    {
        durationSec -=1;
        durationMicroseconds += 1000000;
    }

    //print
    printf("%s.%06ld,", buff, tv.tv_usec);
    cout << ClientIp << ",";
    cout << ClientPort << ",";
    cout << ServerIp << ",";
    cout << SNI << ",";
    cout << Bytes << ",";
    cout << Packets << ",";
    cout << durationSec << "." << setfill('0') << setw(3) << durationMicroseconds/1000 << endl;
}