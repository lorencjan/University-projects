/*  File: reivercrossing.c
 *  Solution: IOS - project 2
 *  Author: Jan Lorenc
 *  Date: 12.4.2019
 *  Faculty: Faculty of information technologies VUT
 *  Desription: Source file for rivercrossing library. 
 *              Contains definitions of all functions in the library.
 */

#include "rivercrossing.h"

/* HANDLING ARGUMENTS */
args_t getArguments(int argc, char *argv[])
{
    args_t args;
    //checks for correct number of arguments
    if(argc != 7)
    {
        fprintf(stderr, "Error: Incorrect number of arguments! "
                        "Please enter exactly 6 arguments.\n");
        goto error;
    }
    //checks for correct digit input
    for(int i=1; i<argc; i++)
    {
        if(!checkNumber(argv[i]))
        {
            fprintf(stderr, "Error: Incorrect format of an argument! "
                            "Please enter only positive integer values.\n");
            goto error;
        }
    }

    //if we've got here, formats are correct and we can convert
    int numOfPeople = (int)strtol(argv[1], NULL, 10);
    int maxHackerGenTime = (int)strtol(argv[2], NULL, 10);
    int maxSerfGenTime = (int)strtol(argv[3], NULL, 10);
    int sailDuration = (int)strtol(argv[4], NULL, 10);
    int pierReturningTime = (int)strtol(argv[5], NULL, 10);
    int pierCapacity = (int)strtol(argv[6], NULL, 10);

    //number of persons must be even and greater than 1
    if(numOfPeople < 2 || numOfPeople % 2 != 0)
    {
        fprintf(stderr, "Error in 1. argument: There must be at least 2 "
                        "people and their number must be even!\n");
        goto error;
    }
    //generation and sail time must be between 0 and 2000
    if( maxHackerGenTime < 0 || maxHackerGenTime > 2000 ||
        maxSerfGenTime   < 0 || maxSerfGenTime   > 2000 ||
        sailDuration     < 0 || sailDuration     > 2000  )
    {
        fprintf(stderr, "Error in 2-4 argument: Person's generation time and sail's "
                        "time cannot be negative or greater than 2000ms!\n");
        goto error;
    }
    //returning to the pier period must be between 20 and 2000
    if(pierReturningTime < 20 || pierReturningTime > 2000)
    {
        fprintf(stderr, "Error in 5. argument: Person cannnot return to the pier in shorter "
                        "time than 20ms but also not after 2000ms has elapse!\n");
        goto error;
    }
    //checks if pier capacity is >= 5
    if((int)strtol(argv[6], NULL, 10) < 5)
    {
        fprintf(stderr, "Error: Pier capacity (last argument) cannot be "
                        "lower then ship capacity, which is %d!\n", SHIP_CAP);
        goto error;
    }

    //if we got here, everything is right, we initialize our struct
    //periods are multiplied by 1000 'cause given time is in miliseconds and usleep takes microseconds
    args.numOfPeople = numOfPeople;
    args.maxHackerGenTime = maxHackerGenTime * 1000;
    args.maxSerfGenTime = maxSerfGenTime * 1000;
    args.sailDuration = sailDuration * 1000;
    args.pierReturningTime = pierReturningTime * 1000;
    args.pierCapacity = pierCapacity;
    return args;

    //if error occures, returns -1 in numOfPeople attribute
    error:
    args.numOfPeople = -1;
    return args;
}
bool checkNumber(char *string)
{
    for(int i=0; string[i] != '\0'; i++)
    {
        if(!isdigit(string[i]))
            return false;
    }
    return true;
}

