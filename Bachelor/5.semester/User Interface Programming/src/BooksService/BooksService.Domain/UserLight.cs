using BooksService.Domain.Entities;

namespace BooksService.Domain
{
    public class UserLight : EntityBase
    {
        public string Login { get; set; }

        public int BookRatingsCount { get; set; }
        public int AuthorRatingsCount { get; set; }
        public int FeedbacksCount { get; set; }
    }
}
