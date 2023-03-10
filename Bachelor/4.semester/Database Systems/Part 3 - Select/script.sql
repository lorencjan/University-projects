-- Projekt: 3.část - SQL skript s několika dotazy SELECT 
-- Předmět: Databázové systémy (IDS)
-- Autoři: Jan Lorenc, Vojtěch Staněk
-- Datum: 2.4.2020
-- Tento soubor obsahuje skript na vytvoření databázových objektů z 2. úkolu obohacený o pár nových insertů a 8 příkazů select

--------------------- VYTVOŘENÍ A NAPLNĚNÍ DATABÁZE Z 2.ÚKOLU ---------------------

---- Smazání tabulek
DROP TABLE Komisar CASCADE CONSTRAINT;
DROP TABLE Sportovec CASCADE CONSTRAINT;
DROP TABLE LaboratorniPracovnik CASCADE CONSTRAINT;
DROP TABLE Laborator CASCADE CONSTRAINT;
DROP TABLE Vzorek CASCADE CONSTRAINT;
DROP TABLE Kontrola CASCADE CONSTRAINT;
DROP TABLE Sport CASCADE CONSTRAINT;
DROP TABLE ZakazanaLatka CASCADE CONSTRAINT;

DROP TABLE PovolenaLatka_Sportovec CASCADE CONSTRAINT;
DROP TABLE Komisar_Kontrola CASCADE CONSTRAINT;
DROP TABLE SportZakazuje_Latku CASCADE CONSTRAINT;
DROP TABLE SportPovolujeMimoSoutez_Latku CASCADE CONSTRAINT;
DROP TABLE Vzorek_Latka CASCADE CONSTRAINT;
DROP TABLE LaboratorniPracovnik_Vzorek CASCADE CONSTRAINT;
DROP TABLE Sportovec_Sport CASCADE CONSTRAINT;

