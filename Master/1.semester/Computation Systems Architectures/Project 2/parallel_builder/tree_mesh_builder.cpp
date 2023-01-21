/**
 * @file    tree_mesh_builder.cpp
 *
 * @author  FULL NAME <xloren15@stud.fit.vutbr.cz>
 *
 * @brief   Parallel Marching Cubes implementation using OpenMP tasks + octree early elimination
 *
 * @date    16.11.2021
 **/

#include <iostream>
#include <math.h>
#include <limits>

#include "tree_mesh_builder.h"

TreeMeshBuilder::TreeMeshBuilder(unsigned gridEdgeSize)
    : BaseMeshBuilder(gridEdgeSize, "Octree")
{

}

unsigned TreeMeshBuilder::marchCubes(const ParametricScalarField &field)
{
    // Suggested approach to tackle this problem is to add new method to
    // this class. This method will call itself to process the children.
    // It is also strongly suggested to first implement Octree as sequential
    // code and only when that works add OpenMP tasks to achieve parallelism.
    Vec3_t<float> cubeOffset(0, 0, 0);
    unsigned totalTriangles = 0;

    #pragma omp parallel
    {
        #pragma omp single
        {
            totalTriangles = treeStep(mGridSize, cubeOffset, field);
        }
    }    

    return totalTriangles;
}

unsigned TreeMeshBuilder::treeStep(size_t cubeSize, const Vec3_t<float> &pos, const ParametricScalarField &field) {

    if (cubeSize <= 1)
        return buildCube(pos, field);

    unsigned totalTriangles = 0;
    size_t newCubeSize = cubeSize / 2;
    Vec3_t<float> middle((pos.x + newCubeSize) * mGridResolution,
                         (pos.y + newCubeSize) * mGridResolution,
                         (pos.z + newCubeSize) * mGridResolution);

    float fieldVal = cubeSize * mGridResolution * sqrt(3) / 2 + field.getIsoLevel();
    if(evaluateFieldAt(middle, field) > fieldVal)
        return 0;

    for(int x = 0; x < 2; x++)
    {
        for(int y = 0; y < 2; y++)
        {
            for(int z = 0; z < 2; z++)
            {                
                #pragma omp task shared(totalTriangles)
                {
                    Vec3_t<float> newPos(pos.x + x * newCubeSize,
                                         pos.y + y * newCubeSize,
                                         pos.z + z * newCubeSize);
                    unsigned triangles = treeStep(newCubeSize, newPos, field);

                    #pragma omp atomic
                    totalTriangles += triangles;
                }
            }
        }
    }

    #pragma omp taskwait
    return totalTriangles;
}

float TreeMeshBuilder::evaluateFieldAt(const Vec3_t<float> &pos, const ParametricScalarField &field)
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

void TreeMeshBuilder::emitTriangle(const BaseMeshBuilder::Triangle_t &triangle)
{   
    // Store generated triangle into vector (array) of generated triangles.
    // The pointer to data in this array is return by "getTrianglesArray(...)" call
    // after "marchCubes(...)" call ends.
    #pragma omp critical (emitTriangle)
    mTriangles.push_back(triangle);
}
