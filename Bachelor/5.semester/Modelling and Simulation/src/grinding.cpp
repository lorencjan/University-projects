/*
 * File: grinding.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Source file for Grinding process class.
 */

#include "grinding.hpp"

Grinding::Grinding(System *system) 
{
    this->system = system;
}

Grinding::~Grinding() {}

void Grinding::Behavior()
{
    Seize(*system->Grinder);
    Wait(GrindingDuration);
    system->GrindedAppleTubs->Add(GrindedAppleTubsProduced);
    Release(*system->Grinder);
    NextProcess(system);
}
