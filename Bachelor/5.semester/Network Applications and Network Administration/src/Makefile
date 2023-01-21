#  File: Makefile
#  Solution: ISA - project - Monitoring of SSL connections
#  Date: 17.10.2020
#  Author: Jan Lorenc
#  Faculty: Faculty of information technologies VUT
#  Desription: Makefile for the ssl sniffer program

CC=g++
CFLAGS=-std=c++17 -Wall -Wextra -Werror -pedantic
LIBS=-lpcap

sslsniff: sslsniff.cpp args.h args.cpp error.h error.cpp connection.h connection.cpp sniffer.h sniffer.cpp
	$(CC) $(CFLAGS) -o $@ $^ $(LIBS)
