//*  Soubor: wordcount.c
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*  Date: 31.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Contains main() function of the project
//*              Reads words from the input, puts them in
//*              the hash table and counts them

#include <stdio.h>
#include <stdlib.h>
#include "io.c"
#include "htab.h"

extern bool htab_iterator_valid(htab_iterator_t it);

int main()
{
  //creates table
  htab_t *table = htab_init(HTAB_SIZE);
  if(!table)
  {
    fprintf(stderr, "Error: Failed to create hash table!\n");
    return -1;
  }

  //allocates memory for key
  char *key = malloc(sizeof(char) * MAX_WORD_LENGTH);
  if(!key)
  {
    goto clear;
  }

  htab_iterator_t iterator;

  int wordLength = get_word(key, MAX_WORD_LENGTH, stdin);
  while(wordLength != EOF)
  {
    //in case of empty space, just skip it
    if(wordLength == 0)
    {
      wordLength = get_word(key, MAX_WORD_LENGTH, stdin);
      continue;
    }
    //add key to the hash table
    iterator = htab_lookup_add(table, key);
    if(!htab_iterator_valid(iterator))
    {
      free(key);
      goto clear;
    }
    //allocates new key
    key = malloc(sizeof(char) * MAX_WORD_LENGTH);
    if(!key)
      goto clear;
    //gets next word
    wordLength = get_word(key, MAX_WORD_LENGTH, stdin);
  }

  //writes the table to stdout
  iterator = htab_begin(table);
  while(htab_iterator_valid(iterator))
  {
    printf("%s\t%u\n", htab_iterator_get_key(iterator), htab_iterator_get_value(iterator));
    iterator = htab_iterator_next(iterator);
  }

  free(key);
  htab_free(table);
  return 0;

clear:
  fprintf(stderr, "Error occured while allocating memory!\n");
  htab_free(table);
  return -1;
}
