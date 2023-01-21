using System;

namespace BooksService.Domain.Entities
{
    public class Feedback : EntityBase
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
