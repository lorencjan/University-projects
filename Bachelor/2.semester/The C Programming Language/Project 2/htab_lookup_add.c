//*  Soubor: htab_init.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 31.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function finds an item in the table according to
//*				 the key or creates one it not successful

#include "htab.h"

htab_iterator_t htab_lookup_add(htab_t * t, const char *key)
{
	//creates an iterator
	htab_iterator_t iterator;
	iterator.t = t;

	//gets index
    uint32_t index = (htab_hash_function(key) % htab_bucket_count(t));
	//searches for item
	struct htab_item *item = t->buckets[index];
	while ( item != NULL )
	{
	    if (!strcmp(item->key, key))
	    {
	      //if found, increase counter and return iterator
		  item->data++;
	      iterator.ptr = item;
	      iterator.idx = index;
		  free((char*)key); //key is already stored in the existent item
	      return iterator;
	    }
	    item = item->next;
	}

	//if we got here, item does not exist, we need to create new one
	item = NULL;  // not necessary but just in case
	item = malloc(sizeof(struct htab_item));
	if(!item)
	{
		fprintf(stderr, "Faild to allocate memmory for a new item!\n");
		return htab_end(t);
	}
    t->size++;
	item->key = (char*)key;
	item->data = 1;
	item->next = NULL;

	//get item on this index
	struct htab_item *bucket_item = t->buckets[index];
	//if item exists, we add after it, if not, we put it there
	if(bucket_item)
	{
        while(bucket_item->next)
			bucket_item=bucket_item->next;
		bucket_item->next = item;
	}
	else
        t->buckets[index] = item;

	iterator.idx = index;
	iterator.ptr = item;
	return iterator;
}
