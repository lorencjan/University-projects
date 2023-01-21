/*
 *  File: args.h
 *  Solution: ISA - project - Monitoring of SSL connections
 *  Date: 17.10.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Header file for Args class
 */

#ifndef ARGS_H
#define ARGS_H

#include <iostream>
#include <string.h>
#include <string>

using namespace std;

/**
 * @brief Helper class for parsing input program arguments.
 */
class Args
{
    public:
        /**
         * @brief Type of sniffing ... r/i (from file/interface).
         */
        char Type;
        /**
         * @brief Name of the file/interface
         */
        string Source;

        Args();
        ~Args();

        /**
         * @brief Validates program arguments and assigns them to the class properties
         * @param argc Number of arguments
         * @param argv Array of arguments
         */
        void Parse(int argc, char *argv[]);
};

#endif