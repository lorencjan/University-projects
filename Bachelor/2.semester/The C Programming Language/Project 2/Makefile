#  File: Makefile
#  Solution: IJC-DU2
#  Date: 23.3.2019
#  Author: Jan Lorenc
#  Fakulta: Faculty of information technologies VUT
#  Description: Compiles .c .cc .h files of the project
#				Compiles tail program both in C and C++
#				Compiles wordcount program with both static and
#				dynamic/shared version of hash table library

CC=gcc
CXX=g++
CFLAGS=-std=c99 -Wall -Wextra -pedantic
CXXFLAGS=-std=c++11 -Wall -pedantic

all: tail tail2 wordcount wordcount-dynamic

# ************* 1. ukol
tail: tail.c
	$(CC) $(CFLAGS) -o $@ $^

tail2: tail2.cc
	$(CXX) $(CXXFLAGS) -o $@ $^

# ************* 2. ukol

LD_LIBRARY_PATH=.	# compiler would look for dynamic library in current folder
LDFLAGS=-shared

# theres too many modules not to put them in a variable
MODULES=htab_begin.c htab_bucket_count.c htab_clear.c htab_end.c htab_free.c htab_hash_func.c htab_init.c htab_iterator_get_key.c htab_iterator_get_value.c htab_iterator_next.c htab_iterator_set_value.c htab_lookup_add.c htab_move.c htab_size.c
STATIC_MODULES=htab_begin_a.o htab_bucket_count_a.o htab_clear_a.o htab_end_a.o htab_free_a.o htab_hash_func_a.o htab_init_a.o htab_iterator_get_key_a.o htab_iterator_get_value_a.o htab_iterator_next_a.o htab_iterator_set_value_a.o htab_lookup_add_a.o htab_move_a.o htab_size_a.o
SHARED_MODULES=htab_begin_so.o htab_bucket_count_so.o htab_clear_so.o htab_end_so.o htab_free_so.o htab_hash_func_so.o htab_init_so.o htab_iterator_get_key_so.o htab_iterator_get_value_so.o htab_iterator_next_so.o htab_iterator_set_value_so.o htab_lookup_add_so.o htab_move_so.o htab_size_so.o

# compiling modules
htab_a: $(MODULES) htab.h
	$(CC) $(CFLAGS) -c htab_begin.c -o htab_begin_a.o
	$(CC) $(CFLAGS) -c htab_bucket_count.c -o htab_bucket_count_a.o
	$(CC) $(CFLAGS) -c htab_clear.c -o htab_clear_a.o
	$(CC) $(CFLAGS) -c htab_end.c -o htab_end_a.o
	$(CC) $(CFLAGS) -c htab_free.c -o htab_free_a.o
	$(CC) $(CFLAGS) -c htab_hash_func.c -o htab_hash_func_a.o
	$(CC) $(CFLAGS) -c htab_init.c -o htab_init_a.o
	$(CC) $(CFLAGS) -c htab_iterator_get_key.c -o htab_iterator_get_key_a.o
	$(CC) $(CFLAGS) -c htab_iterator_get_value.c -o htab_iterator_get_value_a.o
	$(CC) $(CFLAGS) -c htab_iterator_next.c -o htab_iterator_next_a.o
	$(CC) $(CFLAGS) -c htab_iterator_set_value.c -o htab_iterator_set_value_a.o
	$(CC) $(CFLAGS) -c htab_lookup_add.c -o htab_lookup_add_a.o
	$(CC) $(CFLAGS) -c htab_move.c -o htab_move_a.o
	$(CC) $(CFLAGS) -c htab_size.c -o htab_size_a.o

htab_so: $(MODULES) htab.h	
	$(CC) $(CFLAGS) -fPIC -c htab_begin.c -o htab_begin_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_bucket_count.c -o htab_bucket_count_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_clear.c -o htab_clear_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_end.c -o htab_end_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_free.c -o htab_free_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_hash_func.c -o htab_hash_func_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_init.c -o htab_init_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_iterator_get_key.c -o htab_iterator_get_key_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_iterator_get_value.c -o htab_iterator_get_value_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_iterator_next.c -o htab_iterator_next_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_iterator_set_value.c -o htab_iterator_set_value_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_lookup_add.c -o htab_lookup_add_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_move.c -o htab_move_so.o
	$(CC) $(CFLAGS) -fPIC -c htab_size.c -o htab_size_so.o

# static library
libhtab.a: htab_a $(STATIC_MODULES)
	ar rcs libhtab.a $(STATIC_MODULES)

# dynamic library
libhtab.so: htab_so
	$(CC) $(LDFLAGS) -fPIC -o $@ $(SHARED_MODULES) -L$(LD_LIBRARY_PATH)

# compiles programs
wordcount: wordcount.c libhtab.a
	$(CC) $(CFLAGS) -o $@ -static $^

wordcount-dynamic: wordcount.c libhtab.so
	$(CC) $(CFLAGS) -L$(LD_LIBRARY_PATH) -Wl,-rpath=$(LD_LIBRARY_PATH) $^ -o $@ libhtab.so

# cleaning
clean_o:
	rm -f *.o
clean_libs:
	rm -f *.a *.so
