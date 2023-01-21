using BooksService.Domain.Entities;

namespace BooksService.Domain
{
    public class BookLight : EntityBase
    {
        public string Name { get; set; }
        public byte[] Photo { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Isbn { get; set; }
        public int Pages { get; set; }

        public string Genres { get; set; }
        public double Rating { get; set; }
    }
}
