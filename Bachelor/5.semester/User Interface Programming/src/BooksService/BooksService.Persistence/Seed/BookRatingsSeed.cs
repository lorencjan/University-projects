using System;
using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class BookRatingsSeed
    {
        public static List<BookRating> Data => new List<BookRating>
        {
            new BookRating
            {
                Id = 1,
                UserId = 1,
                BookId = 6,
                Number = 8,
                Text = "Jeden ze základních pilířů přístupu k efektivní práci, ať už je tou prací myšleno cokoliv.\nPro mě hodně užitečné zejména kvadranty a myšlenka na konec, no a nesmím zapomenut na ostření pily.\nSouhlasím, že kniha je rozvláčná, ale kdyby si člověk odnesl jen ty základní myšlenky, i to je velký přínos.",
                Date = new DateTime(2018, 5, 18)
            },
            new BookRating
            {
                Id = 2,
                UserId = 2,
                BookId = 6,
                Number = 6,
                Text = "Nechci popirat, ze v knize jsou dobre myslenky a koncepty, ale bohuzel me nudil jejich podil na mnozstvi textu. Mela jsem behem cteni silny pocit, ze autor rad povida, rad se posloucha a opravdu moc rad generalizuje. Naopak pouziti prikladu z jeho rodinneho zivota nebo ze seminaru hodnotim vice mene kladne, to se cetlo dobre. I kdyz nektera prirovnani byla trochu pritazena za vlasy, napriklad dcera alias husa, co snasi zlata vejce, nebo vyvozovani zaveru pro business z toho, ze se mu rozbila sekacka na travu. Nektere situacni priklady jsou typicke pro americke prostredi a na moji zivotni realitu moc nefunguji, ale prave ty rodinne prihody jsou univerzalni a tech je vetsina. Neni to spatne a mozna je to ve svem zanru vyjimecne - knihy o osobnim rozvoji prakticky nectu, ale vracet se k tehle nebudu.",
                Date = new DateTime(2015, 8, 14)
            },
            new BookRating
            {
                Id = 3,
                UserId = 2,
                BookId = 3,
                Number = 10,
                Text = "V době, kdy tato knížka vyšla poprvé, se proslavila hlavně proto, že ji napsal patnáctiletý kluk, což mě zrovna dvakrát neoslovilo navzdory tomu, že v té době mi bylo přibližně stejně. Film mi mé odhodlání knížku nečíst ještě více potvrdil, protože to byl prostě propadák. Avšak po čase jsem si k Eragonovi konečně našla cestu a musím uznat, že knížka je opravdu dobře koncipovaná a s filmem má společný možná tak jen ten název. Eragon je velmi komplexní postava a Safira je jednoduše nádherné svérázné stvoření. A jim prostě nelze než nefandit. Nakonec musím Paoliniho obdivovat za to, že si dokázal všechno tak důkladně promyslet a vymyslet, včetně vlastních jazyků elfů a trpaslíků.",
                Date = new DateTime(2019, 9, 18)
            },
            new BookRating
            {
                Id = 4,
                UserId = 2,
                BookId = 4,
                Number = 10,
                Text = "Ve druhém díle série o Eragonovi se víc a víc otevírá svět Alagaesie. Paradoxně Eragon nepatří mezi mé nejoblíbenější postavy této série, byť ho mám ráda, ale třeba takového Murtagha mám radši (ale ne celou dobu). Naopak takový Roran a části s ním mě příliš nebaví, obešla bych se bez něj. Závěr knihy je tak napínavý, že člověk musí hned sáhnout po třetím dílu. Asi nedovedu hodnotit, jestli byl lepší díl tento, nebo ten první, parádní jsou oba dva.",
                Date = new DateTime(2019, 11, 1)
            },
            new BookRating
            {
                Id = 5,
                UserId = 3,
                BookId = 3,
                Number = 10,
                Text = "Série o dracích od Christophera Paoliniho jeho legendární a krásná.\nOdkaz dračích jezdců mě dojal a nadchl. Jde o úžasné fantasy ze světa nádherných a děsivých tvorů - draků.\nPříběh o přátelství, dobrodružství, osudu a naději. Kniha je čtivá a má tak akorát délku.\nVelice mě zaujalo, že autor byl velice mladý, když knihu napsal, nechápu to. Jde o jednu z nejlepších fantasy sérií. Rozhodně by si ji měl přečíst ten, kdo ji ještě nezná.",
                Date = new DateTime(2017, 1, 8)
            },
            new BookRating
            {
                Id = 6,
                UserId = 3,
                BookId = 4,
                Number = 8,
                Text = "Tuto knihu jsem četl již před nějakou dobou, ale vzpomínám si, že mě velmi bavila. Rozhodně doporučuji.",
                Date = new DateTime(2020, 5, 16)
            },
            new BookRating
            {
                Id = 7,
                UserId = 3,
                BookId = 1,
                Number = 10,
                Text = "Přečteno v el.podobě, doporučuji přečíst zejména pravidlo 10, s autorem naprosto souhlasím, Jordan zveřejňuje na internetu svoje velmi zajímavé přednášky - stojí za shlédnutí!",
                Date = new DateTime(2017, 6, 28)
            },
            new BookRating
            {
                Id = 8,
                UserId = 4,
                BookId = 1,
                Number = 10,
                Text = "Bravo! Pan Psycholog s velkým P. Rád sleduji jeho přednášky a podle mě je to jeden z předních psychologů západního světa. Jeden z mála, který se nebojí říct pravdu do očí a říct ji takovou jaká je, bez toho, aniž by se snažil podlézat současným trendům a postmodernímu populicionalismu, který vede k degradaci všech základních hodnot naší společnosti. Jistě, ne všem se musí líbit...není jednoduché podívat se pravdě do očí.",
                Date = new DateTime(2018, 4, 19)
            },
            new BookRating
            {
                Id = 9,
                UserId = 5,
                BookId = 1,
                Number = 4,
                Text = "Souhlasím že to není kniha na oddech ale skutečně k zamyšlení a na další dobu. V něčem s autorem souhlasím a v něčem naopak vůbec. Nepřišlo mi to až tak přínosné jako spíš k zamyšlení nad chováním lidstva a společností jako takovou. Zajímavé.",
                Date = new DateTime(2017, 4, 14)
            },
            new BookRating
            {
                Id = 10,
                UserId = 5,
                BookId = 22,
                Number = 6,
                Text = "Trochu jsem se obával, že se mi kniha nebude líbit, protože tento druh humorné literatury příliš nevyhledávám - ale nakonec mám z knihy vcelku dobrý pocit, líbila se. Sice mě to neláká zkoušet další pratchettovky, ale už aspoň chápu, proč je tolik čtenářů z jeho knih tak nadšených.",
                Date = new DateTime(2015, 4, 20)
            },
            new BookRating
            {
                Id = 11,
                UserId = 2,
                BookId = 22,
                Number = 10,
                Text = "Nejlepší knížka. Miluju <3",
                Date = new DateTime(2020, 10, 8)
            },
            new BookRating
            {
                Id = 12,
                UserId = 2,
                BookId = 18,
                Number = 6,
                Text = "Příjemné, vtipné, zajímavé. Vědci a vědátoři mají nejen svůj šedý svět čísel a bádání. Bavila mě kniha, bavila jsem se příběhy a něco se i dozvěděla.",
                Date = new DateTime(2016, 5, 6)
            },
            new BookRating
            {
                Id = 13,
                UserId = 4,
                BookId = 22,
                Number = 9,
                Text = "Skvělá pratchettovka, u které se rozhodně nebudete nudit. Elánia-trpaslíka Karotku-lady Berankinovou a celý sbor Noční hlídky si prostě musíte zamilovat.",
                Date = new DateTime(2019, 2, 10)
            },
            new BookRating
            {
                Id = 14,
                UserId = 2,
                BookId = 20,
                Number = 8,
                Text = "Miluji humor obou pánů autorů. Roky jsem se ovšem jejich spojení vyhýbala ze strachu, že nejsou tak úplně kompatibilní.",
                Date = new DateTime(2011, 10, 24)
            },
            new BookRating
            {
                Id = 15,
                UserId = 3,
                BookId = 12,
                Number = 3,
                Text = "Pratchettův humor je populistický jako Troškovy filmy. Jeho knihy jsou prefabrikáty z cihelny.",
                Date = new DateTime(2015, 6, 13)
            },
            new BookRating
            {
                Id = 16,
                UserId = 2,
                BookId = 21,
                Number = 1,
                Text = "Knížka je zdlouhavá, ukecaná (ukecaná tak nějak o ničem) a hlavně nudná.",
                Date = new DateTime(2013, 5, 21)
            },
            new BookRating
            {
                Id = 17,
                UserId = 5,
                BookId = 15,
                Number = 10,
                Text = "'Spousta těch, co žijí, zaslouží smrt. A někdo umírá a zasluhuje život. Můžeš mu ho dát? Potom nevynášej příliš horlivě rozsudky smrti. Protože ani ti nejmoudřejší nedohlédnou do všech konců.' Z deseti bodů bych dal jedenáct.",
                Date = new DateTime(2013, 2, 28)
            },
            new BookRating
            {
                Id = 18,
                UserId = 5,
                BookId = 16,
                Number = 10,
                Text = "'V jedné věci ses nezměnil, můj drahý příteli,' řekl Aragorn. 'Pořád mluvíš v hádankách.' 'Cože? V hádankách?' řekl Gandalf. 'Ne, mluvil jsem nahlas k sobě. To už je zvyk starců: vyberou si nejmoudřejšího z přítomných a k němu mluví; dlouhá vysvětlení, která potřebují mladí, jsou únavná.' Opět mistrovské dílo, 11/10.",
                Date = new DateTime(2013, 2, 28)
            },
            new BookRating
            {
                Id = 19,
                UserId = 5,
                BookId = 17,
                Number = 10,
                Text = "'Spousta lidí chce vědět napřed, co bude na stole; ale ti, kdo se namáhali s přípravou hostiny, rádi zachovávají tajemství; údiv přece dává chvále hlasitější zvuk.' 12/10.",
                Date = new DateTime(2013, 2, 28)
            },
            new BookRating
            {
                Id = 20,
                UserId = 1,
                BookId = 19,
                Number = 10,
                Text = "Tuhle knihu bychom měli zavést jako povinnou četbu. Pak už by snad nikoho z mladé generace nenapadlo volit komunisty...",
                Date = new DateTime(2014, 6, 18)
            },
            new BookRating
            {
                Id = 21,
                UserId = 4,
                BookId = 19,
                Number = 4,
                Text = "Tahle kniha mě vlastně nikdy nelákala. Nakonec jsem ji také přečetl - z povinnosti – byl jsem udolán tím všeobecným nadšením z knihy, tou neustálou glorifikací díla i jejího autora. Výsledek jsem tak trochu čekal – příběh mě nezaujal.",
                Date = new DateTime(2013, 5, 6)
            },
            new BookRating
            {
                Id = 22,
                UserId = 1,
                BookId = 18,
                Number = 9,
                Text = "První kniha, kterou jsem četl ze čtečky a na to, že jsem ji četl jen v metru cestou ze školy a do školy, přečetl jsem ji celkem rychle. Parádní kniha, Feynman byl rozhodně zajímavý a výjimečně inteligentní člověk.",
                Date = new DateTime(2020, 5, 7)
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<BookRating>().HasData(Data);
    }
}
