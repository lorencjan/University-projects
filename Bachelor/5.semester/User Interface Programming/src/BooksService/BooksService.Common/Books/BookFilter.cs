namespace BooksService.Common
{
    public class BookFilter
    {
        public string Name { get; set; }
        public int? Year { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
    }
}