/* INITIALIZATION */
bool initSemaphores()
{
    //creates and initialize semaphores ... 0666 == rw rights
    //first 5 initialize to 1, all are mutexes
    //semFullShip to 4 (capacity of the ship)
    //semSail, semFinishSail, semCrewAshore need to be unlocked first
    if( (semWrite=sem_open(semWriteNAME, O_CREAT | O_EXCL, 0666, 1)) == SEM_FAILED ||
        (semPier=sem_open(semPierNAME, O_CREAT   | O_EXCL, 0666, 1)) == SEM_FAILED ||
        (semEmbark=sem_open(semEmbarkNAME, O_CREAT | O_EXCL, 0666, 1)) == SEM_FAILED ||
        (semDisembark=sem_open(semDisembarkNAME, O_CREAT | O_EXCL, 0666, 1)) == SEM_FAILED ||
        (semFullShip=sem_open(semFullShipNAME, O_CREAT | O_EXCL, 0666, SHIP_CAP)) == SEM_FAILED ||
        (semSail=sem_open(semSailNAME, O_CREAT | O_EXCL, 0666, 0)) == SEM_FAILED ||
        (semFinishSail=sem_open(semFinishSailNAME, O_CREAT | O_EXCL, 0666, 0)) == SEM_FAILED ||
        (semCrewAshore=sem_open(semCrewAshoreNAME, O_CREAT | O_EXCL, 0666, 0)) == SEM_FAILED)
        {
            fprintf(stderr, "Error: Failed to open all semaphores!\n");
            return false;
        }
    return true;
}
bool initSharedMemory()
{
    //opens all shared memories in read-write mode
    if( (shmLogCounterID=shm_open(shmLogCounterNAME, O_CREAT | O_EXCL | O_RDWR, S_IRUSR | S_IWUSR)) == -1 ||
        (shmHackersWaitingCounterID=shm_open(shmHackersWaitingCounterNAME, O_CREAT | O_EXCL | O_RDWR, S_IRUSR | S_IWUSR)) == -1 ||
        (shmSerfsWaitingCounterID=shm_open(shmSerfsWaitingCounterNAME, O_CREAT | O_EXCL | O_RDWR, S_IRUSR | S_IWUSR)) == -1 ||
        (shmNumOfHackersAboardID=shm_open(shmNumOfHackersAboardNAME, O_CREAT | O_EXCL | O_RDWR, S_IRUSR | S_IWUSR)) == -1 ||
        (shmNumOfSerfsAboardID=shm_open(shmNumOfSerfsAboardNAME, O_CREAT | O_EXCL | O_RDWR, S_IRUSR | S_IWUSR)) == -1 ||
        (shmDisembarkCounterID=shm_open(shmDismbarkCounterNAME, O_CREAT | O_EXCL | O_RDWR, S_IRUSR | S_IWUSR)) == -1 ||
        (shmCaptainExistsID=shm_open(shmCaptainExistsNAME, O_CREAT | O_EXCL | O_RDWR, S_IRUSR | S_IWUSR)) == -1 ||
        (shmWhoAboardID=shm_open(shmWhoAboardNAME, O_CREAT | O_EXCL | O_RDWR, S_IRUSR | S_IWUSR)) == -1 )
        {
            fprintf(stderr, "Error: Failed to open all shared memories!\n");
            return false;
        }
    //opens all shared memories in read-write mode
    if( ftruncate(shmLogCounterID, sizeof(int)) == -1            ||
        ftruncate(shmHackersWaitingCounterID, sizeof(int)) == -1 ||
        ftruncate(shmSerfsWaitingCounterID, sizeof(int)) == -1   ||
        ftruncate(shmNumOfHackersAboardID, sizeof(int)) == -1    ||
        ftruncate(shmNumOfSerfsAboardID, sizeof(int)) == -1      ||
        ftruncate(shmDisembarkCounterID, sizeof(int)) == -1      ||
        ftruncate(shmCaptainExistsID, sizeof(bool)) == -1        ||
        ftruncate(shmWhoAboardID, sizeof(int)) == -1              )
        {
            fprintf(stderr, "Error: Failed to set size to all shared memories!\n");
            return false;
        }
    //maps shared memories to their adresses
    if( (shmLogCounter=(int*)mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED, shmLogCounterID, 0)) == MAP_FAILED ||
        (shmHackersWaitingCounter=(int*)mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED, shmHackersWaitingCounterID, 0)) == MAP_FAILED ||
        (shmSerfsWaitingCounter=(int*)mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED, shmSerfsWaitingCounterID, 0)) == MAP_FAILED ||
        (shmNumOfHackersAboard=(int*)mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED, shmNumOfHackersAboardID, 0)) == MAP_FAILED ||
        (shmNumOfSerfsAboard=(int*)mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED, shmNumOfSerfsAboardID, 0)) == MAP_FAILED ||
        (shmDisembarkCounter=(int*)mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED, shmDisembarkCounterID, 0)) == MAP_FAILED ||
        (shmCaptainExists=(bool*)mmap(NULL, sizeof(bool), PROT_READ | PROT_WRITE, MAP_SHARED, shmCaptainExistsID, 0)) == MAP_FAILED ||
        (shmWhoAboard=(int*)mmap(NULL, sizeof(int), PROT_READ | PROT_WRITE, MAP_SHARED, shmWhoAboardID, 0)) == MAP_FAILED )
        {
            fprintf(stderr, "Error: Failed to map all shared memories to their adresses!\n");
            return false;
        }
    //initialize counters to 0
    *shmHackersWaitingCounter = *shmSerfsWaitingCounter = *shmNumOfSerfsAboard = *shmNumOfHackersAboard = *shmDisembarkCounter = 0;
    *shmCaptainExists = false; //theres no captain at the beginning
    *shmWhoAboard = -1; //checks for HACKER (0) or SERF (1) ...till then whatever except those two    
    return true;
}
bool initSemAndShm()
{
    if(initSemaphores() && initSharedMemory())
        return true;
    return false;
}

