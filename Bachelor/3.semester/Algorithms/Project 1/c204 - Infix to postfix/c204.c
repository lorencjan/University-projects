
/* ******************************* c204.c *********************************** */
/*  Předmět: Algoritmy (IAL) - FIT VUT v Brně                                 */
/*  Úkol: c204 - Převod infixového výrazu na postfixový (s využitím c202)     */
/*  Referenční implementace: Petr Přikryl, listopad 1994                      */
/*  Přepis do jazyka C: Lukáš Maršík, prosinec 2012                           */
/*  Upravil: Kamil Jeřábek, září 2019                                         */
/* ************************************************************************** */
/*
** Implementujte proceduru pro převod infixového zápisu matematického
** výrazu do postfixového tvaru. Pro převod využijte zásobník (tStack),
** který byl implementován v rámci příkladu c202. Bez správného vyřešení
** příkladu c202 se o řešení tohoto příkladu nepokoušejte.
**
** Implementujte následující funkci:
**
**    infix2postfix .... konverzní funkce pro převod infixového výrazu 
**                       na postfixový
**
** Pro lepší přehlednost kódu implementujte následující pomocné funkce:
**    
**    untilLeftPar .... vyprázdnění zásobníku až po levou závorku
**    doOperation .... zpracování operátoru konvertovaného výrazu
**
** Své řešení účelně komentujte.
**
** Terminologická poznámka: Jazyk C nepoužívá pojem procedura.
** Proto zde používáme pojem funkce i pro operace, které by byly
** v algoritmickém jazyce Pascalovského typu implemenovány jako
** procedury (v jazyce C procedurám odpovídají funkce vracející typ void).
**
**/

#include "c204.h"
int solved;

/*
** Pomocná funkce untilLeftPar.
** Slouží k vyprázdnění zásobníku až po levou závorku, přičemž levá závorka
** bude také odstraněna. Pokud je zásobník prázdný, provádění funkce se ukončí.
**
** Operátory odstraňované ze zásobníku postupně vkládejte do výstupního pole
** znaků postExpr. Délka převedeného výrazu a též ukazatel na první volné
** místo, na které se má zapisovat, představuje parametr postLen.
**
** Aby se minimalizoval počet přístupů ke struktuře zásobníku, můžete zde
** nadeklarovat a používat pomocnou proměnnou typu char.
*/
void untilLeftPar(tStack *s, char *postExpr, unsigned *postLen)
{
  //maze prvky ze zasobniku, dokud neni prazdny/nenarazi na levou zavorku
  char c; //pomocna promenna pro ukladani aktualniho prvku
  while (!stackEmpty(s))
  {
    stackTop(s, &c); //nactu hodnotu na vrcholu
    stackPop(s);     //smazu ji ze zasobniku ... i kdyby to byla zavorka, tak ta se ma taky smazat -> ok
    //porovnam se zavorkou, bud ukoncim cyklus, nebo pokracuji dal
    if (c == '(')
      break;
    //ulozeni smazane hodnoty
    postExpr[(*postLen)++] = c;
  }
}

/*
** Pomocná funkce doOperation.
** Zpracuje operátor, který je předán parametrem c po načtení znaku ze
** vstupního pole znaků.
**
** Dle priority předaného operátoru a případně priority operátoru na
** vrcholu zásobníku rozhodneme o dalším postupu. Délka převedeného 
** výrazu a taktéž ukazatel na první volné místo, do kterého se má zapisovat, 
** představuje parametr postLen, výstupním polem znaků je opět postExpr.
*/
void doOperation(tStack *s, char c, char *postExpr, unsigned *postLen)
{
  //na vrchol zasobniku ulozime operator pokud...
  if (stackEmpty(s)) //je zasobnik prazdny
  {
    stackPush(s, c);
    return;
  }
  //nebo pokud je na vrcholu bud zavorka nebo operator nizsi priority
  char topChar;
  stackTop(s, &topChar);                                              //prectu hodnotu z vrcholu zasobniku, at mam s cim porovnavat
  if (topChar == '(' ||                                               //na vrcholu zavorka
      ((c == '*' || c == '/') && (topChar == '+' || topChar == '-'))) //operator ma vyssi prioritu
  {
    stackPush(s, c);
    return;
  }
  //v opacnem pripade bereme ze zasobniku nadale
  postExpr[(*postLen)++] = topChar;
  stackPop(s);
  //provadime, dokud nelze operator vlozit na zasobnik
  doOperation(s, c, postExpr, postLen);
}

