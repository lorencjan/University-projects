//======== Copyright (c) 2017, FIT VUT Brno, All rights reserved. ============//
//
// Purpose:     Test Driven Development - priority queue code
//
// $NoKeywords: $ivs_project_1 $tdd_code.cpp
// $Author:     JAN LORENC <xloren15@stud.fit.vutbr.cz>
// $Date:       $2019-02-12
//============================================================================//
/**
 * @file tdd_code.cpp
 * @author JAN LORENC
 *
 * @brief Implementace metod tridy prioritni fronty.
 */

#include <stdlib.h>
#include <stdio.h>

#include "tdd_code.h"

 //============================================================================//
 // ** ZDE DOPLNTE IMPLEMENTACI **
 //
 // Zde doplnte implementaci verejneho rozhrani prioritni fronty (Priority Queue)
 // 1. Verejne rozhrani fronty specifikovane v: tdd_code.h (sekce "public:")
 //    - Konstruktor (PriorityQueue()), Destruktor (~PriorityQueue())
 //    - Metody Insert/Remove/Find a GetHead
 //    - Pripadne vase metody definovane v tdd_code.h (sekce "protected:")
 //
 // Cilem je dosahnout plne funkcni implementace prioritni fronty implementovane
 // pomoci tzv. "double-linked list", ktera bude splnovat dodane testy 
 // (tdd_tests.cpp).
 //============================================================================//

PriorityQueue::PriorityQueue()
{
	m_pHead = nullptr;
}

PriorityQueue::~PriorityQueue()
{
	while (m_pHead != nullptr)
	{
		Element_t *tmp = m_pHead;
		m_pHead = m_pHead->pNext;
		delete tmp;
	}
}

void PriorityQueue::Insert(int value)
{
	//vytvorime novy zaznam
	Element_t *newItem = new Element_t;
	newItem->value = value;

	//pokud se jedna o prvni prvek ve fronte, vlozim zvlast a nastavim root
	if (m_pHead == nullptr || m_pHead->value > value)
	{
		newItem->pPrev = nullptr;
		// kdyby byl seznam prazdny, tak pNext rootu neexistuje -> chyba
		if (m_pHead != nullptr)
		{
			newItem->pNext = m_pHead;
			m_pHead->pPrev = newItem;
		}
		else
			newItem->pNext = nullptr;
		m_pHead = newItem;				//root ukazuje na novy prvni prvek
		return;
	}

	//nejednali se o novou hlavu, zarazuje se algoritmem
	Element_t *currentItem = m_pHead;
	//pokud neni fronta prazdna, projizdim ji, dokud hodnota neni vetsi nez ta aktualniho prvku
	while (currentItem->pNext != nullptr && value > currentItem->pNext->value)
		currentItem = currentItem->pNext;

	//zaradim novy zaznam za aktualni
	newItem->pNext = currentItem->pNext;
	newItem->pPrev = currentItem;
	//nastavim ukazatele aktualniho a puvodne nasledujiciho prvku na novy
	currentItem->pNext = newItem;
	if (newItem->pNext != nullptr)
		newItem->pNext->pPrev = newItem;
}

bool PriorityQueue::Remove(int value)
{
	Element_t *item = Find(value);
	if (item != nullptr)
	{
		//pokud to neni hlava, predchozi ukazuje na nasledujici, jinak menim hlavu
		if (item->pPrev != nullptr)
			item->pPrev->pNext = item->pNext;
		else
			m_pHead = item->pNext;
		//pokud neni posledni, nasledujici bude ukazovat na predchoziho
		if (item->pNext != nullptr)
			item->pNext->pPrev = item->pPrev;
		
		delete item;
		return true;
	}
	else
		return false;
}

PriorityQueue::Element_t *PriorityQueue::Find(int value)
{
	Element_t *currentItem = m_pHead;
	while (currentItem != nullptr)
	{
		if (currentItem->value == value)
			return currentItem;
		currentItem = currentItem->pNext;
	}
	return NULL;
}

PriorityQueue::Element_t *PriorityQueue::GetHead()
{
	return m_pHead;
}

/*** Konec souboru tdd_code.cpp ***/
