//*  Soubor: htab_iterator_next.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 31.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function returns iterator pointing to the next item

#include "htab.h"

htab_iterator_t htab_iterator_next(htab_iterator_t it)
{
	htab_iterator_t iterator;
	iterator.t = it.t;

	//if there is a next item, we stay in the bucket and just get next
	if(it.ptr->next)
	{
		iterator.idx = it.idx;
		iterator.ptr = it.ptr->next;
	}
	else //otherwise it was last in the bucket and we must find another
	{
		size_t index = it.idx+1;
		//index cannot be out of boundaries
		if(index >= htab_bucket_count(it.t))
			return htab_end(it.t);

		//searching for a next bucket
		struct htab_item *item = it.t->buckets[index++];
		while(!item && index < htab_bucket_count(it.t))
		{
			item = it.t->buckets[index++];
		}
		//if item was not found, return htab_end()
		if(!item)
			return htab_end(it.t);
		else //otherwise we have found our next item
		{
			iterator.ptr = item;
			iterator.idx = index-1;
		}
	}
	return iterator;
}