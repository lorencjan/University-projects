# IPK - 2.projekt: ZETA: Sniffer paketů
* Autor: Jan Lorenc (xloren15)
* Datum: 12.4.2020
* Škola: Fakulta informačních technologií VUT Brno

## Obecný popis
Výsledný program tohoto projektu je sniffer schopný zachytávat TCP a UDP pakety na daném síťovém rozhraní.

## Implementace
Program je psán v jazyce C# na platformě .NET Core 3.1 a jeho zdrojové soubory lze nalézt ve složce `src`.
Projekt obsahuje celkem 5 souborů:
* `ipk-sniffer.csproj` obsahuje samotnou konfiguraci projektu.
* `Program.cs` implementuje spouštěcí metodu programu `main()`, ve které se zkontrolují vstupní parametry a následně se spustí zachytávání paketů.
* `Arguments.cs` obsahuje třídu starající se o kontrolu a zpracování vstupních argumentů programu.
* `Sniffer.cs` obsahuje třídu implementující samotné zachytávání paketů a jejich výsledný výpis.
* `SniffedPacket.cs` reprezentuje zachycený paket ve formátu pro výstup.
  
## Použití
Program ke svému chodu vyžaduje pouze nainstalovaný dotnet core 3.1. Mimo základ tohoto frameworku využívá knihovnu SharpPcap 5.1, nicméně ta je zahrnuta formou nuget balíku, tedy se při sestavení automaticky stáhne. Manuálně to lze provést příkazem `dotnet restore ./src/ipk-sniffer.csproj` nebo `make restore`, který tento příkaz obaluje. Sestavit projekt lze podobně: `dotnet build ./src/ipk-sniffer.csproj` nebo `make`/`make build`. Pro mnoho a variabilních argumentů programu nemá makefile příkaz `run`, program se již musí spustit manuálně. Spustitelný program se po sestavení nachází v adresáři `./src/bin/Debug/netcoreapp3.1` a odtud lze spustit dle zadání: `./ipk-sniffer {argumenty}`
Příklad spuštění na referenčním stroji po rozbalení projektu:
`make`
`./src/bin/Debug/netcoreapp3.1/ipk-sniffer -i enp0s3 -n 2` 

## Seznam souborů
README.md
manual.pdf
Makefile
src/ipk-sniffer.csproj
src/Program.cs
src/Arguments.cs
src/Sniffer.cs
src/SniffedPacket.cs