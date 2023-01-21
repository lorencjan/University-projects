/*  Solution: IMP - project - Self-testing of microcontrollers
 *  Date: 3.11.2020
 *  Author: Jan Lorenc
 *  Faculty: Faculty of information technologies VUT
 *  Description: Main source file of the program
 *  IDE: KDS Version: 3.2.0
 */

#include <time.h>
#include "test_functions.h"

/* Initializes mcu */
void mcuInit();
/* Initializes pins */
void pinInit();

int main(void)
{
	/* Init Hw */
	mcuInit();
	pinInit();

	/* Run tests with 1 sec between them to distinguish signal beeps */
	runRegisterTests();
	wait(1000);
	runRamTests();
	wait(1000);
	runPinTests();

	/* Never leave main */
	while(1);
}

void mcuInit()
{
	MCG->C4 |= ( MCG_C4_DMX32_MASK | MCG_C4_DRST_DRS(0x01) );  /* clock subsystem */
	SIM->CLKDIV1 |= SIM_CLKDIV1_OUTDIV1(0x00);
	SIM->COPC = SIM_COPC_COPT(0x00);		  				   /* watchdog */
}

void pinInit()
{
	SIM->SCGC5 |= SIM_SCGC5_PORTA_MASK | SIM_SCGC5_PORTB_MASK; /* set clock for ports A and B */

	/* subject of my pin tests are pins 0-12 ... not 13 as it's beeper which I use as an indication of test results */
	for(uint8_t i = 0; i < 13; i++)
	{
		// use GPIO
		PORTA->PCR[i] = PORT_PCR_MUX(0x01);
		PORTB->PCR[i] = PORT_PCR_MUX(0x01);
	}

	/* set beeper */
	PORTB->PCR[13] |= PORT_PCR_MUX(0x01); /* GPIO */
	PTB->PDDR = GPIO_PDDR_PDD(0x2000);    /* set pin as output */
}

