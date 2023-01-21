
/* c206.c **********************************************************}
{* Téma: Dvousměrně vázaný lineární seznam
**
**                   Návrh a referenční implementace: Bohuslav Křena, říjen 2001
**                            Přepracované do jazyka C: Martin Tuček, říjen 2004
**                                            Úpravy: Kamil Jeřábek, září 2019
**
** Implementujte abstraktní datový typ dvousměrně vázaný lineární seznam.
** Užitečným obsahem prvku seznamu je hodnota typu int.
** Seznam bude jako datová abstrakce reprezentován proměnnou
** typu tDLList (DL znamená Double-Linked a slouží pro odlišení
** jmen konstant, typů a funkcí od jmen u jednosměrně vázaného lineárního
** seznamu). Definici konstant a typů naleznete v hlavičkovém souboru c206.h.
**
** Vaším úkolem je implementovat následující operace, které spolu
** s výše uvedenou datovou částí abstrakce tvoří abstraktní datový typ
** obousměrně vázaný lineární seznam:
**
**      DLInitList ...... inicializace seznamu před prvním použitím,
**      DLDisposeList ... zrušení všech prvků seznamu,
**      DLInsertFirst ... vložení prvku na začátek seznamu,
**      DLInsertLast .... vložení prvku na konec seznamu,
**      DLFirst ......... nastavení aktivity na první prvek,
**      DLLast .......... nastavení aktivity na poslední prvek,
**      DLCopyFirst ..... vrací hodnotu prvního prvku,
**      DLCopyLast ...... vrací hodnotu posledního prvku,
**      DLDeleteFirst ... zruší první prvek seznamu,
**      DLDeleteLast .... zruší poslední prvek seznamu,
**      DLPostDelete .... ruší prvek za aktivním prvkem,
**      DLPreDelete ..... ruší prvek před aktivním prvkem,
**      DLPostInsert .... vloží nový prvek za aktivní prvek seznamu,
**      DLPreInsert ..... vloží nový prvek před aktivní prvek seznamu,
**      DLCopy .......... vrací hodnotu aktivního prvku,
**      DLActualize ..... přepíše obsah aktivního prvku novou hodnotou,
**      DLSucc .......... posune aktivitu na další prvek seznamu,
**      DLPred .......... posune aktivitu na předchozí prvek seznamu,
**      DLActive ........ zjišťuje aktivitu seznamu.
**
** Při implementaci jednotlivých funkcí nevolejte žádnou z funkcí
** implementovaných v rámci tohoto příkladu, není-li u funkce
** explicitně uvedeno něco jiného.
**
** Nemusíte ošetřovat situaci, kdy místo legálního ukazatele na seznam 
** předá někdo jako parametr hodnotu NULL.
**
** Svou implementaci vhodně komentujte!
**
** Terminologická poznámka: Jazyk C nepoužívá pojem procedura.
** Proto zde používáme pojem funkce i pro operace, které by byly
** v algoritmickém jazyce Pascalovského typu implemenovány jako
** procedury (v jazyce C procedurám odpovídají funkce vracející typ void).
**/

#include "c206.h"

int solved;
int errflg;

void DLError()
{
    /*
** Vytiskne upozornění na to, že došlo k chybě.
** Tato funkce bude volána z některých dále implementovaných operací.
**/
    printf("*ERROR* The program has performed an illegal operation.\n");
    errflg = TRUE; /* globální proměnná -- příznak ošetření chyby */
    return;
}

void DLInitList(tDLList *L)
{
    /*
** Provede inicializaci seznamu L před jeho prvním použitím (tzn. žádná
** z následujících funkcí nebude volána nad neinicializovaným seznamem).
** Tato inicializace se nikdy nebude provádět nad již inicializovaným
** seznamem, a proto tuto možnost neošetřujte. Vždy předpokládejte,
** že neinicializované proměnné mají nedefinovanou hodnotu.
**/
    L->Act = NULL;
    L->First = NULL;
    L->Last = NULL;
}