/* 
** Konverzní funkce infix2postfix.
** Čte infixový výraz ze vstupního řetězce infExpr a generuje
** odpovídající postfixový výraz do výstupního řetězce (postup převodu
** hledejte v přednáškách nebo ve studijní opoře). Paměť pro výstupní řetězec
** (o velikosti MAX_LEN) je třeba alokovat. Volající funkce, tedy
** příjemce konvertovaného řetězce, zajistí korektní uvolnění zde alokované
** paměti.
**
** Tvar výrazu:
** 1. Výraz obsahuje operátory + - * / ve významu sčítání, odčítání,
**    násobení a dělení. Sčítání má stejnou prioritu jako odčítání,
**    násobení má stejnou prioritu jako dělení. Priorita násobení je
**    větší než priorita sčítání. Všechny operátory jsou binární
**    (neuvažujte unární mínus).
**
** 2. Hodnoty ve výrazu jsou reprezentovány jednoznakovými identifikátory
**    a číslicemi - 0..9, a..z, A..Z (velikost písmen se rozlišuje).
**
** 3. Ve výrazu může být použit předem neurčený počet dvojic kulatých
**    závorek. Uvažujte, že vstupní výraz je zapsán správně (neošetřujte
**    chybné zadání výrazu).
**
** 4. Každý korektně zapsaný výraz (infixový i postfixový) musí být uzavřen 
**    ukončovacím znakem '='.
**
** 5. Při stejné prioritě operátorů se výraz vyhodnocuje zleva doprava.
**
** Poznámky k implementaci
** -----------------------
** Jako zásobník použijte zásobník znaků tStack implementovaný v příkladu c202. 
** Pro práci se zásobníkem pak používejte výhradně operace z jeho rozhraní.
**
** Při implementaci využijte pomocné funkce untilLeftPar a doOperation.
**
** Řetězcem (infixového a postfixového výrazu) je zde myšleno pole znaků typu
** char, jenž je korektně ukončeno nulovým znakem dle zvyklostí jazyka C.
**
** Na vstupu očekávejte pouze korektně zapsané a ukončené výrazy. Jejich délka
** nepřesáhne MAX_LEN-1 (MAX_LEN i s nulovým znakem) a tedy i výsledný výraz
** by se měl vejít do alokovaného pole. Po alokaci dynamické paměti si vždycky
** ověřte, že se alokace skutečně zdrařila. V případě chyby alokace vraťte namísto
** řetězce konstantu NULL.
*/
char *infix2postfix(const char *infExpr)
{
  //alokujeme pamet pro vystupni retezec
  char *postExpr = (char *)malloc(MAX_LEN);
  if (postExpr == NULL)
  {
    fprintf(stderr, "Error: Failed to allocate memory for output string in infix2postfix function");
    return NULL;
  }
  //alokace zasobniku
  tStack *s = (tStack *)malloc(sizeof(tStack));
  if (s == NULL)
  {
    free(postExpr);
    fprintf(stderr, "Error: Failed to allocate memory for stack in infix2postfix function");
    return NULL;
  }
  //inicializace dat
  stackInit(s);
  unsigned int postLen = 0;
  //pruchod vstupnim retezcem
  int i = 0;
  for (char c = infExpr[i]; c != '\0'; c = infExpr[++i])
  {
    switch (c)
    {
    case '(': //leva zavorka patri na zasobnik
      stackPush(s, c);
      break;
    case ')': //pri prave zavorce vzprazdnime zasobnik k leve
      untilLeftPar(s, postExpr, &postLen);
      break;
    case '+':
    case '-':
    case '*':
    case '/': //operator patri na konec vznikajiciho vystupniho retezce
      doOperation(s, c, postExpr, &postLen);
      break;
    case '=': //konec vyrazu
      //projdeme zasobnik a presuneme prvky na vystupni retezec
      while (!stackEmpty(s))
      {
        stackTop(s, &(postExpr[postLen++]));
        stackPop(s);
      }
      postExpr[postLen++] = '='; //dodani ukoncovaciho znaku
      postExpr[postLen] = '\0';  //ukonceni retezce
      c = '\0';                  //timto ukoncime for cyklus
    default:                     //jinak se jedna o operand a patri do retezce
      //presto vsak pro jistotu overim spravnost znaku
      if ((c >= '0' && c <= '9') || //jedna se o cislici
          (c >= 'a' && c <= 'z') || //jedna se o male pismeno
          (c >= 'A' && c <= 'Z'))   //jedna se o velke pismeno
      {
        postExpr[postLen++] = c;
      }
    }
  }
  //uvolnime zasobnik
  free(s);
  return postExpr;
}

/* Konec c204.c */
