using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;

namespace RockFests.DAL.Seeds
{
    public class FestivalsSeed
    {
        private static readonly List<Festival> Data = new List<Festival>
        {
            new Festival
            {
                Id = 1,
                Name = "Masters of Rock",
                Location = "Berlin",
                StartDate = new DateTime(2021, 2, 12, 16, 0, 0),
                EndDate = new DateTime(2021, 2, 14, 0, 0, 0),
                Description = "On this weekend you'll have the unique opportunity to experience Metallica and Iron Maiden on the same day. On top of it, GNR announced their participation despite of their tight schedule. In addition, Bruce Dickinson and Slash both agreed to have solo performances as well. Moreover, non other than Marilyn Manson is going to finish this festival.",
                MaxTickets = 4000,
                MaxTicketsForUser = 3,
                ReservationDays = 7,
                Price = 250, 
                Currency = "EUR"
            },
            new Festival
            {
                Id = 2,
                Name = "CzechMania",
                Location = "Prague",
                StartDate = new DateTime(2021, 3, 5, 14, 0, 0),
                EndDate = new DateTime(2021, 3, 5, 23, 0, 0),
                Description = "This festival brings you one of the best afternoons one can get. Green Day and Pearl Jam on the same stage, what more is there?",
                MaxTickets = 2500,
                MaxTicketsForUser = 10,
                ReservationDays = 10,
                Price = 2700,
                Currency = "CZK"
            },
            new Festival
            {
                Id = 3,
                Name = "Rockity Rock",
                Location = "London",
                StartDate = new DateTime(2020, 4, 21, 15, 0, 0),
                EndDate = new DateTime(2020, 4, 21, 0, 0, 0),
                Description = "JUST COME!",
                MaxTickets = 4000,
                MaxTicketsForUser = 4000,
                ReservationDays = 5,
                Price = 400,
                Currency = "GBP"
            },
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Festival>().HasData(Data);
    }
}
