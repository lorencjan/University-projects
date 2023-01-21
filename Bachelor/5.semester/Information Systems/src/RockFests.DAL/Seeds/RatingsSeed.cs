using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;

namespace RockFests.DAL.Seeds
{
    public class RatingsSeed
    {
        private static readonly List<BandRating> BandRatings = new List<BandRating>
        {
            new BandRating
            {
                Id = 1,
                Number = 10,
                Text = "The best band ever. Each album has so much to offer. It's rare for any interpret not to have a bad song",
                UserName = "CarolusNovus",
                BandId = 1
            },
            new BandRating
            {
                Id = 2,
                Number = 9,
                Text = "",
                UserName = "admin",
                BandId = 1
            },
            new BandRating
            {
                Id = 3,
                Number = 9,
                Text = "Undoubtedly the most influential metal band of all time. Unfortunately the latest songs are too logn for my taste.",
                UserName = "spectator",
                BandId = 2
            },
            new BandRating
            {
                Id = 4,
                Number = 9,
                Text = "",
                UserName = "CarolusNovus",
                BandId = 2
            }
        };

        private static readonly List<MusicianRating> MusicianRatings = new List<MusicianRating>
        {
            new MusicianRating
            {
                Id = 1,
                Number = 10,
                Text = "I love the hat.",
                UserName = "CarolusNovus",
                MusicianId = 14
            },
            new MusicianRating
            {
                Id = 2,
                Number = 7,
                Text = "",
                UserName = "cashier",
                MusicianId = 14
            },
            new MusicianRating
            {
                Id = 3,
                Number = 8,
                Text = "",
                UserName = "CarolusNovus",
                MusicianId = 11
            },
        };

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BandRating>().HasData(BandRatings);
            modelBuilder.Entity<MusicianRating>().HasData(MusicianRatings);
        }
    }
}
