using System;
using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class NewsPostsSeed
    {
        public static List<NewsPost> Data => new List<NewsPost>
        {
            new NewsPost
            {
                Id = 1,
                Header = "Autogramiáda J. Rowlingové v Praze",
                Text = "Vážení čtěnáři a především skalní fanoušci Harryho Pottera,\nrádi bychom Vám oznámili, že J.Rowlingová potvrdila účast na autogramiádě fantasy autorů v Praze. Určitě si tuto příležitost nenechte ujít!\n\nKdy: 12.2.2021\nKde: Obecní dům, náměstí Republiky 1090/5, 11000 Praha, Staré Město",
                Date = new DateTime(2020, 10, 10),
            },
            new NewsPost
            {
                Id = 2,
                Header = "Sekce Fantasy",
                Text = "Nadšenci do fantasy, zpozorněte! Chystáme se rozšířit naši databázi v oblasti Fantasy, budeme rádi za vaše návrhy, které knihy byste tu chtěli mít. Napsat nám můžete prostřednictvím feedbacku.\nDěkujeme.",
                Date = new DateTime(2020, 10, 21)
            },
            new NewsPost
            {
                Id = 3,
                Header = "Knižní novinky Říjen 2020",
                Text = @"Tento podzim nám dává dost zabrat. Spousty lidí zůstává doma na nucené pracovní pauze, spousty lidí zažívají pocit nejistoty. Ale víte co jisté naopak je? Že i tento týden vyšlo množství perfektních knih, které stojí za vaši pozornost. A že jsou to novinky honosící se velkými jmény, na to vemte jed.
                        Hned odstartujeme nejočekávanější knihou (nebo alespoň dle čtenářských preferencí a zájmů). Autorské duo nesoucí jméno Lars Kepler vydalo již osmý díl ze série s komisařem Joono Linnou. Zrcadlový muž vypráví příběh o mladé dívce, která zmizí cestou ze školy. O pět let později se její tělo objeví v centru Stockholmu. Díky bezpečnostním kamerám se podaří najít očitého svědka, jenže se ukáže, že jde o psychicky nemocného muže. Z toho, co viděl, si vůbec nic nepamatuje. Pomůže Joonovi odhalit pravdu hypnotizér?
                        Druhého volného pokračování se dočkala i série Tatérka. Tentokrát nese název Inkoust. V temných uličkách Brightonu kdosi napadl mladou ženu. Zranění se zprvu nezdají vážná, ale oběť za 24 hodin umírá a na jejím těle se najde smrtící tetování. Po zmizení druhé ženy už inspektor Sullivan ví, že čelí sériovému vrahovi. Jedno ale nečekal: hlavním podezřelým je syn tatérky a jeho bývalé milenky Marni. Dokáže se přenést přes minulost, než se rozsudek smrti objeví na kůži další oběti?
                        A ještě chvilku posečkejme u očekávaných thrillerů tohoto měsíce. Čtvrtá kniha autora Andrew Maynea je na pultech s názvem Temný vzorec. Doktor Theo Cray je proslulý tím, že za využití matematických metod dokáže vypátrat sériové vrahy. Jenže pak zůstává působením patogenu ovlivňujícímu mozkovou činnost a který z něj samotného může udělat zabijáka, sám jen s pochybnostmi o vlastní příčetnosti. Aby zachránil nevinné životy i sám sebe, sleduje Cray každé nové vodítko v případu vyšinuté zdravotní sestry, přestože pomalu ztrácí kontakt s realitou. Musí čelit vlastní temné stránce a založit své pátrání pouze na intuici...
                        Ale v říjnu nevychází jen thrillery. Důkazem je toho Sladká pomsta. Humorně laděná kniha od oblíbeného švédského autora Jonase Jonassona vás nenechá během své nepředvídatelné jízdy napříč kontinenty a světem umění vydechnout.
                        Psí slib je knihou určenou pro milovníky psů. Jedná se opět o třetí díl ze série Psí poslání a zaručuje srdceryvné a dojemné čtení.
                        A na závěr upozorněme ještě na jednu novinku ze žánru fantasy. Kamenné nebe potěší čtenáře série Zlomená země a přinese jim další dobrodružství a vlnu napětí spojenou se zkázou země - tentokrát tou poslední... ",
                Date = new DateTime(2020, 10, 28)
            },
            new NewsPost
            {
                Id = 4,
                Header = "Desátý ročník KNIHOMÁNIE startuje v neděli 1. 11",
                Text = @"KNIHOMÁNIE je každoročně vrcholem knižní sezóny. Jejím hlavním cílem je nabídnout čtenářům to nejlepší, co přináší podzimní knižní trh, a také podpořit knihkupce v kamenných prodejnách.
                        Letošní nabídka 10. ročníku přináší 26 novinek od 18 nakladatelů. Z nich si mohou čtenáři vybírat od 1. do 22. listopadu 2020 v téměř 300 knihkupectvích po celé České republice.
                        I tentokrát jsou v nabídce zajímavé tituly různých žánrů české i světové literatury – thrillery, historické romány, životní osudy, kuchařky, populárně naučná literatura i knihy pro děti. A samozřejmě i letos zákazníci obdrží ke každé zakoupené knize dárek – knihu 222 křížovek.",
                Date = new DateTime(2020, 10, 30)
            },
            new NewsPost
            {
                Id = 5,
                Header = "Kniha roku je vyhlášena!",
                Text = "Druhý ročník čtenářské ankety Kniha roku, jejímž vyhlašovatelem je Nadační fond Čtení tě mění, už zná své vítěze. V hlasování veřejnosti na webu www.kniharoku.cz o nich rozhodlo 119 978 čtenářských hlasů.",
                Date = new DateTime(2020, 11, 17)
            },
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<NewsPost>().HasData(Data);
    }
}