/* DESTRUCTION */
bool closeSemaphores()
{
    //closes file descriptors of semaphores
    if( (sem_close(semWrite)     == -1) ||
        (sem_close(semPier)      == -1) ||
        (sem_close(semEmbark)    == -1) ||
        (sem_close(semDisembark) == -1) ||
        (sem_close(semFullShip)  == -1) ||
        (sem_close(semSail)      == -1) ||
        (sem_close(semFinishSail)== -1) ||
        (sem_close(semCrewAshore)== -1)  )
        {
            fprintf(stderr, "Error: Failed to close all semaphores!\n");
            return false;
        }
    //unlinks(destroys) semaphores
    if( (sem_unlink(semWriteNAME)     == -1) ||
        (sem_unlink(semPierNAME)      == -1) ||
        (sem_unlink(semEmbarkNAME)    == -1) ||
        (sem_unlink(semDisembarkNAME) == -1) ||
        (sem_unlink(semFullShipNAME)  == -1) ||
        (sem_unlink(semSailNAME)      == -1) ||
        (sem_unlink(semFinishSailNAME)== -1) ||
        (sem_unlink(semCrewAshoreNAME)== -1)  )
        {
            fprintf(stderr, "Error: Failed to delete all semaphores from memory!\n");
            return false;
        }
    return true;
}
bool closeSharedMemory()
{
    //unmaps all shared memories from their adresses
    if( munmap(shmLogCounter, sizeof(int)) == -1            ||
        munmap(shmHackersWaitingCounter, sizeof(int)) == -1 ||
        munmap(shmSerfsWaitingCounter, sizeof(int)) == -1   ||
        munmap(shmNumOfHackersAboard, sizeof(int)) == -1    ||
        munmap(shmNumOfSerfsAboard, sizeof(int)) == -1      ||
        munmap(shmDisembarkCounter, sizeof(int)) == -1      ||
        munmap(shmCaptainExists, sizeof(bool)) == -1        ||
        munmap(shmWhoAboard, sizeof(int)) == -1              )
        {
            fprintf(stderr, "Error: Failed to unmap all shared memories from their adresses!\n");
            return false;
        }
    //unlinks all shared memories
    if( shm_unlink(shmLogCounterNAME) == -1            ||
        shm_unlink(shmHackersWaitingCounterNAME) == -1 ||
        shm_unlink(shmSerfsWaitingCounterNAME) == -1   ||
        shm_unlink(shmNumOfHackersAboardNAME) == -1    ||
        shm_unlink(shmNumOfSerfsAboardNAME) == -1      ||
        shm_unlink(shmDismbarkCounterNAME) == -1       ||
        shm_unlink(shmCaptainExistsNAME) == -1         ||
        shm_unlink(shmWhoAboardNAME) == -1              )
        {
            fprintf(stderr, "Error: Failed to delete all shared memories from memory!\n");
            return false;
        }
    //closes file descriptors of shared memories
    if( close(shmLogCounterID) == -1            ||
        close(shmHackersWaitingCounterID) == -1 ||
        close(shmSerfsWaitingCounterID) == -1   ||
        close(shmNumOfHackersAboardID) == -1    ||
        close(shmNumOfSerfsAboardID) == -1         ||
        close(shmDisembarkCounterID) == -1      ||
        close(shmCaptainExistsID) == -1         ||
        close(shmWhoAboardID) == -1              )
        {
            fprintf(stderr, "Error: Failed to close all shared memories!\n");
            return false;
        }
    return true;
}
bool closeSemAndShm()
{
    if(closeSemaphores() && closeSharedMemory())
        return true;
    return false;
}
void killProcess(pid_t pid)
{
    closeSemAndShm();
    kill(pid, SIGTERM);
    exit(1);
}

