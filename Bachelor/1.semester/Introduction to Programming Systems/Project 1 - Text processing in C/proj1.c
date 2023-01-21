/*************************************************/
/***          Projekt 1 - Práce s textem       ***/
/***                   Verze 1                 ***/
/***                 Jan Lorenc                ***/
/***                20. 10. 2018               ***/
/*************************************************/

#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

/* funkce pro nacteni prikazoveho souboru z argumentu programu
   vraci true, jestli soubor argument obsahuje na konci .txt
   -> jedna se o textovy soubor
   vraci false, pokud neni zadan argument, nebo se nejedna o .txt soubor */
bool getCommandFile(char *argument, int numOfArguments)
{
    if(numOfArguments > 2)  //jestli je vic nez 1 argument (mimo samotnou cestu)
    {                       //upozorni a ukonci program
        fprintf(stderr, "Zadali jste mnoho argumentu!\"Zadejte pouze 1 argument - prikazovy soubor.\n");
        return false;
    }
    //zjisteni, zda existuje argument
    if(argument)
    {
       //pokud ano, overim, zda obsahuje priponu .txt
       if(strstr(argument, ".txt") != NULL)
            return true; //obsahuje ".txt" -> vse je ok
       else
       {
            //pokud priponu neobsahuje, vypise uzivateli, co je spatne
            printf("Zadany argument neni soubor formatu .txt!\n"
                   "Zadejte prikazovy soubor formatu .txt!\n");
       }
    }
    else  //pokud argument neexistuje, vypise, ze se do nej nic ani nezapsalo
        printf("Nezadali jste do argumentu zadny .txt soubor s prikazy!\n");
    return false;
}

/* EDITACNI FUNKCE */
// funkce bere za parametry ukazatele na prikaz+1 = CONTENT noveho radku
// a docasny soubor, do docasneho souboru zapise dovy radek (pred aktualni)
void insertNewLine(char *newLine, FILE *tempFile)
{
    int newLineLenght = strlen(newLine);
    newLine[newLineLenght] = '\n';          //pridelame '\n' konec radku
    newLine[newLineLenght+1] = '\0';        //ukoncime retezec
    fputs(newLine, tempFile);               //zapiseme do souboru
}

// funkce bere za parametry ukazatele na prikaz+1 = CONTENT a aktualni radek
// funkce za nynejsi radek prida CONTENT z prikazu aCONTENT
bool insertTextAfter(char *content, char *currentLine, int lineLenght)
{
     //zbaveni se noveho radku z konce retezce
     bool hadEOL =false;
     if(currentLine[strlen(currentLine)-1] == '\n')
     {
         currentLine[strlen(currentLine)-1] = '\0';
         hadEOL = true;
     }
     //prida konec radku, pokud nebyl u puvodniho radku smazan
     if(hadEOL)
     {
         int strLenght = strlen(content);
         content[strLenght] = '\n';
         content[strLenght+1] = '\0';
     }

     //jestli se spojeni contentu a radku vleze do max. velikosti radku, pokracuje
     int combinedLenght = strlen(content)+strlen(currentLine);
     if(combinedLenght < lineLenght)
     {
         strcat(currentLine, content); //pripojeni textu z prikazu k aktualni radce
         return true;
     }
     else
     {
         fprintf(stderr,"Snaha o vytvoreni vetsiho radku nez %d znaku!\n"
                        "Program predcasne ukoncen.\n", lineLenght);
         return false;
     }
}

// funkce bere za parametry ukazatele na prikaz+1 = CONTENT a aktualni radek
// funkce pred nynejsi radek prida CONTENT z prikazu bCONTENT
bool insertTextBefore(char *content, char *currentLine, int lineLenght)
{
    //zkopirovani contentu do pole velikosti aktualniho radku - proti preteceni u strcatu
    char contentCopy[lineLenght];
    strcpy(contentCopy, content);
    //jestli se spojeni contentu a radku vleze do max. velikosti radku, pokracuje
	int combinedLenght = strlen(content)+strlen(currentLine);
    if(combinedLenght <= lineLenght)
    {
        strcat(contentCopy, currentLine);   //pripojeni nynejsi radky k textu z prikazu
        strcpy(currentLine, contentCopy);   //zkopirovani upravene radky do aktualni radky
        return true;
    }
    else //jinak vypise chybu a vrati false -> ukonceni programu
    {
        fprintf(stderr, "Snaha o vytvoreni vetsiho radku nez %d znaku!\n"
                        "Program predcasne ukoncen.\n", lineLenght);
        return false;
    }
}

