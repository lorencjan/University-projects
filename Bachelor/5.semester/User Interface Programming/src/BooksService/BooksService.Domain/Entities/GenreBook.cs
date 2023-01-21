namespace BooksService.Domain.Entities
{
    public class GenreBook
    {
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
