/**
 * University: University of technology (VUT) Brno
 * Faculty: Faculty of information technologies
 * 
 * Project: Implementation of compiler for imperative language IFJ19
 * Subject: IFJ & IAL
 * Team: 007
 * Variant: 1
 * 
 * Authors: Dominik Kaláb   (xkalab11)
 *          Vojtěch Staněk  (xstane45)
 *          Jan Lorenc      (xloren15)
 *          Matěj Pokorný   (xpokor78)
 * 
 * @file generator.h
 * @brief Header file for generator of IFJcode19
 *
 */


#ifndef GENERATOR_H
#define GENERATOR_H


#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include "error.h"
#include "list.h"
#include "stack.h"
#include "scanner.h"
#include "symtable.h"

extern bool actSymtable;
extern bool inLoop;

//enum all available instructions to simplify calling them
enum instructionType {
    //work with frames, calling functions
    MOVE_I,
    CREATEFRAME_I,
    PUSHFRAME_I,
    POPFRAME_I,
    DEFVAR_I,
    CALL_I,
    RETURN_I,
    //work with data stack
    PUSHS_I,
    POPS_I,
    CLEARS_I,
    //arithmetic, relational, boolean and conversion instructions
    ADD_I,
    SUB_I,
    MUL_I,
    DIV_I,
    IDIV_I,
    ADDS_I,
    SUBS_I,
    MULS_I,
    DIVS_I,
    IDIVS_I,
    LT_I,
    GT_I,
    EQ_I,
    LTS_I,
    GTS_I,
    EQS_I,
    AND_I,
    OR_I,
    NOT_I,
    ANDS_I,
    ORS_I,
    NOTS_I,
    INT2FLOAT_I,
    FLOAT2INT_I,
    INT2CHAR_I,
    STRI2INT_I,
    INT2FLOATS_I,
    FLOAT2INTS_I,
    INT2CHARS_I,
    STRI2INTS_I,
    //I/O instructions
    READ_I,
    WRITE_I,
    //working with strings
    CONCAT_I,
    STRLEN_I,
    GETCHAR_I,
    SETCHAR_I,
    //working with types
    TYPE_I,
    //program flow instructions
    LABEL_I,
    JUMP_I,
    JUMPIFEQ_I,
    JUMPIFNEQ_I,
    JUMPIFEQS_I,
    JUMPIFNEQS_I,
    EXIT_I,
    //debug instructions
    BREAK_I,
    DPRINT_I
};

char *skipLabel;

/**
 * @brief prints specified instruction to output
 * @param instruction specifies instruction, this parameter is requiered
 * @param op1 specifies first operand, if unused, use NULL
 * @param op2 specifies second operand, if unused, use NULL
 * @param op3 specifies third operand, if unused, use NULL
 * @return returns 0 or number of error from error.h file
 */
int printInstruction(int instruction, char *op1, char *op2, char *op3);

/**
 * @brief Prints header of IFJcode19 to output
 */
void printStart();

/**
 * @brief Generates names for labels
 * @return Returns an unsued label name
 */
char* getLabel();

/**
 * @brief Generates names for variables
 * @param global Defines whether the variable should be global or local
 * @return Returns an unused variable name
 */
char* getVariableName(int location);

/**
 * @brief Generates names IDs, checks for all kinds of being already defined
 * @param ifjCodeName Resulting name of the variable
 * @param idName Name of the identifier
 * @param isDefined For storing, whether it's already defined
 * @return 0 on success, otherwise error code
 */
int generateNameOfIdentifier(char **ifjCodeName, char *idName, bool *isDefined);

/**
 * @brief Generates instructions for function Inputs
 */
int printInputs();

/**
 * @brief Generates instructions for function Inputi
 */
int printInputi();

/**
 * @brief Generates instructions for function Inputf
 */
int printInputf();

/**
 * @brief Generates instructions for function print
 * @param params List of parameters(tokens) for the print builtin
 */
int printPrint(List *params);

/**
 * @brief Generates instructions for function Len
 * @param str String whose lenght we want to get
 * @param str Defines if it's identifier or literal
 * @param result String name of IFJcode19 result variable
 */
int printLen(char *str, bool isId);

/**
 * @brief Generates instructions for function Substr
 * @param str String whose lenght we want to get
 * @param index Start of the substring
 * @param n Lenght of the substring
 */
void printSubstr(char *str, char *index, char *n);

/**
 * @brief Generates instructions for function Ord
 * @param str String whose lenght we want to get
 * @param index Index of the char in the string
 */
void printOrd(char *str, char *index);

/**
 * @brief Generates instructions for function Chr
 * @param val IFJcode19 ASCII value
 */
void printChr(char *val);

/**
 * @brief Prints instructions needed for the beginning of function definition, rest will be printed as the parser goes through
 * @param name Name of the function to be defined
 * @param params List of function parameters as tokens
 * @param paramsCount Number of parameters
 * @return 0 on success, error code otherwise
 */
int printFunctionDefinition(char *name, List *params, int paramsCount);

/**
 * @brief Prints instructions needed when a function is call, also handles builtin calls
 * @param name Name of the function to be called
 * @param params List of function parameters as tokens
 * @param paramsCount Number of parameters
 * @param params Name of the variable to return to
 * @return 0 on success, error code otherwise
 */
int printFunctionCall(char *name, List *params, int paramsCount, char *retVal);

/**
 * @brief Replaces invalid chars such as ' ' or '\n' and replaces them with \010 and such
 * @param str String to be converted
 * @return String in IFJcode19 format
 */
String *generatorString(char *str);

/**
 * @brief Generates ifj19code instructions for expression specified in postfix lsit
 * @param postfix List containting expression i postfix notation
 * */
void printExpression(List *postfix);

/**
 * @brief Pops top two items on stack and ensures both are int or both are float
 */
//~ void getComparableStack();

/**
 * @brief Converts two topmost ints on stack to floats
 * 
 * Of course checks whether the value is int, then changes type
 */
void ints2Floats();

/**
 * @brief Checks if two topmost operands on stack are not int (used before arithmetic operations)
 */
void checkNone();


#endif
