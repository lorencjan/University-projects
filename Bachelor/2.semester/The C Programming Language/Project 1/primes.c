/*
 *  Soubor: primes.c
 *  Řešení: IJC-DU1 úkol a)
 *  Datum: 22.2.2019
 *  Autor: Jan Lorenc
 *  Fakulta: Fakulta informačních technologií VUT
 *  Přeloženo: gcc 7.3.0
 *  Popis: Hlavní zdrojový soubor úkolu a) s funkcí main()
 *		   pracující s bitovým polem a eratosthenovým sítem
 */

#include <stdio.h>
#include "eratosthenes.c"	//obsahuje i bit_array.h
#include "error.c"

int main()
{
    //bit_array_create(pole, 123000000LU);
    bit_array_alloc(pole, 123000000LU);
	
	Eratosthenes(pole);
	
	int count = 0;
	ulong_t pomocne_pole[10];
	for(ulong_t i = 123000000-1; i > 0 && count < 10; i--)
	{
		if(bit_array_getbit(pole,i) == 0)
		{
			pomocne_pole[9-count] = i;
			count++;
		}
	}
	//vypsat prvocisla mame vzestupne, proto pomocne pole na poradi
	for(int i = 0; i < 10; i++)
		printf("%lu\n", pomocne_pole[i]);
	
	bit_array_free(pole);
    return 0;
}
