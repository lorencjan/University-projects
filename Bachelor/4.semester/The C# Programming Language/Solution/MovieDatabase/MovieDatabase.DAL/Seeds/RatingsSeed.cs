using System;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.DAL.Entities;

namespace MovieDatabase.DAL.Seeds
{
    public class RatingsSeed
    {
        private static readonly Rating[] _data = new Rating[]
        {
            new Rating
            {
                Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                Number = 9,
                Text = "But it's an inspiring tale of men(!) at war in ancient times. The movie, albeit long, moves along a good pace, with mercifully brief romantic and philosophical breaks between the combat scenes. This movie is action, with more than a little thought put into accurately presenting the realities of the tactics used in Greek warfare. Troy is also to be congratulated for not over-armoring the cast like previous Hollywood productions and staying true to the lightness of armor prevalent during the historical period. Lovers of Homer and Greek mythology may be disappointed but keep in mind this film is about the Trojan War, not the Iliad. This war is epic in scale and isn't about poetry. Still, it would be great if Sean Bean were given the opportunity to play Odysseus again. Although not on screen much in Troy, his performance is edgy and true to the legends of the cunning king of Ithaca.",
                MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6")
            },
            new Rating
            {
                Id = new Guid("29f9ee6e-abbb-4cb7-a7f4-48b5adb713b9"),
                Number = 10,
                Text = "The sequence between Hector and Achilles is the best choreographed fight scene amongst epic fight movies. Cons for Jaime Abregana Jr. The harmony of the deathliest attacks of the heroes, usage of the camera plans with minimum keeps the sequence to watch in breathless. Until the warning of the shocking rise of the back music with the first death injury of Hector, you completely forget to inhale. If you don't care the out of the plan insertions of the castle balcony people (King/father, wife, Helene etc) the whole fight sequence is planned and played very well. The rest of the cast is good enough with a special note for Peter O'Toole and Brian Cox. Their lines are well delivered and their characters are believable.",
                MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6")
            },
            new Rating
            {
                Id = new Guid("cfbdd89d-6620-4c19-841f-ebd6ef5c4825"),
                Number = 6,
                Text = "Cinematography - great, acting - great. Music- reused already written works. Plot - none. Just almost 3+ hours worth of some random scenes stiched together without leading anywhere. Watched it twice. Has not changed my opinion. By the time it ended I did not care anymore, I just wanted to pee!!! After sitting there for almost 3 hours. What was the point of this movie? Still wondering. One of the Tarantino's worst (if not worst!).",
                MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792")
            },
            new Rating
            {
                Id = new Guid("2068787a-4352-47b7-9453-85ed8c8664c3"),
                Number = 10,
                Text = "To all the miserable people who have done everything from complain about the dialogue, the budget, the this and the that....who wants to hear it? IF you missed the point of this beyond-beautiful movie, that's your loss. The rest of us who deeply love this movie do not care what you think. I am a thirthysomething guy who has seen thousands of movies in my life, and this one stands in its own entity, in my book. It was not supposed to be a documentary, or a completely factual account of what happened that night. It is the most amazing love story ever attempted. I know that it is the cynical 90's and the millennium has everyone in a tizzy, but come on. Someone on this comments board complained that it made too much money! How lame is that? It made bundles of money in every civilized country on the planet, and is the top grossing film in the planet. I will gladly side with the majority this time around. Okay, cynics, time to crawl back under your rock, I am done.",
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5")
            },
            new Rating
            {
                Id = new Guid("8ecf6824-ab94-4e39-b81d-223afd4cbe50"),
                Number = 10,
                Text = "You can watch this movie in 1997, you can watch it again in 2004 or 2009 or you can watch it in 2015 or 2020, and this movie will get you EVERY TIME. Titanic has made itself FOREVER a timeless classic! I just saw it today (2015) and I was crying my eyeballs out JUST like the first time I saw it back in 1998. This is a movie that is SO touching, SO precise in the making of the boat, the acting and the storyline is BRILLIANT! And the preciseness of the ship makes it even more outstanding! Kate Winslet and Leonardo Dicaprio definitely created a timeless classic that can be watched time and time again and will never get old. This movie will always continue to be a beautiful, painful & tragic movie. 10/10 stars for this masterpiece!",
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5")
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>().HasData(_data);
        }
    }
}
