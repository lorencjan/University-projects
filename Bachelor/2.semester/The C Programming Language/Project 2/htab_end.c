//*  Soubor: htab_end.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 31.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function returns iterator pointing
//*				 behind the last item

#include "htab.h"

htab_iterator_t htab_end(const htab_t * t)
{
	htab_iterator_t iterator = {
		.ptr = NULL,  // nonexistent
		.t = t
	};
	/*search for the last nonempty bucket - so thet the
	  null one after it is the first nonexistent after end
	  (another option is to take the (nonexistent) item after the
	  last item in the last bucket list, but the goal of a hash table
	  is for the keys to have as few same indexes as possible)*/
    int i = (int)htab_bucket_count(t)-1;
	while(!t->buckets[i] && i != 0)
		i--;
	
	//if the table is empty, the last is also the first
	if(i == 0)
		iterator.idx = 0;
	else
		iterator.idx = i+1; //index to the one after nonempty bucket

	return iterator;
}
