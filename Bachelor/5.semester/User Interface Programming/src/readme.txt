Odevzdaný soubor obsahuje vše potřebné kromě knihoven třetích stran pro webovou aplikaci.
Těmi jsou Bootstrap v4.5.2, popper v2, jQuery, Font Awesome Free 5.15.0.
Ty lze jednoduše stáhnout z adresy http://www.stud.fit.vutbr.cz/~xloren15/itu-web-libs.zip.
Zip je třeba rozbalit do složky /Web/BooksWeb/wwwroot/lib.
Dále je třeba získat nuget balíčky služby pro klientské aplikace a pak už jen spustit.

Postup pro Visual Studio:
- Otevřte BooksService.sln v IDE Visual Studio.
- Pravým tlačítkem klikněte na "Solution 'BooksService'" a zvolte "Build Solution"
- Pravým tlačítkem klikněte na projekt "BooksService.Client" a zvolte "Pack".
- Pravým tlačítkem klikněte na projekt "BooksService.Common" a zvolte "Pack".
- Ze složky bin/Debug či bin/Release (v závislosti na nastaveném sestavovacím módu) jednotlivých projektů přesuňte soubory
  BooksService.Client[Common].1.0.0.nupkg do libovolné složky, která bude zdrojem nuget balíčků.
- V Tools->Options->NuGet Package Manager->Package Sources přidejte lokální složku s vašimi nuget balíčky.
- Spusťte službu tlačítkem Run.
- Otevřte BooksWeb.sln a BooksWpf.sln v IDE Visual Studio a taktéž spusťte.
(V případě problémů sestavení z důvodu nenalezených nuget balíčků může pomoci Visual Studio vypnout a zapnout.
Je možné, že si ještě nenačetlo nový zdroj balíčků.)

Příkazový řádek (vyžaduje mít nainstalovaný dotnet core a nuget.exe):
- dotnet pack <cesta_k_BooksService.Client.csproj> --output <nuget_slozka>
- dotnet pack <cesta_k_BooksService.Common.csproj> --output <nuget_slozka>
- nuget sources add -Source <nuget_slozka> -Name <nejake_jmeno>
- dotnet run --project <cesta_k_BooksService.csproj>
- dotnet run --project <cesta_k_BooksWeb.csproj>
- dotnet run --project <cesta_k_BooksWpf.csproj>

Databázi není třeba řešit. Při spuštění služby tato automaticky upraví databázi na nejnovější verzi dle migrací nebo ji vytvoří, pokud neexistuje.
Toto zařizuje třída MigrationStartupFilter. Je však potřeba mít MS SQL Server. Ve Windows tento existuje nativně.

Ve výsledku běží služba na localhost:12345 a web na localhost:5001. Databáze je vytvořená v localdb pod názvem BooksDb.

Cesta ke sdílené technické zprávě na google docs:
https://docs.google.com/document/d/1caeVDowmzpTdYR4_syoA0088W_oc0Qov6mA5ptFd71s/edit#
