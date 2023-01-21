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
 * @file scanner.h
 * @brief Header file for the scanner, which covers the functionality of the lexical analysis
 *
 */

#ifndef SCANNER_H
#define SCANNER_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <ctype.h>
#include "dString.h"
#include "stack.h"

/*** TYPES AND VARIABLES ***/

/**
 * Enum of token types states
 */
#define KEYWORDS 50
#define BUILTINS 60 
typedef enum tokenType
{
    ID_T, STRING_T, INT_T, FLOAT_T,   //types of operands
    ASSIGNMENT_T,                     // =
    EQUAL_T, NOT_EQUAL_T, GREATER_EQUAL_T, LESS_EQUAL_T, GREATER_THAN_T, LESS_THAN_T, // relation operators
    PLUS_OP_T, MINUS_OP_T, MUL_OP_T, DIV_OP_T, INT_DIV_OP_T, // arithmetic operators ...div=='/', int_div=='//'
    EOL_T,                            //end of line has special meaning as it matters for indents
    INDENT_T, DEDENT_T,               //symbolisis changes in indent
    LEFT_BRACKET_T, RIGHT_BRACKET_T,  //brackets
    LEFT_SQUARE_BRACKET_T, RIGHT_SQUARE_BRACKET_T,
    COLON_T, COMA_T,                  //other significant characters
    LEXICAL_ERROR_T,                  //type of token in case of an error during lexical analisis / memory allocation or other
    EOF_T,                            //special token for the end of input program
    UNDEFINED,                        //initial value of all tokens
    IF_T = KEYWORDS, ELSE_T, DEF_T, RETURN_T, NONE_T, PASS_T, WHILE_T,               //keywords (id: 16-)
    INPUTS_T = BUILTINS, INPUTI_T, INPUTF_T, PRINT_T, LEN_T, SUBSTR_T, ORD_T, CHR_T  //built-in functions
} TokenType_t;

/**
 * @struct token
 * @brief Holds the its type and value
 */
typedef struct token
{
    TokenType_t type;
    union
    {
        String *stringVal;
        int intVal;
        double floatVal;
    }; 
} Token_t;

/**
 * Enum of automata states -> resulting token type
 */
typedef enum state
{
    START_S, END_S,                                          // starting and ending states
    INDENT_S,                                                //state where we check for indents
    ID_S,                                                    // identifier
    INT_S, FLOAT_S, TO_SCIENTIFIC_S, SIGN_SCIENTIFIC_S, SCIENTIFIC_S, // numbers decimal
    ZERO_S, HEXA_INT_S, HEXA_FLOAT_S, TO_HEXA_SCIENTIFIC_S, SIGN_HEXA_SCIENTIFIC_S, SCIENTIFIC_HEXA_S, // numbers hexa
    STRING_S, ESCAPE_SEQUENCE_S, HEXA_CHAR_1_S,HEXA_CHAR_2_S,// strings, chars
    ONE_LINE_COMMENT_S, DOCSTRING_S,                         // comments
    DOCSTRING_IN_1_S, DOCSTRING_IN_2_S,                      // states for ", ""  ...before we get to """ (multiline comment)
    DOCSTRING_OUT_1_S, DOCSTRING_OUT_2_S,                    // states for ", "" ...before we get out of multiline comment
    DIV_S, GREATER_S, LESS_S, NOT_EQUAL_S, ASSIGN_S          // multichar operators (assign can get to equal)
} State_t;

/**
 * Stores the type of the last token
 */ 
extern TokenType_t prevToken;
/**
 * In case of dedenting through several levels, remember how many
 * levels we've been through to return correct number of dedents
 */ 
extern int returnDedent;

/**
 * Array of IFJ19 keywords
 */
#define NUM_OF_KEYWORDS 7
extern char *keywords[NUM_OF_KEYWORDS];
/**
 * Array of IFJ19 built-in functions
 */
#define NUM_OF_BUILTINS 8
extern char *builtins[NUM_OF_BUILTINS];


/*** FUNCTIONS ***/

/**
 * @brief Loads a new token. Gets chars one by one and
 *        recognizes the new token using an finite automata
 * @return Pointer to the newly loaded token or NULL on internal error
 */
Token_t *loadToken();

/**
 * @brief Checks whether we can stay on the same level or
 *        an indent/dedent token should be generated
 * @param str Token whose type should be set
 * @param str Current state
 * @return True on successful check, false on internal error
 */
bool checkIndents(Token_t *token, State_t *state);

/**
 * @brief Counts the spaces at the beginning of each new line
 * @return Number of spaces == indent, or -1 on lexical error, or -2 on EOL(empty line, no harm)
 */
int *countSpaces();

/**
 * @brief Checks whether the recognized identifier counts among keywords/bultins or not
 * @param str Recognized identifier to be checked for a keyword
 * @return Enum type value of the found keyword/builtin of 0 if not found
 */
int isKeywordOrBuiltin(char *str);

/**
 * @brief Checks if it can return float token or error
 * @param c Currently loaded char to be returned to the input
 * @param token Current token to be set proper attribute values
 * @param state Current state to be set to the end state
 * @param integer Defines whether we're finishing int or float number
 */
void endNumber(char c, Token_t *token, State_t *state, bool integer);

/**
 * @brief Loads characters and checks if the line is empty (only spaces and tabs)
 * @return 0 if not empty, 1 if empty, 2 if empty until # == one line comment
 */
int checkEmptyLine();

/**
 * @brief Convert hexadecimal char to decimal int, EXPECTS VALID HEXA CHAR (0-9,a-f,A-F)
 * @return Int decimal value of hexa char
 */
int convertHexaCharToDec(char c);

/**
 * @brief Firstly it frees the stringVal as it's allocated by itself and then the token itself
 */
void freeToken(Token_t *token);

#endif
