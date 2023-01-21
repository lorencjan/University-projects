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
 * @file list.h
 * @brief Header file for list structure
 *
 */

#ifndef LIST_H
#define LIST_H

#include <stdio.h>
#include <stdlib.h>
#include "scanner.h"

/**
 * @struct item
 * @brief Structure for items of the list
 */
typedef struct item
{
    Token_t *value;
    int index;
    struct item *next;
} listItem;

/**
 * @struct list
 * @brief Structure of a one-way linked list
 */
typedef struct linkedList
{
    listItem *first;   //First item in list (=head)
    listItem *current; //Current item in list (for iteration)
} List;

/**
 * @brief Initialize an item of the list
 *
 * @return Pointer to initialized item
 */
listItem *initItem();

/**
 * @brief Initialize a list
 *
 * @return Pointer to initialized list
 */
List *initList();

/**
 * @brief Delete a list
 *
 * @param list List to be deleted
 */
void delList(List *list);

/**
 * @brief Append (add to the end) an item
 *
 * @param list List to add to
 * @param value Value of the item
 *
 * @return Index of inserted element, -1 if failed
 */
int appendList(List *list, Token_t *value);

/**
 * @brief Search a list for an item with matchin value
 *
 * @param list List to search
 * @param value Value to search for
 *
 * @return Index of found item, -1 if not found
 */
int searchList(List *list, Token_t *value);

/**
 * @brief Search a list by index
 *
 * @param list List to search
 * @param index Index to search
 *
 * @return Value of item with found index, otherwise NULL
 */
Token_t *searchListByIndex(List *list, int index);

/**
 * @brief Print out a list (mainly for debugging)
 *
 * @param list List to pring
 */
void printList(List *list);

#endif
