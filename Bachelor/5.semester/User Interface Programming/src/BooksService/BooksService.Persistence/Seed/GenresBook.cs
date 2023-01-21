using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class GenreBooksSeed
    {
        public static List<GenreBook> Data => new List<GenreBook>
        {
            new GenreBook{BookId = 1, GenreId = 36},
            new GenreBook{BookId = 1, GenreId = 37},
            new GenreBook{BookId = 2, GenreId = 12},
            new GenreBook{BookId = 3, GenreId = 35},
            new GenreBook{BookId = 3, GenreId = 21},
            new GenreBook{BookId = 4, GenreId = 35},
            new GenreBook{BookId = 4, GenreId = 21},
            new GenreBook{BookId = 5, GenreId = 35},
            new GenreBook{BookId = 5, GenreId = 21},
            new GenreBook{BookId = 6, GenreId = 47},
            new GenreBook{BookId = 7, GenreId = 35},
            new GenreBook{BookId = 7, GenreId = 21},
            new GenreBook{BookId = 7, GenreId = 46},
            new GenreBook{BookId = 8, GenreId = 47},
            new GenreBook{BookId = 9, GenreId = 30},
            new GenreBook{BookId = 10, GenreId = 35},
            new GenreBook{BookId = 11, GenreId = 35},
            new GenreBook{BookId = 12, GenreId = 21},
            new GenreBook{BookId = 13, GenreId = 36},
            new GenreBook{BookId = 14, GenreId = 35},
            new GenreBook{BookId = 15, GenreId = 21},
            new GenreBook{BookId = 16, GenreId = 21},
            new GenreBook{BookId = 17, GenreId = 21},
            new GenreBook{BookId = 18, GenreId = 6},
            new GenreBook{BookId = 19, GenreId = 53},
            new GenreBook{BookId = 20, GenreId = 21},
            new GenreBook{BookId = 21, GenreId = 21},
            new GenreBook{BookId = 22, GenreId = 21}
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<GenreBook>().HasData(Data);
    }
}
