/**
 * University: University of technology (VUT) Brno
 * Faculty: Faculty of information technologies
 *
 * Project: Implementation of a compiler for imperative language IFJ19
 * Subject: IFJ & IAL
 * Team: 007
 * Variant: 1
 *
 * Authors: Dominik Kaláb   (xkalab11)
 *          Vojtěch Staněk  (xstane45)
 *          Jan Lorenc      (xloren15)
 *          Matěj Pokorný   (xpokor78)
 *
 * @file list.c
 * @brief Implementation of list structure and its methods
 *
 */

#include "list.h"

listItem *initItem()
{
    listItem *newItem = malloc(sizeof(listItem));
    if (newItem == NULL)
        return NULL;

    newItem->index = -1;
    newItem->value = NULL;
    newItem->next = NULL;

    return newItem;
}

List *initList()
{
    List *newList = malloc(sizeof(List));
    if (newList == NULL)
        return NULL;

    newList->first = NULL;
    newList->current = NULL;

    return newList;
}

void delList(List *list)
{
    if (list == NULL)
        return;

    while(list->first)
    {
        listItem *tmp = list->first;
        list->first = list->first->next;
        freeToken(tmp->value);
        tmp->value = NULL;
        free(tmp);
        tmp = NULL;
    }
    free(list);
    list = NULL;
}

int appendList(List *list, Token_t *value)
{
    if (list == NULL || value == NULL)
        return -1;
    if (list->first == NULL)
    {
        list->first = initItem();
        if (list->first == NULL)
            return -1;

        list->first->index = 0;
        list->first->value = value;

        list->current = list->first; //First item in list so set current to it
        return 0;
    }

    int idx = 1; //index counter
    list->current = list->first;
    for(; list->current->next != NULL; idx++)
    { //iterate to the end
        list->current = list->current->next;
    }

    listItem *newItem = initItem();
    newItem->index = idx;
    newItem->value = value;

    list->current->next = newItem;
    return idx;
}

int searchList(List *list, Token_t *value)
{
    if (list == NULL || value == NULL)
        return -1;
    if (list->first == NULL)
        return -1;

    list->current = list->first;
    if (list->current->value == value)
        return list->current->index;

    for (; list->current->next != NULL; list->current = list->current->next)
    {
        if (list->current->value == value)
            return list->current->index;
    }

    return -1;
}

Token_t *searchListByIndex(List *list, int index)
{
    if (list == NULL || index < 0)
        return NULL;
    if (list->first == NULL)
        return NULL;

    list->current = list->first;
    if (list->current->index == index)
        return list->current->value;

    for (; list->current->next != NULL; list->current = list->current->next)
    {
        if (list->current->index == index)
            return list->current->value;
    }

    return NULL;
}

void printList(List *list)
{
    if (list == NULL) return;
    if (list->first == NULL){
        printf(" --- List empty --- \n");
        return;
    }

    list->current = list->first;
    printf(" --- LIST PRINT --- \n");
    //~ printf("Index: %d, value: %d\n", list->current->index, list->current->value->type);

    for (; list->current != NULL; list->current = list->current->next)
    {
        printf("Index: %d, value: %d\n", list->current->index, list->current->value->type);
    }
}
