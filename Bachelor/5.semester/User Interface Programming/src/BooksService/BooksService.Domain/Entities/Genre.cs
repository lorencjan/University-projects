using System.Collections.Generic;

namespace BooksService.Domain.Entities
{
    public class Genre : EntityBase
    {
        public string Name { get; set; }

        public List<GenreBook> Books { get; set; }
    }
}
