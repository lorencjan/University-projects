/*
 * File: nextProcess.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Source file for nextProcess function.
 */

#include "nextProcess.hpp"

void NextProcess(System *system)
{   
    // press has highest priority
    if(system->Presses->Free() && system->GrindedAppleTubs->Consume())
    {
        (new Pressing(system))->Activate();
    }
    // otherwise grind if possible
    else if(!system->Grinder->Busy() && system->AppleCrates->Consume())
    {
        (new Grinding(system))->Activate();
    }
    // go collecting apples if there's nothing else to do
    else
    {
        (new AppleCollecting(system))->Activate();
    }
}