// funkce bere za parametry ukazatele na prikaz, aktualni radek a docasny soubor
// do souboru zapise aktualni radek, ktery se jiz totiz nebude upravovat a nacte od uzivatele dalsi
// vraci 1, jestli je vse ok, 0, jestli nejsou data na vstupu, -1 pokud u prikazu nN je N zaporne/neni cislo
int getAnotherLine(char *command, char *currentLine, int currLineSize, FILE *tempFile)
{
    fputs(currentLine, tempFile); //nejprve zapise stavajici radek do souboru
    //jedna-li se o prikaz 'nHODNOTA', nacte a zapise do souboru HODNOTA radku
    if(command[1] != '\0')
    {
       // zkopirovani prikazu do retezce (bez pocatecniho prikazoveho pismena), abych upravou tohoto retezce
       // nemenil puvodni prikaz (je to totiz ukazatel) a mohl by byt znovu pouzit pri cyklu prikazu goto
       char str[strlen(command)-1];
       strcpy(str, command+1);
       int num;
       if(sscanf(str, "%d", &num) && num>0) //pretypovani retezce na int
       {
           //pokracujeme pouze pokud pretypovani bylo uspesne - ciselny prikaz, mame se presunout na n-ty radek
           //proto n-1 radku nacteme a zapiseme bez uprav a pote vratime ten n-ty radek pro dalsi upravy
           for(int i=0; i<num-1; i++)
           {
              if(fgets(currentLine, currLineSize, stdin))  //jestli se nacetl dalsi radek
                fputs(currentLine, tempFile);              //zapise ho ho docasneho souboru
              else                                         //v opacnem pripade
                return -1;                                  //nejsou data na vstupu, vraci 0
           }
       }
       else
       {
           fprintf(stderr, "Snaha o nacteni N radku kde N je zaporne/neni cislo!\n"
                           "Program predcasne ukoncen.");
           return 0;       //chybny prikaz -> vraci -1
       }
    }
    if(fgets(currentLine, currLineSize, stdin))
       return 1;    //jsou data na vstupu, vraci 1
    else
       return -1;    //nejsou data na vstupu, vraci 0
}

// funkce smaze dany/e radek/ky, funguje podobne jako getAnotherLine(),
// pouze dany radek nezapise do docasneho souboru - tim ho vlastne preskoci
int deleteLine(char *command, char *currentLine, int currLineSize)
{
    //jedna-li se pouze o prikaz 'd', misto aktualniho radku nacte novy
    if(command[1] == '\0')
    {
        if(!fgets(currentLine, currLineSize, stdin)) //kontrola predcasneho konce
           return -1;
    }
    else // ma-li smazat vice radku, bere dalsi radky a ignoruje je
    {
       // zkopirovani prikazu do retezce (bez pocatecniho prikazoveho pismena), abych upravou tohoto retezce
       // nemenil puvodni prikaz (je to totiz ukazatel) a mohl by byt znovu pouzit pri cyklu prikazu goto
       char str[strlen(command)-1];
       strcpy(str, command+1);
       int num;
       if(sscanf(str, "%d", &num) && num>0) //pretypovani retezce na int
       {
           //pokracujeme pouze pokud pretypovani bylo uspesne - ciselny prikaz
           //mame smazat na n radku, proto n-1 radku nacteme a nechame byt
           //bez uprav a pote vratime ten n-ty radek jako prazdny retezec
           for(int i=0; i<num; i++)
           {
               if(fgets(currentLine, currLineSize, stdin) == NULL)  //kontrola predcasneho konce
                 return 0;
           }
       }
       else
       {
           fprintf(stderr, "Snaha o smazani N radku kde N je zaporne/neni cislo!\n"
                           "Program predcasne ukoncen.");
           return 0;   //chybny prikaz -> vraci -1
       }
    }
    return 1;
}

