using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class UserFavoriteBooksSeed
    {
        public static List<UserFavoriteBook> Data => new List<UserFavoriteBook>
        {
            new UserFavoriteBook{UserId = 1, BookId = 2},
            new UserFavoriteBook{UserId = 1, BookId = 12},
            new UserFavoriteBook{UserId = 1, BookId = 20},
            new UserFavoriteBook{UserId = 2, BookId = 4},
            new UserFavoriteBook{UserId = 3, BookId = 6},
            new UserFavoriteBook{UserId = 3, BookId = 7},
            new UserFavoriteBook{UserId = 4, BookId = 18},
            new UserFavoriteBook{UserId = 5, BookId = 19}
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<UserFavoriteBook>().HasData(Data);
    }
}