void DLDisposeList(tDLList *L)
{
    /*
** Zruší všechny prvky seznamu L a uvede seznam do stavu, v jakém
** se nacházel po inicializaci. Rušené prvky seznamu budou korektně
** uvolněny voláním operace free. 
**/
    //nemuzu volat implementovane funkce (zadani), takze vse rucne
    //nastavim na zacatek a postupne proiteruju ke konci
    tDLElemPtr active = L->First;
    while (active != NULL)
    {
        tDLElemPtr next = active->rptr;
        free(active);
        active = next;
    }

    //uvedu do stavu po inicializaci
    L->Act = NULL;
    L->First = NULL;
    L->Last = NULL;
}

void DLInsertFirst(tDLList *L, int val)
{
    /*
** Vloží nový prvek na začátek seznamu L.
** V případě, že není dostatek paměti pro nový prvek při operaci malloc,
** volá funkci DLError().
**/
    //alokace noveho prvku
    tDLElemPtr elmt = (tDLElemPtr)malloc(sizeof(struct tDLElem));
    if (elmt == NULL)
    {
        DLError();
        return;
    }
    //inicializace noveho prvku
    elmt->data = val;
    elmt->lptr = NULL;     //prvni prvek, vlevo nikoho nema
    elmt->rptr = L->First; //nasleduje dosavadni prvni prvek
    //jestli prvni neexistuje, je to zatim prazdny seznam ->prvni je i posledni
    if (L->First == NULL)
    {
        L->Last = elmt;
    }
    else //jinak je treba novy prvek napojit na dosavadni prvni
    {
        L->First->lptr = elmt;
    }
    L->First = elmt; //v kazdem pripade se stava novym prvkem
}

void DLInsertLast(tDLList *L, int val)
{
    /*
** Vloží nový prvek na konec seznamu L (symetrická operace k DLInsertFirst).
** V případě, že není dostatek paměti pro nový prvek při operaci malloc,
** volá funkci DLError().
**/
    //alokace noveho prvku
    tDLElemPtr elmt = (tDLElemPtr)malloc(sizeof(struct tDLElem));
    if (elmt == NULL)
    {
        DLError();
        return;
    }
    //inicializace noveho prvku
    elmt->data = val;
    elmt->rptr = NULL;    //posledni prvek, vpravo nikoho nema
    elmt->lptr = L->Last; //predchazi dosavadni posledni prvek
    //jestli posledni neexistuje, je to zatim prazdny seznam -> prvni je i posledni
    if (L->Last == NULL)
    {
        L->First = elmt;
    }
    else //jinak je treba novy prvek napojit na dosavadni posledni
    {
        L->Last->rptr = elmt;
    }
    L->Last = elmt; //v kazdem pripade se stava novym prvkem
}

void DLFirst(tDLList *L)
{
    /*
** Nastaví aktivitu na první prvek seznamu L.
** Funkci implementujte jako jediný příkaz (nepočítáme-li return),
** aniž byste testovali, zda je seznam L prázdný.
**/
    L->Act = L->First;
}

void DLLast(tDLList *L)
{
    /*
** Nastaví aktivitu na poslední prvek seznamu L.
** Funkci implementujte jako jediný příkaz (nepočítáme-li return),
** aniž byste testovali, zda je seznam L prázdný.
**/
    L->Act = L->Last;
}

void DLCopyFirst(tDLList *L, int *val)
{
    /*
** Prostřednictvím parametru val vrátí hodnotu prvního prvku seznamu L.
** Pokud je seznam L prázdný, volá funkci DLError().
**/
    if (L->First == NULL)
    {
        DLError();
    }
    else
    {
        *val = L->First->data;
    }
}

void DLCopyLast(tDLList *L, int *val)
{
    /*
** Prostřednictvím parametru val vrátí hodnotu posledního prvku seznamu L.
** Pokud je seznam L prázdný, volá funkci DLError().
**/
    if (L->Last == NULL)
    {
        DLError();
    }
    else
    {
        *val = L->Last->data;
    }
}

