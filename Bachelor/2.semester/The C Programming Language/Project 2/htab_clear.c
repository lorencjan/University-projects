//*  Soubor: htab.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 30.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Clears items of given table

#include "htab.h"

void htab_clear(htab_t * t)
{
  	//iterate through all buckets and deallocate its items
	htab_iterator_t iterator = htab_begin(t);
	htab_iterator_t prevIter;
	while(htab_iterator_valid(iterator))
	{
		prevIter = iterator;
		iterator = htab_iterator_next(iterator);
		free(prevIter.ptr->key);
		free(prevIter.ptr);
	}
 	//nulls the table
	t->size = 0;
  	for(size_t i=0; i<t->arr_size; i++)
		t->buckets[i] = NULL;
}