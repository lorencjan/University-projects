using System;

namespace BooksService.Domain.Entities
{
    public class NewsPost : EntityBase
    {
        public string Header { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
