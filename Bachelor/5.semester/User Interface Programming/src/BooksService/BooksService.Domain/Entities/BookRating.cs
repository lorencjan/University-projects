namespace BooksService.Domain.Entities
{
    public class BookRating : RatingBase
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
