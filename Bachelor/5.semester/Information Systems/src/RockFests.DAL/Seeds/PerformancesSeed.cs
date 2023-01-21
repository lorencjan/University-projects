using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;

namespace RockFests.DAL.Seeds
{
    public class PerformancesSeed
    {
        private static readonly List<Performance> Data = new List<Performance>
        {
            new Performance
            {
                Id = 1,
                Start = new DateTime(2021, 2, 12, 16, 0, 0),
                End = new DateTime(2021, 2, 12, 18, 30, 0),
                StageId = 1,
                BandId = 2
            },
            new Performance
            {
                Id = 2,
                Start = new DateTime(2021, 2, 12, 19, 0, 0),
                End = new DateTime(2021, 2, 12, 21, 0, 0),
                StageId = 2,
                MusicianId = 14
            },
            new Performance
            {
                Id = 3,
                Start = new DateTime(2021, 2, 12, 21, 30, 0),
                End = new DateTime(2021, 2, 13, 0, 0, 0),
                StageId = 1,
                BandId = 1
            },
            new Performance
            {
                Id = 4,
                Start = new DateTime(2021, 2, 13, 15, 0, 0),
                End = new DateTime(2021, 2, 13, 17, 0, 0),
                StageId = 2,
                MusicianId = 8
            },
            new Performance
            {
                Id = 5,
                Start = new DateTime(2021, 2, 13, 17, 30, 0),
                End = new DateTime(2021, 2, 13, 19, 30, 0),
                StageId = 1,
                BandId = 3
            },
            new Performance
            {
                Id = 6,
                Start = new DateTime(2021, 2, 13, 21, 0, 0),
                End = new DateTime(2021, 2, 13, 23, 0, 0),
                StageId = 2,
                MusicianId = 11
            },
            new Performance
            {
                Id = 7,
                Start = new DateTime(2021, 3, 5, 14, 0, 0),
                End = new DateTime(2021, 3, 5, 16, 30, 0),
                StageId = 3,
                BandId = 4
            },
            new Performance
            {
                Id = 8,
                Start = new DateTime(2021, 3, 5, 19, 0, 0),
                End = new DateTime(2021, 3, 5, 22, 0, 0),
                StageId = 3,
                BandId = 5
            },
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Performance>().HasData(Data);
    }
}
