using System;
using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class FeedbacksSeed
    {
        public static List<Feedback> Data => new List<Feedback>
        {
            new Feedback
            {
                Id = 1,
                Text = "Dobrý den, kamarád mi doporučil sérii Kolo času, mohli byste ji sem prosím přidat? Docela by mě zajímala a věřím, že i jiné čtenáře. Děkuji.",
                Date = new DateTime(2020, 8, 2),
                UserId = 1,
            },
            new Feedback
            {
                Id = 2,
                Text = "Zdravím, mohli byste dát vědˇět čtenářům, že Megaknihy.cz mají teď Black Friday akci. Ať jim to neunikne!",
                Date = new DateTime(2020, 9, 7),
                UserId = 1,
            },
            new Feedback
            {
                Id = 3,
                Text = "spamujeme, muhaha",
                Date = new DateTime(2020, 10, 11),
                UserId = 5,
            },
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Feedback>().HasData(Data);
    }
}
