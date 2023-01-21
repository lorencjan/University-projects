/*
 * File: system.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Source file for System class.
 */

#include "system.hpp"

System::System(int numberOfPresses)
{
    Grinder = new Facility("Grinder");
    Presses = new Store("Presses", numberOfPresses);
    AppleCrates = new ProductStack("Apple crates", 4);
    GrindedAppleTubs = new ProductStack("Grinded apple tubs", 2);
    Waste = new ProductStack("Waste", 3);
    BucketsOfJuice = 0;
    CollectionStats = new Stat("Collection durations");
    PressingStats = new Stat("Pressing durations");
}

System::~System()
{
    Output();

    delete Grinder;
    delete Presses;
    delete AppleCrates;
    delete GrindedAppleTubs;
    delete Waste;
    delete CollectionStats;
    delete PressingStats;
}

void System::Output()
{
    cout << "******************** SIMULATION RESULTS ********************" << endl;
    cout << endl;
    cout << "+----------------------------------------------------------+" << endl;
    cout << "| Buckets of juice produced: " << BucketsOfJuice << setw(31 - to_string(BucketsOfJuice).length()) << "|" << endl;
    cout << "+----------------------------------------------------------+" << endl;
    cout << endl;

    Grinder->Output();
    Presses->Output();
    AppleCrates->Output();
    GrindedAppleTubs->Output();
    Waste->Output();
    CollectionStats->Output();
    cout << endl;
    PressingStats->Output();
}
