/*
 *  Soubor: error.h
 *  Řešení: IJC-DU1 úkol b)
 *  Datum: 22.2.2019
 *  Autor: Jan Lorenc
 *  Fakulta: Fakulta informačních technologií VUT
 *  Přeloženo: gcc 7.3.0
 *  Popis: Knihovna obsahující deklarace chybových funkcí
 */

#ifndef _ERROR_H
#define _ERROR_H 1

#include<stdarg.h>
#include<stdio.h>
#include<stdlib.h>

//vypise chybove hlaseni ve formatu CHYBA: +fmt
void warning_msg(const char *fmt, ...);

//vypise chybove hlaseni ve formatu CHYBA: +fmt
//a ukonci program exitem
void error_exit(const char *fmt, ...);

#endif