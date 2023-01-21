using System.Collections.Generic;

namespace BooksService.Domain.Entities
{
    public class User : EntityBase
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public List<Feedback> Feedbacks { get; set; }
        public List<UserFavoriteBook> FavoriteBooks { get; set; }
        public List<UserFavoriteAuthor> FavoriteAuthors { get; set; }
        public List<BookRating> BookRatings { get; set; }
        public List<AuthorRating> AuthorRatings { get; set; }
    }
}