void DLDeleteFirst(tDLList *L)
{
    /*
** Zruší první prvek seznamu L. Pokud byl první prvek aktivní, aktivita 
** se ztrácí. Pokud byl seznam L prázdný, nic se neděje.
**/
    if (L->First != NULL) // podminka neprazneho seznamu
    {
        tDLElemPtr elmt = L->First;
        //potencialni ruseni aktivity
        if (L->First == L->Act)
        {
            L->Act = NULL;
        }
        //kontrola jednoprvkoveho seznamu
        if (L->First == L->Last)
        {
            L->First = NULL;
            L->Last = NULL;
        }
        else //jinak pouze posuneme prvni prvek
        {
            L->First = elmt->rptr;
            L->First->lptr = NULL;
        }
        free(elmt);
    }
}

void DLDeleteLast(tDLList *L)
{
    /*
** Zruší poslední prvek seznamu L. Pokud byl poslední prvek aktivní,
** aktivita seznamu se ztrácí. Pokud byl seznam L prázdný, nic se neděje.
**/
    if (L->Last != NULL) // podminka neprazneho seznamu
    {
        tDLElemPtr elmt = L->Last;
        //potencialni ruseni aktivity
        if (L->Last == L->Act)
        {
            L->Act = NULL;
        }
        //kontrola jednoprvkoveho seznamu
        if (L->First == L->Last)
        {
            L->First = NULL;
            L->Last = NULL;
        }
        else //jinak pouze posuneme posledni prvek
        {
            L->Last = elmt->lptr;
            L->Last->rptr = NULL;
        }
        free(elmt);
    }
}

void DLPostDelete(tDLList *L)
{
    /*
** Zruší prvek seznamu L za aktivním prvkem.
** Pokud je seznam L neaktivní nebo pokud je aktivní prvek
** posledním prvkem seznamu, nic se neděje.
**/
    //rusime pouze pokud je co rusit viz. zadani
    if (L->Act != NULL && L->Act != L->Last)
    {
        tDLElemPtr elmt = L->Act->rptr;
        //kontrola, jestli nerusime posledniho
        if (elmt == L->Last)
        {
            L->Last = elmt->lptr;
        }
        else //jinak aktivni prvek bude mit za praveho souseda prvek vedle ruseneho
        {
            L->Act->rptr = elmt->rptr;
        }
        //napojime aktivniho na jeho noveho souseda
        L->Act->rptr->lptr = L->Act;
        //uvolnime prvek
        free(elmt);
    }
}

void DLPreDelete(tDLList *L)
{
    /*
** Zruší prvek před aktivním prvkem seznamu L .
** Pokud je seznam L neaktivní nebo pokud je aktivní prvek
** prvním prvkem seznamu, nic se neděje.
**/
    //rusime pouze pokud je co rusit viz. zadani
    if (L->Act != NULL && L->Act != L->First)
    {
        tDLElemPtr elmt = L->Act->lptr;
        //kontrola, jestli nerusime prvni
        if (elmt == L->First)
        {
            L->First = elmt->rptr;
        }
        else //jinak aktivni prvek bude mit za leveho souseda prvek vedle ruseneho
        {
            L->Act->lptr = elmt->lptr;
        }
        //napojime aktivniho na jeho noveho souseda
        L->Act->lptr->rptr = L->Act;
        //uvolnime prvek
        free(elmt);
    }
}

