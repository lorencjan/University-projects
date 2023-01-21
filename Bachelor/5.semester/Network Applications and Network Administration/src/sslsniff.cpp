/*
 *  File: sslsniff.cpp
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Main source file for the ssl connections sniffer
 */

#include "sniffer.h"
#include "args.h"

int main(int argc, char *argv[])
{
    Args args = Args();
    args.Parse(argc, argv);

    Sniffer sniffer = Sniffer(args.Type, args.Source);
    sniffer.Sniff();
    
    return 0;
}