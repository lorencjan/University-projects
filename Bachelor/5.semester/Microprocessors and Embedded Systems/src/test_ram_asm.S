/* File: test_ram_asm.S
 * Solution: IMP - project - Self-testing of microcontrollers
 * Date: 5.11.2020
 * Author: Jan Lorenc
 * Faculty: Faculty of information technologies VUT
 * Description: ARM assembly source file containing function to test RAM memory */

/* Function tests 32B of memory from an address specified by parameter.
   It uses MarchX algorithm for the test.
   Returns 0 on success or 1 on error */

	.globl   testRam			 /* makes it visible for .c code */
    .type    testRam, %function  /* says it's a function */

testRam:						 /* function entry point */
	.fnstart

	/* counter ... in R0 is start address from the parameter */
	MOV R1, #0

	/* fill the memory block with zeros and backup the current values to stack */
	zeroLoop:
		LDR R2, [R0, R1]		 /* load from memory with current offset (== counter value) */
		PUSH {R2}				 /* backup the value */
		MOV R2, #0				 /* set zero */
		STR R2, [R0, R1]		 /* save zero */
		ADD R1, R1, #4			 /* increment counter by 4B == reg size == loaded mem block */
		CMP R1, #32				 /* check for the end ... we test only 32B at once */
		BNE zeroLoop			 /* repeat if not there yet */

	/* checking the zero values and writing ones */
	MOV R1, #0					 /* reset counter */
	zeroTestLoop:
		LDR R2, [R0, R1]		 /* load from memory with current offset (== counter value) */
		CMP R2, #0				 /* compare with 0 */
		BNE error				 /* test fails if it's not zero that we set previously */
		LDR R2, =0x01010101		 /* set one to each byte in the register (out of 4) */
		STR R2, [R0, R1]		 /* save the ones */
		ADD R1, R1, #4			 /* increment counter */
		CMP R1, #32				 /* check for the end */
		BNE zeroTestLoop

	/* checking the ones and writing back zeros (in descending order) */
	oneTestLoop:
		SUB R1, R1, #4			 /* decrement first (we have 32 in counter and we want 28) */
		LDR R2, [R0, R1]		 /* load from memory with current offset (== counter value) */
		LDR R3, =0x01010101
		CMP R2, R3		 		 /* check ones (again we have 4B -> 0x01010101) */
		BNE error				 /* test fails if there are not ones */
		MOV R2, #0		 		 /* set zeros back */
		STR R2, [R0, R1]		 /* save the zeros */
		CMP R1, #0				 /* check for the end */
		BGE oneTestLoop

	/* check the zeros again and restore the original values */
	MOV R1, #28					 /* we need to go in descending order again, because the original values are on the stack */
	backupRestoreLoop:
		LDR R2, [R0, R1]		 /* load from memory with current offset (== counter value) */
		CMP R2, #0				 /* compare with 0 */
		BNE error				 /* test fails if it's not zero */
		POP {R2}				 /* get the original value from stack */
		STR R2, [R0, R1]		 /* save the original value back */
		BNE backupRestoreLoop	 /* repeat if not there yet */

	/* return successfully */
	MOV R0, #0
	BX LR

	/* return with failure */
	error:
		MOV R0, #1
		BX LR

	.fnend
