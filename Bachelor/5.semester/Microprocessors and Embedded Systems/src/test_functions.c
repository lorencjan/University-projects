/*  Solution: IMP - project - Self-testing of microcontrollers
 *  Date: 3.11.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Header file for test functions
 */

#include "test_functions.h"

void runRegisterTests()
{
	if(testRegisters() == 1) /* error -> 3 beeps */
	{
		beep();
		wait(200);
		beep();
		wait(200);
		beep();
	}
	else
	{
		beep();
	}
}


void runRamTests()
{
	/* we'll test 16kB */
	int error = 0;
	for(uint32_t i = 0x20000000; i < 0x00003fff; i += 0x20)
	{
		if(testRam(i) == 1)
		{
			error = 1;
			break;
		}
	}

	/* again ... 3 beeps on error, otherwise 1 for success */
	if(error == 1)
	{
		beep();
		wait(200);
		beep();
		wait(200);
		beep();
	}
	else
	{
		beep();
	}
}


void runPinTests()
{
	/* set 13 pins (0-12) of both PORTA and PORTB as output */
	PTA->PDDR = 0x1fff;
	PTB->PDDR = 0x1fff;

	PTA->PDOR = 0x1fff;    /* set ones to all pins from PORTA */
	PTB->PDOR = PTA->PDOR; /* set values of PORTA pins to PORTB pins */

	/* PORTB pins should all contain ones */
	if(PTB->PDOR != 0x1fff)
		goto error;

	/* clear pin values */
	PTA->PDOR = 0x0000;
	PTB->PDOR = 0x0000;

	/* pins should be cleared */
	if(PTA->PDOR != 0x0000 || PTB->PDOR != 0x0000)
		goto error;

	/* now do the same in reverse */
	PTB->PDOR = 0x1fff;    /* set ones to all pins from port B */
	PTA->PDOR = PTB->PDOR; /* set values of PORTB pins to PORTA pins */

	/* PORTA pins should all contain ones */
	if(PTA->PDOR != 0x1fff)
		goto error;

	/* one beep for success */
	beep();
	return;

	/* three beeps for failure */
	error:
		beep();
		wait(200);
		beep();
		wait(200);
		beep();
}

void wait(uint32_t milliseconds)
{
    clock_t clk = clock();
    while (clock() < clk + milliseconds);
}

void delay(uint32_t cycles)
{
  for(uint32_t i = 0; i < cycles; i++);
}

void beep()
{
	for (uint16_t i = 0; i < 500; i++)
	{
    	PTB->PDOR = GPIO_PDOR_PDO(0x2000);
    	delay(500);
    	PTB->PDOR = GPIO_PDOR_PDO(0x0000);
    	delay(500);
    }
}

