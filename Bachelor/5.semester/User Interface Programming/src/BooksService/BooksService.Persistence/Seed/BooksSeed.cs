using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class BooksSeed
    {
        public static List<Book> Data => new List<Book>
        {
            new Book
            {
                Id = 1,
                Name = "12 pravidel pro život",
                Photo = ImagesSeed._12_pravidel,
                Year = 2018,
                Isbn = "978-80-257-2792-8",
                Pages = 424,
                Description = "Co by měl znát každý člověk v moderním světě? ptá se renomovaný psycholog Jordan B. Peterson a jeho odpověď na tuto nesnadnou otázku v sobě jedinečně propojuje odvěké pravdy a ohromující objevy nejsoučasnějšího vědeckého výzkumu."
            },
            new Book
            {
                Id = 2,
                Name = "Kuchařka pro dceru",
                Photo = ImagesSeed.kucharka,
                Year = 2010,
                Isbn = "978-80-905806-1-9",
                Pages = 408,
                Description = "Kniha Kuchařka pro dceru se zrodila už před 13 lety a od té doby se stala velice úspěšnou. Autorka Jana Florentýna Zatloukalová v ní popisuje své začátky, kdy v kuchyni úplně tápala, protože ji maminka nenaučila vařit. Jelikož ví, jak těžké bylo připravit chutný pokrm bez jakékoliv zkušenosti, rozhodla se, že se podělí s ostatními o odpovědi na základní otázky, co se vaření týkají. Impulzem pro vydání kuchařky byla pro autorku mimo jiné těžká nemoc její dcery, kdy s ní trávila dlouhé dny v nemocnici. Ačkoliv dcera boj s nemocí nakonec prohrála, rozhodla se autorka kuchařku věnovat právě jí - dceři, se kterou již nikdy nebude moct vařit v jedné kuchyni a nebude jí tak moci předat tajemství svého kuchařského umění. Proto ho předává aspoň vám...."
            },
            new Book
            {
                Id = 3,
                Name = "Eragon",
                Photo = ImagesSeed.eragon,
                Year = 2003,
                Isbn = "80-7200-978-8",
                Pages = 477,
                Description = "Eragon je fantasy román Christophera Paoliniho, první díl pentalogie Odkaz Dračích jezdců. Vypráví o mladíkovi Eragonovi, který nalezne kouzelný kámen v horách, z něhož se vyklube dračí mládě a jeho obyčejný život se převrátí naruby ze dne na den. Je nucen převzít odpovědnost za osud království, v němž vládne zlý a krutý král Galbatorix. Vypravuje se na cestu se svým učitelem Bromem a zažívá spoustu dobrodružství. V prvé řadě hodlá vystopovat Ra'zaky (královy posluhovače), kteří zabili jeho strýce – vychovatele – a v neposlední řadě ochránit obyvatele tolik sužované země. Eragon má však před sebou dlouhou cestu, neboť jeho dosavadní schopnosti nejsou ještě tak velké, aby mohl vzdorovat takto silným nepřátelům. Přestože autor nepřinesl do žánru fantasy žádný nový zásadní prvek a pro dějová schémata i postavy a rasy našel inspiraci u jiných autorů (Tolkienův Pánprstenů, Hvězdné války G. Lucase), jde o dílo pozoruhodně propracované, zejména s přihlédnutím k věku autora, který Eragona začal psát již v 15 letech."
            },
            new Book
            {
                Id = 4,
                Name = "Eldest",
                Photo = ImagesSeed.eldest,
                Year = 2005,
                Isbn = "978-80-253-0962-9",
                Pages = 654,
                Description = "Eldest je druhý díl fantasy cyklu Odkaz Dračích jezdců spisovatele Christophera Paoliniho."
            },
            new Book
            {
                Id = 5,
                Name = "Brisingr",
                Photo = ImagesSeed.brisingr,
                Year = 2010,
                Isbn = "978-80-253-0963-6",
                Pages = 664,
                Description = "Brisingr je třetí díl pentalogie Odkaz Dračích jezdců. Jejím tvůrcem je mladý spisovatel Christopher Paolini. Kniha vyšla 20. září 2008 v Severní Americe a v únoru 2009 v České republice. Brisingr je ale také Eragonův nový meč, ale teké ve Starověkém jazyce 'oheň'."
            },
            new Book
            {
                Id = 6,
                Name = "7 návyků skutečně efektivních lidí",
                Photo = ImagesSeed._7_navyku,
                Year = 1989,
                Isbn = "978-80-7261-156-0",
                Pages = 376,
                Description = "Nové vydání nejúspěšnější knihy renomovaného autora motivační literatury a experta na rodinnou problematiku a vedení lidí charakterizuje 7 návyků, které je třeba si osvojit, abychom jednali co možná nejefektivněji, a které jsou základem pro vytvoření harmonické, cílevědomé a integrované osobnosti. Čím větším změnám a obtížnějším úkolům musíme čelit, tím naléhavější je potřeba se jimi řídit. Aktualizované vydání knihy, která je považována za jednu z nejvlivnějších knih století a jež byla přeložena do více než 40 světových jazyků, vychází ve zcela novém překladu Aleše Lisy. Čtenáři, kteří by si chtěli osvojit '8. návyk' a chtěli-by přejít od efektivnosti k výjimečnosti, přivítají i pokračování této knihy, novinku Dr. Coveyho '8. návyk. Od efektivnosti k výjimečnosti'."
            },
            new Book
            {
                Id = 7,
                Name = "Talisman",
                Photo = ImagesSeed.talisman,
                Year = 1984,
                Isbn = "80-86030-03-2",
                Pages = 636,
                Description = "Život nebo smrt? Boj dobra a zla ve skutečném i mystickém světě... Vydejte se se statečným Putujícím Jackem do tajuplných Teritorií pro záhadný Talisman, který jediný může spasit krásnou královnu Lauru DeLoessian..."
            },
            new Book
            {
                Id = 8,
                Name = "8. návyk",
                Photo = ImagesSeed._8_navyk,
                Year = 2005,
                Isbn = "80-7261-138-0",
                Pages = 376,
                Description = "Kniha proslulého autora a rádce, jehož '7 návyků vůdčích osobností' je považováno za jednu z nejvlivnějších knih 20. století, se zaměřuje na osvojování si potřebného nového návyku, jenž v nové době informací a znalostí umožní člověku nalézt cestu k prosazení osobní výjimečnosti a k sebeuspokojení v pracovním i v osobním životě. Musíme se naučit řídit se svým vnitřním hlasem (najít sebe sama, být si vědomi své jedinečnosti a rozvíjet své vrozené nadání) a pomáhat druhým, aby totéž dokázali i oni. Covey tedy zároveň ukazuje, jak principy upevňující 8. návyk a umožňující přechod od efektivnosti k výjimečnosti uplatnit také v životě organizací."
            },
            new Book
            {
                Id = 9,
                Name = "Řbitov zviřátek",
                Photo = ImagesSeed.rbitov_zviratek,
                Year = 1983,
                Isbn = "80-85601-88-5",
                Pages = 336,
                Description = "Rodina Creedových se usadí ve starém domě v půvabném prostředí venkova státu Maine. Otec lékař, krásná žena, rozkošné děti. Zdálo by se, že nic nemůže překazit jejich štěstí... Avšak objevuje se Cosi, co mrazí krev v žilách, Cosi strašnějšího než sama smrt. Cosi nesrovnatelně příšernějšího a silnějšího..."
            },
            new Book
            {
                Id = 10,
                Name = "Zelená míle",
                Photo = ImagesSeed.green_mile,
                Year = 1966,
                Isbn = "80-7306-024-8",
                Pages = 344,
                Description = "V roce 1932 se Amerika zmítala v otřesech velké krize a život se zdál být trpký a obtížný, jako ještě nikdy předtím. Příběh se odehrává ve věznici Cold Mountain, kam jsou transportováni odsouzenci na smrt. John Coffey je člověk, který si trest smrti zaslouží více než kdokoliv jiný. Tento mohutný obr byl odsouzen za smrt dvou malých dívek. Řekli byste možná, že cesta k poslední odplatě bude krátká a milosrdná – ale sami poznáte, jak se můžete mýlit. Doprovodíme společně Johna Coffeye na jeho poslední cestě, podobné zlému snu...."
            },
            new Book
            {
                Id = 11,
                Name = "Alchymista",
                Photo = ImagesSeed.alchymista,
                Year = 2015,
                Isbn = "978-80-257-1218-4",
                Pages = 204,
                Description = "Román Alchymista, největší brazilský bestseller všech dob, byl dosud přeložen do 34 jazyků a prodalo se ho na 10 milionů výtisků. Takřka pohádkové vyprávění o cestě španělského pastýře za zakopaným pokladem, vycházející z příběhu o splněném snu ze sbírky Tisíc a jedna noc, je zároveň výzvou k naplnění vlastního osudu i poznáním posvátnosti světa, v němž žijeme. Na cestě za oním dvojím pokladem, plné zkoušek a důležitých setkání, je třeba dbát všech znamení a s neustálou trpělivostí a odvahou ( jako alchymista sledující proměnu obyčejného kovu ve zlato) přetvořit samu svou osobnost."
            },
            new Book
            {
                Id = 12,
                Name = "Pyramidy",
                Photo = ImagesSeed.pyramidy,
                Year = 1995,
                Isbn = "80-85609-69-X",
                Pages = 334,
                Description = "Pokud jste kdykoliv toužili vládnout nějakému velkému uskupení, pak je tato kniha stvořená přímo Vám - ukáže Vám totiž, že to vubec není sranda. Zkuste být faraon v pubertě, který sebou nesmí nosit peníze, je obsluhován dívkami v oblečení o celkové rozloze dvou centimetrů čtverečních, budou si o Vás myslet, že můžete za den i noc, že díky Vám roste kukuřice a že se díky Vám mají všichni dobře a k tomu všemu ještě vybuchne jedna Velká pyramida. Přitom všechno, co byste opravdu chtěli, je udělat něco pro dobro lidu a starobylý střed města."
            },
            new Book
            {
                Id = 13,
                Name = "Poutník",
                Photo = ImagesSeed.poutnik,
                Year = 2010,
                Isbn = "978-80-257-0282-6",
                Pages = 284,
                Description = "Tato kniha, která získala autorovi mimořádnou popularitu, je podrobným líčením jeho pěší pouti do Santiaga de Compostela a zhodnocením získané duchovní zkušenosti. Po stopách středověkých poutníků se Paulo pod vedením učitele a přítele Petra vypravil, aby našel meč, který mu byl při slavnostním rituálu jednoho esoterického řádu pro přílišnou pýchu a posedlost zázraky odepřen a ukryt kdesi na svatojakubské cestě. Během náročných zkoušek, s jejichž pomocí poutník jednotlivé úseky cesty za ztraceným mečem zdolává, se vyvíjí i složitý vztah učedníka a mistra ve vzájemném dávání a přijímání."
            },
            new Book
            {
                Id = 14,
                Name = "Vyzvědačka",
                Photo = ImagesSeed.vyzvedacka,
                Year = 2016,
                Isbn = "978-80-257-1932-9",
                Pages = 168,
                Description = "V Coelhově románu Vyzvědačka ožívá skutečný příběh Maty Hari, slavné kurtizány obviněné ze špionáže, která byla před sto lety popravena pro velezradu. Mata Hari byla tanečnice, šokující a těšící publikum za první světové války, a stala se důvěrnicí některých z nejbohatších a nejmocnějších mužů té doby. Odvážila se překonat moralismus a provinční mentalitu počátků 20. století, ale nakonec za to zaplatila vlastním životem. Když v pařížském vězení čekala na popravu, jedním z jejích posledních přání bylo pero a papír, aby mohla psát dopisy."
            },
            new Book
            {
                Id = 15,
                Name = "Pán prstenů: Společenstvo prstenu",
                Photo = ImagesSeed.spolecenstvo_prstenu,
                Year = 2012,
                Isbn = "978-80-257-0746-3",
                Pages = 430,
                Description = "Nechte se okouzlit strhujícím příběhem kouzelného prstenu, který hraje klíčovou roli v boji dobra a zla v dávné Středozemi. Přidejte se ke Společenstvu prstenu, složeného z hobitů, lidí, kouzelníka, elfa i trpaslíka, kteří se vydávají do země Mordoru s jediným cílem - zničit kouzelný prsten a tak porazit moc Saurona, pána zla. Společně s nimi prožijete mnohá dobrodružství, stanete se svědky nelítostných bitev a navštívíte kouzelné kraje, kde se setkáte s obry, draky a dalšími dobrými i zlými kouzelnými tvory."
            },
            new Book
            {
                Id = 16,
                Name = "Pán prstenů: Dvě věže",
                Photo = ImagesSeed.dve_veze,
                Year = 2012,
                Isbn = "978-80-257-0747-0",
                Pages = 361,
                Description = "Ilustrovaná verze druhého svazku světoznámé trilogie, zaujímající dnes čelné místo v klasické světové fantasy. Druhá část Pána prstenů vypráví, jak se vedlo každému členu Společenstva Prstenu od jeho rozbití až do příchodu Velké tmy a propuknutí Války o Prsten, o níž se bude vyprávět ve třetí a poslední části. Ilustracemi knihu opatřil dosud nepřekonaný 'tolkienovský' ilustrátor Alan Lee."
            },
            new Book
            {
                Id = 17,
                Name = "Pán prstenů: Návrat krále",
                Photo = ImagesSeed.navrat_krale,
                Year = 2012,
                Isbn = "978-80-257-0748-7",
                Pages = 467,
                Description = "Tato třetí a poslední část, Pána prstenů, bude vyprávět o střetnutí Gandalfovy a Sauronovy strategie až po konečnou katastrofu a konec Velké tmy. Síly dobra a zla se utkávají v rozhodující bitvě. Statečný hobit Frodo, jemuž byl svěřen kouzelný Prsten, a všichni jeho přátelé bojující na straně dobra se zúčastní obrovské Války o Prsten. Bude mít více síly Sauron, nebo Gandalf? Jak skončí Velká tma? A skončí vůbec? Ocitne se překrásná Středozemě se všemi svými trpaslíky, elfy, hobity, enty a dalšími pozoruhodnými bytostmi v područí temných sil Mordoru – nebo se skutečně navrátí spravedlivý král a usedne na trůn svých předků?"
            },
            new Book
            {
                Id = 18,
                Name = "To snad nemyslíte vážně, pane Feynmanne!",
                Photo = ImagesSeed.feynman,
                Year = 2013,
                Isbn = "978-80-7299-096-2",
                Pages = 296,
                Description = "Neuvěřitelné a rozmarné příhody ze života jednoho z největších teoretických fyziků 20. století Richarda P. Feynmana. Kromě mnoha jiných činností byl také vynikajícím hráčem na bongo a výborným malířem, ale především byl prototypem nekonvenčního, zvídavého a moudrého člověka."
            },
            new Book
            {
                Id = 19,
                Name = "1984",
                Photo = ImagesSeed._1984,
                Year = 2015,
                Isbn = "978-80-257-1479-9",
                Pages = 320,
                Description = "1984, dnes již klasické dílo antiutopického žánru, je bezesporu jedním z nejpozoruhodnějších románů 20. století a brilantní analýzou všech totalitárních systémů. 'Antiutopie' má i svého 'antihrdinu': je jím Winston Smith, obyčejný, ne příliš statečný člověk, který si nepřeje nic světoborného: chce jen poznat skutečnou, ne zmanipulovanou podobu minulosti, věřit tomu, co vidí na vlastní oči a mít svobodu tvrdit, že dva a dva jsou čtyři. Tyto elementární nároky jsou však přímým ohrožením moci, postavené na lži, a proto je Winstonova vzpoura proti obludnému organismu, jehož je nepatrnou buňkou, předem odsouzená k nezdaru. Přesto mu autor vkládá do úst jistou naději, působivější o to, že vyslovenou v mučírně: 'Nějak už vám to selže, něco vás zdolá. Život vás zdolá.' Orwell napsal svůj román v roce 1947. Na rozdíl od mnohých v té době nepodlehl euforii poválečného optimismu vyhlídek na věčný mír, a jeho dílo je proto sugestivním obrazem politické perverze, kterou končí v dějinách každé revoluční hnutí."
            },
            new Book
            {
                Id = 20,
                Name = "Dobrá znamení",
                Photo = ImagesSeed.dobra_znameni,
                Year = 2019,
                Isbn = "978-80-7197-729-2",
                Pages = 408,
                Description = "Děti, zahrávat si s Armagedonem může být nebezpečné! Nezkoušejte to ve vlastním bytě! Crowly je padlý anděl, teoreticky tedy ďábel, prakticky však člověk. Celé stovky let je naturalizovaný na Zemi, kam byl nasazen jako 'spící' agent. Miluje staré automobily a populární hudbu a všeobecně dvacáté století, protože to je mnohem lepší než století sedmnácté, a to zase bylo mnohem přijatelnější než to čtrnácté. Žije si skvěle, může si to totiž dovolit. Jistě tímto dalším příběhem T. Pratchetta a N. Gaimana nebudete zklamáni. Čeká vás setkání s Ním. Nastal totiž čas narození nového Satana. Jenom rozpoznat, který z těch dvou roztomilých novorozených chlapečků to je."
            },
            new Book
            {
                Id = 21,
                Name = "Malí bohové",
                Photo = ImagesSeed.mali_bohove,
                Year = 1997,
                Isbn = "80-7197-084-0",
                Pages = 399,
                Description = "Když Brutovi do melounového záhonku spadne z nebe malá želvička, ještě netuší, že na něj brzy promluví jeho bůh. Brutovi nastávají krušné chvíle, když mu jeho bůh přikazuje, aby svrhnul skorumpovanou církev. Ve svém putování narazí také na pronásledovaného filosofa, který tvrdí, že se Zeměplocha pluje na zádech čtyř slonů, kteří putují na krunýři velké Želvy. Jak neslýchané! "
            },
            new Book
            {
                Id = 22,
                Name = "Stráže! Stráže!",
                Photo = ImagesSeed.straze,
                Year = 2007,
                Isbn = "80-85609-70-3",
                Pages = 416,
                Description = "Určitě každý z nás někdy viděl uhlí. Tak přesně to se stává z lidí, kteří chodí nočním Ankh-Morporkem a mají opravdu smůlu. To je úkol jako stvořený pro městskou hlídku v čele se Samem Elániem a jeho věrnou družinou, která se skládá ze seržanta Tračníka, desátníka Nóblhóchá ( který musí mít doklady na to, že je to člověk ) a nově příchozího dvoumetrového trpaslíka Karotku Rudykopalssona."
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Book>().HasData(Data);
    }
}
