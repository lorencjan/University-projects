#!/bin/bash
#PBS -q qexp
#PBS -A dd-21-22
#PBS -l select=1:ncpus=36,vtune=2020_update3-GCC 
#PBS -l walltime=0:15:00
#PBS -m e
#PBS -N AVS-vtune


cd $PBS_O_WORKDIR
ml CMake intel-compilers/2021.1.2 VTune/2020_update3-GCC 

rm -rf build_vtune
mkdir build_vtune
cd build_vtune

CC=icc CXX=icpc cmake ..
make

for threads in 18 32; do
    for builder in "ref" "loop" "tree"; do
        rm -rf vtune-${builder}-${threads}
        vtune -collect threading -r vtune-${builder}-${threads} -app-working-dir . -- ./PMC --builder ${builder} -t ${threads} --grid 128 ../data/bun_zipper_res3.pts
    done
done