using System;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.DAL.Entities;

namespace MovieDatabase.DAL.Seeds
{
    public class PeopleSeed
    {
        private static readonly Person[] _data = new Person[]
        {
            new Person
            {
                Id = new Guid("9f64adab-623b-4125-8e03-f64a4bc7edb0"),
                FirstName = "James",
                LastName = "Cameron",
                Age = 65,
                Photo = SeedImages.JamesCameron,
                Country = "Canada"
            },
            new Person
            {
                Id = new Guid("6a6399f6-3a1c-4d60-b2d1-9f5b1b2494b5"),
                FirstName = "Leonardo",
                LastName = "DiCaprio",
                Age = 45,
                Photo = SeedImages.LeonardoDiCaprio,
                Country = "USA"
            },
            new Person
            {
                Id = new Guid("f9008976-566d-40ab-b2bc-137bab9fb64d"),
                FirstName = "Kate",
                LastName = "Winslet",
                Age = 44,
                Photo = SeedImages.KateWinslet,
                Country = "Great Britain"
            },
            new Person
            {
                Id = new Guid("4a95bfcc-0882-4a68-a008-114bbbaf2ba1"),
                FirstName = "Wolfgang",
                LastName = "Peterson",
                Age = 78,
                Photo = SeedImages.WolfgangPeterson,
                Country = "Germany"
            },
            new Person
            {
                Id = new Guid("74fa3424-b2e8-470d-92eb-011b75290055"),
                FirstName = "Brad",
                LastName = "Pitt",
                Age = 56,
                Photo = SeedImages.BradPitt,
                Country = "USA"
            },
            new Person
            {
                Id = new Guid("dccb08fd-1460-4f20-a26a-7eb8dc6ba5e5"),
                FirstName = "Eric",
                LastName = "Bana",
                Age = 51,
                Photo = SeedImages.EricBana,
                Country = "Australia"
            },
            new Person
            {
                Id = new Guid("0b6ebbbd-30f3-4041-8d8f-9fa7441bb03d"),
                FirstName = "Orlando",
                LastName = "Bloom",
                Age = 43,
                Photo = SeedImages.OrlandoBloom,
                Country = "Great Britain"
            },
            new Person
            {
                Id = new Guid("0fb32496-f0bb-46f6-a577-476d033225ff"),
                FirstName = "Diene",
                LastName = "Kruger",
                Age = 43,
                Photo = SeedImages.DieneKruger,
                Country = "Germany"
            },
            new Person
            {
                Id = new Guid("e47f9e0d-35b3-47a6-9417-1180546da691"),
                FirstName = "Quentin",
                LastName = "Tarantino",
                Age = 56,
                Photo = SeedImages.QuentinTarantino,
                Country = "USA"
            },
            new Person
            {
                Id = new Guid("805c86f2-ab1f-4848-87de-fc5745be91ee"),
                FirstName = "Margot",
                LastName = "Robbie",
                Age = 29,
                Photo = SeedImages.MargotRobbie,
                Country = "Australia"
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasData(_data);
        }
    }
}