---- Vytvoření tabulek
-- datové tabulky
CREATE TABLE Komisar(
    Id INT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    Jmeno VARCHAR(20) NOT NULL,
    Prijmeni VARCHAR(30) NOT NULL,
    DatumNarozeni DATE NOT NULL,
    Pohlavi CHAR(1),
    Narodnost VARCHAR(30),
    Adresa VARCHAR(50),
    Email VARCHAR(60),
    Telefon VARCHAR(20),
    Zamereni VARCHAR(40) NOT NULL,
    CONSTRAINT CHK_PohlaviK CHECK (Pohlavi = 'M' OR Pohlavi = 'Z')
);
CREATE TABLE Sportovec(
    Id INT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    Jmeno VARCHAR(20) NOT NULL,
    Prijmeni VARCHAR(30) NOT NULL,
    DatumNarozeni DATE NOT NULL,
    Pohlavi CHAR(1) NOT NULL,       
    Narodnost VARCHAR(30) NOT NULL,
    Adresa VARCHAR(50) NOT NULL,
    Email VARCHAR(60) NOT NULL,
    Telefon VARCHAR(20) NOT NULL,
    CONSTRAINT CHK_PohlaviS CHECK (Pohlavi = 'M' OR Pohlavi = 'Z')
);
CREATE TABLE LaboratorniPracovnik(
    Id INT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    Jmeno VARCHAR(20) NOT NULL,
    Prijmeni VARCHAR(30) NOT NULL,
    DatumNarozeni DATE NOT NULL,
    Pohlavi CHAR(1),
    Narodnost VARCHAR(30),
    Adresa VARCHAR(50),
    Email VARCHAR(60),
    Telefon VARCHAR(20),
    LaboratorId INT NOT NULL,
    CONSTRAINT CHK_PohlaviLP CHECK (Pohlavi = 'M' OR Pohlavi = 'Z')
);
CREATE TABLE Laborator(
    Id INT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    Adresa VARCHAR(50) NOT NULL,
    Akreditace VARCHAR(60) NOT NULL
);
CREATE TABLE Vzorek(
    CiselnyKod INT NOT NULL,
    Typ VARCHAR(5) NOT NULL,
    Vyznam VARCHAR(10) NOT NULL,
    Vysledek CHAR(10),
    CONSTRAINT CHK_Typ CHECK (Typ = 'Krev' OR Typ = 'Moc'),
    CONSTRAINT CHK_Vyznam CHECK (Vyznam = 'Hlavni' OR Vyznam = 'Kontrolni'),
    CONSTRAINT CHK_Vysledek CHECK (Vysledek = 'Pozitivni' OR Vysledek = 'Negativni' OR Vysledek = 'Neprukazne' OR Vysledek = NULL)
);
CREATE TABLE Kontrola(
    Id INT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    Cas DATE NOT NULL,
    Misto VARCHAR(30) NOT NULL,
    PrilezitostKonani VARCHAR(100) NOT NULL,
    SportovecZastizen VARCHAR(3),
    PovolenOdklad VARCHAR(3),
    SportovecId INT NOT NULL,
    SportId INT NOT NULL,
    LatkaId INT NOT NULL,
    HlavniVzorekId INT,
    KontrolniVzorekId INT,
    CONSTRAINT CHK_SportovecZastizen CHECK (SportovecZastizen = 'Ano' OR SportovecZastizen = 'Ne'),
    CONSTRAINT CHK_PovolenOdklad CHECK (PovolenOdklad = 'Ano' OR PovolenOdklad = 'Ne')
);
CREATE TABLE Sport(
    Id INT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    Nazev VARCHAR(30) NOT NULL,
    Typ VARCHAR(50)
);
CREATE TABLE ZakazanaLatka(
    Id INT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    Nazev VARCHAR(50) NOT NULL,
    Popis VARCHAR(200),
    RokZakazani INT,
    Kategorie VARCHAR(50)
);
-- spojovací tabulky
CREATE TABLE PovolenaLatka_Sportovec(
    SportovecId INT NOT NULL,
    LatkaId INT NOT NULL,
    CasOd DATE NOT NULL,
    CasDo DATE NOT NULL
);
CREATE TABLE Komisar_Kontrola(
    KomisarId INT NOT NULL,
    KontrolaId INT NOT NULL
);
CREATE TABLE SportZakazuje_Latku(
    SportId INT NOT NULL,
    LatkaId INT NOT NULL
);
CREATE TABLE SportPovolujeMimoSoutez_Latku(
    SportId INT NOT NULL,
    LatkaId INT NOT NULL
);
CREATE TABLE Vzorek_Latka(
    VzorekId INT NOT NULL,
    LatkaId INT NOT NULL
);
CREATE TABLE LaboratorniPracovnik_Vzorek(
    LaboratorniPracovnikId INT NOT NULL,
    VzorekId INT NOT NULL
);
CREATE TABLE Sportovec_Sport(
    SportovecId INT NOT NULL,
    SportId INT NOT NULL
);

---- Definice primárních klíčů
ALTER TABLE Komisar ADD CONSTRAINT PK_Komisar PRIMARY KEY (Id);
ALTER TABLE Sportovec ADD CONSTRAINT PK_Sportovec PRIMARY KEY (Id);
ALTER TABLE LaboratorniPracovnik ADD CONSTRAINT PK_LaboratorniPracovnik PRIMARY KEY (Id);
ALTER TABLE Laborator ADD CONSTRAINT PK_Laborator PRIMARY KEY (Id);
ALTER TABLE Vzorek ADD CONSTRAINT PK_Vzorek PRIMARY KEY (CiselnyKod);
ALTER TABLE Kontrola ADD CONSTRAINT PK_Kontrola PRIMARY KEY (Id);
ALTER TABLE Sport ADD CONSTRAINT PK_Sportr PRIMARY KEY (Id);
ALTER TABLE ZakazanaLatka ADD CONSTRAINT PK_ZakazanaLatka PRIMARY KEY (Id);

