//*  Soubor: htab_init.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 30.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function initializes new hash table and returns it

#include "htab.h"

htab_t *htab_init(size_t n)
{
 	//allocates new table with space for the bucket array as well
    htab_t *table = malloc(sizeof(htab_t) + sizeof(struct htab_item*)*n);
 	if(!table)
        return NULL;
	//removing trash
    for(size_t i=0; i<n; i++)
    {
		table->buckets[i] = NULL;
	}

 	//initializes the values
 	table->size = 0;
 	table->arr_size = n;

 	return table;
}
