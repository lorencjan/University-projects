using System;

namespace BooksService.Common
{
    [Flags]
    public enum BookIncludeFeatures
    {
        Genres = 1,
        Authors = 2,
        Ratings = 4,
        All = Genres | Authors | Ratings
    }
}