ALTER TABLE PovolenaLatka_Sportovec ADD CONSTRAINT PK_PovolenaLatka_Sportovec PRIMARY KEY (SportovecId, LatkaId);
ALTER TABLE Komisar_Kontrola ADD CONSTRAINT PK_Komisar_Kontrola PRIMARY KEY (KomisarId, KontrolaId);
ALTER TABLE SportZakazuje_Latku ADD CONSTRAINT PK_SportZakazuje_Latku PRIMARY KEY (SportId, LatkaId);
ALTER TABLE SportPovolujeMimoSoutez_Latku ADD CONSTRAINT PK_SportPovolujeMimoSoutez_Latku PRIMARY KEY (SportId, LatkaId);
ALTER TABLE Vzorek_Latka ADD CONSTRAINT PK_Vzorek_Latka PRIMARY KEY (VzorekId, LatkaId);
ALTER TABLE LaboratorniPracovnik_Vzorek ADD CONSTRAINT PK_LaboratorniPracovnik_Vzorek PRIMARY KEY (LaboratorniPracovnikId, VzorekId);
ALTER TABLE Sportovec_Sport ADD CONSTRAINT PK_Sportovec_Sport PRIMARY KEY (SportovecId, SportId);

-- cizí klíče
ALTER TABLE LaboratorniPracovnik ADD CONSTRAINT FK_PracujeVLaboratori FOREIGN KEY (LaboratorId) REFERENCES Laborator ON DELETE CASCADE;
ALTER TABLE Kontrola ADD CONSTRAINT FK_KontrolujeSportovce FOREIGN KEY (SportovecId) REFERENCES Sportovec ON DELETE CASCADE;
ALTER TABLE Kontrola ADD CONSTRAINT FK_KontrolujeSport FOREIGN KEY (SportId) REFERENCES Sport ON DELETE CASCADE;
ALTER TABLE Kontrola ADD CONSTRAINT FK_KontrolujeLatku FOREIGN KEY (LatkaId) REFERENCES ZakazanaLatka ON DELETE CASCADE;
ALTER TABLE Kontrola ADD CONSTRAINT FK_KontrolujeHlavniVzorek FOREIGN KEY (HlavniVzorekId) REFERENCES Vzorek ON DELETE CASCADE;
ALTER TABLE Kontrola ADD CONSTRAINT FK_KontrolujeVedlejsiVzorek FOREIGN KEY (KontrolniVzorekId) REFERENCES Vzorek ON DELETE CASCADE;

ALTER TABLE PovolenaLatka_Sportovec ADD CONSTRAINT FK_SportovecMaPovoleno FOREIGN KEY (SportovecId) REFERENCES Sportovec ON DELETE CASCADE;
ALTER TABLE PovolenaLatka_Sportovec ADD CONSTRAINT FK_PovolenaLatkaSportovci FOREIGN KEY (LatkaId) REFERENCES ZakazanaLatka ON DELETE CASCADE;

ALTER TABLE Komisar_Kontrola ADD CONSTRAINT FK_KontrolujiciKomisar FOREIGN KEY (KomisarId) REFERENCES Komisar ON DELETE CASCADE;
ALTER TABLE Komisar_Kontrola ADD CONSTRAINT FK_ProvadenaKontrola FOREIGN KEY (KontrolaId) REFERENCES Kontrola ON DELETE CASCADE;

ALTER TABLE SportZakazuje_Latku ADD CONSTRAINT FK_ZakazujiciSport FOREIGN KEY (SportId) REFERENCES Sport ON DELETE CASCADE;
ALTER TABLE SportZakazuje_Latku ADD CONSTRAINT FK_ZakazanaLatka FOREIGN KEY (LatkaId) REFERENCES ZakazanaLatka ON DELETE CASCADE;

ALTER TABLE SportPovolujeMimoSoutez_Latku ADD CONSTRAINT FK_PovolujiciSport FOREIGN KEY (SportId) REFERENCES Sport ON DELETE CASCADE;
ALTER TABLE SportPovolujeMimoSoutez_Latku ADD CONSTRAINT FK_PovolenaLatka FOREIGN KEY (LatkaId) REFERENCES ZakazanaLatka ON DELETE CASCADE;

