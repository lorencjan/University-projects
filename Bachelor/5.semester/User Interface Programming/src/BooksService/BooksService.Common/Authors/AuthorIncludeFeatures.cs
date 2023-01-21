using System;

namespace BooksService.Common
{
    [Flags]
    public enum AuthorIncludeFeatures
    {
        JoinBooks = 1,
        Books = 2,
        Ratings = 4
    }
}
