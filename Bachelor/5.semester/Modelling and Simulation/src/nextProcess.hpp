/*
 * File: nextProcess.hpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Header file for nextProcess function.
 */

#ifndef NEXT_PROCESS_H
#define NEXT_PROCESS_H

#include <simlib.h>
#include "appleCollecting.hpp"
#include "grinding.hpp"
#include "pressing.hpp"
#include "system.hpp"
        
/**
 * @brief Decides for a person in the system what should he do next.
 */
void NextProcess(System *system);

#endif
