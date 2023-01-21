/*
 * File: args.hpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Header file for Args class.
 */

#ifndef ARGS_H
#define ARGS_H

#include <iostream>
#include <string>

using namespace std;

/**
 * @brief Helper class for parsing input program arguments.
 */
class Args
{
    public:
        /**
         * @brief Number of people collecting/processing apples.
         */
        int People;
        /**
         * @brief Number of presses in the system
         */
        int Presses;

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
