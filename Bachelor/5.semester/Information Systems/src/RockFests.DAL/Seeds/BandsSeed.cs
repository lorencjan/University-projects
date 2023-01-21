using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;

namespace RockFests.DAL.Seeds
{
    public class BandsSeed
    {
        private static readonly List<Band> Data = new List<Band>
        {
            new Band
            {
                Id = 1,
                Name = "Metallica",
                Genre = "Trash metal",
                Photo = SeedImages.metallica_logo,
                Country = "USA",
                FormationYear = 1981,
                Description = "Metallica is an American heavy metal band. The band was formed in 1981 in Los Angeles by vocalist/guitarist James Hetfield and drummer Lars Ulrich, and has been based in San Francisco for most of its career. The band's fast tempos, instrumentals and aggressive musicianship made them one of the founding \"big four\" bands of thrash metal, alongside Megadeth, Anthrax and Slayer. Metallica's current lineup comprises founding members and primary songwriters Hetfield and Ulrich, longtime lead guitarist Kirk Hammett, and bassist Robert Trujillo."
            },
            new Band
            {
                Id = 2,
                Name = "Iron Maiden",
                Genre = "Heavy metal",
                Photo = SeedImages.iron_maiden_logo,
                Country = "England",
                FormationYear = 1975,
                Description = "Iron Maiden are an English heavy metal band formed in Leyton, East London, in 1975 by bassist and primary songwriter Steve Harris. The band's discography has grown to 39 albums, including 16 studio albums, 12 live albums, four EPs, and seven compilations.Iron Maiden are considered one of the most influential and successful heavy metal bands in history, with The Sunday Times reporting in 2017 that the band have sold over 100 million copies of their albums worldwide."
            },
            new Band
            {
                Id = 3,
                Name = "Guns N' Roses",
                Genre = "Hard rock",
                Photo = SeedImages.guns_and_roses_logo,
                Country = "USA",
                FormationYear = 1985,
                Description = "Guns N' Roses, often abbreviated as GNR, is an American hard rock band from Los Angeles, California, formed in 1985. When they signed to Geffen Records in 1986, the band comprised vocalist Axl Rose, lead guitarist Slash, rhythm guitarist Izzy Stradlin, bassist Duff McKagan, and drummer Steven Adler. The current lineup consists of Rose, Slash, McKagan, keyboardist Dizzy Reed, guitarist Richard Fortus, drummer Frank Ferrer and keyboardist Melissa Reese. In their early years, the band's hedonism and rebelliousness drew comparisons to the early Rolling Stones and earned them the nickname \"the most dangerous band in the world\". The band's classic lineup, along with later members Reed and drummer Matt Sorum, was inducted into the Rock and Roll Hall of Fame in 2012, its first year of eligibility. Guns N' Roses have sold more than 100 million records worldwide, including 45 million in the United States, making them one of the best-selling acts in history."
            },
            new Band
            {
                Id = 4,
                Name = "Pearl Jam",
                Genre = "Grunge, Alternative rock",
                Photo = SeedImages.pearl_jam_logo,
                Country = "USA",
                FormationYear = 1990,
                Description = "Pearl Jam is an American rock band formed in 1990 in Seattle, Washington. The band's lineup consists of founding members Jeff Ament (bass guitar), Stone Gossard (rhythm guitar), Mike McCready (lead guitar), and Eddie Vedder (lead vocals, guitar), as well as Matt Cameron (drums), who joined in 1998. Keyboardist Boom Gaspar has also been a touring/session member with the band since 2002. Drummers Jack Irons, Dave Krusen, Matt Chamberlain, and Dave Abbruzzese are former members of the band. The band sold nearly 32 million albums in the United States by 2012, and by 2018, they had sold more than 85 million albums worldwide.[3] Pearl Jam outsold many of its contemporary alternative rock bands from the early 1990s, and is considered one of the most influential bands of the decade."
            },
            new Band
            {
                Id = 5,
                Name = "Green Day",
                Genre = "Punk rock",
                Photo = SeedImages.green_day_logo,
                Country = "USA",
                FormationYear = 1987,
                Description = "Green Day is an American rock band formed in the East Bay of California in 1987 by lead vocalist and guitarist Billie Joe Armstrong and bassist and backing vocalist Mike Dirnt. For much of the band's career, they have been a trio with a drummer Tré Cool, who replaced John Kiffmeyer in 1990 before the recording of the band's second studio album, Kerplunk (1991). Touring guitarist Jason White became a full-time member in 2012, but returned to his role as a touring member in 2016. Green Day has sold more than 75 million records worldwide, making them one of the best-selling music artists of all time."
            },
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Band>().HasData(Data);
    }
}
