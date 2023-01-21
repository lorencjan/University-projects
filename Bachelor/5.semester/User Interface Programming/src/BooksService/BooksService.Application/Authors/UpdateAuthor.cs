using System.Threading;
using System.Threading.Tasks;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Authors
{
    public class UpdateAuthor : IRequest
    {
        public Author Author { get; set; }
    }

    public class UpdateAuthorHandler : AsyncRequestHandler<UpdateAuthor>
    {
        private readonly BooksDbContext _booksDbContext;

        public UpdateAuthorHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        protected override async Task Handle(UpdateAuthor request, CancellationToken cancellationToken)
        {
            var author = request.Author;

            var toUpdate = await _booksDbContext.Authors.SingleAsync(x => x.Id == author.Id, cancellationToken);
            
            toUpdate.FirstName = author.FirstName;
            toUpdate.LastName = author.LastName;
            toUpdate.Photo = author.Photo;
            toUpdate.Country = author.Country;
            toUpdate.BirthDate = author.BirthDate;
            toUpdate.Biography = author.Biography;

            await _booksDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
