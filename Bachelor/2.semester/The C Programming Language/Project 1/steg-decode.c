/*
 *  Soubor: steg-decode.c
 *  Řešení: IJC-DU1 úkol b)
 *  Datum: 22.2.2019
 *  Autor: Jan Lorenc
 *  Fakulta: Fakulta informačních technologií VUT
 *  Přeloženo: gcc 7.3.0
 *  Popis: Hlavní zdrojový soubor úkolu b) s funkcí main()
 *		   pracující s ppm knihovnou a eratosthenovým sítem
 */

#include <stdio.h>
#include <stdbool.h>
#include "eratosthenes.c"	//obsahuje i bit_array.h
#include "ppm.c"
#include "error.c"

int main(int argc, char *argv[])
{
    //kontrola argumentu
    if(argc != 2)
    	error_exit("Program nema spravny pocet argumentu!\n"
    			   "Zadejte jeden argument a to obrazek formatu .ppm\n");

    //nacteni obrazku
    struct ppm* obrazek = ppm_read(argv[1]);
    if(!obrazek)
    	error_exit("Obrazek %s se nepodarilo nacist!\n", argv[1]);

    /*** dekodovani ***/
    
    //vytvoreni bitoveho pole o velikosti 8000*8000*3 viz limit ze zadani
    //do makra nelze davat promennou ziskanou az za behu, proto nelze 
    //zadat presnou velikost zobrazovanych dat
    bit_array_alloc(bitove_pole, LIMIT);
    Eratosthenes(bitove_pole);

    //potrebne promenne
    char znak = 0;
    int bit = 0, posun = 0;
    bool nulovy_znak = false;

    //hledame prvocisla od 19 do skutecne velikosti dat - nyni jiz muzeme
    ulong_t velikost = 3*obrazek->xsize*obrazek->ysize;
    for(ulong_t i = 19; i < velikost; i++)
    {
        if(bit_array_getbit(bitove_pole, i) == 0)
        {
            //ziskani bitu
            bit = (ulong_t)(obrazek->data[i] & 1);
            //nastaveni bitu
            if(bit == 1) 
                znak |= (1 << posun++ % CHAR_BIT);
            else
                znak &= ~(1 << posun++ % CHAR_BIT);

            //pokud je znak naplnen = nacetlo se 8 bitu, vytisknem ho
            if(posun == CHAR_BIT)
            {
                if(znak == '\0')
                {
                    nulovy_znak = true;
                    break;
                } 
                printf("%c", znak);
                posun = 0;          //je treba pak jeste vynulovat
            }
        }
    }
    printf("\n");

    //uvolneni pameti
    free(bitove_pole);
    ppm_free(obrazek);

    //pokud neni retezec ukoncen nulovym znakem, ukonci program chybou
    if(!nulovy_znak)
       error_exit("Retezec neni ukoncen nulovym znakem!\n");

    return 0;
}
