//======== Copyright (c) 2018, FIT VUT Brno, All rights reserved. ============//
//
// Purpose:     Red-Black Self balancing Tree public interface
//
// $NoKeywords: $ivs_project_1 $red_black_tree.h
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

#pragma once

#ifndef RED_BLACK_TREE_H_
#define RED_BLACK_TREE_H_

#include <utility>
#include <vector>

#include "red_black_tree_lib.h"

/**
 * @brief The BinaryTree class
 * Samo-vyvazujici se binarni strom, kde hodnoty jsou ulozeny pouze ve vnitrnich
 * uzlech (uzly krom listu). Automaticke vyvazovani stromu je umozneno rozdelenim
 * uzlu do dvou mnozin (cervene a cerne).
 */
class BinaryTree
{
public:
    /**
     * @brief The Color_t enum
     * Barva uzlu stromu. Mimo tridu BinaryTree dostupne jako: "BinaryTree::RED".
     */
    enum Color_t {
        RED = ::Color_t::RED,
        BLACK = ::Color_t::BLACK
    };

    /**
     * @brief The Node_t struct
     * Struktura uzlu stromu.
     */
    typedef ::Node_t Node_t;
    /*{
        Node_t *pParent;    ///< Ukazatel na rodice uzlu, nebo NULL v pripade korene.
        Node_t *pLeft;      ///< Ukazatel na leveho potomka uzlu, nebo NULL pro listovy uzel.
        Node_t *pRight;     ///< Ukazatel na praveho potomka uzlu, nebo NULL pro listovy uzel.
        Color_t color;      ///< Barva uzlu (Color_t), tj. RED nebo BLACK.

        int key;            ///< Hodnota/klic tohoto uzlu.
    };*/

    /**
     * @brief BinaryTree
     * Konstruktor prazdneho binarniho stromu.
     */
    BinaryTree() {
        BTCreate(&m_pRoot);
    }

    /**
     * @brief BinaryTree
     * Destruktor binarniho stromu, odstrani vsechny uzly v nem obsazene.
     */
    ~BinaryTree() {
        BTDestroy(&m_pRoot);
    }

    /**
     * @brief InsertNode
     * Pokusi se vlozit novy uzel s hodnotou "key", nebo nalezne jiz existujici
     * uzel s touto hodnotou (hodnoty uzlu musi byt unikatni).
     * @param key Nova hodnota vkladana do stromu.
     * @return Vraci dvojici (true, ukazatel na novy uzel), pokud uzel jeste
     * neexistoval, nebo (false, ukazatel na existujici uzel), pokud uzel s danou
     * hodnotou jiz existuje.
     */
    std::pair<bool, Node_t *> InsertNode(int key) {

        Node_t *pNewNode = NULL;
        bool bIsNew = BTInsertNode(&m_pRoot, key, &pNewNode);

        return std::make_pair(bIsNew, pNewNode);
    }

    /**
     * @brief InsertNodes
     * Pokusi se vlozit uzly ze seznamu "keys", nebo nalezne, ty ktere jiz existuji.
     * @param keys        Seznam klicu, ktere maji byt vlozeny do stromu.
     * @param outNewNodes Vystupni pole ktere pro kazdou hodnotu v "keys" obsahuje
     *                    dvojici (true, ukazatel na novy uzel), pokud byl uzel
     *                    vlozen jako novy, nebo (false, ukazatel na extistujici uzel),
     *                    pokud uzel s danou hodnotou jiz existuje.
     */
    void InsertNodes(const std::vector<int> &keys,
                     std::vector<std::pair<bool, Node_t *> > &outNewNodes) {
        outNewNodes.clear();

        std::vector<Node_t *> newNodes(keys.size());
        std::vector<int> newNodesState(keys.size());
        BTInsertNodeMany(&m_pRoot, keys.size(), &keys[0], &newNodes[0],
                &newNodesState[0]);

        for(size_t i = 0; i < keys.size(); ++i)
            outNewNodes.push_back(std::make_pair(newNodesState[i] != 0, newNodes[i]));
    }

    /**
     * @brief DeleteNode
     * Pokusi se odstranit uzel s hodnotou "key"
     * @param key Hodnota uzlu, ktery ma byt odstranen.
     * @return Vraci true, pokud je uzel nalezen a odstranen, jinak false.
     */
    bool DeleteNode(int key) {
        return BTDeleteNode(&m_pRoot, key);
    }

    /**
     * @brief FindNode
     * Pokusi se nalezt uzel s hodnotou "key"
     * @param key Hodnota hledaneho uzlu
     * @return Vraci ukazatel na nalezeny uzel, nebo NULL, pokud takovy uzel
     * neni nalezen.
     */
    Node_t *FindNode(int key) const {
        return BTFindNode(m_pRoot, key);
    }

    /**
     * @brief GetLeafNodes
     * Projde cely strom a sestavi pole listovych uzlu, ktere nemaji potomky.
     * @param outLeafNodes Pole naplnene ukazateli na listove uzly (pole je
     * nejdrive vyprazdneno).
     */
    void GetLeafNodes(std::vector<Node_t *> &outLeafNodes) {
        size_t count = 0;

        BTGetLeafNodes(m_pRoot, &count, NULL);

        outLeafNodes.resize(count);

        if(count > 0)
            BTGetLeafNodes(m_pRoot, &count, &outLeafNodes[0]);
    }

    /**
     * @brief GetAllNodes
     * Projde cely strom a sestavi pole ukazatelu na vsechny uzly ve stromu.
     * @param outAllNodes Pole naplnene ukazateli na uzly ve stromu (pole je
     * nejdrive vyprazdneno).
     */
    void GetAllNodes(std::vector<Node_t *> &outAllNodes) {
        size_t count = 0;

        BTGetAllNodes(m_pRoot, &count, NULL);

        outAllNodes.resize(count);

        if(count > 0)
            BTGetAllNodes(m_pRoot, &count, &outAllNodes[0]);
    }

    /**
     * @brief GetNonLeafNodes
     * Projde cely strom a sestavi pole ukazatelu na vsechny NE-listove
     * uzly stromu (tedy s alespon 1 potomkem).
     * @param outNonLeafNodes Pole naplnene ukazateli na NE-listove uzly
     * ve stromu (pole je nejdrive vyprazdneno).
     */
    void GetNonLeafNodes(std::vector<Node_t *> &outNonLeafNodes) {
        size_t count = 0;

        BTGetNonLeafNodes(m_pRoot, &count, NULL);

        outNonLeafNodes.resize(count);

        if(count > 0)
            BTGetNonLeafNodes(m_pRoot, &count, &outNonLeafNodes[0]);
    }

    /**
     * @brief GetRoot
     * @return Vraci ukazatel na korenovy uzel stromu, nebo NULL, pokud je strom prazdny.
     */
    Node_t *GetRoot() { return m_pRoot; }

protected:
    Node_t *m_pRoot;    ///< Ukazatel na koren stromu.
};


#endif // RED_BLACK_TREE_H_
