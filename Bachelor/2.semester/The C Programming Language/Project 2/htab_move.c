//*  Soubor: htab_move.c
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*  Date: 30.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function creates new hash table, moves data from second table
//*				       to it and clears the second table

#include "htab.h"

htab_t *htab_move(size_t n, htab_t *from)
{
  //allocates a new table
  htab_t *newTable = htab_init(n);
  if(!newTable)
  {
    fprintf(stderr, "Error occured while allocating memory!\n");
    return NULL;
  }
  //inicialization of the new table
  newTable->size = htab_size(from);
  newTable->arr_size = n;

  //get first item from second table
  htab_iterator_t itemFrom = htab_begin(from);
  //check if it exists, otherwise second table is empty and job is done
  if(!htab_iterator_valid(itemFrom))
    return newTable;
  
  //index of item from first table - initialized negative so that first "if" would pass
  int lastIndex = -1;
  //copying all items
  do
  {
    //if in the same bucket, don't copy anything -> those are already in second table (ptr chain)
    if(lastIndex != itemFrom.idx)
    {
        //gets the index
        uint32_t i = (htab_hash_function(itemFrom.ptr->key) % n);
        //get item on this index
        struct htab_item *bucket_item = newTable->buckets[i];
        //if item exists, we add after it, if not, we put it there ... same as htab_lookup_add
        if(bucket_item)
        {
            while(bucket_item->next)
                bucket_item=bucket_item->next;
            bucket_item->next = itemFrom.ptr;
        }
        else
            newTable->buckets[i] = itemFrom.ptr;
    }
    //get the next item and save index of current one
    lastIndex = itemFrom.idx;
    itemFrom = htab_iterator_next(itemFrom);
  }
  while(htab_iterator_valid(itemFrom));

  //second table should end up empty - nulls all used buckets
  for(uint32_t i=0; i<htab_bucket_count(from); i++)
    from->buckets[i] = NULL;
  from->size = 0;

  return newTable;
}