bool getPatternAndReplacement(char *pattern, char *replacement, char *command, char *divider)
{
    if(command[2] == divider[0])   //nejprve se zkontroluje, jestli existuje vzor ...command[0] = 's'/'S'
    {                              //command[1] = prvni delic a jestli command[2] je zase delic, pak chybi vzor
        fprintf(stderr,"Prikaz 's' nenalezl vzor!\nProgram je predcasne ukoncen!\n");
        return false;              //v tomto pripade vraci false, ze nenasel
    }
    char *array[3] = {"","",""};  //kdyby chybela nahrada, at je predem implementovana jako prazdny retezec
    char *str = strtok(command, divider); //rozdelim kopii prikazu podle mezer - jeji vyuziti
    int count=0;
    while (str != NULL)
    {
        array[count++] = str;
        str = strtok(NULL, divider);
    }
    // 1. v poli je prikaz, 2.pattern, 3. replacement
    strcpy(pattern, array[1]);
    strcpy(replacement, array[2]);
    return true;
}
//funkce ziska delic pro vzor a nahrad u funkce s/S PATTERN REPLACEMENT
bool getDevider(char *divider, char *command)
{
   if(command[1] == ' ' && command[1] != 'S')     // s PATTERN REPLACEMENT
      sprintf(divider, "%c", command[1]);         // do delice se na mezera
   else if((int)command[1] > 20)                  // s/S:PATTERN:REPLACEMENT vyzaduje tisknutelne znaky jako delic
      sprintf(divider, "%c", command[1]);         // v ASCI tabulce jsou 1-20 netisknutelne a od 20 tisknutelne
   else
   {
       fprintf(stderr, "Nenalezl se delic vzoru a nahrady ve spravnem formatu u prikazu %c:PATTERN:REPLACEMENT!\n"
                       "Program predcasne ukoncen.\n", command[0]);
       return false;
   }
   return true;
}
//funkce zpracovava prikazy s PATERN REPLACEMENT,s:PATERN:REPLACEMENT i S:PATERN:REPLACEMENT
//nejprve rozdeli prikazy na slova dle PATERN a REPLACEMENT, pote najde
//v radku prvni vyskyt PATERN a nahradi ho slovem v REPLACEMENT
//vraci 1, pokud je vse ok, 0 pokud se vyskytla chyba a -1 pokud vzor neni na radku
int replacePart(char *pattern, char *replacement, char *currentLine, char *handledPart, int lineLenght)
{
    bool overflow = true;
    //pomocny retezec obbsahujici aktualni radek od vyskztu vzoru
    char *tempStr = strstr(currentLine, pattern);
    if(tempStr)
    {
        //definice casti pred a za vzorem
        char firstPart[lineLenght]; //velikost stejna jako radku, nemuze prekrocit
        int lenghtOfFirstPart = strlen(currentLine)-strlen(tempStr);
        char secondPart[strlen(tempStr)-strlen(pattern)+1];          // +1 pro nulovy znak
        //zkopirovani casti pred vzorem do promenne
        memcpy(firstPart, currentLine, lenghtOfFirstPart);
        firstPart[lenghtOfFirstPart] = '\0'; 					     //pridani nuloveho znaku
        //zkopirovani casti za vzorem do promenne
        strcpy(secondPart, tempStr+strlen(pattern));
        secondPart[strlen(secondPart)] = '\0'; 					     //pridani nuloveho znaku
        //spojovani a kontrola preteceni
        int resultLenght = strlen(firstPart)+strlen(replacement);    //deklarace promenne drzici delku retezce po spojeni
        if(resultLenght <= lineLenght)                               //spojeni casti pred a nahrady za vzor
        {                                                            //pokud by nedoslo k rpeteceni
            strcat(firstPart, replacement);
            resultLenght = strlen(handledPart)+strlen(firstPart);
            if(resultLenght <= lineLenght)  //zkopirovani casti pred vzorem+vzor do zpracovaneho
            {                                                        //pokud by pak zpracovana cast nebyla vetsi nez delka radku
                 strcat(handledPart, firstPart);
                 resultLenght = strlen(firstPart)+strlen(secondPart);
                 if(resultLenght <= lineLenght)
                 {
                     strcat(firstPart, secondPart);                  //pripojeni i druhe casti
                     overflow = false;
                 }
            }
        }
        if(!overflow)
        {                                        //pokud je vse v mezich pameti
             strcpy(currentLine, firstPart);     //prekopiruje se vysledek do aktualniho radku
             return 1;	                         //vraci 1 jako uspech
        }
        else    //pokud by doslo k hrozbe preteceni, vypise chybu a vrati 0 - konec programu
        {
            fprintf(stderr, "U prikazu s nahrazovani vzoru byla snaha vytvorit radek delsi nez %d znaku!\n"
                            "Program predcasne ukoncen.\n", lineLenght);
            return 0;
        }
    }
    return -1; //pokud vyor nebyl nalezen, vrati -1
}

