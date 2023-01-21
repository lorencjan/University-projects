using System;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Ratings
{
    public class CreateBookRating : IRequest<int>
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
    }

    public class CreateBookRatingHandler : IRequestHandler<CreateBookRating, int>
    {
        private readonly BooksDbContext _booksDbContext;

        public CreateBookRatingHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<int> Handle(CreateBookRating request, CancellationToken cancellationToken)
        {
            var created = await _booksDbContext.BookRatings.AddAsync(new BookRating
            {
                Number = request.Number,
                Text = request.Text,
                Date = DateTime.Now,
                BookId = request.BookId,
                UserId = request.UserId
            }, cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
            return created.Entity.Id;
        }
    }
}