ALTER TABLE Vzorek_Latka ADD CONSTRAINT FK_PozitivniVzorek FOREIGN KEY (VzorekId) REFERENCES Vzorek ON DELETE CASCADE;
ALTER TABLE Vzorek_Latka ADD CONSTRAINT FK_ObsazenaLatka FOREIGN KEY (LatkaId) REFERENCES ZakazanaLatka ON DELETE CASCADE;

ALTER TABLE LaboratorniPracovnik_Vzorek ADD CONSTRAINT FK_VzorekVyplnuje FOREIGN KEY (LaboratorniPracovnikId) REFERENCES LaboratorniPracovnik ON DELETE CASCADE;
ALTER TABLE LaboratorniPracovnik_Vzorek ADD CONSTRAINT FK_VyplnujiciVzorek FOREIGN KEY (VzorekId) REFERENCES Vzorek ON DELETE CASCADE;

ALTER TABLE Sportovec_Sport ADD CONSTRAINT FK_SportProvaden FOREIGN KEY (SportovecId) REFERENCES Sportovec ON DELETE CASCADE;
ALTER TABLE Sportovec_Sport ADD CONSTRAINT FK_ProvozujiciSport FOREIGN KEY (SportId) REFERENCES Sport ON DELETE CASCADE;

---- Naplnění vzorovými daty
INSERT INTO Komisar VALUES (1, 'Jan', 'Novák', TO_DATE('1968-03-13', 'YYYY-MM-DD'), 'M', 'Česká', 'Kounicova 28, Brno', 'jan.novak@example.com', '00420123456789', 'Steroidy');
INSERT INTO Komisar VALUES (2, 'Jana', 'Nováková', TO_DATE('1969-06-03', 'YYYY-MM-DD'), 'Z', 'Česká', 'Kounicova 29, Brno', 'jana.novakova@example.com', '00420987654321', 'Stimulanty');
INSERT INTO Komisar VALUES (3, 'Dana', 'Hliněná', TO_DATE('1988-11-30', 'YYYY-MM-DD'), 'Z', 'Slovenská', 'Technická 12, Brno', 'hlinena@feec.vutbr.cz', '+421553467661', 'Anabolika');

INSERT INTO Sportovec (Jmeno, Prijmeni, DatumNarozeni, Pohlavi, Narodnost, Adresa, Email, Telefon) VALUES ('Martina', 'Sáblíková', TO_DATE('1987-05-27', 'YYYY-MM-DD'), 'Z', 'Česká', 'Velký Osek 13', 'sablikova@topsport.cz', '+420975312468');
INSERT INTO Sportovec (Jmeno, Prijmeni, DatumNarozeni, Pohlavi, Narodnost, Adresa, Email, Telefon) VALUES ('Emil', 'Zátopek', TO_DATE('1922-09-22', 'YYYY-MM-DD'), 'M', 'Česká', 'Kopřivnice 79', 'zatopek@velikan.cz', '+420235641709');
INSERT INTO Sportovec (Jmeno, Prijmeni, DatumNarozeni, Pohlavi, Narodnost, Adresa, Email, Telefon) VALUES ('Jaromír', 'Jágr', TO_DATE('1975-02-15', 'YYYY-MM-DD'), 'M', 'Česká', 'U Stdionu 68, Kladno', 'OGJardaJagr@topsport.cz', '+420686868686');

INSERT INTO Laborator VALUES (1, 'Výzkumná 682, Praha', 'Anabolika, Steroidy');
INSERT INTO Laborator VALUES (2, 'Nevýzkumná 331, Olomouc', 'Stimulanty, Steroidy');

INSERT INTO LaboratorniPracovnik (Jmeno, Prijmeni, DatumNarozeni, Pohlavi, Narodnost, Adresa, Email, Telefon, LaboratorId) VALUES ('Dana', 'Balcarová', TO_DATE('1973-12-15', 'YYYY-MM-DD'), 'Z', 'Česká', 'Výzkumná 682, Praha', 'balcarova@laboratory.com', '+420173527976', 1);
INSERT INTO LaboratorniPracovnik (Jmeno, Prijmeni, DatumNarozeni, LaboratorId) VALUES ('Marie', 'Curie', TO_DATE('1856-06-06', 'YYYY-MM-DD'), 2);

INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903250001, 'Krev', 'Hlavni', 'Negativni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903250002, 'Krev', 'Kontrolni', 'Pozitivni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903250003, 'Moc', 'Hlavni', 'Pozitivni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903250004, 'Moc', 'Kontrolni', 'Negativni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903250005, 'Krev', 'Hlavni', 'Pozitivni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903250006, 'Krev', 'Kontrolni', 'Negativni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903250007, 'Krev', 'Hlavni', 'Pozitivni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903250008, 'Krev', 'Kontrolni', 'Pozitivni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903260001, 'Moc', 'Hlavni', 'Pozitivni');
INSERT INTO Vzorek (CiselnyKod, Typ, Vyznam, Vysledek) VALUES (201903260002, 'Moc', 'Kontrolni', 'Negativni');

INSERT INTO Sport (Nazev) VALUES ('Hokej');
INSERT INTO Sport (Nazev) VALUES ('Rychrobruslení');
INSERT INTO Sport (Nazev) VALUES ('Běh');

INSERT INTO ZakazanaLatka (Nazev, Kategorie, RokZakazani) VALUES ('Alkohol', 'Alkoholy', 2004);
INSERT INTO ZakazanaLatka (Nazev, Kategorie, RokZakazani) VALUES ('Oxandrolon', 'Anabolické steroidy', 2008);
INSERT INTO ZakazanaLatka (Nazev, Kategorie, RokZakazani) VALUES ('Ritalin', 'Stimulanty', 2016);

INSERT INTO Kontrola VALUES (1, TO_DATE('2019-03-25 11:24:00','YYYY-MM-DD hh24:mi:ss'), 'Stadion Kladno', 'Při soutěži', 'Ano', 'Ne', (SELECT Id FROM Sportovec WHERE Jmeno='Jaromír' AND Prijmeni='Jágr'), (SELECT Id FROM Sport WHERE Nazev='Hokej'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Ritalin'), 201903250001, 201903250002);
INSERT INTO Kontrola VALUES (2, TO_DATE('2019-03-25 11:40:00','YYYY-MM-DD hh24:mi:ss'), 'Stadion Kladno', 'Při soutěži', 'Ano', 'Ne', (SELECT Id FROM Sportovec WHERE Jmeno='Jaromír' AND Prijmeni='Jágr'), (SELECT Id FROM Sport WHERE Nazev='Hokej'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Oxandrolon'), 201903250003, 201903250004);
INSERT INTO Kontrola VALUES (3, TO_DATE('2019-03-25 11:51:00','YYYY-MM-DD hh24:mi:ss'), 'Stadion Kladno', 'Při soutěži', 'Ano', 'Ne', (SELECT Id FROM Sportovec WHERE Jmeno='Jaromír' AND Prijmeni='Jágr'), (SELECT Id FROM Sport WHERE Nazev='Hokej'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Alkohol'), 201903250005, 201903250006);
INSERT INTO Kontrola VALUES (4, TO_DATE('2019-03-25 14:37:12','YYYY-MM-DD hh24:mi:ss'), 'Stadion Velký Osek', 'Mimo soutěž', 'Ano', 'Ne', (SELECT Id FROM Sportovec WHERE Jmeno='Martina' AND Prijmeni='Sáblíková'), (SELECT Id FROM Sport WHERE Nazev='Rychrobruslení'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Alkohol'), 201903250007, 201903250008);
INSERT INTO Kontrola VALUES (5, TO_DATE('2019-03-25 18:02:00','YYYY-MM-DD hh24:mi:ss'), 'Stadion Kopřivnice', 'Mimo soutěž', 'Ne', NULL, (SELECT Id FROM Sportovec WHERE Jmeno='Emil' AND Prijmeni='Zátopek'), (SELECT Id FROM Sport WHERE Nazev='Běh'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Oxandrolon'), NULL, NULL);
INSERT INTO Kontrola VALUES (6, TO_DATE('2019-03-26 09:13:00','YYYY-MM-DD hh24:mi:ss'), 'Stadion Kopřivnice', 'Mimo soutěž', 'Ano', 'Ne', (SELECT Id FROM Sportovec WHERE Jmeno='Emil' AND Prijmeni='Zátopek'), (SELECT Id FROM Sport WHERE Nazev='Běh'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Oxandrolon'), 201903260001, 201903260002);

INSERT INTO PovolenaLatka_Sportovec VALUES ((SELECT Id FROM Sportovec WHERE Jmeno='Martina' AND Prijmeni='Sáblíková'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Ritalin'), TO_DATE('2019-01-01', 'YYYY-MM-DD'), TO_DATE('2020-12-31', 'YYYY-MM-DD'));

INSERT INTO Komisar_Kontrola VALUES (1, 1);
INSERT INTO Komisar_Kontrola VALUES (1, 2);
INSERT INTO Komisar_Kontrola VALUES (1, 3);
INSERT INTO Komisar_Kontrola VALUES (2, 4);
INSERT INTO Komisar_Kontrola VALUES (3, 5);
INSERT INTO Komisar_Kontrola VALUES (3, 6);

INSERT INTO SportZakazuje_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Rychrobruslení'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Alkohol'));
INSERT INTO SportZakazuje_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Rychrobruslení'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Oxandrolon'));
INSERT INTO SportZakazuje_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Hokej'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Oxandrolon'));
INSERT INTO SportZakazuje_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Hokej'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Ritalin'));
INSERT INTO SportZakazuje_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Běh'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Ritalin'));
INSERT INTO SportZakazuje_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Běh'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Oxandrolon'));

INSERT INTO SportPovolujeMimoSoutez_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Běh'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Oxandrolon'));
INSERT INTO SportPovolujeMimoSoutez_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Hokej'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Alkohol'));
INSERT INTO SportPovolujeMimoSoutez_Latku VALUES ((SELECT Id FROM Sport WHERE Nazev='Hokej'), (SELECT Id FROM ZakazanaLatka WHERE Nazev='Ritalin'));

