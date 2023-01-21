/*
 * File: pressing.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Source file for Pressing process class.
 */

#include "pressing.hpp"

Pressing::Pressing(System* system)
{
    this->system = system;
}

Pressing::~Pressing() {}

void Pressing::Behavior()
{
    Enter(*system->Presses);
    auto pressingDuration = UniformGenerator(MinPressingDuration, MaxPressingDuration);
    Wait(pressingDuration);
    (*system->PressingStats)(pressingDuration);
    system->BucketsOfJuice += BucketsOfJuiceProduced;
    system->Waste->Add(BucketsOfTrashProduced);
    Leave(*system->Presses);
    
    while(system->Waste->Consume())
    {
        Wait(TakeOutWasteDuration);
    }
    
    NextProcess(system);
}
