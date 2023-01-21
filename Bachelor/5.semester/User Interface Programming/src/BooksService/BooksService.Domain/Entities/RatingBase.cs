using System;

namespace BooksService.Domain.Entities
{
    public abstract class RatingBase : EntityBase
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
