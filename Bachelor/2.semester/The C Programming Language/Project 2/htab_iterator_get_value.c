//*  Soubor: htab_iterator_get_value.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 31.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Function returns the value of given iterator

#include "htab.h"

uint32_t htab_iterator_get_value(htab_iterator_t it)
{
	return it.ptr->data;
}