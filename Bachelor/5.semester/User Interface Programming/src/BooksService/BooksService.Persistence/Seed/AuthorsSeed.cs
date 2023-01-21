using System;
using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class AuthorsSeed
    {
        public static List<Author> Data => new List<Author>
        {
            new Author
            {
                Id = 1,
                FirstName = "Paul",
                LastName = "Coelho",
                Photo = ImagesSeed.paul_coelho,
                Country = "Brazílie",
                BirthDate = new DateTime(1947, 8, 24),
                Biography = "Narodil se v brazilské středostavovské rodině. V mládí koketoval s drogami, magií Aleistera Crowleye. Pro svou touhu psát, kterou jeho rodiče neschvalovali, byl celkem třikrát poslán do psychiatrické léčebny, kde se musel podrobit elektrošokové terapii. Živil se jako dramatik, novinář, televizní scenárista a písňový textař brazilských zpěváků. V roce 1996 byl jmenován zvláštním poradcem programu UNESCO pro duchovní sbližování a dialog mezi kulturami. Je podruhé ženatý. V 80. letech se přiklonil se ke katolicismu, vykonal pouť do Santiaga de Compostela a zážitky z ní zužitkoval ve své první úspěšné knize Poutník - Mágův deník. Celosvětovou proslulost mu přineslo pohádkové podobenství Alchymista vydané v roce 1988, které se stalo největším brazilským bestsellerem všech dob. Ve svých knihách, jejichž hrdiny jsou převážně lidé hledající změnu v životním stereotypu, nabádá k duchovnímu rozvoji a zodpovědnému přístupu k životu. Píše také fejetony uveřejňované v mnoha světových novinách.",
            },
            new Author
            {
                Id = 2,
                FirstName = "John R. R.",
                LastName = "Tolkien",
                Photo = ImagesSeed.tolkien,
                Country = "Velká Británie",
                BirthDate = new DateTime(1892, 1, 3),
                Biography = "John Ronald Reuel Tolkien byl anglický spisovatel, filolog a univerzitní profesor, nejvíce známý jako autor Hobita a Pána prstenů. Tolkien se narodil v Bloemfonteinu, hlavním městě Oranžského svobodného státu ( tehdy součást Velké Británie), Arthuru Tolkienovi, řediteli tamní bankovní pobočky, a jeho ženě Mabel Tolkienové (rozené Suffieldové). Tolkienova rodina pocházela ze Saska, ale žila v Anglii už od 18. století. Ve věku tří let odcestoval do Anglie spolu s matkou, která nemohla přivyknout africkému podnebí. Nejprve bydleli u příbuzných na farmě Bag End v Worcestershire, což bylo pravděpodobně inspirací pro název Dno pytle v jeho knihách. Otec však v Jižní Africe onemocněl revmatickou horečkou a zemřel na krvácení do mozku. Na Univerzitě v Oxfordu působil v letech 1925–1945 jako profesor staré angličtiny, v letech 1945–1959 pak jako profesor anglického jazyka a literatury. Byl význačným jazykovědcem a znalcem staré angličtiny a staré severštiny. Spolu s nejbližším přítelem C. S. Lewisem by členem literárního diskusního klubu Inklings („Tušitelé“ nebo „Inkousťata“). 28. března 1972 obdržel od královny Alžběty II. Řád britského impéria. Kromě Hobita, Pána prstenů, vědeckých pojednání a překladů zahrnuje Tolkienovo dílo množství textů z pozůstalosti o historii fiktivního světa Středozemě, v níž se Hobit a Pán prstenů odehrávají. Část z nich pod názvem Silmarillion uspořádal a vydal autorův syn Christopher. Tolkien pro všechny tyto příběhy používal slovo legendárium. Zbylá Tolkienova díla jsou pohádky a příběhy původně vyprávěné jeho dětem a nevztahují se přímo ke Středozemi. Pro neutuchající popularitu a vliv je Tolkien často považován za nejdůležitějšího z otců žánru moderní hrdinské fantasy.",
            },
            new Author
            {
                Id = 3,
                FirstName = "George",
                LastName = "Orwell",
                Photo = ImagesSeed.george_orwell,
                Country = "Bengálsko",
                BirthDate = new DateTime(1903, 6, 25),
                Biography = "Eric Arthur Blair (25. června 1903 Motihari, Bengálsko – 21. ledna 1950 v Londýně), známější spíše pod svým literárním pseudonymem George Orwell, byl britským novinářem, esejistou a spisovatelem. Světovou popularitu si získaly jeho alegorické antiutopické romány Farma zvířat a 1984, popisující nehumánnost totalitních ideologií.",
            },
            new Author
            {
                Id = 4,
                FirstName = "Terry",
                LastName = "Pratchett",
                Photo = ImagesSeed.terry_pratchett,
                Country = "Velká Británie",
                BirthDate = new DateTime(1948, 4, 28),
                Biography = "Narodil se 28. dubna 1948 v Beaconsfieldu. Hlavním zdrojem jeho vědomostí se zpočátku stala veřejná knihovna. Ve třinácti letech ve školním časopise vyšla krátká povídka The Hades Business. Tehdy se začala jeho budoucnost, co se týče povolání, vyjasňovat. Po The Carpet Poeple (Kobercové) následovala The Dark Side of The Sun (Odvrácená strana slunce) a Strata (1981). V roce 1970 se přestěhoval se svou ženou Lynn do domu v Rowberrow v Somersetu, kde se jim narodila dcera Rhianna. Když v budoucnu zjistil, že dům už není možné dále zvětšovat, rodina se v roce 1993 přestěhovala do jihozápadního Salisbury. Do roku 1970 pracoval v Bucks Free Press, poté přešel do Western Dialy Press. V roce 1980 byl Terry jmenován oficiálním tiskovým mluvčím Central Electricity Generating Board a nesl odpovědnost za tři jaderné elektrárny. Barva kouzel byla vydaná v roce 1983, tím také začala série dílů o Zeměploše. Následovaly Lehké fantastično, Čaroprávnost, Mort, Magický prazdroj, Soudné sestry, Pyramidy, Stráže! Stráže!, Erik, Pohyblivé obrázky, Sekáč, Čarodějky na cestách, Malí bohové, Dámy a pánové, Muži ve zbrani, Těžké melodično, Zajímavé časy, Maškaráda, Otec prasátek, Hrrr na ně!, Nohy z jílu, Poslední kontinent, Carpe jugulum, Pátý elefant, Pravda, Zloděj času, Noční hlídka, Poslední hrdina, Podivný regiment. Terry Pratchett zemřel 12. března 2015 ve svých 66 letech po dlouhém boji s Alzheimerovou chorobou.",
            },
            new Author
            {
                Id = 5,
                FirstName = "Neil",
                LastName = "Gaiman",
                Photo = ImagesSeed.neil_gaiman,
                Country = "Velká Británie",
                BirthDate = new DateTime(1960, 11, 10),
                Biography = "Neil Gaiman (* 10. listopadu 1960, Portchester, Hampshire) byl původním povoláním žurnalista se zaměřením na sci-fi a fantasy. Slávu si získal scénaři ke komiksům Sandman, v roce 1991 napsal scénář k televiznímu seriálu Nikdykde a stejnojmennou knihu vycházející ze seriálu. S Terrym Pratchettem napsal román z žánru humorné fantasy Dobrá znamení, příběh o zrození Antikrista a blížícím se Armageddonu, aktivně sabotovaném andělem Azirafalem a démonem Crowleym, kteří si tento svět příliš oblíbili, a kterým by ta sabotáž šla určitě mnohem lépe, kdyby se v té porodnici tenkrát nepomíchaly ty malé děti. Napsal také román Američtí bohové, na který volně navázal knihou Anansiho chlapci. Jednou z jeho posledních knih je Hvězdný prach. Kromě románů píše Neil Gaiman také písně, básně, povídky a filmové scénáře. Kromě již zmíněného seriálu Nikdykde napsal scénář k filmu Maska zrcadla, na kterém spolupracoval s režisérem filmu Davem McKeanem. Jeho romány Hvězdný prach a Koralina se dočkaly filmové adaptace, Hvězdný prach s Robertem DeNiro Michelle Pfeiffer a Claire Danes v hlavních rolích a Koralina, kde hraje Dakota Fanning a Teri Hatcher, se právě natáčí (březen 2008). Většinu díla Neila Gaimana je možno zařadit pod žánr urbanfantasy, což je, trochu zjednodušeně, fantasy odehrávající se na pozadí skutečného světa. Neil o sobě tvrdí, že od mala toužil vyprávět příběhy, k čemuž se dá dodat snad jen to, že většina jeho čtenářů hodnotí jeho knihy a příběhy jako originální, čtivé a velmi osobité.",
            },
            new Author
            {
                Id = 6,
                FirstName = "Richard P.",
                LastName = "Feyman",
                Photo = ImagesSeed.richard_feynman,
                Country = "USA",
                BirthDate = new DateTime(1918, 5, 11),
                Biography = "Richard Phillips Feynman patřil k největším fyzikům 20. století. Studoval Massachusettský technologický institut (MIT), doktorát složil na univerzitě v Princetonu. Během druhé světové války pracoval na vývoji jaderné bomby (projekt Manhattan). Po skončení války pracoval krátce na Cornellově univerzitě a od roku 1951 až do své smrti působil v Kalifornském technickém institutu (Caltech). V roce 1965 mu byla spolu se dvěma dalšími fyziky udělena Nobelova cena. Vypracoval techniku popisu reakcí elementárních částic poskytující alternativní náhled na chápání kvantové fyziky (Feynmanovy diagramy). Velmi významná byla jeho pedagogická činnost, dodnes jsou pro přehlednost a názornost vysoce ceněné a používané jeho sbírky přednášek a další knihy. Richard Phillips Feynman byl třikrát ženatý. První žena mu zemřela v mladém věku. Se třetí ženou Gwen měl 2 děti. Zemřel na selhání ledvin způsobené rakovinou.",
            },
            new Author
            {
                Id = 7,
                FirstName = "Ralph",
                LastName = "Leighton",
                Photo = ImagesSeed.ralph_leighton,
                Country = "USA",
                BirthDate = new DateTime(1949, 11, 13),
                Biography = "Ralph Leighton je americký autor životopisů, filmový producent a přítel zesnulého fyzika Richarda Feynmana. Je synem zesnuléhofyzika Roberta B. Leightona , který byl také blízký osobní přítel Feynmana. Oženil se s Phoebe Kwan, mají dvě děti, Nicole a Ian.",
            },
            new Author
            {
                Id = 8,
                FirstName = "Jordan",
                LastName = "Peterson",
                Photo = ImagesSeed.jordan,
                Country = "Kanada",
                BirthDate = new DateTime(1962, 6, 12),
                Biography = "Jordan Bernt Peterson je kanadský klinický psycholog, kulturní kritik a profesor psychologie na Torontské univerzitě. Jeho hlavními oblastmi studia jsou abnormální, sociální a osobnostní psychologie, se zvláštním zájmem o psychologii náboženských a ideologických přesvědčení, a posouzení a zlepšení osobnosti a výkonu.",
            },
            new Author
            {
                Id = 9,
                FirstName = "Jana Florentýna",
                LastName = "Zatloukalová",
                Photo = ImagesSeed.florentyna,
                Country = "Česká republika",
                BirthDate = new DateTime(1975, 10, 17),
                Biography = "Jana F. Zatloukalová, matka 4 dětí, byla jednou z dívek, které toho doma, co se týče vaření, zrovna moc nepochytily. Když ji prázdné hrnce ve vlastní kuchyni přinutily se do vaření pustit, marně si nad hromadami zakoupených kuchařek lámala hlavu, co je to 'dusit do měkka' nebo 'středně vyhřátá trouba'. Nakonec se vaření stalo jejím koníčkem a za množstvím kuchařských experimentů se objevují úspěchy. Přispívá svými recepty do několika časopisů, pokračuje v psaní kuchařek a začala se věnovat projektu na podporu tradičních českých chutí a surovin nazvaném Chuťopis.",
            },
            new Author
            {
                Id = 10,
                FirstName = "Christopher",
                LastName = "Paolini",
                Photo = ImagesSeed.Paolini,
                Country = "USA",
                BirthDate = new DateTime(1983, 11, 17),
                Biography = "Christopher Paolini (* 17. listopadu 1983 Los Angeles) je americký spisovatel, autor knih Eragon, Eldest, Brisingr, Inheritance a Poutník, čarodějnice a červ . Tyto knihy jsou součástí, až na Poutník, čarodějnice a červ, který na tuto tetralogii navazuje, Odkazu Dračích jezdců. Třetí díl, Brisingr měl být závěrečný, ale nestalo se tak. čtvrtý díl, Inheritance, se v USA a na dalších vybraných trzích objevil 8. listopadu 2011; v ČR vyšel v březnu 2012. Román Poutník, čarodějnice a červ byl vydán roku 2019. Příběh se odehrává ve fiktivním fantasy světě Alagaësia a vypráví příběh mladého muže Eragona, který se po nalezení dračího vejce stane Dračím jezdcem, a jeho dračice Safiry. Narodil se v listopadu roku 1983 v Jižní Kalifornii. Mimo několika let na Aljašce žil se svými rodiči, mladší sestrou Angelou a kočkou s kokršpanělem v Montaně. K tvorbě ho inspirovala místní příroda a půvab zdejších měst.",
            },
            new Author
            {
                Id = 11,
                FirstName = "Stephen",
                LastName = "Covey",
                Photo = ImagesSeed.Covey,
                Country = "USA",
                BirthDate = new DateTime(1932, 10, 24),
                Biography = "Stephen R. Covey (* 24. října 1932, Salt Lake City, Utah – 16. července 2012, Idaho Falls, Idaho) byl uznávaný lektor a trenér leadershipu, přednášející na univerzitě Jon M. Huntsman School of Business v Utahu, USA a autor bestselleru 7 návyků skutečně efektivních lidí. Zemřel na následky nehody na kole, kterou měl v dubnu 2012.",
            },
            new Author
            {
                Id = 12,
                FirstName = "Stephen",
                LastName = "King",
                Photo = ImagesSeed.King,
                Country = "USA",
                BirthDate = new DateTime(1947, 9, 21),
                Biography = "Stephen Edwin King (* 21. září 1947, Portland, Maine) je americký spisovatel hororů, jeden z nejproduktivnějších a čtenářsky nejúspěšnějších autorů současnosti, který se plných osm let léčil ze závislosti na kokainu. Řada jeho knih a povídek byla zfilmována. Mezi jeho slavná díla patří knihy Carrie, To, Mrtvá zóna a série Temná věž, ve filmovém zpracování prosluly především na jeho dílech postavené snímky Vykoupení z věznice Shawshank a Zelená míle.",
            },
            new Author
            {
                Id = 13,
                FirstName = "Peter",
                LastName = "Straub",
                Photo = ImagesSeed.Straub,
                Country = "USA",
                BirthDate = new DateTime(1943, 3, 2),
                Biography = "Peter Francis Straub (2. března 1943 v Milwaukee, Wisconsin, USA) je americký spisovatel, který se stal známý svými hororovými romány. Spolu se Stephenem Kingem patří k nejvýznamnějším představitelům žánru fantasy v anglickojazyčné literatuře. Peter Straub vyrůstal v americkém spolkovém státu Wisconsin společně se svými dvěma mladšími bratry. Otec byl obchodník, matka pracovala jako nemocniční sestra. Přáli si, aby se Peter stal lékařem nebo luteránským pastorem. Již od školních let ale Petra zajímala především dobrodružná literatura. V první třídě základní školy prodělal autonehodu, při které takřka přišel o život. V jejím důsledku až do svých 20 let koktal. Jako střední školu si zvolil Milwaukee Country Day School. Titul bakaláře anglického jazyka získal Straub v roce 1965 na Universitě ve Wisconsinu–Madisonu, magisterský titul ve stejném oboru o rok později na Columbia University. Krátce učil na střední škole, kterou sám vystudoval. Roku 1969 se odstěhoval do Dublinu. Na tamní univerzitě získal titul Ph.D. a začal se věnovat profesionálně psaní. V Irsku a v Anglii prožil 10 let, pak se vrátil do USA. V současné době žije v Londýně. První velký úspěch zaznamenal jeho román z roku 1977 If You Could See Me Now. Straubovy romány získaly mnohá oceněí, mj. World Fantasy Award, British Fantasy Award a Bram Stoker Award. Dlouholeté přátelství ho pojí se Stephenem Kingem, se kterým napsal Talisman a jeho pokračování Černý dům.",
            },
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Author>().HasData(Data);
    }
}
