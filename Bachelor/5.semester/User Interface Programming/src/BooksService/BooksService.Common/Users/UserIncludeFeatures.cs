using System;

namespace BooksService.Common
{
    [Flags]
    public enum UserIncludeFeatures
    {
        Feedbacks = 1,
        BookRatings = 2,
        AuthorRatings = 4
    }
}