//funkce bere ukazatel na citac v cyklu prikazu a dany prikaz, z prikazu vezme cislo poradi
//prikazu na ktere ma skocit -2, prvni -1 protoze prikazy mame v poli a to se indexuje od 0
//podruhe -1 protoze se stihne dokoncit iterace a citac se opet navysi o 1 -> proto -2
bool goTo(char *command, int commandLenght, FILE *commandFile, bool *detectNewLine)
{
    if(*detectNewLine == false)
    {
        fprintf(stderr, "Hrozi zacykleni prikazu goto!\nProgram predcasne ukoncen.\n");
        return 0;
    }
    int numOfLine;
    char tmpStr[commandLenght];
    if(sscanf(command, "%d", &numOfLine) && numOfLine>0) //pretypovani retezce na int
    {
       fseek( commandFile, 0, SEEK_SET);                //nastavim kurzow na zacatek souboru
       for(int i = 1; i<numOfLine; i++)
          if(fgets(tmpStr, sizeof(tmpStr), commandFile) == NULL)
          {
             fprintf(stderr, "Prikaz goto se snazi skocit na neexistujici prikaz!\n"
                             "Program je predcasne ukoncen.\n");
             return false;
          }
    }
    else //jestli sscanf selhal nebo je cislo zaporne
    {    //vypise hlasku a vrati false -> v mainu ukonci program
        fprintf(stderr, "Prikaz goto nema kam skocit!\n"
                        "Hodnota je zaporna, nulova nebo se nejedna o cislo.\n"
                        "Program je predcasne ukoncen.\n");
             return false;
    }
    return true;
}

/* nepovinne funkce */
//funkce bere nove radkz dokud nenarazi na takovy, ve kterem je vzor
bool getPatternLine(char *pattern, char *currentLine, int currLineSize, FILE *tempFile)
{
    fputs(currentLine, tempFile);                   //zapise stavajici radek
    while(fgets(currentLine, currLineSize, stdin))  //dokud neni radek neni prazdny, zapisuje a bere dalsi
        if(strstr(currentLine, pattern))            //pokud je v radku vzor
            return true;                            //funkce je uspesne ukoncena
    return false;                                   //pokud se nenasel vzor a byl ukoncen input, funkce konci
}

//funcke prida aktualnimu retezci znak noveho radku, vzhledem k tomu, ze kazdy radek
//je timto znakem implicitne zakoncen, vytvori to prazdny radek ... nebo lze znovu doplnit po smazani
void appendEndOfLine(char *currentLine)
{
    int currLineLenght = strlen(currentLine);
    currentLine[currLineLenght] = '\n';
    currentLine[currLineLenght+1] = '\0';
}

