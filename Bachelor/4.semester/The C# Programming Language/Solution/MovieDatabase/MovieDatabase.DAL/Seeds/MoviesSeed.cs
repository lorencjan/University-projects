using System;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Enums;

namespace MovieDatabase.DAL.Seeds
{
    public class MoviesSeed
    {
        private static readonly Movie[] _data = new Movie[]
        {
            new Movie
            {
                Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                OriginalName = "Titanic",
                CzechName = "Titanic",
                Genre = Genre.Catasthrophic,
                TitlePhoto = SeedImages.Titanic,
                Country = "USA",
                Year = 1997,
                Duration = 194,
                Description = "Titanic is a 1997 American epic romance and disaster film directed, written, co-produced, and co-edited by James Cameron. Incorporating both historical and fictionalized aspects, the film is based on accounts of the sinking of the RMS Titanic, and stars Leonardo DiCaprio and Kate Winslet as members of different social classes who fall in love aboard the ship during its ill-fated maiden voyage. Cameron's inspiration for the film came from his fascination with shipwrecks; he felt a love story interspersed with the human loss would be essential to convey the emotional impact of the disaster. Production began in 1995, when Cameron shot footage of the actual Titanic wreck. The modern scenes on the research vessel were shot on board the Akademik Mstislav Keldysh, which Cameron had used as a base when filming the wreck. Scale models, computer-generated imagery, and a reconstruction of the Titanic built at Baja Studios were used to re-create the sinking. The film was co-financed by Paramount Pictures and 20th Century Fox; the former handled distribution in North America while the latter released the film internationally. It was the most expensive film ever made at the time, with a production budget of $200 million."
            },
            new Movie
            {
                Id = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                OriginalName = "Troy",
                CzechName = "Trója",
                Genre = Genre.Historical,
                TitlePhoto = SeedImages.Troy,
                Country = "USA",
                Year = 2004,
                Duration = 157,
                Description = "Troy is a 2004 epic historical war drama film directed by Wolfgang Petersen and written by David Benioff. Produced by units in Malta, Mexico and Britain's Shepperton Studios, the film features an ensemble cast led by Brad Pitt, Eric Bana, and Orlando Bloom. It is loosely based on Homer's Iliad in its narration of the entire story of the decade-long Trojan War—condensed into little more than a couple of weeks, rather than just the quarrel between Achilles and Agamemnon in the ninth year. Achilles leads his Myrmidons along with the rest of the Greek army invading the historical city of Troy, defended by Hector's Trojan army. The end of the film (the sack of Troy) is not taken from the Iliad, but rather from Quintus Smyrnaeus's Posthomerica as the Iliad concludes with Hector's death and funeral. Troy made over $497 million worldwide, temporarily placing it in the #60 spot of top box office hits of all time. It received a nomination for the Academy Award for Best Costume Design and was the 8th highest-grossing film of 2004."
            },
            new Movie
            {
                Id = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"),
                OriginalName = "Once Upon a Time in Hollywood",
                CzechName = "Tenkrát v Hollywoodu",
                Genre = Genre.Comedy,
                TitlePhoto = SeedImages.OnceUponATimeInHollywood,
                Country = "USA",
                Year = 2019,
                Duration = 161,
                Description = "Once Upon a Time in Hollywood is a 2019 comedy-drama film written and directed by Quentin Tarantino. Produced by Columbia Pictures, Bona Film Group, Heyday Films, and Visiona Romantica and distributed by Sony Pictures Releasing, it is a co-production between the United States and the United Kingdom. It features a large ensemble cast led by Leonardo DiCaprio, Brad Pitt, and Margot Robbie. Set in 1969 Los Angeles, the film follows an actor and his stunt double, as they navigate the changing film industry, and features multiple storylines in a modern fairy tale tribute to the final moments of Hollywood's golden age."
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().HasData(_data);
        }
    }
}
