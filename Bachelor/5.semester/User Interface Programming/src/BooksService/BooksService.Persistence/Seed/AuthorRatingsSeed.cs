using System;
using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class AuthorRatingsSeed
    {
        public static List<AuthorRating> Data => new List<AuthorRating>
        {
            new AuthorRating
            {
                Id = 1,
                UserId = 2,
                AuthorId = 2,
                Number = 10,
                Text = "Nejlepší autor všech dob. 20/10.",
                Date = new DateTime(2018, 4, 17)
            },
            new AuthorRating
            {
                Id = 2,
                UserId = 3,
                AuthorId = 2,
                Number = 10,
                Text = "Tolkiena považuji za mistra ve světě fantasy. To jak dokázal brilantně vymodelovat nový svět s celou historií a i jazyky jednotlivých národů je naprosto úžasné. Vše do sebe zapadá a nikde nic nedrhne. Na jeho knihy nedám dopustit. Snaha dnešních autorů se snad ani nepřibližuje jeho talentu... snad se k němu přiblížila Rowlingová, i když mnozí se mnou budou asi nesouhlasit. Jen škoda, že své dílo nestil dokončit... bůh ví jak by to vlastně všechno vypadalo až by byl se vším opravdu spokojený a měl vše dokončené.",
                Date = new DateTime(2018, 7, 30)
            },
            new AuthorRating
            {
                Id = 3,
                UserId = 4,
                AuthorId = 2,
                Number = 8,
                Text = "Geniální spisovatel, který definoval pojem fantasy. Vytvořil celý svůj fantasy svět, rasy, země, jazyky... Rád přicházel se svými jazyky a nechával lidi mluvit jimi. V knihách Hobit a trilogii Pán Prstenů se zrcadlí jeho bravurní znalosti a vzdělání v oblasti staré angličtiny a staré severštiny.",
                Date = new DateTime(2019, 11, 5)
            },
            new AuthorRating
            {
                Id = 4,
                UserId = 5,
                AuthorId = 4,
                Number = 2,
                Text = "Obdivuji odhodlání, obdivuji tvůrčí elán, ale bohužel mě tento spisovatel a jeho styl psaní vůbec nenadchnul. Na mě až moc násilný humor, až moc absurdní, ale ne v dobrém slova smyslu. Asi mám jinak nastavené meze vtipu :)",
                Date = new DateTime(2020, 8, 1)
            },
            new AuthorRating
            {
                Id = 5,
                UserId = 1,
                AuthorId = 3,
                Number = 10,
                Text = "George Orwell byl úžasný spisovatel, jeho díla mají hloubku, především se to týká jeho sociálních románů. Je známý především díky Farmě zvířat a 1984, ale já bych rozhodně doporučovala přečíst i Barmské dny, Cestu k Wigan Pier či Na dně v Paříži a Londýně, opravdu stojí za to. ",
                Date = new DateTime(2014, 8, 24)
            },
            new AuthorRating
            {
                Id = 6,
                UserId = 2,
                AuthorId = 5,
                Number = 8,
                Text = "Jsou autoři, od kterých můžete přečíst deset jejich knížek a nedokáží vás oslovit. A pak jsou autoři, od kterých vám stačí jedno dílo, a to vás ohromí natolik, že se vám nesmazatelně dostane pod kůži. A právě mezi ty druhé patří Neil Gaiman. Čtení jeho Hvězdného prachu považuju za jeden ze svých nejlepších a nejsilnějších čtenářských zážitků.",
                Date = new DateTime(2017, 4, 16)
            },
            new AuthorRating
            {
                Id = 7,
                UserId = 4,
                AuthorId = 7,
                Number = 7,
                Text = "Zajímavý člověk, jeho jediná kniha s R. Feynmannem je velice dobrá. Škoda, že v práci nepokračoval.",
                Date = new DateTime(2020, 8, 7)
            },
            new AuthorRating
            {
                Id = 8,
                UserId = 1,
                AuthorId = 8,
                Number = 10,
                Text = "Velmi inteligentní a motivující člověk, jenž se nebojí říkat věci, které dnešní společnost tak nerada slyší. Jeho výklad je srozumitelný a má hlavu i patu. Jeho přednášky dokáží člověka nakopnout.",
                Date = new DateTime(2020, 8, 7)
            },
            new AuthorRating
            {
                Id = 9,
                UserId = 3,
                AuthorId = 10,
                Number = 9,
                Text = "Hrdina mého dospívání! Právě Paolinimu a jeho Eragonovi vděčím za to, že jsem se tak po hlavě vrhla do čtení a za svou lásku k fantasy. Ve třinácti letech pro mě byl patnáctiletý kluk, který vydal světový bestseller vzorem a má za to můj obdiv i teď. I když jsem s dalšími roky našla v knihách chyby, které jsem předtím neviděla, bude mít Odkaz dračích jezdců v mém srdci nezastoupitelné místo. Škoda, že o něm už dlouho nebylo slyšet. Ráda bych si od tohoto autora přečetla i další knihy.",
                Date = new DateTime(2020, 8, 7)
            },
            new AuthorRating
            {
                Id = 10,
                UserId = 4,
                AuthorId = 10,
                Number = 8,
                Text = "Christopher Paolini je autor s nevídaným talentem. Jeho knihy jsou úžasné, čtivé a plné skrytých myšlenek. Svým stylem psaní mě ihned vtáhl do děje. Některé jeho myšlenky a postřehy mi daly hodně věcí do života. Určitě stojí za to si od něho něco přečíst!!!",
                Date = new DateTime(2020, 8, 7)
            },
            new AuthorRating
            {
                Id = 11,
                UserId = 1,
                AuthorId = 11,
                Number = 7,
                Text = "Stephen R. Covey se nesmazatelně zapsal do historie svými dnes už klasickými 'Sedmi návyky'. Jeho další práce týkající se osobního rozvoje, ale především teorie řízení jsou neméně důležité. Zabýval se tzv. leadershipem, tj. vedením lidí, aby dělali 'správné' věci, což je poněkud jiné než kdyby jenom dělali věci 'správně'.",
                Date = new DateTime(2020, 8, 7)
            },
            new AuthorRating
            {
                Id = 12,
                UserId = 2,
                AuthorId = 11,
                Number = 8,
                Text = "Stephen R. Covey je absolutní špička v knihách osobního rozvoje. Autor svůj život svůj život zasvětil principům jednání mezi lidmi takovým způsobem jaký dnes nemá obdoby. Šel skutečně do hloubky a v jeho knihách jsou skutečně rozpoznány cesty k zlepšení osobního života i vztahů ve svém okolí. V knihách nenajdete žádná klišé, jen pot a tvrdou dřinu. Coveyho teorie by se měly učit a zařadit do výuky škol nejdůležitějšímu předmětu VZTAHY KOLEM NÁS I V NÁS :)",
                Date = new DateTime(2020, 8, 7)
            },
            new AuthorRating
            {
                Id = 13,
                UserId = 2,
                AuthorId = 12,
                Number = 9,
                Text = "King připomíná aktivní vysílač, díky němuž přijímá z vesmíru do své hlavy, tuny kvalitních nápadů. Z nich pak sestaví jako architekt, fungující příběh. A nechá ho, napospas čtenářům. Poté se ozve hromové zvolání: Stephen King je Génius. Za mě, jeho NeJ - Misery, Osvícení a Řbitov domácích zvířátek, četl jsem 12 knih a i ta nejhořší si drží laťku poměrně vysoko, když člověk srovnává autory a za sebe můžu říci že mě bavily všechny. King je jen jeden a jde zřejmě o nejlepšího spisovatele, když uvážím že některé jeho zasadní díla, čekají stále na mou Premiéru. Tak smekám.",
                Date = new DateTime(2020, 8, 7)
            },
            new AuthorRating
            {
                Id = 14,
                UserId = 5,
                AuthorId = 12,
                Number = 10,
                Text = "Pojem a výraz Stephen King zná snad každý pořádný čtenář napínavých thrilerů a temných hororů. Tento člověk mě dostal už někdy v mých patnácti letech a od těch dob jsem se nikdy nedokázal nabažit jeho knih. První knihu, kterou jsem od něho četl byla Žhářka, nebyla moc výrazná, ale natolik mě nalákala na další mistrovo knihy, že jsem nabýdku přijal a do dnes od ní neupustil. Asi nejlepší příběh, který jsem od něj četl je Zelená míle, ale to je jen malá špice celého obrovského ledovce, který na nás King po celý život zuřivně valí. Přečetl jsem snad vše, co se u nás dalo sehnat. Mám ještě pár restíků a samozřejmě se těším na jeho další knihy. Doufám, že bude ještě dlouho psát. Každopádně je to nezapomenutelná osobnost, kterou zná snad celý svět.",
                Date = new DateTime(2020, 8, 7)
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AuthorRating>().HasData(Data);
    }
}
