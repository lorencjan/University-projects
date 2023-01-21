//*  Soubor: htab_free.h
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 30.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: Completely removes hash table

#include "htab.h"

void htab_free(htab_t * t)
{
    htab_clear(t);
	free(t);
}
