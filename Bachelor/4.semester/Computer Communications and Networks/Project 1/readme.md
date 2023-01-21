# IPK - 1.projekt: HTTP resolver doménových jmen
* Autor: Jan Lorenc (xloren15)
* Datum: 22.2.2020
* Škola: Fakulta informačních technologií VUT Brno

## Obecný popis
Výsledný program tohoto projektu je server poslouchající na zadaném portu.
Umí zpracovat celkem dvě operace. První je `GET`, která vyžaduje dotaz `resolve` s parametry `name` reprezentující jméno serveru / jeho ip adresu a `type` specifikující, jestli chceme zadaný `name` přeložit na ip adresu či jméno serveru. Druhou operací je `POST` fungující naprosto stejně, pouze vstup je zadán ve formě souboru se seznamem cílů ve formátu "`name`:`type`", dále vyžaduje dotaz `dns-query` v url.

## Implementace
Server je psán v jazyce C# na platformě .NET Core 3.1 a jeho zdrojové soubory lze nalézt ve složce `src`.
Projekt obsahuje celkem 3 soubory:
* `Server.csproj` obsahuje samotnou konfiguraci projektu
* `Program.cs` implementuje spouštěcí metodu programu `main()`, ve které se zkontrolují vstupní parametry a následně se spustí server. Program vyžaduje právě jeden parametr a to v rozsahu `UInt16`, neboť jiných hodnot socket nemůže nabývat.
* `Server.cs` je hlavní třídou programu. V konstruktoru převezme jméno serveru a číslo portu, na kterém má poslouchat. V metodě `Run()` pak dochází ke spuštění serveru, tedy napojení na endpoint, poslouchání klientských požadavků a vyřizování již zmíněných operací `GET` a `POST`.
  
## Použití
Program ke svému chodu nevyžaduje žádné zvláštní zacházení. Je třeba mít pouze nainstalovaný dotnet core 3.1 a poté stačí zadat `dotnet run CISLO_PORTU`, případně přidat `-p CESTA_K_PROJEKTU`, pokud nejsme ve stejné složce. V případě nainstalovaného nástroje `make` stačí ze staženého adresáře zadat `make run PORT=CISLO_PORTU`.