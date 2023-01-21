/*
 * File: pressing.hpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Header file for Pressing process class.
 */

#ifndef PRESSING_H
#define PRESSING_H

#include <simlib.h>
#include "system.hpp"
#include "nextProcess.hpp"
#include "generators.hpp"

/**
 * @brief Class for the apple pressing process.
 */
class Pressing : public Process
{
    private:
        /**
         * @brief Minimum duration of pressing process.
         */
        const int MinPressingDuration = 25;
        /**
         * @brief Maximum duration of pressing process.
         */
        const int MaxPressingDuration = 35;
        /**
         * @brief How many buckets of juice does the press produce.
         */
        const int BucketsOfJuiceProduced = 1;
        /**
         * @brief How many buckets of fruit trash does the press produce.
         */
        const int BucketsOfTrashProduced = 1;
        /**
         * @brief How much time does it take to take out waste.
         */
        const int TakeOutWasteDuration = 5;

        /**
         * @brief Accessor to the system segments.
         */
        System* system;

    public:

        Pressing(System* system);
        ~Pressing();

        void Behavior() override;
};

#endif
