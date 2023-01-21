/**
 * @file BatchMandelCalculator.cc
 * @author Jan Lorenc <xloren15@stud.fit.vutbr.cz>
 * @brief Implementation of Mandelbrot calculator that uses SIMD paralelization over small batches
 * @date 25.10.2021
 */

#include <iostream>
#include <string>
#include <vector>
#include <algorithm>

#include <immintrin.h>
#include <stdlib.h>
#include <stdexcept>

#include "BatchMandelCalculator.h"

BatchMandelCalculator::BatchMandelCalculator (unsigned matrixBaseSize, unsigned limit) :
	BaseMandelCalculator(matrixBaseSize, limit, "BatchMandelCalculator")
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

BatchMandelCalculator::~BatchMandelCalculator() {
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


int * BatchMandelCalculator::calculateMandelbrot () {
	int batchSize = 32;
	for (int iBatch = 0; iBatch < height / batchSize; iBatch++)
	{
		const int iGlobal = iBatch * batchSize;
		for(int jBatch = 0; jBatch < width / batchSize; jBatch++)
		{
			const int jGlobal = jBatch * batchSize;
			for (int i = 0; i < batchSize; i++)
			{
				const int idxBase = width * (iGlobal + i) + jGlobal;
				for (int k = 0; k < limit; k++)
				{
					int sum = 0;

					#pragma omp simd reduction(+:sum)
					for(int j = 0; j < batchSize; j++)
					{
						float xb = xBase[idxBase + j];
						float yb = yBase[idxBase + j];
						float x = xMem[idxBase + j];
						float y = yMem[idxBase + j];
						float xx = x * x;
						float yy = y * y;
						float condSum = xx + yy;
						
						sum = condSum > 4.0f ? sum + 1 : sum;
						data[idxBase + j] = condSum > 4.0f ? data[idxBase + j] : k + 1;
						xMem[idxBase + j] = condSum > 4.0f ? x : xx - yy + xb;
						yMem[idxBase + j] = condSum > 4.0f ? y : 2.0f * x * y + yb;
					}

					if (sum == batchSize)
						break;
				}
			}
		}
	}
	return data;
}