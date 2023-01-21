namespace BooksService.Domain.Entities
{
    public class AuthorRating : RatingBase
    {
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
