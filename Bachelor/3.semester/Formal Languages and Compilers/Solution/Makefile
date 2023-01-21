CC=gcc
CFLAGS=-std=c11 -Wall -Werror -Wextra -pedantic

all: IFJ2019

IFJ2019: main.o list.h list.o stack.h stack.o symtable.h dString.h dString.o symtable.o scanner.h scanner.o parser.h parser.o generator.o generator.h
	$(CC) list.o stack.o symtable.o dString.o scanner.o parser.o generator.o main.o -o IFJ2019 $(CFLAGS)

list.o: list.c scanner.h
	$(CC) -c -o list.o list.c $(CFLAGS)

%.o: %.c
	$(CC) -c -o $@ $< $(CFLAGS)

clean:
	rm -rf *.o *.out test-list test-stack test-symtable test-string test-scanner test-builtins parser_test IFJ2019 vgcore*

test: IFJ2019
	chmod 755 testy/run_all_tests.sh
	./testy/run_all_tests.sh
