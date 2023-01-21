using System;
using System.Collections.Generic;

namespace BooksService.Domain.Entities
{
    public class Author : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] Photo { get; set; }
        public string Country { get; set; }
        public DateTime BirthDate { get; set; }
        public string Biography { get; set; }

        public List<AuthorBook> Books { get; set; }
        public List<AuthorRating> Ratings { get; set; }
        public List<UserFavoriteAuthor> FavoredBy { get; set; }
    }
}
