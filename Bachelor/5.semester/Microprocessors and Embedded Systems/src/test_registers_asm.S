/* File: test_registers_asm.S
 * Solution: IMP - project - Self-testing of microcontrollers
 * Date: 3.11.2020
 * Author: Jan Lorenc
 * Faculty: Faculty of information technologies VUT
 * Description: ARM assembly source file containing function to test cpu registers for stuck-at error */

/* Function tests all standard lower registers, because to the rest of them the thumb
   instructions cannot write, on "stuck-at" error using Checkerboard algorithm and
   PC register as it's thumb writable and the test is quite unique
   returns 0 on success or 1 on error */

	.globl   testRegisters				/* makes it visible for .c code */
    .type    testRegisters, %function   /* says it's a function */

testRegisters:							/* function entry point */
	.fnstart

	/* reference values ... to be changed to r0 and r1 */
	LDR R1, =0xAAAAAAAA					/* using LDR because MOV can move either from reg to reg */
	LDR R2, =0x55555555					/* or max 8bit (== max 255) immediate value to a register */

	/* test R0 register */
	LDR R0, =0xAAAAAAAA					/* load first pattern */
	CMP R0, R1							/* check for correct value */
	BNE error							/* if not equal, exit with error */
	LDR R0, =0x55555555					/* load second pattern */
	CMP R0, R2							/* check for correct value */
	BNE error							/* if not equal, exit with error */

	LDR R0, =0xAAAAAAAA					/* R0 is the new reference value for 0xAAAAAAAA pattern */
	/* test R1 register */
	LDR R1, =0xAAAAAAAA
	CMP R1, R0
	BNE error
	LDR R1, =0x55555555
	CMP R1, R2
	BNE error

	LDR R1, =0x55555555					/* R1 is the new reference value for 0x55555555 pattern */
	/* test R2 register */
	LDR R2, =0xAAAAAAAA
	CMP R2, R0
	BNE error
	LDR R2, =0x55555555
	CMP R2, R1
	BNE error

	/* test R3 register */
	LDR R3, =0xAAAAAAAA
	CMP R3, R0
	BNE error
	LDR R3, =0x55555555
	CMP R3, R1
	BNE error

	/* test R4 register */
	LDR R4, =0xAAAAAAAA
	CMP R4, R0
	BNE error
	LDR R4, =0x55555555
	CMP R4, R1
	BNE error

	/* test R5 register */
	LDR R5, =0xAAAAAAAA
	CMP R5, R0
	BNE error
	LDR R5, =0x55555555
	CMP R5, R1
	BNE error

	/* test R6 register */
	LDR R6, =0xAAAAAAAA
	CMP R6, R0
	BNE error
	LDR R6, =0x55555555
	CMP R6, R1
	BNE error

	/* test R7 register */
	LDR R7, =0xAAAAAAAA
	CMP R7, R0
	BNE error
	LDR R7, =0x55555555
	CMP R7, R1
	BNE error

	/* test PC register */
	MOV R2, PC						/* save PC */
	ADD R2, R2, #8					/* in R2 is PC+8, in PC is PC+4 (in comparison to the start of the test) */
	CMP R2, PC						/* second instruction since start -> PC should be incremented by 8 by now */
	BNE error

	/* return successfully */
	MOVS R0, #0						/* return with value 0 to identify as success */
	BX LR							/* jump to the position defined by LR == return address */

	/* return with failure */
	error:
		MOVS R0, #1
		BX LR

	.fnend
