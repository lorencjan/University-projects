/*  File: reivercrossing.h
 *  Solution: IOS - project 2
 *  Author: Jan Lorenc
 *  Date: 12.4.2019
 *  Faculty: Faculty of information technologies VUT
 *  Desription: Contains definitions of all needed variables and
 *              declarations of functions for rivercrossing problem
 */

#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>     //strtol()
#include <ctype.h>      //isdigit()
#include <time.h>       //srand()
#include <semaphore.h>  //semaphores
#include <sys/mman.h>   //shared memory
#include <fcntl.h>      //O_FLAGS
#include <unistd.h>     //fork()
#include <sys/types.h>  //pid_t
#include <sys/wait.h>   //wait(), waitpid()

/* DATA SCTRUCTURES AND VARIABLES */

//semaphore names
#define semWriteNAME      "/semWrite"
#define semPierNAME       "/semPier"
#define semEmbarkNAME     "/semEmbark"
#define semDisembarkNAME  "/semDismbark"
#define semFullShipNAME   "/semFullShip"
#define semSailNAME       "/semSail"
#define semFinishSailNAME "/semFinishSail"
#define semCrewAshoreNAME "/semCrewAshore"

//shared memory names
#define shmLogCounterNAME            "/shmLogCounter"
#define shmHackersWaitingCounterNAME "/shmHackersWaitingCounter"
#define shmSerfsWaitingCounterNAME   "/shmSerfsWaitingCounter"
#define shmNumOfHackersAboardNAME    "/shmNumOfHackersAboard"
#define shmNumOfSerfsAboardNAME      "/shmNumOfSerfsAboard"
#define shmDismbarkCounterNAME       "/shmDismbarkCounter"
#define shmCaptainExistsNAME         "/shmCaptainExists"
#define shmWhoAboardNAME             "/shmWhoAboard"

//constants
#define SHIP_CAP 4
enum type {HACKER, SERF, BOTH};

//log file
FILE *output;

//program arguments
typedef struct arguments
{
    int numOfPeople;
    int maxHackerGenTime;
    int maxSerfGenTime;
    int sailDuration;
    int pierReturningTime;
    int pierCapacity;
} args_t;

//semaphores
sem_t *semWrite,     //mutex for writing into the (1 at the time)
      *semPier,      //mutex just for entering the pier (amount is checked by counters)
      *semEmbark,    //mutex for embarking the boat (one by one, amount checked by counter)
      *semDisembark, //mutex for disembarking the boat (one by one, amount checked by counter)
      *semFullShip,  //for 4 processes, locks with persons getting aboard
      *semSail,      //once the ship is full, this one unlocks and they can sail out
      *semFinishSail,//once the sail is over (he's awakening unlocks it), they can start disembarking
      *semCrewAshore;//once the crew is all ashore, captain is allowed to leave 

//shared memory
int *shmLogCounter;            //counts lines(actions) in the output file
int shmLogCounterID;
int *shmHackersWaitingCounter; //counts number of hackers in the boarding queue
int shmHackersWaitingCounterID;
int *shmSerfsWaitingCounter;   //counts number of hackers in the boarding queue
int shmSerfsWaitingCounterID;
int *shmNumOfHackersAboard;    //counts hackers embarking
int shmNumOfHackersAboardID;
int *shmNumOfSerfsAboard;      //counts serf embarking
int shmNumOfSerfsAboardID;
int *shmDisembarkCounter;      //counts people disembarking
int shmDisembarkCounterID;
bool *shmCaptainExists;        //controls, if a captain is chosen
int shmCaptainExistsID;
int *shmWhoAboard;             //controls, who's aboard
int shmWhoAboardID;

/* FUNCTIONS */

//***** handling program arguments
//gets program arguments, checks for their correctness, puts them in a struct
//returns -1 in numOfPeople attribute if error occured
args_t getArguments(int argc, char *argv[]);
//checks whether argument is a digit
bool checkNumber();

//***** constructors
//opens/creates semaphores and initializes them
bool initSemaphores();
//opens/creates shared memories and sets their sizes
bool initSharedMemory();
//creates and initializes both semaphores and shared memory
bool initSemAndShm();

//***** destructors
//closes and unlinks all semaphores
bool closeSemaphores();
//closes and unlinks all shared memory
bool closeSharedMemory();
//closes and unlinks both semaphores and shared memory
bool closeSemAndShm();
//kills current process
void killProcess();

//***** program logic
//function returns a random number between zero and given number
int randomNumber(int max);
//increase embarking counter(lets another person aboard), holds the fifth who tries to embark, 
//if the ship is full, lets it sail and returns bool value if the person is the captain
bool embark(int type);
/*creates new process ... either a hacker or a serf
 *handles most of the logic and work with semaphores
 *writes to the output log file
 */
bool createProcess(char *type, int typesNumber, int typeIntVal, args_t *args, FILE *output);


