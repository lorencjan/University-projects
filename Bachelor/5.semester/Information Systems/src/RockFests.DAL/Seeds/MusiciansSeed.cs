using System;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;

namespace RockFests.DAL.Seeds
{
    public class MusiciansSeed
    {
        private static readonly Musician[] Data = new Musician[]
        {
            new Musician
            {
                Id = 1,
                FirstName = "James",
                LastName = "Hetfield",
                Born = new DateTime(1963, 8, 3),
                Photo = SeedImages.metallica_james_hetfield,
                Country = "USA",
                Role = "vocals, guitar",
                Biography = "James Alan Hetfield is an American musician and songwriter best known for being the co-founder, lead vocalist/rhythm guitarist and main songwriter for the heavy metal band Metallica. Hetfield is mainly known for his intricate rhythm playing, but occasionally performs lead guitar duties and solos, both live and in the studio. Hetfield co-founded Metallica in October 1981 after answering an advertisement by drummer Lars Ulrich in the Los Angeles newspaper The Recycler. Metallica has won nine Grammy Awards and released ten studio albums, three live albums, four extended plays and 24 singles."
            },
            new Musician
            {
                Id = 2,
                FirstName = "Lars",
                LastName = "Urlich",
                Born = new DateTime(1963, 12, 26),
                Photo = SeedImages.metallica_lars_urlich,
                Country = "Denmark",
                Role = "drums",
                Biography = "Lars Ulrich (/ˈʊlrɪk/; Danish: [ˈlɑːs ˈulˀʁek]) is a Danish musician, songwriter, and record producer. He gained worldwide fame as the drummer and co-founder of American heavy metal band Metallica. The son and grandson respectively of tennis players Torben and Einer Ulrich, he played tennis in his youth and moved to Los Angeles at age 16 to train professionally. However, rather than playing tennis, Ulrich began playing drums. After publishing an advertisement in The Recycler, Ulrich met vocalist/guitarist James Hetfield and formed Metallica. Along with Hetfield, Ulrich has songwriting credits on almost all of the band's songs. He was the face of the band during the Napster controversy. Later in his career, Ulrich began hosting the It's Electric podcast, in which he speaks with other prominent musicians."
            },
            new Musician
            {
                Id = 3,
                FirstName = "Kirk",
                LastName = "Hammett",
                Born = new DateTime(1962, 11, 18),
                Photo = SeedImages.metallica_kirk_hammett,
                Country = "USA",
                Role = "guitar",
                Biography = "Kirk Lee Hammett is an American musician who has been the lead guitarist and a contributing songwriter for heavy metal band Metallica since 1983. Before joining Metallica he formed and named the band Exodus. In 2003, Hammett was ranked 11th on Rolling Stone's list of The 100 Greatest Guitarists of All Time. In 2009, Hammett was ranked number 15 in Joel McIver's book The 100 Greatest Metal Guitarists."
            },
            new Musician
            {
                Id = 4,
                FirstName = "Robert",
                LastName = "Trujillo",
                Born = new DateTime(1964, 10, 23),
                Photo = SeedImages.metallica_robert_trujillo,
                Country = "USA",
                Role = "bass",
                Biography = "Robert Trujillo (/truːˈhiːjoʊ/ troo-HEE-yoh, Spanish: [roˈβeɾto tɾuˈxiʎo]) is an American musician and songwriter, best known for being the bassist of heavy metal band Metallica since 2003. He was a member of crossover thrash band Suicidal Tendencies, funk metal supergroup Infectious Grooves, and heavy metal band Black Label Society, and has worked with Jerry Cantrell and Ozzy Osbourne."
            },

            new Musician
            {
                Id = 5,
                FirstName = "Steve",
                LastName = "Harris",
                Born = new DateTime(1956, 3, 12),
                Photo = SeedImages.iron_maiden_steve_harris,
                Country = "England",
                Role = "bass, keyboard",
                Biography = "Stephen Percy Harris (born 12 March 1956) is an English musician, bassist, keyboardist, singer, backing vocalist, primary songwriter and founder of the British heavy metal band Iron Maiden. He has been the band's only constant member since their inception in 1975 and one of only two to have appeared on all of their albums, the other being guitarist Dave Murray.Harris has a recognisable and popular style of bass playing, particularly the \"gallop\" which can be found on several Iron Maiden recordings, such as the singles \"Run to the Hills\" and \"The Trooper\". In addition to his role as the band's bass player, writer and backing vocalist, he has undertaken many other roles for the group, such as producing and co-producing their albums, directing and editing their live videos and performing studio keyboards and synthesizers. He has been cited as one of the greatest heavy metal bassists. On 24 September 2012, Harris released his debut solo album, British Lion, which was followed on the 17 January 2020 by The Burning."
            },
            new Musician
            {
                Id = 6,
                FirstName = "Dave",
                LastName = "Murray",
                Born = new DateTime(1956, 12, 23),
                Photo = SeedImages.iron_maiden_dave_murray,
                Country = "England",
                Role = "guitar",
                Biography = "David Michael Murray is an English guitarist and songwriter. He was one of the earliest members of the British heavy metal band Iron Maiden and, along with the group's founder, bassist and primary songwriter Steve Harris, Murray is the only member who has appeared on all of the band's releases. Growing up in various areas of London, Murray became a member of a skinhead gang before he took an interest in rock music at 15 and formed his own band, Stone Free, with childhood friend Adrian Smith. After leaving school at 15,[2] he regularly answered advertisements which appeared in Melody Maker before auditioning for Iron Maiden in 1976. A short while later, Murray was sacked following an argument with the group's lead vocalist, Dennis Wilcock, and spent six months in Smith's band, Urchin. In the spring of 1978, following Wilcock's departure, Murray was asked to rejoin Iron Maiden, in which he has remained to this day."

            },
            new Musician
            {
                Id = 7,
                FirstName = "Adrian",
                LastName = "Smith",
                Born = new DateTime(1957, 2, 27),
                Photo = SeedImages.iron_maiden_adrian_smith,
                Country = "England",
                Role = "guitar",
                Biography = "Adrian Frederick \"H\" Smith is an English guitarist and pianist, best known as a member of British Heavy Metal band Iron Maiden. Smith grew up in London and became interested in rock music at 15. He soon formed a friendship with future Iron Maiden guitarist Dave Murray, who inspired him to take up the guitar. After leaving school at 16, he formed a band called Urchin, which he led until their demise in 1980. He joined Iron Maiden in November 1980, replacing Dennis Stratton. Following a short-lived solo project called ASAP, he left Iron Maiden in 1990 and, after a year-long hiatus, formed the bands The Untouchables and, subsequently, Psycho Motel. In 1997, Psycho Motel was put on hold when Smith joined the band of former Iron Maiden singer Bruce Dickinson. Smith and Dickinson would go on to return to Iron Maiden in 1999, after which the band gained new success. Smith has a current side project called Primal Rock Rebellion."
            },
            new Musician
            {
                Id = 8,
                FirstName = "Bruce",
                LastName = "Dickinson",
                Born = new DateTime(1958, 8, 7),
                Photo = SeedImages.bruce_dickinson,
                Country = "England",
                Role = "vocals", 
                Biography = "Paul Bruce Dickinson an English singer and songwriter. He is known for his work as the lead singer of the heavy metal band Iron Maiden since 1981, and is renowned for his wide-ranging operatic vocal style and energetic stage presence. Dickinson began his career in music fronting small pub bands in the 1970s while attending school in Sheffield and university in London. In 1979, he joined British new wave heavy metal band Samson, with whom he gained some popularity under the stage name \"Bruce Bruce\" and performed on two studio records. He left Samson in 1981 to join Iron Maiden, replacing Paul Di'Anno, and debuted on their 1982 album The Number of the Beast. During his first tenure in the band, they issued a series of US and UK platinum and gold albums in the 1980s."
            },
            new Musician
            {
                Id = 9,
                FirstName = "Nicko",
                LastName = "McBrain",
                Born = new DateTime(1952, 6, 5),
                Photo = SeedImages.iron_maiden_nicko_mcbrain,
                Country = "England",
                Role = "drums",
                Biography = "Michael Henry \"Nicko\" McBrain (born 5 June 1952) is an English musician and drummer of the British heavy metal band Iron Maiden, which he joined in 1982. Having played in small pub bands from the age of 14, after graduating school McBrain paid his bills with session work before he joined a variety of artists, such as Streetwalkers, Pat Travers, and the French political band, Trust. He joined Iron Maiden in time to debut on their fourth album (replacing Clive Burr), Piece of Mind (1983), and has remained with them since, contributing to a total of thirteen studio releases."
            },
            new Musician
            {
                Id = 10,
                FirstName = "Janick",
                LastName = "Gers",
                Born = new DateTime(1957, 1, 27),
                Photo = SeedImages.iron_maiden_janick_gers,
                Country = "England",
                Role = "guitar",
                Biography = "Janick Robert Gers (/ˈjænɪk ˈɡɜːrz/) is an English musician and one of the three guitarists in Iron Maiden. He was also previously a member of the bands Gillan and White Spirit."
            },

            new Musician
            {
                Id = 11,
                FirstName = "Marilyn",
                LastName = "Manson",
                Born = new DateTime(1969, 1, 5),
                Photo = SeedImages.marilyn_manson,
                Country = "USA",
                Role = "vocals",
                Biography = "Brian Hugh Warner (born January 5, 1969), known professionally as Marilyn Manson, is an American singer, songwriter, record producer, actor, painter,[1] writer,[2] and former music journalist. He is known for his controversial stage personality and image as the lead singer of the band of the same name, which he co-founded with guitarist Daisy Berkowitz in 1989 and of which he remains the only constant member. Like the other founding members of the band, his stage name was formed by combining and juxtaposing the names of two opposing American cultural icons: a sex symbol and a serial killer; in Manson's case, actress Marilyn Monroe and criminal Charles Manson. Manson is best known for music released in the 1990s, most notably the albums Portrait of an American Family (1994), Antichrist Superstar (1996) and Mechanical Animals (1998) which earned him a reputation in mainstream media as a controversial figure and negative influence on young people when combined with his public image.[3][4] In the U.S.alone, three of the band's albums have been awarded platinum status and three more went gold, and the band has had eight releases debut in the top 10, including two No. 1 albums. Manson has been ranked at No. 44 on the list of the \"Top 100 Heavy Metal Vocalists\" by Hit Parader and, along with his band, has been nominated for four Grammy Awards. Manson made his film debut as an actor in David Lynch's Lost Highway (1997), and has since appeared in a variety of minor roles and cameos. He was interviewed in Michael Moore's political documentary about gun violence, Bowling for Columbine, discussing possible motivations for the 1999 Columbine massacre, after media speculation that the shooters were avid fans of Manson's music; he denied allegations that his music was a contributory factor. In September 2002, his first art show, The Golden Age of Grotesque, was held at the Los Angeles Contemporary Exhibitions center. At a 2010 exhibition at Kunsthalle gallery in Vienna, he unveiled Genealogies of Pain, a series of 20 paintings on which he collaborated with Lynch.[5]"
            },

            new Musician
            {
                Id = 12,
                FirstName = "Axl",
                LastName = "Rose",
                Born = new DateTime(1962, 2, 6),
                Photo = SeedImages.guns_and_roses_axl_rose,
                Country = "USA",
                Role = "vocals",
                Biography = "W. Axl Rose is an American musician, singer, songwriter and record producer. He is the lead vocalist and lyricist of the hard rock band Guns N' Roses, and has also been the band's sole constant member since its inception in 1985. He has also toured with Australian rock band AC/DC, deputising for Brian Johnson. Rose has been named one of the greatest singers of all time by various media outlets, including Rolling Stone and NME."
            },
            new Musician
            {
                Id = 13,
                FirstName = "Duff",
                LastName = "McKagan",
                Born = new DateTime(1964, 2, 5),
                Photo = SeedImages.guns_and_roses_duff_mckagan,
                Country = "USA",
                Role = "bass",
                Biography = "Michael Andrew \"Duff\" McKagan, sometimes credited as Duff \"Rose\" McKagan, is an American multi-instrumentalist, singer, songwriter and author. He is best known for his twelve-year tenure as the bassist of the hard rock band Guns N' Roses, with whom he achieved worldwide success in the late 1980s and early 1990s. McKagan rejoined the band in 2016, following their induction into the Rock and Roll Hall of Fame. Toward the end of his first tenure with Guns N' Roses, McKagan released a solo album, Believe in Me (1993), and formed the short-lived supergroup Neurotic Outsiders. Following his departure from Guns N' Roses in 1997, McKagan briefly reunited with his pre-success Seattle punk band 10 Minute Warning, before forming the still-active hard rock band Loaded, in which he performs lead vocals and rhythm guitar. Between 2002 and 2008, he played bass in the supergroup Velvet Revolver with his former Guns N' Roses bandmates Slash and Matt Sorum. He briefly performed with Alice in Chains in 2006, with Jane's Addiction in 2010 and joined the supergroup Hollywood Vampires in 2016."
            },
            new Musician
            {
                Id = 14,
                FirstName = "Slash",
                LastName = "",
                Born = new DateTime(1965, 7, 23),
                Photo = SeedImages.slash,
                Country = "England",
                Role = "guitar",
                Biography = "Saul Hudson, better known as Slash, is an English-American musician, songwriter, record producer, and film producer. He is best known as the lead guitarist of the American hard rock band Guns N' Roses, with whom he achieved worldwide success in the late 1980s and early 1990s. Slash has received critical acclaim and is considered one of the greatest guitarists in rock history. In 1993, Slash formed the side project Slash's Snakepit and in 1996 he left Guns N' Roses and co-founded the supergroup Velvet Revolver, which re-established him as a mainstream performer in the mid to late 2000s. Slash has released four solo albums: Slash (2010), featuring an array of guest musicians, Apocalyptic Love (2012), World on Fire (2014) and Living the Dream (2018) recorded with his band, Myles Kennedy and the Conspirators. He returned to Guns N' Roses in 2016."
            },
            new Musician
            {
                Id = 15,
                FirstName = "Dizzy",
                LastName = "Reed",
                Born = new DateTime(1963, 6, 18),
                Photo = SeedImages.guns_and_roses_dizzy_reed,
                Country = "USA",
                Role = "keyboard",
                Biography = "Darren Arthur Reed, better known by his stage name Dizzy Reed, is an American musician and occasional actor. He is best known as the keyboardist for the rock band Guns N' Roses, with whom he has played, toured, and recorded since 1990. Aside from lead singer Axl Rose, Reed is the longest-standing member of Guns N' Roses, and was the only member of the band to remain from their Use Your Illusion era, until the early 2016 return of guitarist Slash and bass guitarist Duff McKagan.In 2012, Reed was inducted into the Rock and Roll Hall of Fame as a member of Guns N' Roses, although he did not attend the ceremony. He was also a member of the Australian-American supergroup The Dead Daisies with his Guns N' Roses bandmate Richard Fortus, ex-Whitesnake member Marco Mendoza, ex-Mötley Crüe frontman John Corabi and session drummer Brian Tichy."
            },
            new Musician
            {
                Id = 16,
                FirstName = "Richard",
                LastName = "Fortus",
                Born = new DateTime(1966, 11, 17),
                Photo = SeedImages.guns_and_roses_richard_fortus,
                Country = "USA",
                Role = "guitar",
                Biography = "Richard Fortus is an American guitarist. He has been a member of the hard rock band Guns N' Roses since 2002, and with whom he has recorded one studio album.[1][2] Fortus has also collaborated extensively with The Psychedelic Furs frontman Richard Butler and fellow Guns N' Roses bandmate Frank Ferrer. Aside from lead singer Axl Rose, and keyboardist Dizzy Reed, Fortus is the longest tenured member of Guns N' Roses, having been with the band continuously since 2002."
            },
            new Musician
            {
                Id = 17,
                FirstName = "Frank",
                LastName = "Ferrer",
                Born = new DateTime(1966, 3, 25),
                Photo = SeedImages.guns_and_roses_frank_ferrer,
                Country = "USA",
                Role = "drums",
                Biography = "Frank Ferrer is an American rock drummer. He is best known as the drummer for American rock band Guns N' Roses, with whom he has played, toured, and recorded since 2006. Ferrer was also a member of The Psychedelic Furs, Love Spit Love as well as The Beautiful. He has recorded and worked with several high profile musicians including Robi \"Draco\" Rosa, Tool, Gordon Gano, PJ Harvey, Tommy Stinson, Nena, Frank Black of The Pixies, Neil Young, Perry Farrell and Cheetah Chrome of The Dead Boys."
            },
            new Musician
            {
                Id = 18,
                FirstName = "Melissa",
                LastName = "Reese",
                Born = new DateTime(1990, 3, 1),
                Photo = SeedImages.guns_and_roses_melissa_reese,
                Country = "USA",
                Role = "vocals",
                Biography = "Melissa Reese is an American musician and model who has collaborated with Bryan \"Brain\" Mantia and is a current member of hard rock band Guns N' Roses."
            },

            new Musician
            {
                Id = 19,
                FirstName = "Eddie",
                LastName = "Vedder",
                Born = new DateTime(1964, 12, 23),
                Photo = SeedImages.pearl_jam_eddie_vedder,
                Country = "USA",
                Role = "vocals",
                Biography = "Eddie Jerome Vedder (born Edward Louis Severson III; December 23, 1964) is an American musician, multi-instrumentalist and singer-songwriter, best known as the lead vocalist, one of three guitarists, and the primary lyricist of the American rock band Pearl Jam. He also appeared as a guest vocalist in Temple of the Dog, the one-off tribute band dedicated to the late singer Andrew Wood.Vedder is known for his powerful baritone vocals. He was ranked at number 7 on a list of \"Best Lead Singers of All Time\", based on a readers' poll compiled by Rolling Stone.[4] In 2007, Vedder released his first solo album as a soundtrack for the film Into the Wild (2007). His second album Ukulele Songs and a live DVD titled Water on the Road were released in 2011. In 2017, Vedder was inducted into the Rock and Roll Hall of Fame as a member of Pearl Jam.[5]"
            },
            new Musician
            {
                Id = 20,
                FirstName = "Mike",
                LastName = "McCready",
                Born = new DateTime(1966, 4, 5),
                Photo = SeedImages.pearl_jam_mike_mcready,
                Country = "USA",
                Role = "guitar",
                Biography = "Michael David McCready is an American musician who serves as the lead guitarist for the American rock band Pearl Jam. Along with Jeff Ament, Stone Gossard, and Eddie Vedder, he is one of the founding members of the band. McCready was also a member of the side project bands Flight to Mars, Temple of the Dog, Mad Season and The Rockfords. He was inducted into the Rock and Roll Hall of Fame as a member of Pearl Jam on April 7, 2017."
            },
            new Musician
            {
                Id = 21,
                FirstName = "Stone",
                LastName = "Gossard",
                Born = new DateTime(1966, 7, 20),
                Photo = SeedImages.pearl_jam_stone_gossard,
                Country = "USA",
                Role = "guitar",
                Biography = "Stone Carpenter Gossard (born July 20, 1966) is an American musician who serves as the rhythm and additional lead guitarist, also co-lyricist for the American rock band Pearl Jam. Along with Jeff Ament, Mike McCready, and Eddie Vedder, he is one of the founding members of the band.Gossard is also known for his work prior to Pearl Jam with the Seattle-based grunge bands Green River and Mother Love Bone. In addition to his performing career, he has been active in the music industry as a producer and the owner of a record label and recording studio. Gossard is also a member of the bands Temple of the Dog and Brad. He released his first solo album Bayleaf in 2001; his second, Moonlander, followed in 2013. Gossard was inducted into the Rock and Roll Hall of Fame as a member of Pearl Jam on April 7, 2017.[1]"
            },
            new Musician
            {
                Id = 22,
                FirstName = "Jeff",
                LastName = "Ament",
                Born = new DateTime(1963, 3, 10),
                Photo = SeedImages.pearl_jam_jeff_ament,
                Country = "USA",
                Role = "bass",
                Biography = "Jeffrey Allen Ament is an American musician and songwriter who is best known as the bassist of the American rock band Pearl Jam, which he co-founded alongside Stone Gossard, Mike McCready, and Eddie Vedder. Prior to his work with Pearl Jam, Ament was part of the 1980s Seattle-based grunge rock bands Green River and Mother Love Bone. He is known particularly for playing with the fretless bass, upright bass, and twelve-string bass guitars. Ament is also a member of the bands Temple of the Dog, Three Fish, RNDM, and Tres Mts. In 2008, Ament released his first solo album, Tone. His second solo release, While My Heart Beats, followed in 2012, and his third in 2018: Heaven/Hell. Ament was inducted into the Rock and Roll Hall of Fame as a member of Pearl Jam on April 7, 2017. He was also recognized as one of the top hard rock/metal bassists of all time by Loudwire in 2016, being placed at #52 on the list."
            },
            new Musician
            {
                Id = 23,
                FirstName = "Matt",
                LastName = "Cameron",
                Born = new DateTime(1962, 11, 28),
                Photo = SeedImages.pearl_jam_matt_cameron,
                Country = "USA",
                Role = "drums",
                Biography = "Matthew David Cameron is an American musician who is the drummer for Pearl Jam. After getting his start with the Seattle, Washington-based bands Bam Bam and Skin Yard, he first gained fame as the drummer for Soundgarden, which he joined in 1986, appeared on every studio album, and remained in until the band's break-up in 1997. In 1998, Cameron was invited to play on Pearl Jam's U.S. Yield Tour. He soon became a permanent member and has remained in the band ever since. In 2010, Soundgarden reunited, and Cameron remained with them until the death of Soundgarden's lead singer Chris Cornell on May 18, 2017, and their subsequent disbandment two years later."
            },

            new Musician
            {
                Id = 24,
                FirstName = "Billie",
                LastName = "Armstrong",
                Born = new DateTime(1972, 2, 17),
                Photo = SeedImages.green_day_billie_armstrong,
                Country = "USA",
                Role = "vocals, guitar",
                Biography = "Billie Joe Armstrong is an American singer, songwriter, musician, record producer, and actor. Armstrong serves as the lead vocalist, primary songwriter, and lead guitarist of the punk rock band Green Day, co-founded with Mike Dirnt. He is also a guitarist and vocalist for the punk rock band Pinhead Gunpowder, and provides lead vocals for Green Day's side projects Foxboro Hot Tubs, The Network, The Longshot and The Coverups. Raised in Rodeo, California, Armstrong developed an interest in music at a young age, and recorded his first song at the age of five. He met Mike Dirnt while attending elementary school, and the two instantly bonded over their mutual interest in music, forming the band Sweet Children when the two were 14 years old. The band changed its name to Green Day, and would later achieve commercial success. Armstrong has also pursued musical projects outside of Green Day's work, including numerous collaborations with other musicians. In 1997, to coincide with the release of Nimrod, Armstrong founded Adeline Records in Oakland to help support other bands releasing music, and signed bands such as The Frustrators, AFI and Dillinger Four. The record company later came under the management of Pat Magnarella and finally shut down in August 2017."
            },
            new Musician
            {
                Id = 25,
                FirstName = "Mike",
                LastName = "Dirnt",
                Born = new DateTime(1972, 5, 4),
                Photo = SeedImages.green_day_mike_dirnt,
                Country = "USA",
                Role = "bass",
                Biography = "Michael Ryan Pritchard, better known by his stage name Mike Dirnt, is an American musician, singer, and songwriter. He is best known as the co-founder, bassist, backing and occasional lead vocalist and former guitarist of Green Day. He has also played in several other bands, including the Frustrators. His stage name was originally a nickname that his friends from grade school gave him, as he constantly played \"air bass/guitar\" and made a \"dirnt, dirnt, dirnt\" noise while pretending to pluck the strings."
            },
            new Musician
            {
                Id = 26,
                FirstName = "Tré",
                LastName = "Cool",
                Born = new DateTime(1972, 12, 9),
                Photo = SeedImages.green_day_tre_cool,
                Country = "Germany",
                Role = "drums",
                Biography = "Frank Edwin Wright III, better known by his stage name Tré Cool, is a German-born American musician, singer, and songwriter best known as the drummer for the punk rock band Green Day. He replaced the band's former drummer, John Kiffmeyer, in 1990 as Kiffmeyer felt that he should focus on college. Cool has also played in The Lookouts, Samiam, Dead Mermaids, Bubu and the Brood and the Green Day side projects The Network and the Foxboro Hot Tubs."
            },
            new Musician
            {
                Id = 27,
                FirstName = "Jason",
                LastName = "White",
                Born = new DateTime(1973, 11, 11),
                Photo = SeedImages.green_day_jason_white,
                Country = "USA",
                Role = "touring guitar",
                Biography = "Jason White is an American musician and singer, best known as the touring rhythm and lead guitarist for the American punk rock band Green Day. He has played as a touring member of Green Day since 1997. He is also the guitarist/vocalist for the Californian punk band Pinhead Gunpowder, and co-founder of Adeline Records alongside Billie Joe Armstrong, and guitarist and lead vocalist for the Green Day side project The Coverups. In late 2014, he was diagnosed with tonsil cancer, which has since been treated."
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Musician>().HasData(Data);
    }
}