/* LOGIC */
int randomNumber(int max)
{
   srand(time(NULL));
   //in case of wrong arguments, return 0 ... handling dividing zero exc and negative numbers
   return (max > 0) ? rand() % max : 0;
}
bool embark(int type)
{
    //if the ship is full but another group is ready, wait here, otherwise just filling the ship
    sem_wait(semFullShip);
    //controls who/what combination is aboard
    switch(*shmWhoAboard)
    {
        case BOTH: break;
        case HACKER: if(type==SERF)
                        *shmWhoAboard = BOTH; //else it's hacker -> stays the same
                    break;
        case SERF: if(type==HACKER)
                        *shmWhoAboard = BOTH; //else it's serf -> stays the same
                    break;
        default: *shmWhoAboard = (type==SERF) ? SERF : HACKER; //first evaluation
    }
    //if it is a hacker, increment their count
    if(type==HACKER)
        (*shmNumOfHackersAboard)++;
    else
        (*shmNumOfSerfsAboard)++;
    
    //checks for the captainship -> first one will become a captain
    bool IamCaptain = false;
    if(!(*shmCaptainExists))
        IamCaptain = *shmCaptainExists = true;
    //increments number of peole aboard, if full, unlock sail
    if(*shmNumOfHackersAboard + *shmNumOfSerfsAboard == SHIP_CAP)
        sem_post(semSail);
    return IamCaptain;
}
bool createProcess(char *type, int typesNumber, int typeIntVal, args_t *args, FILE *output)
{
    //creating new process
    pid_t child = fork();
    if(child < 0)       //failed to fork itself
        return false;   //handling is in main()
    if(child != 0)      //if not 0, it's parent who has no business here
        return true;    //just return, no error has occured
    
    //writing that the process starts
    sem_wait(semWrite);
    fprintf(output, "%-10d: %s %-10d: starts\n", ++(*shmLogCounter), type, typesNumber);
    sem_post(semWrite);

    //attempt to get on the pier
    //the loop here could be considered as busy waiting, but we're not using it to access shared memory
    //but to to be able to write about process status, put it to sleep and then reattempt
    //(couldn't do it being stuck on a semaphore lock)
    bool success = false;
    while(!success)
    {
        //locks the pier so that values don't change during the condition
        sem_wait(semPier);
        
        //pier cannot be full
        //(here we're going to manipulate with shared values -> critical section X locked by semaphore)
        if(*shmHackersWaitingCounter + *shmSerfsWaitingCounter >= args->pierCapacity)
        {
            //THEN WAIT FOR A RANDOM TIME BEFORE ANOTHER ATTEMPT
            //writes that he leaves
            sem_wait(semWrite);
            fprintf(output, "%-10d: %s %-10d: %-18s: %-6d: %d\n", ++(*shmLogCounter), type,
                    typesNumber, "leaves queue", *shmHackersWaitingCounter, *shmSerfsWaitingCounter);
            sem_post(semWrite);
            //unlock the pier for other processes before going to sleep
            sem_post(semPier);
            //sleep
            usleep(randomNumber(args->pierReturningTime));
            //writes that he's back
            sem_wait(semWrite);
            fprintf(output, "%-10d: %s %-10d: is back\n", ++(*shmLogCounter), type, typesNumber);
            sem_post(semWrite);
        }
        else //it can get on the pier
        {
            if(typeIntVal==SERF)
                (*shmSerfsWaitingCounter)++;
            else
                (*shmHackersWaitingCounter)++;

            sem_wait(semWrite);
            fprintf(output, "%-10d: %s %-10d: %-18s: %-6d: %d\n", ++(*shmLogCounter), type,
                    typesNumber, "waits", *shmHackersWaitingCounter, *shmSerfsWaitingCounter);
            sem_post(semWrite);

            sem_post(semPier);
            success = true;
        }
    }
    //attempt to set sail
    //again a waiting loop, but again not for synchronization, one person alone cannot
    //board the ship, he simply has to wait for others...once they have the correct numbers,
    //4 of them embark (get to the CS) one by one (using a semaphore lock)
    success = false;
    bool IamCaptain = false; // 1 of the 4 is captain, we need to know if it's the current process
    while(!success)
    {
        sem_wait(semEmbark);
        if(*shmHackersWaitingCounter >= SHIP_CAP && typeIntVal==HACKER && //4 hackers are ready and the process is a hacker
           *shmWhoAboard != SERF && *shmWhoAboard != BOTH)                //only hackers or nobody is aboard
        {
            IamCaptain = embark(typeIntVal);
            success = true;
        }    
        else if(*shmSerfsWaitingCounter >= SHIP_CAP && typeIntVal==SERF && //4 serf are ready and the process is a serf
                *shmWhoAboard != HACKER && *shmWhoAboard != BOTH)          //only serfs or nobody is aboard
        {
            IamCaptain = embark(typeIntVal);
            success = true;
        }
        //2 and 2 situation -> need to check the maximum of 2 from each type
        else if((*shmHackersWaitingCounter >= SHIP_CAP/2 && *shmSerfsWaitingCounter >= SHIP_CAP/2) &&
                ((typeIntVal == HACKER && *shmNumOfHackersAboard < 2 && *shmNumOfSerfsAboard   <= 2) ||
                 (typeIntVal == SERF   && *shmNumOfSerfsAboard   < 2 && *shmNumOfHackersAboard <= 2) ))
        {
                IamCaptain = embark(typeIntVal);
                success = true;
        }
        sem_post(semEmbark);
    }
    //captain can free space on the pier, but not before the ship is ready to go
    if(IamCaptain)
    {
        sem_wait(semSail);
        //frees the pier
        *shmHackersWaitingCounter -= *shmNumOfHackersAboard;
        *shmSerfsWaitingCounter -= *shmNumOfSerfsAboard;
        //captain prints that he's boarding ... would be more convenient in embark() function
        //but here we know all are aboard and pier must be freed when printing this
        sem_wait(semWrite);
        fprintf(output, "%-10d: %s %-10d: %-18s: %-6d: %d\n", ++(*shmLogCounter), type,
                typesNumber, "boards", *shmHackersWaitingCounter, *shmSerfsWaitingCounter);
        sem_post(semWrite);

        //than captain will sleep -> simulates sailing
        usleep(randomNumber(args->sailDuration));
        //unlocks finished sail semaphore 3 times -> allows the rest to disembark
        for(int i=0; i<3; i++)
            sem_post(semFinishSail);
        //waits until the rest of the crew leaves the ship
        sem_wait(semCrewAshore);
        //prints he's leaving
        sem_wait(semWrite);
        fprintf(output, "%-10d: %s %-10d: %-18s: %-6d: %d\n", ++(*shmLogCounter), type,
                typesNumber, "captain exits", *shmHackersWaitingCounter, *shmSerfsWaitingCounter);
        sem_post(semWrite);
        //frees the ship
        *shmNumOfHackersAboard = 0;
        *shmNumOfSerfsAboard = 0;
        *shmDisembarkCounter = 0;
        *shmCaptainExists = false;
        *shmWhoAboard = -1;
        for(int i=0; i<SHIP_CAP; i++)
            sem_post(semFullShip);
    }
    else //if not a captain, waits until captain gives the permision to disembark
    {
        sem_wait(semFinishSail);
        //then disembarks
        sem_wait(semDisembark);
        sem_wait(semWrite);
        //prints the message
        fprintf(output, "%-10d: %s %-10d: %-18s: %-6d: %d\n", ++(*shmLogCounter), type,
                typesNumber, "member exits", *shmHackersWaitingCounter, *shmSerfsWaitingCounter);
        //check if it's last to disembark to allow captain disembark as well
        if(++(*shmDisembarkCounter) >= 3)
            sem_post(semCrewAshore);
        sem_post(semWrite);
        sem_post(semDisembark);
    }
    //if we got here, all went well and process can end
    exit(0);
}
