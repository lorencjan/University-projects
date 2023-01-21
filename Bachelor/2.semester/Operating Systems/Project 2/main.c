/*  File: main.c
 *  Solution: IOS - project 2
 *  Author: Jan Lorenc
 *  Date: 12.4.2019
 *  Faculty: Faculty of information technologies VUT
 *  Desription: Contains main() function with the logic of the project.
 *              Uses rivercrossing.h library for creation of hackers
 *              and serfs and conducting the algorithm. Ends with termination
 *              signal or when last hacker/serf is back ashore.
 */

#include "rivercrossing.h"

int main(int argc, char* argv[])
{
    //gets the arguments
    args_t args = getArguments(argc, argv);
    if(args.numOfPeople == -1)
        return 1;

    //opens log for writing actions
    output = fopen("proj2.out", "w");
    if(!output)
    {
        fprintf(stderr, "Error: Failed to open output log file!\n");
        return 1;
    }
    setbuf(output, NULL);

    //initializes semaphores and shared memory
    if(!initSemAndShm())
    {
        closeSemAndShm(); //some could have been created - destroys those
        goto error;
    }
    //expecting interrupts -> free resources
    signal(SIGTERM, killProcess);
    signal(SIGINT, killProcess);

    //creates helper processes
    pid_t hacker = 0, serf = 0;
    //hacker
    hacker = fork();
    if(hacker == 0) //child
    {
        signal(SIGTERM, killProcess);
        //creates as many hackers as given in the arguments
        for(int i=1; i <= args.numOfPeople; i++)
        {
            //controls failure
            if(!createProcess("HACK", i, HACKER, &args, output))
            {
                fprintf(stderr, "Error: Failed to create new hacker!\n");
                killProcess(serf);
                goto error;
            }
            //generating in a random interval
            usleep(randomNumber(args.maxHackerGenTime));
        }
        //waits for all its children
        for(int i=1; i <= args.numOfPeople; i++)
            wait(NULL);
        exit(0);
    }
    else if(hacker < 0) //failed to fork itself
    {
        fprintf(stderr, "Error: Failed to create helper process for hackers!\n");
        killProcess(serf);
        goto error;
    }
    //else parent continues
    //serf
    serf = fork();
    if(serf == 0)
    {
        signal(SIGTERM, killProcess);
        //creates as many serfs as given in the arguments
        //indexes must be different than for hackers
        for(int i=1; i <= args.numOfPeople; i++)
        {
            //controls failure
            if(!createProcess("SERF", i, SERF, &args, output))
            {
                fprintf(stderr, "Error: Failed to create new serf!\n");
                killProcess(hacker);
                goto error;
            }
            //generating in a random interval
            usleep(randomNumber(args.maxSerfGenTime));
        }
        //waits for all its children
        for(int i=0; i<args.numOfPeople; i++)
            wait(NULL);
        exit(0);
    }
    else if(serf < 0) //failed to fork itself
    {
        fprintf(stderr, "Error: Failed to create helper process for serfs!\n");
        killProcess(hacker);
        goto error;
    }
    //else parent continues

    //wait for children
    waitpid(hacker, NULL, 0);
    waitpid(serf, NULL, 0);

    //free resources
    closeSemAndShm();
    fclose(output);
    return 0;

    error:
    fclose(output);
    return 1;
}