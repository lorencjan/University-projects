/*  Solution: IMP - project - Self-testing of microcontrollers
 *  Date: 3.11.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Header file for test functions
 */

#ifndef TEST_FUNCTIONS_H
#define TEST_FUNCTIONS_H

#include "MKL05Z4.h"
#include <time.h>

extern int testRegisters();
extern int testRam(int hexaStartAddress);

/*
 * Runs the assembly testRegisters() function and handles result.
 * It makes 1 beep on success, 3 beeps on failure to let the user know.
 */
void runRegisterTests();

/*
 * Runs the assembly testRam() function and handles result.
 * It makes 1 beep on success, 3 beeps on failure to let the user know.
 */
void runRamTests();

/*
 * Tests pins against stuck-at error. It tests pins 0-12 of both PORTA and PORTB.
 * It sets output of PORTA pins as input to PORTB pins and compares the values.
 * Then the same the other way around.
 * It makes 1 beep on success, 3 beeps on failure to let the user know.
 */
void runPinTests();

/* Waits for N milliseconds */
void wait(uint32_t milliseconds);

/* Delays execution for given empty loop cycles */
void delay(uint32_t cycles);

/* Beeps via beeper (PTB13) - 500 periods for rectangle signal */
void beep();

#endif