//funkce bere jako parametrz prikazovy i zapisovaci soubor, udaje o aktualnim radku a o prikazu
//funkce zjisti, o jaky prikaz se jedna a dle toho zavola prislusnou funkci
int executeCommand(char *command, int commandLenght, FILE *commandFile, FILE *tempFile, char *currentLine, int lineLenght, bool *detectNewLine)
{
    //promenne k funkcim s a S PATTERN
    const int lenghtOfPatternAndReplacement = 1000;
    const int lenghtOfDivider = 2;
    char pattern[lenghtOfPatternAndReplacement];
    char replacement[lenghtOfPatternAndReplacement];
    char divider[lenghtOfDivider];
    char handledPart[lineLenght];
    handledPart[0] = '\0';     //nastaveni delky retezce na nulu - potreba k prvnimu volani strlen

    if(command[0] == 'n' || command[0] == 'd') //prikaz n/d znamena, ze senacte dalsi radek
      *detectNewLine = true;                   //posouvame se tedy ve vsupu, nedojde k zacykleni

    switch(command[0])         //dle prvniho znaku z prikazu zavola prislusnou funkci
    {
        case 'i': insertNewLine(command+1, tempFile);
             break;
        case 'a': if(!insertTextAfter(command+1, currentLine, lineLenght))
                  	return 0;
             break;
        case 'b': if(!insertTextBefore(command+1, currentLine, lineLenght))
                   	return 0;
             break;
        case 'n': ;int result=getAnotherLine(command, currentLine, lineLenght,tempFile);
                    switch(result)
                    {
                        case -1: return -1;   //pokud funkce vraci -1, pak jiz na vstupu nejsou data
                        case 0: return 0;     //ukoncime program
                    }
             break;
        case 'd': switch(deleteLine(command, currentLine, lineLenght))
                  {
                     case -1: return -1;   //pokud funcke vraci -1, pak jiz na vstupu nejsou data
                     case 0: return 0;     //ukoncime program
                  }
             break;
        case 'r': currentLine[strlen(currentLine)-1] = '\0'; //posune nulovy znak na pozici \n, tim \n odstrani
             break;
        case 'q': return -1;  //prikaz q se chova jako konec vstupu
             break;
        case 's': handledPart[0] = '\0';                                                      //vynulovani zpracovane casti radku
                  if(getDevider(divider, command) &&                                          //nejprve vyhleda delic vzoru a nahrady
                     getPatternAndReplacement(pattern, replacement, command, divider) &&      //najde-li delic, ziska i vzor a nahradu
                     replacePart(pattern, replacement, currentLine, handledPart, lineLenght)) //najde-li i vzor s nahradou, vymeni je
                     ;
		     	 else
                     return 0;                                                                //pokud cokoliv vyse selze, ukoncuje program
             break;
        case 'S': if(getDevider(divider, command) &&                                         //nejprve vyhleda delic vzoru a nahrady
                     getPatternAndReplacement(pattern, replacement, command, divider))       //najde-li delic, ziska i vzor a nahradu
                  {
                      handledPart[0] = '\0';    //vynulovani zpracovane casti radku
                      bool endLoop = false;
                      while(!endLoop)
                      switch(replacePart(pattern, replacement, currentLine+strlen(handledPart), handledPart, lineLenght))
                      {
                          case 1: break;
                          case 0: return false;
                             break;
                          case -1: endLoop = true;
                             break;
                       }
                  }
                  else
                     return 0;
             break;
        case 'g': if(!goTo(command+1, commandLenght, commandFile, detectNewLine))
                  	return 0;
             break;
        /* nepovinne */
        case 'f': if(!getPatternLine(command+1, currentLine, lineLenght, tempFile))
                     return -1; //pokud funcke vraci false, pak jiz na vstupu nejsou data
             break;
        case 'c':;char *pattern = strstr(command, " ")+1; //ziska vzor z prikazu
                  if(pattern) //jestli vzor existuje
                  {           //a na aktualnim radku se vyskytuje, pak preskoci
                     if(strstr(currentLine, pattern))
                        if(!goTo(command+1, commandLenght, commandFile, detectNewLine))
                           return 0;
                  }
                  else //jestli vzor neni, vypise chybu a ukonci program
                  {
                      fprintf(stderr, "Prikaz 'c' neobsahuje vzor!\nProgram predcasne ukoncen.\n");
                      return 0;
                  }
             break;
        case 'e': appendEndOfLine(currentLine);
             break;
        default: if(command[0])
                 {
                     fprintf(stderr, "Prikaz '%c' neni platny!\nProgram predcasne ukoncen.\n", command[0]);
                     return 0;
                 }
       }
       return 1;
}

