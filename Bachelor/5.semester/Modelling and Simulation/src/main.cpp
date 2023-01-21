/*
 * File: main.cpp
 * Solution: IMS - project
 *			 Implementation of discrete simulator with SHO support
 * Date: 14.11.2020
 * Author: Jan Lorenc & Vojtěch Staněk
 * Faculty: Faculty of information technologies VUT
 * Desription: Main source file for the simulator.
 */

#include <simlib.h>
#include "args.hpp"
#include "system.hpp"
#include "appleCollecting.hpp"

#define START_TIME 0
#define END_TIME 6 * 60 // let's say 6h of making apple juice

int main(int argc, char *argv[])
{
    auto args = Args();
    args.Parse(argc, argv);

    auto system = new System(args.Presses);
    
    Init(START_TIME, END_TIME);
    //there's no generator, put all people there at once
    for(int i = 0; i < args.People; i++)
    {
        (new AppleCollecting(system))->Activate();
    }

    Run();

    delete system;
    return 0;
}
