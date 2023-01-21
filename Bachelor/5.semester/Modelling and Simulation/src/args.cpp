/*
 * File: args.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Source file for Args class.
 */

#include "args.hpp"

Args::Args()
{
    People = 5;
    Presses = 1;
}

Args::~Args() {}

void Args::Parse(int argc, char *argv[])
{
    if(argc == 1)
        return;

    //checks for -h argument which prints some basic info about the program usage
    if(argc == 2 && string(argv[1]).compare("-h") == 0)
    {
        cout << "This is a program simulating apple collection and transformation to juice." << endl;
        cout << "Program arguments:"  << endl;
        cout << "--people=<number> [optional] - Number of people in the simulation (defaults to 5)" << endl;
        cout << "--presses=<number> [optional] - Number of presses in the system (defaults to 1)"  << endl;
        cout << "Sample usage: ./simulator --people=10 --presses=2" << endl;
        exit(0);
    }
    //check the correct number of arguments
    if(argc > 3)
    {
        cerr << "Invalid number of program arguments. Program can have up to 2 arguments." << endl;
        cerr << "See ./simulator -h for more info." << endl;
        exit(1);
    }
    //check the valid format
    for(int i = 1; i < argc; i++)
    {
        if(string(argv[i]).find("--people=") == 0)
        {
            int people = atoi((char*)(argv[i] + 9));
            if(people == 0)
            {
                cerr << "Invalid number of people. Please specify a valid integer number." << endl;
                exit(1);
            }
            People = people;
        }
        else if(string(argv[i]).find("--presses=") == 0)
        {
            int presses = atoi((char*)(argv[i] + 10));
            if(presses == 0)
            {
                cerr << "Invalid number of presses. Please specify a valid integer number." << endl;
                exit(1);
            }
            Presses = presses;
        }
        else
        {
            cerr << "Invalid program arguments." << endl;
            cerr << "See ./simulator -h for more info." << endl;
            exit(1);
        }
    }
}
