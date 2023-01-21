using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class AuthorBooksSeed
    {
        public static List<AuthorBook> Data => new List<AuthorBook>
        {
            new AuthorBook{BookId = 1, AuthorId = 8},
            new AuthorBook{BookId = 2, AuthorId = 9},
            new AuthorBook{BookId = 3, AuthorId = 10},
            new AuthorBook{BookId = 4, AuthorId = 10},
            new AuthorBook{BookId = 5, AuthorId = 10},
            new AuthorBook{BookId = 6, AuthorId = 11},
            new AuthorBook{BookId = 7, AuthorId = 12},
            new AuthorBook{BookId = 7, AuthorId = 13},
            new AuthorBook{BookId = 8, AuthorId = 11},
            new AuthorBook{BookId = 9, AuthorId = 12},
            new AuthorBook{BookId = 10, AuthorId = 12},
            new AuthorBook{BookId = 11, AuthorId = 1},
            new AuthorBook{BookId = 12, AuthorId = 4},
            new AuthorBook{BookId = 13, AuthorId = 1},
            new AuthorBook{BookId = 14, AuthorId = 1},
            new AuthorBook{BookId = 15, AuthorId = 2},
            new AuthorBook{BookId = 16, AuthorId = 2},
            new AuthorBook{BookId = 17, AuthorId = 2},
            new AuthorBook{BookId = 18, AuthorId = 6},
            new AuthorBook{BookId = 18, AuthorId = 7},
            new AuthorBook{BookId = 19, AuthorId = 3},
            new AuthorBook{BookId = 20, AuthorId = 4},
            new AuthorBook{BookId = 20, AuthorId = 5},
            new AuthorBook{BookId = 21, AuthorId = 4},
            new AuthorBook{BookId = 22, AuthorId = 4},
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AuthorBook>().HasData(Data);
    }
}
