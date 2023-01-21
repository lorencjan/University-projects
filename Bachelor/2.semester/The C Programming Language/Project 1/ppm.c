/*
 *  Soubor: ppm.c
 *  Řešení: IJC-DU1 úkol b)
 *  Datum: 22.2.2019
 *  Autor: Jan Lorenc
 *  Fakulta: Fakulta informačních technologií VUT
 *  Přeloženo: gcc 7.3.0
 *  Popis: Zdrojový soubor implementující funkce z ppm.h
 */

#include <stdio.h>
#include <stdlib.h>
#include "ppm.h"

//omezeni velikosti zobrazovanych dat nastaveno na 8000*8000*3 dle zadani
#define LIMIT 192000000LU 

struct ppm * ppm_read(const char * filename)
{
	//nacteni obrazku
	FILE *ppm_image = fopen(filename, "r");
	if(!ppm_image)
	{
		warning_msg("Nepodarilo se otevrit soubor %s!\n", filename);
		goto vratit;
	}

	//nacteni velikosti
	unsigned size_x, size_y;
	if(fscanf(ppm_image, "P6%u\n %u\n255\n", &size_x, &size_y) != 2)
	{
		warning_msg("Nepodarilo se nacist velikost obrazku %s!\n", filename);
		goto zavrit;
	}

	//kontrola velikosti s limitem
	unsigned long velikost = 3 * size_x * size_y;
	if(velikost > LIMIT)
	{
		warning_msg("Obrazek %s je prilis velky pro zpracovani!\n", filename);
		goto zavrit;
	}

	//alokace pameti pro data
	struct ppm* obrazek = malloc(sizeof(struct ppm) + velikost);
	if(obrazek == NULL)
	{
		warning_msg("Nepodarilo se naalokovat pamet pro obrazek %s!\n", filename);
		goto zavrit;
	}

	//nacteni dat do struktury
	obrazek->xsize = size_x;
	obrazek->ysize = size_y;
	//do pole data ctu po jednom bytu celkem 3*size_x*size_y bytu z ppm obr
	unsigned long loadedBytes = fread(&obrazek->data, sizeof(char), velikost, ppm_image);
	//kontrola, jestli nedoslo pri cteni k chybe
	if(ferror(ppm_image))
	{
		warning_msg("Doslo k chybe pri nacitani obrazku %s!\n", filename);
		goto uvolnit;
	}
	//kontrola, jestli jsme nacetli vsechno
	if(loadedBytes != velikost)
	{
		warning_msg("Nacetlo se pouze %lu / %lu bytu!\n", loadedBytes, velikost);
		goto uvolnit;
	}
	//nasledujici znak by mel byt EOF a pocet nactenych by se mel rovnat velikosti
	if(fgetc(ppm_image) != EOF)
	{
		warning_msg("Obrazek %s se nenacetl celt!\n", filename);
		goto uvolnit;
	}

	fclose(ppm_image);
	return obrazek;

	//navesti pro goto chain
	uvolnit: free(obrazek);
	zavrit:	 fclose(ppm_image);
	vratit:  return NULL;
}


void ppm_free(struct ppm *p)
{
	free(p);
}