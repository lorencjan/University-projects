using System;
using BooksService.Domain.Entities;

namespace BooksService.Domain
{
    public class AuthorLight : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] Photo { get; set; }
        public string Country { get; set; }
        public DateTime BirthDate { get; set; }

        public int BookCount { get; set; }
        public double Rating { get; set; }
    }
}