void DLPostInsert(tDLList *L, int val)
{
    /*
** Vloží prvek za aktivní prvek seznamu L.
** Pokud nebyl seznam L aktivní, nic se neděje.
** V případě, že není dostatek paměti pro nový prvek při operaci malloc,
** volá funkci DLError().
**/
    //deje se neco jen, pokud existuje aktivni
    if (L->Act != NULL)
    {
        //alokace noveho prvku
        tDLElemPtr elmt = (tDLElemPtr)malloc(sizeof(struct tDLElem));
        if (elmt == NULL)
        {
            DLError();
            return;
        }
        //inicializace noveho prvku
        elmt->data = val;
        elmt->lptr = L->Act;       //vkladame za aktivni->predchazejici je aktivni
        elmt->rptr = L->Act->rptr; //pravy soused aktivniho je nyni pravy soused noveho
        L->Act->rptr = elmt;       //napojeni noveho na aktivni
        //kontrole posledniho prvku
        if (L->Act == L->Last)
        {
            L->Last = elmt;
        }
        else //navazani noveho na jeho praveho souseda
        {
            elmt->rptr->lptr = elmt;
        }
    }
}

void DLPreInsert(tDLList *L, int val)
{
    /*
** Vloží prvek před aktivní prvek seznamu L.
** Pokud nebyl seznam L aktivní, nic se neděje.
** V případě, že není dostatek paměti pro nový prvek při operaci malloc,
** volá funkci DLError().
**/
    //deje se neco jen, pokud existuje aktivni
    if (L->Act != NULL)
    {
        //alokace noveho prvku
        tDLElemPtr elmt = (tDLElemPtr)malloc(sizeof(struct tDLElem));
        if (elmt == NULL)
        {
            DLError();
            return;
        }
        //inicializace noveho prvku
        elmt->data = val;
        elmt->rptr = L->Act;       //vkladame pred aktivni->nasledujici je aktivni
        elmt->lptr = L->Act->lptr; //levy soused aktivniho je nyni levy soused noveho
        L->Act->lptr = elmt;       //napojeni noveho na aktivni
        //kontrole prvniho prvku
        if (L->Act == L->First)
        {
            L->First = elmt;
        }
        else //navazani noveho na jeho leveho souseda
        {
            elmt->lptr->rptr = elmt;
        }
    }
}

void DLCopy(tDLList *L, int *val)
{
    /*
** Prostřednictvím parametru val vrátí hodnotu aktivního prvku seznamu L.
** Pokud seznam L není aktivní, volá funkci DLError ().
**/
    if (L->Act == NULL)
    {
        DLError();
    }
    else
    {
        *val = L->Act->data;
    }
}

void DLActualize(tDLList *L, int val)
{
    /*
** Přepíše obsah aktivního prvku seznamu L.
** Pokud seznam L není aktivní, nedělá nic.
**/
    if (L->Act != NULL)
    {
        L->Act->data = val;
    }
}

void DLSucc(tDLList *L)
{
    /*
** Posune aktivitu na následující prvek seznamu L.
** Není-li seznam aktivní, nedělá nic.
** Všimněte si, že při aktivitě na posledním prvku se seznam stane neaktivním.
**/
    if (L->Act != NULL) //pouze pokud je aktivni
    {
        if (L->Act == L->Last) //pokud je posledni, zrusi aktivitu
        {
            L->Act = NULL;
        }
        else //jinak ativitu posune dal
        {
            L->Act = L->Act->rptr;
        }
    }
}

void DLPred(tDLList *L)
{
    /*
** Posune aktivitu na předchozí prvek seznamu L.
** Není-li seznam aktivní, nedělá nic.
** Všimněte si, že při aktivitě na prvním prvku se seznam stane neaktivním.
**/
    if (L->Act != NULL) //pouze pokud je aktivni
    {
        if (L->Act == L->First) //pokud je prvni, zrusi aktivitu
        {
            L->Act = NULL;
        }
        else //jinak ativitu posune zpet
        {
            L->Act = L->Act->lptr;
        }
    }
}

int DLActive(tDLList *L)
{
    /*
** Je-li seznam L aktivní, vrací nenulovou hodnotu, jinak vrací 0.
** Funkci je vhodné implementovat jedním příkazem return.
**/
    return L->Act != NULL;
}

/* Konec c206.c*/
