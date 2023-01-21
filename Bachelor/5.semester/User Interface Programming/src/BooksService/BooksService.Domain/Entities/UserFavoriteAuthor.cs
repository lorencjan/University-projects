namespace BooksService.Domain.Entities
{
    public class UserFavoriteAuthor
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}