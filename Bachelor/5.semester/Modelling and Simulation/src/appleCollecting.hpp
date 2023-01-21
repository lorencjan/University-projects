/*
 * File: appleCollecting.hpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Header file for AppleCollecting process class.
 */

#ifndef APPLE_COLLECTING_H
#define APPLE_COLLECTING_H

#include <simlib.h>
#include "system.hpp"
#include "nextProcess.hpp"
#include "generators.hpp"

/**
 * @brief Class for the apple collection process.
 */
class AppleCollecting : public Process
{
    private:
        /**
         * @brief How long does it take to collect an apple crate.
         */
        const int CollectionDuration = 15;
        /**
         * @brief Apple crates filled in one run.
         */
        const int AppleCratesFilled = 1;

        /**
         * @brief Accessor to the system segments.
         */
        System* system;

    public:

        AppleCollecting(System *system);
        ~AppleCollecting();

        void Behavior() override;
};

#endif
