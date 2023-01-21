/*
 *  File: args.cpp
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Source file for Args class
 */

#include "args.h"

Args::Args() {}

Args::~Args() {}

void Args::Parse(int argc, char *argv[])
{
	//checks for -h argument which prints some basic info about the program usage
    if(argc == 2 && strcmp(argv[1], "-h") == 0)
    {
        cout << "This is a SSL sniffer program. It notifies about any ssl/tls connections that are being made." << endl;
        cout << "Program arguments:"  << endl;
        cout << "-r <file> - Optional argument specifying .pcapng file which should be processed"  << endl;
        cout << "-i <interface> - Optional argument specifying on which network interface the progam should sniff"  << endl;
        cout << "There can be specified only one of these arguments, at least one is required, though" << endl;
        exit(0);
    }
    //check the correct number of arguments
    if(argc == 1 || (argc == 2 && strcmp(argv[1], "-h") != 0) || argc > 3)
    {
        cerr << "Invalid number of program arguments. Program requires 2 arguments." << endl;
        cerr << "-r <file> or -i <interface>" << endl;
        cerr << "See ./sslsniff -h for more info." << endl;
        exit(1);
    }
    //check the valid format
    if(strcmp(argv[1], "-r") != 0 && strcmp(argv[1], "-i") != 0)
    {
        char* arg = strcmp(argv[1], "-h") == 0 ? argv[2] : argv[1];
        cerr << "Unknown argument " << arg << ". Specify \"-r <file>\" or \"-i <interface>\"." << endl;
        cerr << "See ./sslsniff -h for more info." << endl;
        exit(1);
    }
    
    Type = argv[1][1];
    Source = (string)argv[2];
}