//*  Soubor: io.c
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*  Date: 30.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: This module contains a function that reads a word
//*              from file, puts it in array and returns its length

#include <stdio.h>
#include <stdbool.h>
#include <ctype.h>

static bool longWordError = false;

int get_word(char *s, int max, FILE *f)
{
    int counter=0;
    int c;

    while(counter < max)
    {
        c = getc(f);
        // returns EOF when end of file
        if(c==EOF)
            return EOF;
        // returns words length if word ends
        if(isspace(c))
        {
            s[counter] = '\0';
            return counter;
        }
        s[counter++] = c;
    }

    //if we got here, word is too long, skip the rest and print error
    while((c=fgetc(f)))
        if(isspace(c))
            break;
    //check if error has not occured
    if (!longWordError)
    {
      longWordError = true;
      fprintf(stderr, "Error: At least one word is longer then maximum size of %d characters!\n", max);
    }

    s[counter] = '\0';
    return max;
}
