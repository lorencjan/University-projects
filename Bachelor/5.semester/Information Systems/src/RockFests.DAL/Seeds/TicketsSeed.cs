using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;

namespace RockFests.DAL.Seeds
{
    public class TicketsSeed
    {
        private static readonly List<Ticket> Data = new List<Ticket>
        {
            new Ticket
            {
                Id = 1,
                IsPaid = true,
                PaymentDue = DateTime.Now.AddDays(7),
                VariableSymbol = 1,
                Count = 1,
                FestivalId = 1,
                UserId = 5
            },
            new Ticket
            {
                Id = 2,
                IsPaid = false,
                PaymentDue = DateTime.Now.AddDays(10),
                VariableSymbol = 2,
                Count = 4,
                FestivalId = 2,
                UserId = 5
            },
            new Ticket
            {
                Id = 3,
                IsPaid = false,
                PaymentDue = DateTime.Now.AddDays(7),
                VariableSymbol = 3,
                Count = 2,
                FestivalId = 1,
                UserId = 5
            },
            new Ticket
            {
                Id = 4,
                IsPaid = true,
                PaymentDue = DateTime.Now.AddDays(10),
                VariableSymbol = 4,
                Count = 1,
                FestivalId = 2,
                UserId = 5
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Ticket>().HasData(Data);
    }
}
