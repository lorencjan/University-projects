using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksService.Persistence.Migrations
{
    public partial class NewNewsPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BookRatings",
                keyColumn: "Id",
                keyValue: 1,
                column: "Text",
                value: @"Jeden ze základních pilířů přístupu k efektivní práci, ať už je tou prací myšleno cokoliv.
Pro mě hodně užitečné zejména kvadranty a myšlenka na konec, no a nesmím zapomenut na ostření pily.
Souhlasím, že kniha je rozvláčná, ale kdyby si člověk odnesl jen ty základní myšlenky, i to je velký přínos.");

            migrationBuilder.UpdateData(
                table: "BookRatings",
                keyColumn: "Id",
                keyValue: 5,
                column: "Text",
                value: @"Série o dracích od Christophera Paoliniho jeho legendární a krásná.
Odkaz dračích jezdců mě dojal a nadchl. Jde o úžasné fantasy ze světa nádherných a děsivých tvorů - draků.
Příběh o přátelství, dobrodružství, osudu a naději. Kniha je čtivá a má tak akorát délku.
Velice mě zaujalo, že autor byl velice mladý, když knihu napsal, nechápu to. Jde o jednu z nejlepších fantasy sérií. Rozhodně by si ji měl přečíst ten, kdo ji ještě nezná.");

            migrationBuilder.UpdateData(
                table: "NewsPosts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Text",
                value: @"Vážení čtěnáři a především skalní fanoušci Harryho Pottera,
rádi bychom Vám oznámili, že J.Rowlingová potvrdila účast na autogramiádě fantasy autorů v Praze. Určitě si tuto příležitost nenechte ujít!

Kdy: 12.2.2021
Kde: Obecní dům, náměstí Republiky 1090/5, 11000 Praha, Staré Město");

            migrationBuilder.UpdateData(
                table: "NewsPosts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: @"Nadšenci do fantasy, zpozorněte! Chystáme se rozšířit naši databázi v oblasti Fantasy, budeme rádi za vaše návrhy, které knihy byste tu chtěli mít. Napsat nám můžete prostřednictvím feedbacku.
Děkujeme.");

            migrationBuilder.InsertData(
                table: "NewsPosts",
                columns: new[] { "Id", "Date", "Header", "Text" },
                values: new object[,]
                {
                    { 3, new DateTime(2020, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Knižní novinky Říjen 2020", @"Tento podzim nám dává dost zabrat. Spousty lidí zůstává doma na nucené pracovní pauze, spousty lidí zažívají pocit nejistoty. Ale víte co jisté naopak je? Že i tento týden vyšlo množství perfektních knih, které stojí za vaši pozornost. A že jsou to novinky honosící se velkými jmény, na to vemte jed.
                                        Hned odstartujeme nejočekávanější knihou (nebo alespoň dle čtenářských preferencí a zájmů). Autorské duo nesoucí jméno Lars Kepler vydalo již osmý díl ze série s komisařem Joono Linnou. Zrcadlový muž vypráví příběh o mladé dívce, která zmizí cestou ze školy. O pět let později se její tělo objeví v centru Stockholmu. Díky bezpečnostním kamerám se podaří najít očitého svědka, jenže se ukáže, že jde o psychicky nemocného muže. Z toho, co viděl, si vůbec nic nepamatuje. Pomůže Joonovi odhalit pravdu hypnotizér?
                                        Druhého volného pokračování se dočkala i série Tatérka. Tentokrát nese název Inkoust. V temných uličkách Brightonu kdosi napadl mladou ženu. Zranění se zprvu nezdají vážná, ale oběť za 24 hodin umírá a na jejím těle se najde smrtící tetování. Po zmizení druhé ženy už inspektor Sullivan ví, že čelí sériovému vrahovi. Jedno ale nečekal: hlavním podezřelým je syn tatérky a jeho bývalé milenky Marni. Dokáže se přenést přes minulost, než se rozsudek smrti objeví na kůži další oběti?
                                        A ještě chvilku posečkejme u očekávaných thrillerů tohoto měsíce. Čtvrtá kniha autora Andrew Maynea je na pultech s názvem Temný vzorec. Doktor Theo Cray je proslulý tím, že za využití matematických metod dokáže vypátrat sériové vrahy. Jenže pak zůstává působením patogenu ovlivňujícímu mozkovou činnost a který z něj samotného může udělat zabijáka, sám jen s pochybnostmi o vlastní příčetnosti. Aby zachránil nevinné životy i sám sebe, sleduje Cray každé nové vodítko v případu vyšinuté zdravotní sestry, přestože pomalu ztrácí kontakt s realitou. Musí čelit vlastní temné stránce a založit své pátrání pouze na intuici...
                                        Ale v říjnu nevychází jen thrillery. Důkazem je toho Sladká pomsta. Humorně laděná kniha od oblíbeného švédského autora Jonase Jonassona vás nenechá během své nepředvídatelné jízdy napříč kontinenty a světem umění vydechnout.
                                        Psí slib je knihou určenou pro milovníky psů. Jedná se opět o třetí díl ze série Psí poslání a zaručuje srdceryvné a dojemné čtení.
                                        A na závěr upozorněme ještě na jednu novinku ze žánru fantasy. Kamenné nebe potěší čtenáře série Zlomená země a přinese jim další dobrodružství a vlnu napětí spojenou se zkázou země - tentokrát tou poslední... " },
                    { 4, new DateTime(2020, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Desátý ročník KNIHOMÁNIE startuje v neděli 1. 11", @"KNIHOMÁNIE je každoročně vrcholem knižní sezóny. Jejím hlavním cílem je nabídnout čtenářům to nejlepší, co přináší podzimní knižní trh, a také podpořit knihkupce v kamenných prodejnách.
                                        Letošní nabídka 10. ročníku přináší 26 novinek od 18 nakladatelů. Z nich si mohou čtenáři vybírat od 1. do 22. listopadu 2020 v téměř 300 knihkupectvích po celé České republice.
                                        I tentokrát jsou v nabídce zajímavé tituly různých žánrů české i světové literatury – thrillery, historické romány, životní osudy, kuchařky, populárně naučná literatura i knihy pro děti. A samozřejmě i letos zákazníci obdrží ke každé zakoupené knize dárek – knihu 222 křížovek." },
                    { 5, new DateTime(2020, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kniha roku je vyhlášena!", "Druhý ročník čtenářské ankety Kniha roku, jejímž vyhlašovatelem je Nadační fond Čtení tě mění, už zná své vítěze. V hlasování veřejnosti na webu www.kniharoku.cz o nich rozhodlo 119 978 čtenářských hlasů." }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "1s2SxGaHNw3d2aMvhnX5XLNwE9UDXb25DtyEtj5LSCY=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "oePtRbZish+mWvzemVbd7fGKPP+zKSbBjIs8X2NAd/E=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "qiJHGeZNohbZQXB621kpFBxxk54ezOvJGbDp4s+Dgkk=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "GwdBvKhgOD2f0CIFo2VEG0VoYDk29yDI0+ESrFmXnI0=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Password",
                value: "jypWKta/UcB6O2aivMeBtWzl/qqAI1EVMKveX282ADc=");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NewsPosts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NewsPosts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NewsPosts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "BookRatings",
                keyColumn: "Id",
                keyValue: 1,
                column: "Text",
                value: @"Jeden ze základních pilířů přístupu k efektivní práci, ať už je tou prací myšleno cokoliv.
Pro mě hodně užitečné zejména kvadranty a myšlenka na konec, no a nesmím zapomenut na ostření pily.
Souhlasím, že kniha je rozvláčná, ale kdyby si člověk odnesl jen ty základní myšlenky, i to je velký přínos.");

            migrationBuilder.UpdateData(
                table: "BookRatings",
                keyColumn: "Id",
                keyValue: 5,
                column: "Text",
                value: @"Série o dracích od Christophera Paoliniho jeho legendární a krásná.
Odkaz dračích jezdců mě dojal a nadchl. Jde o úžasné fantasy ze světa nádherných a děsivých tvorů - draků.
Příběh o přátelství, dobrodružství, osudu a naději. Kniha je čtivá a má tak akorát délku.
Velice mě zaujalo, že autor byl velice mladý, když knihu napsal, nechápu to. Jde o jednu z nejlepších fantasy sérií. Rozhodně by si ji měl přečíst ten, kdo ji ještě nezná.");

            migrationBuilder.UpdateData(
                table: "NewsPosts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Text",
                value: @"Vážení čtěnáři a především skalní fanoušci Harryho Pottera,
rádi bychom Vám oznámili, že J.Rowlingová potvrdila účast na autogramiádě fantasy autorů v Praze. Určitě si tuto příležitost nenechte ujít!

Kdy: 12.2.2021
Kde: Obecní dům, náměstí Republiky 1090/5, 11000 Praha, Staré Město");

            migrationBuilder.UpdateData(
                table: "NewsPosts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Text",
                value: @"Nadšenci do fantasy, zpozorněte! Chystáme se rozšířit naši databázi v oblasti Fantasy, budeme rádi za vaše návrhy, které knihy byste tu chtěli mít. Napsat nám můžete prostřednictvím feedbacku.
Děkujeme.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "Rl9fIogoo15GWQnDevLx/Yyyx12uFrRPUfadYLDH6h0=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "B5dUHscXFnb7qaT0ibr3YXta1FBlSAhmIvnCexEwTJg=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "yMAEyxq/Yv5rXbPzw3AEBpU8jSGBAv1+gfBFkN5UnxY=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "JqBuyQbCpf0AnUr1A67hYlOiwCs5TrnmMJvSfFHlCI4=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Password",
                value: "cVKSZbaByoNvKiEJwy1edgDFiv+K7uqApS22Wk5fBOg=");
        }
    }
}
