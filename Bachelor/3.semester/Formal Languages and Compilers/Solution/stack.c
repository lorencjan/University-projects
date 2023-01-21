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
 * @file stack.c
 * @brief Implementation of stack structure and functions
 *
 */

#include "stack.h"

stackItem_t *initStackItem()
{
    stackItem_t *newitem = malloc(sizeof(stackItem_t));
    if(newitem == NULL) return NULL;

    newitem->data = NULL;
    newitem->next = NULL;
    return newitem;
}

void initStack(Stack *s)
{
    if(s == NULL) return;
    s->top = NULL;
}

bool isStackEmpty(Stack *s)
{
    if(s == NULL) return true; //REALLY SHOULD NOT HAPPEN
    
    return (s->top == NULL) ? true : false;
}

bool push(Stack *s, void *data)
{
    if(s == NULL || data == NULL) return false; 

    stackItem_t *newitem = initStackItem();
    if(!newitem) return false;
    
    newitem->data = data;
    newitem->next = s->top; //If s->top == NULL, nothing happens
    s->top = newitem; //Top now points to new item on the top of stack
    return true;
}

void pop(Stack *s)
{
    if(s == NULL) return;
    if(isStackEmpty(s)) return;

    stackItem_t *holder = s->top->next; //Can be NULL
    free(s->top);
    s->top = holder; //Can be NULL
}

stackItem_t *top(Stack *s)
{
    if(s == NULL) return NULL;
    if(isStackEmpty(s)) return NULL; //Should be an error, not a return
    return s->top;
}

void delStack(Stack *s)
{
    if(s == NULL || isStackEmpty(s)) return;
    while(s->top){
        pop(s);
    }
}
