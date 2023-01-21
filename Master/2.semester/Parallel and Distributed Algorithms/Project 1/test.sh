#!/bin/bash

#pocet cisel bud zadam nebo 10 :)
if [ $# -lt 1 ];then 
    numbers=8;
else
    numbers=$1;
fi;

#preklad cpp zdrojaku
mpic++ --prefix /usr/local/share/OpenMPI -o oems oems.cpp

#vyrobeni souboru s random cisly
dd if=/dev/random bs=1 count=$numbers of=numbers

#spusteni
mpirun --prefix /usr/local/share/OpenMPI -oversubscribe -np 19 oems

#uklid
rm -f oems numbers