//*  Soubor: htab.h
//*  Řešení: IJC-DU2
//*	 License: (Public domain)
//*	 Date: 30.3.2019
//*  Author: Jan Lorenc (base from Ing. Petr Peringer, Ph.D)
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: This is the header file for hash C hash table

// header guards
#ifndef __HTABLE_H__
#define __HTABLE_H__

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stddef.h>     // size_t
#include <stdint.h>			// uint32_t
#include <stdbool.h>    // bool

/* Up to size around 30000 the speed of the program increases significantly.
   After that just by small amount, for instance 60000 was faster only for 0.4 sec.
   This is the speed milestone and more would be just wasting memory for small inputs. 
   Why 30011 and not 30000: Better distribution. Hash tables have generally size of a
   prime number to avoid grouping values into a small number of buckets*/
#define HTAB_SIZE 30011
//maximum length of input word
#define MAX_WORD_LENGTH 127

// hash table
typedef struct htab
{
	size_t size;				          // number of items
	size_t arr_size;			        // number of buckets
  struct htab_item *buckets[];  // array of buckets
}htab_t;  						          // typedef according to the task

// iterator to table
struct htab_item
{
	char *key;
	uint32_t data;
	struct htab_item *next;
};               

// iterator
typedef struct htab_iterator
{
    struct htab_item *ptr;      // pointer to item
    const htab_t *t;            // in which table
    int idx;                    // in which bucket
} htab_iterator_t;              // typedef according to the task

// hash function
uint32_t htab_hash_function(const char *str);

/*** functions that work with the table ***/

// constructor - creates and initializes new table
// n = number of buckets (.arr_size)
htab_t *htab_init(size_t n);

// move constructor - creates and initializes new table by moving data
// from table *from ... *from ends up empty but still allocated
htab_t *htab_move(size_t n, htab_t *from);

// returns number of items (.size)
size_t htab_size(const htab_t * t);
// returns number of buckets (.arr_size)
size_t htab_bucket_count(const htab_t * t);

// In table t finds item with "key" and
// - returns it if found
// - creates it and returns it if not found
htab_iterator_t htab_lookup_add(htab_t * t, const char *key);

// returns iterator marking first item
htab_iterator_t htab_begin(const htab_t * t);
// returns iterator marking (nonexistent) first item after end
htab_iterator_t htab_end(const htab_t * t);
// moves iterator to the next item in the table or to htab_end(t)
htab_iterator_t htab_iterator_next(htab_iterator_t it);

// tests if iterator points to valid item
inline bool htab_iterator_valid(htab_iterator_t it)
{
	return it.ptr!=NULL;
}
// compares two iterators for equality
inline bool htab_iterator_equal(htab_iterator_t it1, htab_iterator_t it2) 
{
  return it1.ptr==it2.ptr && it1.t == it2.t;
}

// returns key, target has to exist
const char * htab_iterator_get_key(htab_iterator_t it);
// returns value (.data), target has to exist
uint32_t htab_iterator_get_value(htab_iterator_t it);
// rewrites value (.data), target has to exist
uint32_t htab_iterator_set_value(htab_iterator_t it, uint32_t val);


// removes all items, table ends up empty
void htab_clear(htab_t * t);
// destructor - destroys table (calls htab_clear())
void htab_free(htab_t * t);

#endif // __HTABLE_H__
