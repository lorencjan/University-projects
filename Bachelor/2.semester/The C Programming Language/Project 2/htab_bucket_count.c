//*  Soubor: htab_bucked_count.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 30.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function returns number of buckets in the table

#include "htab.h"

size_t htab_bucket_count(const htab_t * t)
{
	return t->arr_size;
}