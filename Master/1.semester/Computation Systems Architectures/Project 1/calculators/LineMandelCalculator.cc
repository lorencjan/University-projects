/**
 * @file LineMandelCalculator.cc
 * @author Jan Lorenc <xloren15@stud.fit.vutbr.cz>
 * @brief Implementation of Mandelbrot calculator that uses SIMD paralelization over lines
 * @date 25.10.2021
 */
#include <iostream>
#include <string>
#include <vector>
#include <algorithm>

#include <immintrin.h>
#include <stdlib.h>


#include "LineMandelCalculator.h"


LineMandelCalculator::LineMandelCalculator (unsigned matrixBaseSize, unsigned limit) :
	BaseMandelCalculator(matrixBaseSize, limit, "LineMandelCalculator")
{
	int alignment = 64;
	data = (int *)(_mm_malloc(height * width * sizeof(int), alignment));
	xBase = (float *)(_mm_malloc(height * width * sizeof(int), alignment));
	yBase = (float *)(_mm_malloc(height * width * sizeof(int), alignment));
	xMem = (float *)(_mm_malloc(height * width * sizeof(int), alignment));
	yMem = (float *)(_mm_malloc(height * width * sizeof(int), alignment));
	
	for(int i = 0; i < height; i++)
	{
		int idxBase = width * i;

		#pragma omp for simd
		for(int j = 0; j < width; j++)
		{
			data[idxBase + j] = 0;
			xBase[idxBase + j] = x_start + j * dx;
			yBase[idxBase + j] = y_start + i * dy;
			xMem[idxBase + j] = xBase[idxBase + j];
			yMem[idxBase + j] = yBase[idxBase + j];
		}
	}
}

LineMandelCalculator::~LineMandelCalculator() {
	_mm_free(data);
	_mm_free(xBase);
	_mm_free(yBase);
	_mm_free(xMem);
	_mm_free(yMem);
	data = NULL;
    xBase = NULL;
    yBase = NULL;
    xMem = NULL;
    yMem = NULL;
}


int * LineMandelCalculator::calculateMandelbrot () {
	for (int i = 0; i < height; i++)
	{
		for (int k = 0; k < limit; k++)
		{
			int sum = 0;

			#pragma omp simd reduction(+:sum)
			for(int j = 0; j < width; j++)
			{
				int idx = width * i + j;
				int val = data[idx];
				float xb = xBase[idx];
				float yb = yBase[idx];
				float x = xMem[idx];
				float y = yMem[idx];
				float xx = x * x;
				float yy = y * y;
				bool cont = xx + yy <= 4.0f;
				
				sum = !cont ? sum + 1 : sum;
				data[idx] = cont ? k + 1 : val;
				xMem[idx] = xx - yy + xb;
				yMem[idx] = 2.0f * x * y + yb;
			}

			if (sum == width)
				break;
		}
	}
	return data;
}
