using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksService.Persistence.Migrations
{
    public partial class AuthorRatingServiceUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 8,
                column: "Date",
                value: new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 9,
                column: "Date",
                value: new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 10,
                column: "Date",
                value: new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 11,
                column: "Date",
                value: new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 12,
                column: "Date",
                value: new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 13,
                column: "Date",
                value: new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 14,
                column: "Date",
                value: new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                value: "yG+rYn1CGjHWejESORvxA0IwMmnooCaqegkpA/h1VYU=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Password",
                value: "YBPx1o/ozoTZDtBZYCUAzttyWxp8/vVblt9eXKtjWLc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Password",
                value: "LPzNdUO9Lyf1GvbFZLa4RVcys7YoBikwGGXNLPrPisY=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "Password",
                value: "G74t5RtO6WMmQf+mYNF11o5eNrqUfjzw72ZhJdlmawk=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Password",
                value: "F0PyAa8X8oGq22R7IiWBsxRnrX/m1r6Ouz5WZDd3dUI=");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 8,
                column: "Date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 9,
                column: "Date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 10,
                column: "Date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 11,
                column: "Date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 12,
                column: "Date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 13,
                column: "Date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AuthorRatings",
                keyColumn: "Id",
                keyValue: 14,
                column: "Date",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
    }
}
