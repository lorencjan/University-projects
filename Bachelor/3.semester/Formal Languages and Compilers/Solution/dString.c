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
 * @file dString.c
 * @brief Implementation of String structure and functions
 *
 */

#include "dString.h"

String *initString()
{
    //Create new String
    String *newstring = malloc(sizeof(String));
    if(newstring == NULL) return NULL;
    
    //Create space for the string itself
    newstring->string = calloc(SIZE, sizeof(char));
    if(newstring->string == NULL)
    {
        free(newstring);
        return NULL;
    }
    
    newstring->maxSize = SIZE;
    return newstring;
}

int appendString(String *s, char *val)
{
    if(s == NULL || s->string == NULL || val == NULL) return -1;
    
    //Check if there is enough space allocated
    while((strlen(s->string) + strlen(val)) >= s->maxSize)
    {
        //If there is not enough space, realloc
        s->string = realloc(s->string, sizeof(char) * (s->maxSize + SIZE));
        if(s->string == NULL)
        {
            delString(s);
            return -1;
        }
        s->maxSize += SIZE;
    }
    
    if(strcat(s->string, val) == NULL) return -1;

    return strlen(s->string);
}

void delString(String *s)
{
    if(!s) return;
    
    if(s->string)
    {
        free(s->string);
        s->string = NULL; 
    }                                           
    free(s);
    s = NULL;
}

void printString(String *s)
{
    if(s == NULL || s->string == NULL) return;
    
    printf("%s\n", s->string);
}
