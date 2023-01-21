using System.Collections.Generic;
using BooksService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Persistence.Seed
{
    public class UserFavoriteAuthorsSeed
    {
        public static List<UserFavoriteAuthor> Data => new List<UserFavoriteAuthor>
        {
            new UserFavoriteAuthor{UserId = 1, AuthorId = 2},
            new UserFavoriteAuthor{UserId = 1, AuthorId = 5},
            new UserFavoriteAuthor{UserId = 2, AuthorId = 2},
            new UserFavoriteAuthor{UserId = 2, AuthorId = 4},
            new UserFavoriteAuthor{UserId = 3, AuthorId = 6},
            new UserFavoriteAuthor{UserId = 4, AuthorId = 7},
            new UserFavoriteAuthor{UserId = 4, AuthorId = 11},
            new UserFavoriteAuthor{UserId = 4, AuthorId = 12}
        };

        public static void Seed(ModelBuilder modelBuilder)
            => modelBuilder.Entity<UserFavoriteAuthor>().HasData(Data);
    }
}
