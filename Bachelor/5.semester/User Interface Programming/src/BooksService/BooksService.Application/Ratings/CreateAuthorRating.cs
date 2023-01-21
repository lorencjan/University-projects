using System;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;

namespace BooksService.Application.Ratings
{
    public class CreateAuthorRating : IRequest<int>
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public int UserId { get; set; }
    }

    public class CreateAuthorRatingHandler : IRequestHandler<CreateAuthorRating, int>
    {
        private readonly BooksDbContext _booksDbContext;

        public CreateAuthorRatingHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<int> Handle(CreateAuthorRating request, CancellationToken cancellationToken)
        {
            var created = await _booksDbContext.AuthorRatings.AddAsync(new AuthorRating
            {
                Number = request.Number,
                Text = request.Text,
                Date = DateTime.Now,
                AuthorId = request.AuthorId,
                UserId = request.UserId
            }, cancellationToken);
            await _booksDbContext.SaveChangesAsync(cancellationToken);
            return created.Entity.Id;
        }
    }
}