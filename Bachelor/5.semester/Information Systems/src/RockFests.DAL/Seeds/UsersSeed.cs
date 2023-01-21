using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RockFests.DAL.Entities;
using RockFests.DAL.Enums;
using RockFests.DAL.StaticServices;

namespace RockFests.DAL.Seeds
{
    public class UsersSeed
    {
        private static readonly List<User> Data = new List<User>
        {
            new User
            {
                Id = 1,
                Login = "admin",
                FirstName = "admin",
                LastName = "admin",
                Email = "admin@admin.com",
                Password = Encryption.HashPassword("admin"),
                AccessRole = AccessRole.Admin
            },
            new User
            {
                Id = 2,
                Login = "organizer",
                FirstName = "organizer",
                LastName = "organizer",
                Email = "organizer@organizer.com",
                Password = Encryption.HashPassword("organizer"),
                AccessRole = AccessRole.Organizer
            },
            new User
            {
                Id = 3,
                Login = "cashier",
                FirstName = "cashier",
                LastName = "cashier",
                Email = "cashier@cashier.com",
                Password = Encryption.HashPassword("cashier"),
                AccessRole = AccessRole.Cashier
            },
            new User
            {
                Id = 4,
                Login = "spectator",
                FirstName = "spectator",
                LastName = "spectator",
                Email = "spectator@spectator.com",
                Password = Encryption.HashPassword("spectator"),
                AccessRole = AccessRole.Spectator
            },
            new User
            {
                Id = 5,
                Login = "CarolusNovus",
                Password = Encryption.HashPassword("karelnovy"),
                AccessRole = AccessRole.Spectator,
                FirstName = "Karel",
                LastName = "Nový",
                Email = "karel.novy@email.cz",
                Phone = "724 874 159"
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<User>().HasData(Data);
    }
}
