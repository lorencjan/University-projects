using System.Collections.Generic;

namespace BooksService.Domain.Entities
{
    public class Book : EntityBase
    {
        public string Name { get; set; }
        public byte[] Photo { get; set; }
        public int Year { get; set; }
        public string Isbn { get; set; }
        public int Pages { get; set; }
        public string Description { get; set; }

        public List<GenreBook> Genres { get; set; }
        public List<AuthorBook> Authors { get; set; }
        public List<BookRating> Ratings { get; set; }
        public List<UserFavoriteBook> FavoredBy { get; set; }
    }
}
