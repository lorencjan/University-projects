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
 * @file stack.h
 * @brief Header file for stack structure
 *
 */

#ifndef STACK_H
#define STACK_H

#include <stdbool.h>
#include "scanner.h"

/**
 * @struct stackItem 
 * @brief Struct of item in stack
 */
typedef struct stackItem
{
    void *data;
    struct stackItem *next; //Pointer to item below this one
} stackItem_t;

/**
 * @struct Stack
 * @brief Structure of Stack itself with pointer to top
 */
typedef struct
{
    stackItem_t *top;
} Stack;

/**
 * @brief Initializes new stack item
 * 
 * @return Pointer to new stack item
 */
stackItem_t *initStackItem();

/**
 * @brief Initializes the stack
 * 
 * @param s Pointer to stack to init
 */
void initStack(Stack *s);

/**
 * @brief Push an item (token) to stack (on top)
 * 
 * @param s     Pointer to stack
 * @param data  Pointer to data itself
 * @return      True on success, false on internal error
 */
bool push(Stack *s, void *data);

/**
 * @brief Pop an item from stack
 * 
 * Does nothing if stack is empty.
 * 
 * @param s Stack from which to pop
 */
void pop(Stack *s);

/**
 * @brief Checks if stack is empty
 * 
 * @param s Stack to check
 * 
 * @return True if stack is empty, False if not
 */
bool isStackEmpty(Stack *s);

/**
 * @brief Reads the top value from stack
 * 
 * Reads the value from stack. Stack is not changed by this function.
 * 
 * @param s Stack to read from
 * 
 * @return Pointer to top item in stack
 */
stackItem_t *top(Stack *s);

/**
 * @brief Delete and free the stack
 * 
 * @param s Stack to delete - free
 */
void delStack(Stack *s);

#endif
