/*
 *  Soubor: bit_array.h
 *  Řešení: IJC-DU1 úkol a)
 *  Datum: 22.2.2019
 *  Autor: Jan Lorenc
 *  Fakulta: Fakulta informačních technologií VUT
 *  Přeloženo: gcc 7.3.0
 *  Popis: Knihovna obsahující makra a inline funkce
 *		   pracující s bitovým polem
 */

#ifndef _BIT_ARRAY_H
#define _BIT_ARRAY_H 1

#include <limits.h>
#include "error.h"

//zkratka unsigned long = 8bytu = 64bitu (vetsinou a i na mém Ubuntu)
typedef unsigned long ulong_t;      
//zkratka pro ulong pole pro bity
typedef ulong_t bit_array_t[];

/* Vytvoreni statickeho pole o zadane velikosti v bitech
	velikost/(sizeof(ulong_t)*8) = velikost / 64 = pocet ulongu do pole
	velikost%(sizeof(ulong_t)*8) = prebyvajici bity
	+1 - prvni prvek (ulong) obsahujici pocet bitu
*/
#define bit_array_create(jmeno_pole, velikost) \
	_Static_assert(velikost > 0, "Bitove pole musi obsahovat alespon 1 bit!"); \
	ulong_t jmeno_pole[((velikost/(sizeof(ulong_t)*CHAR_BIT))\
	+(velikost % (sizeof(ulong_t)*CHAR_BIT) > 0 ? 1 : 0))+1] = {0};\
	jmeno_pole[0] = velikost

// Vytvoreni dynamickeho pole o zadane velikosti v bitech
#define bit_array_alloc(jmeno_pole, velikost) \
		_Static_assert(velikost > 0, "Bitove pole musi obsahovat alespon 1 bit!"); \
		ulong_t *jmeno_pole = malloc(sizeof(ulong_t) * velikost / (sizeof(ulong_t)*CHAR_BIT) + \
							  		((velikost % (sizeof(ulong_t)*CHAR_BIT) > 0) ? 1 : 0)+1); \
		if(jmeno_pole == NULL) \
			error_exit("bit_array_alloc: Chyba alokace paměti\n"); \
		jmeno_pole[0] = velikost; \
		for(unsigned long i = 1; i < velikost / (sizeof(ulong_t)*CHAR_BIT) + ((velikost % (sizeof(ulong_t)*CHAR_BIT) > 0) ? 1 : 0)+1; i++)\
			jmeno_pole[i] = 0; 

//dealokace dynamicky alokovaneho pole
#define bit_array_free(jmeno_pole) free(jmeno_pole)


/*** pokud prelozeno s USE_INLINE pouziji se inline funkce, jinak makra ***/
#ifdef USE_INLINE
	//ziskani poctu prvku v poli z prvniho policka
	extern inline ulong_t bit_array_size(bit_array_t jmeno_pole) { return jmeno_pole[0];}
	
	//nastaveni hodnoty bitu na dane pozici
	extern inline void bit_array_setbit(bit_array_t jmeno_pole, ulong_t index, ulong_t vyraz)
	{	
        if(index < 0 || index > bit_array_size(jmeno_pole))
        	error_exit("bit_array_getbit: Index %lu mimo rozsah 0..%lu\n",
               			index, bit_array_size(jmeno_pole));

        int pocet_bitu = sizeof(ulong_t)*CHAR_BIT;
        int i = index / pocet_bitu +1;     //ziskame index v poli ... +1 kvuli prvnimu prvku pole
        int offset = index % pocet_bitu;   //ziskame pozici bitu na indexu pole
        if(vyraz)
      		jmeno_pole[i] |= ((ulong_t)1 << offset);  //pokud je vyraz 1 operator 'or' vzdy nasadi 1
      	else 
      		jmeno_pole[i] &= ~((ulong_t)1 << offset); //pokud je vyraz 0 operator 'and' vzdy nasadi 0
	}

	//ziskani hodnoty daneho bitu
	extern inline int bit_array_getbit(bit_array_t jmeno_pole, ulong_t index)
	{
		if(index < 0 || index > bit_array_size(jmeno_pole))
        {
        	error_exit("bit_array_getbit: Index %lu mimo rozsah 0..%lu\n",
               			index, bit_array_size(jmeno_pole)); 
        	return 0;
        }
        int pocet_bitu = sizeof(ulong_t)*CHAR_BIT;
	    int i = index / pocet_bitu + 1;
	    int offset = index % pocet_bitu;
		return (jmeno_pole[i] & ((ulong_t)1 << offset)) != 0;
	}
#else
	//ziskani poctu prvku v poli z prvniho policka
	#define bit_array_size(jmeno_pole) jmeno_pole[0]

	//nastaveni hodnoty bitu na dane pozici
	//vyuziti smycky do...while na alokaci lokalnich promennych
	#define bit_array_setbit(jmeno_pole, index, vyraz) \
			do \
			{ \
				if(index < 0 || index > bit_array_size(jmeno_pole)) \
	        		error_exit("bit_array_setbit: Index %lu mimo rozsah 0..%lu\n", \
               					index, bit_array_size(jmeno_pole)); \
	        	\
		        int pocet_bitu = sizeof(ulong_t)*CHAR_BIT;\
		        int i = index / pocet_bitu +1; \
		        int offset = index % pocet_bitu; \
		        if(vyraz) \
		      		jmeno_pole[i] |= ((ulong_t)1 << offset); \
		      	else \
		      		jmeno_pole[i] &= ~((ulong_t)1 << offset); \
			} \
			while(0)

	//ziskani hodnoty daneho bitu, makro vsak nemuze vracet hodnotu na return
	//proto je treba dlouheho neprehledneho vyrazu...jinak ekvivalentni s inline funkci
	#define bit_array_getbit(jmeno_pole, index) \
			(index < 0 || index > bit_array_size(jmeno_pole)) ? \
				error_exit("bit_array_getbit: Index %lu mimo rozsah 0..%lu\n", \
               				index, bit_array_size(jmeno_pole)), 0 : \
			(jmeno_pole[index/(sizeof(ulong_t)*CHAR_BIT)+1] & \
			((ulong_t)1 << (index%(sizeof(ulong_t)*CHAR_BIT))))
#endif
#endif
