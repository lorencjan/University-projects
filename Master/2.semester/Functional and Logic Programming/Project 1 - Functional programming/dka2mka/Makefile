# Solution: FLP - project 1 - dka-2-mka
# File: Makefile
# Author: Jan Lorenc
# Login: xloren15
# University: Brno University of technology: Faculty of information technology
# Date: 21.2.2022

NAME := flp21-fun
ARGS := -i -t

build: $(NAME)

$(NAME): src/*.hs
	ghc --make -Wall -o $@ $^

run: $(NAME)
	./$< $(ARGS)

clean:
	rm -f ./src/*.o ./src/*.hi $(NAME)
