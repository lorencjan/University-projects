//*  Soubor: htab_hash_func.c
//*  Řešení: IJC-DU2
//*  Author: Jan Lorenc
//*	 Date: 30.3.2019
//*  Faculty: Faculty of information technologies VUT
//*  Compiler: gcc 7.3.0
//*  Desription: This module contains hash function for htab (hash table)

#include "htab.h"

#ifdef HASHTEST
//if defined during compilation, use my own hash function
uint32_t htab_hash_function(const char *str)
{
    uint32_t hash = 0;
    while(*str != '\0')
    {
        hash += (uint32_t)*str;
        str++;
    }
    return hash;
}
#else //otherwise use the one given
uint32_t htab_hash_function(const char *str)
{
    uint32_t h=0;     // musí mít 32 bitů
    const unsigned char *p;
    for(p=(const unsigned char*)str; *p!='\0'; p++)
        h = 65599*h + *p;
    return h;
}
#endif