INSERT INTO Vzorek_Latka VALUES (201903250003, (SELECT Id FROM ZakazanaLatka WHERE Nazev='Oxandrolon'));

INSERT INTO LaboratorniPracovnik_Vzorek VALUES ((SELECT Id FROM LaboratorniPracovnik WHERE Jmeno='Marie' AND Prijmeni='Curie'), 201903250001);
INSERT INTO LaboratorniPracovnik_Vzorek VALUES ((SELECT Id FROM LaboratorniPracovnik WHERE Jmeno='Marie' AND Prijmeni='Curie'), 201903250003);
INSERT INTO LaboratorniPracovnik_Vzorek VALUES ((SELECT Id FROM LaboratorniPracovnik WHERE Jmeno='Marie' AND Prijmeni='Curie'), 201903250005);
INSERT INTO LaboratorniPracovnik_Vzorek VALUES ((SELECT Id FROM LaboratorniPracovnik WHERE Jmeno='Marie' AND Prijmeni='Curie'), 201903250007);
INSERT INTO LaboratorniPracovnik_Vzorek VALUES ((SELECT Id FROM LaboratorniPracovnik WHERE Jmeno='Dana' AND Prijmeni='Balcarová'), 201903260001);

INSERT INTO Sportovec_Sport VALUES((SELECT Id FROM Sportovec WHERE Jmeno='Jaromír' AND Prijmeni='Jágr'), (SELECT Id FROM Sport WHERE Nazev='Hokej'));
INSERT INTO Sportovec_Sport VALUES((SELECT Id FROM Sportovec WHERE Jmeno='Martina' AND Prijmeni='Sáblíková'), (SELECT Id FROM Sport WHERE Nazev='Rychrobruslení'));
INSERT INTO Sportovec_Sport VALUES((SELECT Id FROM Sportovec WHERE Jmeno='Emil' AND Prijmeni='Zátopek'), (SELECT Id FROM Sport WHERE Nazev='Běh'));

