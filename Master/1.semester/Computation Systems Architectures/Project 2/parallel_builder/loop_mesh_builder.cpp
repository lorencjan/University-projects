/**
 * @file    loop_mesh_builder.cpp
 *
 * @author  FULL NAME <xloren15@stud.fit.vutbr.cz>
 *
 * @brief   Parallel Marching Cubes implementation using OpenMP loops
 *
 * @date    16.11.2021
 **/

#include <iostream>
#include <math.h>
#include <limits>

#include "loop_mesh_builder.h"

LoopMeshBuilder::LoopMeshBuilder(unsigned gridEdgeSize)
    : BaseMeshBuilder(gridEdgeSize, "OpenMP Loop")
{

}

unsigned LoopMeshBuilder::marchCubes(const ParametricScalarField &field)
{
    // 1. Compute total number of cubes in the grid.
    size_t totalCubesCount = mGridSize*mGridSize*mGridSize;

    unsigned totalTriangles = 0;

    // 2. Loop over each coordinate in the 3D grid.
    #pragma omp parallel for reduction(+: totalTriangles) schedule(static)
    for(size_t i = 0; i < totalCubesCount; ++i)
    {
        // 3. Compute 3D position in the grid.
        Vec3_t<float> cubeOffset( i % mGridSize,
                                 (i / mGridSize) % mGridSize,
                                  i / (mGridSize*mGridSize));

        // 4. Evaluate "Marching Cube" at given position in the grid and
        //    store the number of triangles generated.
        unsigned triangles = buildCube(cubeOffset, field);

        #pragma omp atomic
        totalTriangles += triangles;
    }

    // 5. Return total number of triangles generated.
    return totalTriangles;
}

float LoopMeshBuilder::evaluateFieldAt(const Vec3_t<float> &pos, const ParametricScalarField &field)
{
    // 1. Store pointer to and number of 3D points in the field
    //    (to avoid "data()" and "size()" call in the loop).
    const Vec3_t<float> *pPoints = field.getPoints().data();
    const unsigned count = unsigned(field.getPoints().size());

    float value = std::numeric_limits<float>::max();

    // 2. Find minimum square distance from points "pos" to any point in the
    //    field.
    for(unsigned i = 0; i < count; ++i)
    {
        float distanceSquared  = (pos.x - pPoints[i].x) * (pos.x - pPoints[i].x);
        distanceSquared       += (pos.y - pPoints[i].y) * (pos.y - pPoints[i].y);
        distanceSquared       += (pos.z - pPoints[i].z) * (pos.z - pPoints[i].z);

        // Comparing squares instead of real distance to avoid unnecessary
        // "sqrt"s in the loop.
        value = std::min(value, distanceSquared);
    }

    // 3. Finally take square root of the minimal square distance to get the real distance
    return sqrt(value);
}

void LoopMeshBuilder::emitTriangle(const BaseMeshBuilder::Triangle_t &triangle)
{
    // Store generated triangle into vector (array) of generated triangles.
    // The pointer to data in this array is return by "getTrianglesArray(...)" call
    // after "marchCubes(...)" call ends.
    #pragma omp critical (emitTriangle)
    mTriangles.push_back(triangle);
}
