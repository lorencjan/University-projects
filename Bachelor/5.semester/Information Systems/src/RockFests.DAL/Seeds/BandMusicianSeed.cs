using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;

namespace RockFests.DAL.Seeds
{
    public class BandMusicianSeed
    {
        private static readonly List<BandMusician> Data = new List<BandMusician>
        {
            //Metallica
            new BandMusician
            {
                BandId = 1,
                MusicianId = 1
            },
            new BandMusician
            {
                BandId = 1,
                MusicianId = 2
            },
            new BandMusician
            {
                BandId = 1,
                MusicianId = 3
            },
            new BandMusician
            {
                BandId = 1,
                MusicianId = 4
            },

            //Iron Maiden
            new BandMusician
            {
                BandId = 2,
                MusicianId = 5
            },
            new BandMusician
            {
                BandId = 2,
                MusicianId = 6
            },
            new BandMusician
            {
                BandId = 2,
                MusicianId = 7
            },
            new BandMusician
            {
                BandId = 2,
                MusicianId = 8
            },
            new BandMusician
            {
                BandId = 2,
                MusicianId = 9
            },
            new BandMusician
            {
                BandId = 2,
                MusicianId = 10
            },

            //Guns N' Roses
            new BandMusician
            {
                BandId = 3,
                MusicianId = 12
            },
            new BandMusician
            {
                BandId = 3,
                MusicianId = 13
            },
            new BandMusician
            {
                BandId = 3,
                MusicianId = 14
            },
            new BandMusician
            {
                BandId = 3,
                MusicianId = 15
            },
            new BandMusician
            {
                BandId = 3,
                MusicianId = 16
            },
            new BandMusician
            {
                BandId = 3,
                MusicianId = 17
            },
            new BandMusician
            {
                BandId = 3,
                MusicianId = 18
            },

            //Pearl Jam
            new BandMusician
            {
                BandId = 4,
                MusicianId = 19
            },
            new BandMusician
            {
                BandId = 4,
                MusicianId = 20
            },
            new BandMusician
            {
                BandId = 4,
                MusicianId = 21
            },
            new BandMusician
            {
                BandId = 4,
                MusicianId = 22
            },
            new BandMusician
            {
                BandId = 4,
                MusicianId = 23
            },

            //Green Day
            new BandMusician
            {
                BandId = 5,
                MusicianId = 24
            },
            new BandMusician
            {
                BandId = 5,
                MusicianId = 25
            },
            new BandMusician
            {
                BandId = 5,
                MusicianId = 26
            },
            new BandMusician
            {
                BandId = 5,
                MusicianId = 27
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<BandMusician>().HasData(Data);
    }
}
