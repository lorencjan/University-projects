Pokud je požadováno přemístění nákladu z jednoho místa do druhého, vozík si materiál vyzvedne do 1 minuty.

  - AMB_SUBJECT, náklad == materiál? Zřejmě ano, může být nejednoznačné. Ve zbytku textu se používá materiál.
  - IMPLICIT, k čemu se vztahuje 1 minuta? Od zadání požadavku, od uvolnění vozíku ...?

*Pokud je požadováno přemístění materiálu z jednoho místa do druhého, vozík si materiál vyzvedne do 1 minuty od obdržení požadavku.*

Pokud se to nestihne, materiálu se nastavuje prioritní vlastnost.

  - AMB_REFERENCE, zde ani není problém, jakžto tu vzniká. V dalších větách se materiál s prioritní vlastností implicitně označuje za prioritní, tedy používá se nejednoznačná reference na to. Lepší zde specifikovat, než všude jinde opravovat.

*Pokud se to nestihne, materiálu se nastavuje prioritní vlastnost a je nazýván prioritním materiálem.*

Každý prioritní materiál musí být vyzvednutý vozíkem do 1 minuty od nastavení prioritního požadavku.

  - UNSPECIFIED_SUBJECT, nespecifikován prioritní požadavek, zřejmě se jedná o nastavení prioritní vlastnosti.
  - DANGLING_ELSE, co se stane, pokud se to nestihne?

*Každý prioritní materiál musí být vyzvednutý vozíkem do 1 minuty od nastavení prioritní vlastnosti materiálu. V opačném případě dojde k vyvolání výjimky.*

V tomto režimu zůstává, dokud nevyloží všechen takový materiál.

  - AMB_REFERENCE, mezi řádky to dává smysl, ale výrazy `tento režim` a `takový materiál` jsou minimálně podezřelé.

*V režimu pouze-vykládka vozík zůstává, dokud nevyloží všechen prioritní materiál.*

Normálně vozík během své jízdy může nabírat a vykládat další materiály v jiných zastávkách.

  - AMB_SUBJECT, normálně se zřejmě myslí při neprioritním nákladu, ale to jasně specifikováno.
  - DANGLING_ELSE, tato věta nevylučuje, že by prioritní vozík taky nemohl přikládat/vykládat po cestě, byť je myšleno, že to právě nemůže (režim pouze-vykládka) a že to lze pouze v normálu.

*Pokud vozík nepřeváží prioritní materiál, tak během své jízdy může nabírat a vykládat další materiály v jiných zastávkách. V opačném případě pouze může vyložit prioritní náklad v jeho místě určení.*

Na jednom místě může vozík akceptovat nebo vyložit jeden i více materiálů.

  - AMB_STATEMENT, akceptovat náklad zní divně a nikde se tento výraz nepoužil. Může dojít k mýlce, co to vlastně znamená.

*Na jednom místě může vozík naloži nebo vyložit jeden i více materiálů.*

Pořadí vyzvednutí materiálu nesouvisí s pořadím vytváření požadavku.

  - AMB_SUBJECT, vytváření jakých požadavků? Zde to může být požadavek na cokoliv.

*Pořadí vyzvednutí materiálu nesouvisí s pořadím vytváření požadavku k jeho přemístění.*

Vozík neakceptuje materiál, pokud jsou všechny jeho sloty obsazené nebo by jeho převzetím byla překročena maximální nosnost.

  - AMB_STATEMENT, opět použito `akceptovat materiál`.
  - AMB_SUBJECT, nosnost čeho? Zřejmě vozíku, ale opět to lze i špatně vyložit.
  - OTHER, co se stane s požadavkem, pokud ho vozík nemůže zpracovat (akceptovat materiál)? Uloží si ho na potom, nebo odpoví systému a ten to zadá jinému vozíku? Druhá varianta dává větší smysl, neboť se redukuje čekání a vozík zná své sloty, váhu i materiál (z požadavku), takže může odpovědět, jestli to zvládne.

*Vozík odmítne požadavek, pokud jsou všechny jeho sloty obsazené nebo by jeho převzetím byla překročena maximální nosnost vozíku.*