/* FUNKCE MAIN */
int main(int argc, char *argv[])
{
    if(!getCommandFile(argv[1], argc)) // jestli soubor neni v argumentu, ukonci program
        return 0;

    //otevreni souboru s prikazy
    FILE* commandFile = fopen(argv[1], "r");
    if(!commandFile)             // pokud se soubor neotevrel, vypise chybu a ukonci program
    {
        fprintf(stderr, "Soubor se nepodarilo otevrit.\nZadejte spravne jmeno prikazoveho souboru.\n");
        return 0;
    }

    FILE *tempFile = tmpfile(); // vytvoreni docasneho souboru pro prubezny zapis

    // precteni prvniho radku od uzivatele
    const int lineLenght = 2000; //omezeni v zadani je minimalne 1000 znaku, tak zdvojnasobuji
    char currentLine[lineLenght];
    fgets(currentLine, lineLenght, stdin);

    bool endOfInput = false;        // kontroluje, jestli jsou data na vstupu
    bool detectNewLine = false;     //kontroluje, jestli mezi znovunactenymi prikazy je n/d, jinak zacykleni

    unsigned const int commandLenght = 1001; //prikaz omezuji na 1000 znaku +1 na \0
    char command[commandLenght];    //definice prikazu

    // cyklus projizdeni soubor s prikazy dokud nedojdou prikazy/data na vstupu == prikaz neni 'q'
    while(fgets(command, commandLenght+1, commandFile) && endOfInput == false)
    {
       if(strlen(command) > commandLenght-1) //zkontrolujeme, jestli prikaz neni prilis velky
       {
           fprintf(stderr, "Prikaz '%c' obsahuje prilis mnoho znaku. Maximalni povoleny pocet je %d znaku.\n"
                           "Program predcasne ukoncen.\n", command[0], commandLenght-1);
           fclose(commandFile);   //zavreme soubor a ukoncime program
           return 0;
       }
       //oddeli prikazu novy radek
       if(command[strlen(command)-1] == '\n')
          command[strlen(command)-1] = '\0';

       //vezme prikaz a provede ho, jestli vraci false, ukonci program
       switch(executeCommand(command, commandLenght, commandFile, tempFile, currentLine, lineLenght, &detectNewLine))
       {
           case 1: continue;              //nic se nedeje, vse je ok
              break;
           case -1:endOfInput = true;     //dosel vstup
              break;
           case 0: fclose(commandFile);   //zavreme soubor a ukoncime program
                   return 0;
       }
    }
    if(fclose(commandFile)) //zavreni souboru s prikazy
        fprintf(stderr, "Nepodarilo se zavrit prikazovy soubor!\n");

    //jestli je vice vstupnich radku nez pro kolik je prikazu, zkopiruj zbytek
    //dokud neni praydny radek nebo pokud se editace ukoncila prikazem 'q'
    while(endOfInput==false)
    {
        fputs(currentLine, tempFile); //zapise jeste posledni prikaz
        if(fgets(currentLine, lineLenght, stdin) == NULL) //jestli dalsi neni, konci cyklus
            endOfInput = true;
    }

    rewind(tempFile); //presunuti se na yacatek docasneho souboru pro tisk

    // kdyby se 2 plne radky spojily, tak at to stale utahne, vic uz vsak ne
    char modifiedLine[2*lineLenght];
    // tisk z docasneho souboru do stdout
    while(fgets(modifiedLine, sizeof(modifiedLine), tempFile))
        printf("%s", modifiedLine);

    return 0;
}
