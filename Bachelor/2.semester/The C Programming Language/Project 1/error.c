/*
 *  Soubor: error.c
 *  Řešení: IJC-DU1 úkol b)
 *  Datum: 22.2.2019
 *  Autor: Jan Lorenc
 *  Fakulta: Fakulta informačních technologií VUT
 *  Přeloženo: gcc 7.3.0
 *  Popis: Hlavní zdrojový soubor implementující funkce z error.h
 */

#include "error.h"

void warning_msg(const char *fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	fprintf(stderr,"CHYBA: ");
	vfprintf(stderr, fmt, args);
	va_end(args);
}

void error_exit(const char *fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	fprintf(stderr,"CHYBA: ");
	vfprintf(stderr, fmt, args);
	va_end(args);
	exit(1);
}