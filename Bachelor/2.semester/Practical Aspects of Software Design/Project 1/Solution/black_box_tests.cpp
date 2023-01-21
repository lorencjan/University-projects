//======== Copyright (c) 2017, FIT VUT Brno, All rights reserved. ============//
//
// Purpose:     Red-Black Tree - public interface tests
//
// $NoKeywords: $ivs_project_1 $black_box_tests.cpp
// $Author:     JAN LORENC <xloren15@stud.fit.vutbr.cz>
// $Date:       $2019-02-12
//============================================================================//
/**
 * @file black_box_tests.cpp
 * @author JAN LORENC
 *
 * @brief Implementace testu binarniho stromu.
 */

#include <vector>

#include "gtest/gtest.h"

#include "red_black_tree.h"

 //============================================================================//
 // ** ZDE DOPLNTE TESTY **
 //
 // Zde doplnte testy Red-Black Tree, testujte nasledujici:
 // 1. Verejne rozhrani stromu
 //    - InsertNode/DeleteNode a FindNode
 //    - Chovani techto metod testuje pro prazdny i neprazdny strom.
 // 2. Axiomy (tedy vzdy platne vlastnosti) Red-Black Tree:
 //    - Vsechny listove uzly stromu jsou *VZDY* cerne.
 //    - Kazdy cerveny uzel muze mit *POUZE* cerne potomky.
 //    - Vsechny cesty od kazdeho listoveho uzlu ke koreni stromu obsahuji
 //      *STEJNY* pocet cernych uzlu.
 //============================================================================//

using namespace std;

 /** testy pro prazdny strom **/

 /**pro nektere funkci zkusime N (10) hodnot -
	teoreticky ne nutne, prakticky je vsak lepsi otestovat vice nez jednu hodnotu*/
#define N 10

class EmptyTree : public ::testing::Test
{
protected:
	BinaryTree *binaryTree;
	virtual void SetUp()
	{
		binaryTree = new BinaryTree();
	}
	virtual void TearDown() override
	{
		delete binaryTree;
	}
};

//test vlozeni
TEST_F(EmptyTree, InsertNode)
{
	ASSERT_TRUE(binaryTree->GetRoot() == NULL);

	pair<bool, Node_t *> pair = binaryTree->InsertNode(5);
	ASSERT_TRUE(get<0>(pair));
	ASSERT_FALSE(binaryTree->GetRoot() == NULL);
}
//test smazani
TEST_F(EmptyTree, DeleteNode)
{
	for (int i = 0; i < N; i++)
		ASSERT_FALSE(binaryTree->DeleteNode(i));
}
//test hledani
TEST_F(EmptyTree, FindNode)
{
	for (int i = 0; i < N; i++)
		ASSERT_EQ(binaryTree->FindNode(i), nullptr);
}

/** testy pro neprazdny strom **/

//fixture trida - naplni se K (10) hodnotami
#define K 10

class NonEmptyTree : public ::testing::Test
{
protected:
	BinaryTree *binaryTree;
	virtual void SetUp()
	{
		binaryTree = new BinaryTree();
		for (int i = 0; i < K; i++)
		{
			binaryTree->InsertNode(i);
		}
	}
	virtual void TearDown() override
	{
		delete binaryTree;
	}
};

//test vlozeni
TEST_F(NonEmptyTree, InsertNode)
{
	//do K jsou vytvorene, proto by se pri snaze vlozit melo vratit false
	for (int i = 0; i < K; i++)
	{
		pair<bool, Node_t *> pair = binaryTree->InsertNode(i);
		ASSERT_FALSE(get<0>(pair));
	}
	//od K by se meli uz bez problemu tvorit
	for (int i = K; i < 2 * K; i++)
	{
		pair<bool, Node_t *> pair = binaryTree->InsertNode(i);
		ASSERT_TRUE(get<0>(pair));
	}
}
//test vlozeni
TEST_F(NonEmptyTree, DeleteNode)
{
	for (int i = 0; i < K; i++)
	{
		//do K jsou vytvorene, proto by meli jit smazat
		ASSERT_TRUE(binaryTree->DeleteNode(i));
		//pokud se smazali uspesne, podruhe uz by mela funkce vracet false
		ASSERT_FALSE(binaryTree->DeleteNode(i));
	}
}
TEST_F(NonEmptyTree, FindNode)
{
	//do K jsou vytvorene, proto by meli jit najit
	for (int i = 0; i < K; i++)
	{
		ASSERT_NE(binaryTree->FindNode(i), nullptr);
	}
	//od K uz neexistuji, nelze tedy nalezt
	for (int i = K; i < 2 * K; i++)
	{
		ASSERT_EQ(binaryTree->FindNode(i), nullptr);
	}
}

/**testy axiomu**/
//fixture trida
class TreeAxioms : public ::testing::Test
{
protected:
	BinaryTree *binaryTree;
	virtual void SetUp()
	{
		binaryTree = new BinaryTree();
		//vytvoreni 10 zkusebnich uzlu
		for (int i = 0; i < 10; i++)
			binaryTree->InsertNode(i);
	}
	virtual void TearDown() override
	{
		delete binaryTree;
	}
};
//prvni axiom
TEST_F(TreeAxioms, Axiom1)
{
	vector<Node_t *> leafNodes;
	binaryTree->GetLeafNodes(leafNodes);
	for (unsigned int i = 0; i < leafNodes.size(); i++)
	{
		ASSERT_EQ(leafNodes[i]->color, BinaryTree::BLACK);
	}
}
//druhy axiom
TEST_F(TreeAxioms, Axiom2)
{
	vector<Node_t *> leafNodes;
	binaryTree->GetAllNodes(leafNodes);
	for (unsigned int i = 0; i < leafNodes.size(); i++)
	{
		if (leafNodes[i]->color == BinaryTree::RED)
		{
			ASSERT_EQ(leafNodes[i]->pLeft->color, BinaryTree::BLACK);
			ASSERT_EQ(leafNodes[i]->pRight->color, BinaryTree::BLACK);
		}
	}
}
//treti axiom
TEST_F(TreeAxioms, Axiom3)
{
	//ziskani korenu
	Node_t *pRoot = binaryTree->GetRoot();
	//nacteni listu
	vector<Node_t *> leafNodes;
	binaryTree->GetLeafNodes(leafNodes);
	//pole poctu cernych uzlu od kazdeho listu
	vector<int> blackNodesCounts;
	for (unsigned int i = 0; i < leafNodes.size(); i++)
	{
		//kazda cesta ma minimalne 1 cerny uzel (list)
		int blackNodesCount = 1;
		//nacitani rodicu az ke koreni a kontrolovani barvy
		Node_t *pParent = leafNodes[i]->pParent;
		while (pParent != pRoot)
		{
			if (pParent->color == BinaryTree::BLACK)
				blackNodesCount++;
			pParent = pParent->pParent;
		}
		//pridani poctu cernych uzlu do pole
		blackNodesCounts.push_back(blackNodesCount);
	}
	//vsechny hodnoty pole musi mit stejnou hodnotu
	int count = blackNodesCounts[0];
	for (unsigned int i = 1; i < blackNodesCounts.size(); i++)
	{
		ASSERT_EQ(count, blackNodesCounts[i]);
	}
}
/*** Konec souboru black_box_tests.cpp ***/
