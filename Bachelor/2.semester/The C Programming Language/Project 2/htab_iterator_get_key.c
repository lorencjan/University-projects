//*  Soubor: htab_iterator_get_key.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 31.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function returns the key of given iterator

#include "htab.h"

const char * htab_iterator_get_key(htab_iterator_t it)
{
	return it.ptr->key;
}