using System.Collections.Generic;
using System.Net.Security;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class UsersSeed
    {
        public static List<User> Data => new List<User>
        {
            new User
            {
                Id = 1,
                Login = "user",
                Password = Encryption.HashPassword("user")
            },
            new User
            {
                Id = 2,
                Login = "Volánek",
                Password = Encryption.HashPassword("volanekpremierem")
            },
            new User
            {
                Id = 3,
                Login = "Jarda",
                Password = Encryption.HashPassword("jaryn123")
            },
            new User
            {
                Id = 4,
                Login = "Knihomol",
                Password = Encryption.HashPassword("kniznikritik2020")
            },
            new User
            {
                Id = 5,
                Login = "FantasyNerd",
                Password = Encryption.HashPassword("TolkienWannabe")
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<User>().HasData(Data);
    }
}
