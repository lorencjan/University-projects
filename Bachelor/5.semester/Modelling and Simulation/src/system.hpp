/*
 * File: system.hpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Header file for System class.
 */

#ifndef SYSTEM_H
#define SYSTEM_H

#include <iostream>
#include <simlib.h>
#include "productStack.hpp"

using namespace std;

/**
 * @brief The class wraps all segments of the system.
 */
class System
{
    public:
        /**
         * @brief Apple grinder.
         */
        Facility* Grinder;

        /**
         * @brief Apple presses.
         */
        Store* Presses;

        /**
         * @brief Apple crates storage.
         */
        ProductStack* AppleCrates;
        
        /**
         * @brief Apple crates storage.
         */
        ProductStack* GrindedAppleTubs;

        /**
         * @brief Apple remains to be thrown out after pressing process.
         */
        ProductStack* Waste;

        /**
         * @brief Number of buckets of produces apple juice.
         */
        int BucketsOfJuice;

        /**
         * @brief Keeps track of apple collection durations.
         */
        Stat* CollectionStats;
        /**
         * @brief Keeps track of apple pressing durations.
         */
        Stat* PressingStats;

        System(int numberOfPresses);
        ~System();

        /**
         * @brief Writes out the simulation statistice.
         */
        void Output();
};

#endif
