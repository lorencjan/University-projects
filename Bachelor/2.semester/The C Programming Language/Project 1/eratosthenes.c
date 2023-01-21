/*
 *  Soubor: eratosthenes.c
 *  Řešení: IJC-DU1 úkol a) i b)
 *  Datum: 22.2.2019
 *  Autor: Jan Lorenc
 *  Fakulta: Fakulta informačních technologií VUT
 *  Přeloženo: gcc 7.3.0
 *  Popis: Zdrojový soubor implementující funkci na
 *		   na vypocet Eratosthenova sita
 */

#include <stdio.h>
#include <math.h>
#include "bit_array.h"

void Eratosthenes(bit_array_t bitove_pole)
{
	ulong_t pocet_bitu = bit_array_size(bitove_pole);
	double limit = sqrt(pocet_bitu);
	
	bit_array_setbit(bitove_pole, 0, 1);
	bit_array_setbit(bitove_pole, 1, 1);
	for(ulong_t i = 2; i < limit; i++)
		if(bit_array_getbit(bitove_pole, i) == 0)
			for(ulong_t j = i*i; j < pocet_bitu; j+=i)
				bit_array_setbit(bitove_pole, j, 1);
}