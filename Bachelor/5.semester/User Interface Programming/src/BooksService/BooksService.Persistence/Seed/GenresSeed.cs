using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class GenresSeed
    {
        public static List<Genre> Data => new List<Genre>
        {
            new Genre {Id = 1, Name = "Akční"},
            new Genre {Id = 2, Name = "Dobrodružné"},
            new Genre {Id = 3, Name = "Architektura"},
            new Genre {Id = 4, Name = "Umění"},
            new Genre {Id = 5, Name = "Autobiografie"},
            new Genre {Id = 6, Name = "Biografie"},
            new Genre {Id = 7, Name = "Obchod"},
            new Genre {Id = 8, Name = "Dětské"},
            new Genre {Id = 9, Name = "Clasické"},
            new Genre {Id = 10, Name = "Komedie"},
            new Genre {Id = 11, Name = "Komiks"},
            new Genre {Id = 12, Name = "Kuchařky"},
            new Genre {Id = 13, Name = "Řemeslné"},
            new Genre {Id = 14, Name = "Detektivky"},
            new Genre {Id = 15, Name = "Deníky"},
            new Genre {Id = 16, Name = "Slovníky"},
            new Genre {Id = 17, Name = "Drama"},
            new Genre {Id = 18, Name = "Ekonomie"},
            new Genre {Id = 19, Name = "Encyklopedie"},
            new Genre {Id = 20, Name = "Pohádky"},
            new Genre {Id = 21, Name = "Fantasy"},
            new Genre {Id = 22, Name = "Jídlo"},
            new Genre {Id = 23, Name = "Zahrada"},
            new Genre {Id = 24, Name = "Průvodce"},
            new Genre {Id = 25, Name = "Zdraví"},
            new Genre {Id = 26, Name = "Historické"},
            new Genre {Id = 27, Name = "Historická fikce"},
            new Genre {Id = 28, Name = "Koníčky"},
            new Genre {Id = 29, Name = "Domácnost"},
            new Genre {Id = 30, Name = "Horor"},
            new Genre {Id = 31, Name = "Deník"},
            new Genre {Id = 32, Name = "Manga"},
            new Genre {Id = 33, Name = "Vzpomínky"},
            new Genre {Id = 34, Name = "Záhady"},
            new Genre {Id = 35, Name = "Román"},
            new Genre {Id = 36, Name = "Filozofie"},
            new Genre {Id = 37, Name = "Psychologie"},
            new Genre {Id = 38, Name = "Obrázkové knihy"},
            new Genre {Id = 39, Name = "Poezie"},
            new Genre {Id = 40, Name = "Politika"},
            new Genre {Id = 41, Name = "Náboženství"},
            new Genre {Id = 42, Name = "Recenze"},
            new Genre {Id = 43, Name = "Romantika"},
            new Genre {Id = 44, Name = "Satira"},
            new Genre {Id = 45, Name = "Věda"},
            new Genre {Id = 46, Name = "Sci-fi"},
            new Genre {Id = 47, Name = "Osobní rozvoj"},
            new Genre {Id = 48, Name = "Sport"},
            new Genre {Id = 49, Name = "Učebnice"},
            new Genre {Id = 50, Name = "Thriller"},
            new Genre {Id = 51, Name = "Cestování"},
            new Genre {Id = 52, Name = "Western"},
            new Genre {Id = 53, Name = "Dystopie"},
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<Genre>().HasData(Data);
    }
}
