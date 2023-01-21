//*  Soubor: htab_begin.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 31.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function returns first item in the table

#include "htab.h"

htab_iterator_t htab_begin(const htab_t * t)
{
	htab_iterator_t iterator;
	iterator.t = t;
	//iterates through items until one (first) is found
    size_t i=0;
	while(i < htab_bucket_count(t) && !t->buckets[i])
		i++;
	
    //checks if the table is empty
    if(i == htab_bucket_count(t))
    {
        iterator.ptr = NULL;
	}
	else
    {
		iterator.ptr = t->buckets[i];
		iterator.idx = i;
	}
	return iterator;
}
