/*
 * File: appleCollecting.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Source file for AppleCollecting process class.
 */

#include "appleCollecting.hpp"

AppleCollecting::AppleCollecting(System *system)
{
    this->system = system;
}

AppleCollecting::~AppleCollecting() {}

void AppleCollecting::Behavior()
{
    auto collectionDuration = ExponentialGenerator(CollectionDuration);
    Wait(collectionDuration);
    (*system->CollectionStats)(collectionDuration);
    system->AppleCrates->Add(AppleCratesFilled);
    NextProcess(system);
}
