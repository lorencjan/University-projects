#  Soubor: Makefile
#  Řešení: IJC-DU1
#  Datum: 22.2.2019
#  Autor: Jan Lorenc
#  Fakulta: Fakulta informačních technologií VUT
#  Přeloženo: gcc 7.3.0
#  Popis: Jedná se o makefile, který přeloží a případně i spustí *.c a *.h soubory

CC=gcc
CFLAGS=-O2 -std=c11 -Wall -pedantic

all: primes steg-decode

run: primes steg-decode
	time ./primes 
	time ./primes-i 

primes: primes.c bit_array.h eratosthenes.c error.c
	$(CC) $(CFLAGS) primes.c -lm -o primes
	$(CC) $(CFLAGS) -DUSE_INLINE primes.c -lm -o primes-i

steg-decode: steg-decode.c error.h error.c ppm.c ppm.h eratosthenes.c bit_array.h
	$(CC) $(CFLAGS) steg-decode.c -lm -o steg-decode