--------------------- 3.CAST - SELECT PRIKAZY ---------------------

--*-- Povinné 2 selecty s propojením alespoň 2 tabulek --*--

-- Dotaz vybere jména všech sportovců s pozitivním testem krve
SELECT DISTINCT S.Jmeno, S.Prijmeni
FROM Sportovec S 
JOIN Kontrola K ON S.Id = K.SportovecId 
JOIN Vzorek V_h ON K.HlavniVzorekId = V_h.CiselnyKod
JOIN Vzorek V_k ON K.KontrolniVzorekId = V_k.CiselnyKod
WHERE (V_h.Typ = 'Krev' AND V_h.Vysledek = 'Pozitivni') OR (V_k.Typ = 'Krev' AND V_k.Vysledek = 'Pozitivni');

-- Dotaz vybere zakázané látky, které byly prokázány v některých z testů
SELECT DISTINCT L.Nazev
FROM ZakazanaLatka L
JOIN Vzorek_Latka V_L ON L.Id = V_L.LatkaId
JOIN Vzorek V ON V_L.VzorekId = V.CiselnyKod
WHERE V.Vysledek = 'Pozitivni';

--*-- Povinný select s propojením alespoň 3 tabulek --*--

-- Dotaz vybere komisaře, kteří prováděli kontroly, u nichž jsou oba vzorky pozitivní
SELECT DISTINCT K.Jmeno, K.Prijmeni
FROM Komisar K
JOIN Komisar_Kontrola K_K ON K.Id = K_K.KomisarId
JOIN Kontrola C ON K_K.KontrolaId = C.Id
JOIN Vzorek V_h ON C.HlavniVzorekId = V_h.CiselnyKod
JOIN Vzorek V_k ON C.KontrolniVzorekId = V_k.CiselnyKod
WHERE V_h.Vysledek = 'Pozitivni' AND V_k.Vysledek = 'Pozitivni';

--*-- Povinné 2 selecty s klauzulí GROUP BY --*--

-- Dotaz vybere počet vzorků pro každého laboratorního pracovníka
SELECT LP.Prijmeni, Count(*) Pocet
FROM LaboratorniPracovnik LP, LaboratorniPracovnik_Vzorek LV
WHERE LP.Id = LV.LaboratorniPracovnikId
GROUP BY LP.Prijmeni;

-- Dotaz vybere počet kontrol u jednotlivých sportovců
SELECT S.jmeno, S.Prijmeni, Count(*) Pocet_kontrol
FROM Sportovec S, Kontrola K
WHERE S.Id = K.SportovecId
GROUP BY S.jmeno, S.Prijmeni;

--*-- Povinný select s predikátem EXISTS --*--

-- Dotaz vybere název sportu a zakázané látky, kde je tato látka mimo soutěž povolena
SELECT S.Nazev Sport, ZL.Nazev Zakazana_latka
FROM Sport S, ZakazanaLatka ZL
WHERE EXISTS (SELECT * FROM SportPovolujeMimoSoutez_Latku SL WHERE S.Id = SL.SportId AND ZL.Id = SL.LatkaId);

--*-- Povinný select s predikátem IN pro vnořený select --*--

-- Dotaz vybere název, kategorii a rok zakázání látky, která byla nalezena u nějakého běžce
SELECT L.Nazev, L.Kategorie, L.RokZakazani Rok_Zakazani
FROM ZakazanaLatka L
WHERE L.Id IN 
    (SELECT K.LatkaId 
     FROM Kontrola K 
     JOIN Vzorek V_h ON K.HlavniVzorekId = V_h.CiselnyKod
     JOIN Vzorek V_k ON K.KontrolniVzorekId = V_k.CiselnyKod
     WHERE (V_h.Vysledek = 'Pozitivni' OR V_k.Vysledek = 'Pozitivni') AND
            K.SportId IN (SELECT Id FROM Sport WHERE Nazev='Běh'));