/**
 * University: University of technology (VUT) Brno
 * Faculty: Faculty of information technologies
 * 
 * Project: Implementation of a compiler for imperative language IFJ19
 * Subject: IFJ & IAL
 * Team: 007
 * Variant: 1
 * 
 * Authors: Dominik Kaláb   (xkalab11)
 *          Vojtěch Staněk  (xstane45)
 *          Jan Lorenc      (xloren15)
 *          Matěj Pokorný   (xpokor78)
 * 
 * @file dString.h
 * @brief Header file for (dynamic) string structure
 *
 */
 
#ifndef STRING_H
#define STRING_H

#include <string.h>
#include <stdio.h>
#include <stdlib.h>

#define SIZE 8 //Default starting and realloc size of String

/**
 * @struct String
 * @brief Structure for dynamic string (as in higher languages)
 */
typedef struct
{
    char *string;
    size_t maxSize;
} String;

/**
 * @brief Initializes new string
 * 
 * @return Pointer to initialized string or NULL if error
 */
String *initString();

/**
 * @brief Append to string
 * 
 * @param s     String to append to
 * @param val   Value to append
 * 
 * @return New length of string s, -1 if error
 */
int appendString(String *s, char *val);

/**
 * @brief Delete and free a string
 * 
 * @param s String to delete
 */
void delString(String *s);

/**
 * @brief Print a string to stdout
 * 
 * @param s String to pring
 */
void printString(String *s);

#endif
