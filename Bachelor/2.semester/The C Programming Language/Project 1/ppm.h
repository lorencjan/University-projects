/*
 *  Soubor: ppm.h
 *  Řešení: IJC-DU1 úkol b)
 *  Datum: 22.2.2019
 *  Autor: Jan Lorenc
 *  Fakulta: Fakulta informačních technologií VUT
 *  Přeloženo: gcc 7.3.0
 *  Popis: Knihovna obsahující deklarace struktur
 *		   a funkcí k načtení .ppm obrázku
 */

#ifndef _PPM_H
#define _PPM_H 1

//struktura obsahujici rozmery obrazku a jednotlive pixely o velikosti 3B
struct ppm
{
    unsigned xsize;
    unsigned ysize;
    char data[];    // RGB bajty, celkem 3*xsize*ysize
};

//dynamicky alokuje strukturu ppm, do ktere nacte obsah zouboru ppm
struct ppm * ppm_read(const char * filename);

//uvolni pamet naalokovanou funkci ppm_read
void ppm_free(struct ppm *p);

#endif