//======== Copyright (c) 2018, FIT VUT Brno, All rights reserved. ============//
//
// Purpose:     Red-Black Self balancing Tree private interface
//
// $NoKeywords: $ivs_project_1 $red_black_tree_lib.h
// $Authors:    Filip Vaverka <ivaverka@fit.vutbr.cz>
//              David Grochol <igrochol@fit.vutbr.cz>
// $Date:       $2018-02-08
//============================================================================//
/**
 * @file red_black_tree.h
 * @author Filip Vaverka
 * @author David Grochol
 *
 * @brief Definice rozhrani binarniho stromu.
 */

#include <stdlib.h>

#pragma once

#ifndef RED_BLACK_TREE_LIB_H_
#define RED_BLACK_TREE_LIB_H_

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

/**
 * @brief The Color_t enum
 * Barva uzlu stromu.
 */
enum Color_t {
    RED = 0,
    BLACK
};

/**
 * @brief The Node_t struct
 * Struktura uzlu stromu.
 */
typedef struct Node_t {
    struct Node_t *pParent; ///< Ukazatel na rodice uzlu, nebo NULL v pripade korene.
    struct Node_t *pLeft;   ///< Ukazatel na leveho potomka uzlu, nebo NULL pro listovy uzel.
    struct Node_t *pRight;  ///< Ukazatel na praveho potomka uzlu, nebo NULL pro listovy uzel.
    int color;              ///< Barva uzlu (Color_t), tj. RED nebo BLACK.

    int key;                ///< Hodnota/klic tohoto uzlu.
} Node_t;

void BTCreate(Node_t **ppRoot);
void BTDestroy(Node_t **ppRoot);

int BTInsertNode(Node_t **ppRoot, int key, Node_t **ppOutNode);
void BTInsertNodeMany(Node_t **ppRoot, size_t count, const int *pKeys,
                      Node_t **ppOutNodes, int *pOutStates);
int BTDeleteNode(Node_t **ppRoot, int key);
Node_t *BTFindNode(Node_t *pRoot, int key);
void BTGetLeafNodes(Node_t *pRoot, size_t *pOutNodesCount, Node_t **ppOutNodes);
void BTGetAllNodes(Node_t *pRoot, size_t *pOutNodesCount, Node_t **ppOutNodes);
void BTGetNonLeafNodes(Node_t *pRoot, size_t *pOutNodesCount, Node_t **ppOutNodes);

#ifdef __cplusplus
}
#endif // __cplusplus

#endif // RED_BLACK_TREE_LIB_H_
