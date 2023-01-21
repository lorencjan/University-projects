using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;

namespace RockFests.DAL.Seeds
{
    public class StagesSeed
    {
        private static readonly List<Stage> Data = new List<Stage>
        {
            new Stage
            {
                Id = 1,
                Name = "Main Stage",
                FestivalId = 1
            },
            new Stage
            {
                Id = 2,
                Name = "Solos Stage",
                FestivalId = 1
            },
            new Stage
            {
                Id = 3,
                Name = "The Stage",
                FestivalId = 2
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Stage>().HasData(Data);
    }
}
