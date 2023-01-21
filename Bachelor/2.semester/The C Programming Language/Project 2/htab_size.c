//*  Soubor: htab_size.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 30.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function returns size of the hash table

#include "htab.h"

size_t htab_size(const htab_t * t)
{
	return t->size;
}