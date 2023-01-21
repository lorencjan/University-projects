/*
 * File: grinding.hpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Header file for Grinding process class.
 */

#ifndef GRINDING_H
#define GRINDING_H

#include <simlib.h>
#include "system.hpp"
#include "nextProcess.hpp"

/**
 * @brief Class for the apple grinding process.
 */
class Grinding : public Process
{
    private:
        /**
         * @brief How long does it take to grind apples.
         */
        const int GrindingDuration = 3;
        /**
         * @brief How many tubs of grinded apples does the grinder produce.
         */
        const int GrindedAppleTubsProduced = 2;

        /**
         * @brief Accessor to the system segments.
         */
        System* system;
    
    public:
        Grinding(System* system);
        ~Grinding();

        void Behavior() override;
};

#endif
