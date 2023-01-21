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
 * @file parser.h
 * @brief Header file for the parser, which covers the functionality of the syntactic analysis
 *
 */


#ifndef PARSER_H
#define PARSER_H

#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include "stack.h"
#include "symtable.h"
#include "list.h"
#include "scanner.h"
#include "error.h"
#include "generator.h"

/**
 * @brief starts LL grammar analysis
 * @return 0 for successful run or one of errors from error.h     
 */    
int analyse();

/**
 * @brief implements rules for <prog> nonterminal symbol
 * @return 0 for successful run or one of errors from error.h     
 */    
int prog();

/**
 * @brief implements rules for <statement> nonterminal symbol
 * @return 0 for successful run or one of errors from error.h     
 */    
int statement();

/**
 * @brief implements rules for <params> nonterminal symbol
 * @param funcName holds string value of previous id token for usage in semaintic checks
 * @param definition true if this function is called from definition context
 * @param assignId 
 * @return 0 for successful run or one of errors from error.h     
 */    
int params(char *funcName, bool definition, char * assignId);

/**
 * @brief implements rules for <param> nonterminal symbol
 * @param params list into which parameter will be loaded
 * @param paramCount counts amount of params for semantic checks in parent function
 * @param definition true if this function is called from definition context
 * @return 0 for successful run or one of errors from error.h     
 */    
int param(List *params, int *paramCount, bool definition);

/**
 * @brief implements rules for <assignOrFunc> nonterminal symbol
 * @param IdName holds string value of previous id token for usage in semaintic checks
 * @return 0 for successful run or one of errors from error.h     
 */    
int assignOrFunc(char * IdName);

/**
 * @brief implements rules for <item> nonterminal symbol
 * @return 0 for successful run or one of errors from error.h     
 */    
int item();

/**
 * @brief implements rules for <builtIn> nonterminal symbol
 * @return 0 for successful run or one of errors from error.h     
 */    
int builtIn();

/**
 * @brief implements rules for <eols> nonterminal symbol
 * @return 0 for successful run or one of errors from error.h     
 */    
int eols();

/**
 * @brief implements expression analysis with help of postfix 
 * @param resType will hold type of result of expression
 * @return 0 for successful run or one of errors from error.h     
*/  
int expr(int *resType);

/**
 * @brief delStack frees only stackItems, but tokens have inner dynamic types -> need to be freed
 * @param s Stack of pointers   
 */    
void freeTokenStack(Stack *s);
/**
 * @brief Deep copies one token to another
 * @param s Token to be copied
 * @return Copy of the token, NULL on internal error
 */ 
Token_t *deepCopyToken(Token_t *old);
#endif
