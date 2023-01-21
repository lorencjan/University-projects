using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksService.Common;
using BooksService.Domain.Entities;
using BooksService.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BooksService.Application.Feedbacks
{
    public class GetFeedbacks : IRequest<List<Feedback>>
    {
        public string UserFilter { get; set; }
        public SortFeedbacksBy? SortFeedbacksBy { get; set; }
        public SortOrdering? SortOrdering { get; set; }
    }

    public class GetFeedbacksHandler : IRequestHandler<GetFeedbacks, List<Feedback>>
    {
        private readonly BooksDbContext _booksDbContext;

        public GetFeedbacksHandler(BooksDbContext booksDbContext) => _booksDbContext = booksDbContext;

        public async Task<List<Feedback>> Handle(GetFeedbacks request, CancellationToken cancellationToken)
        {
            var query = _booksDbContext.Feedbacks.Include(x => x.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.UserFilter))
                query = query.Where(x => EF.Functions.Like(x.User.Login, $"%{request.UserFilter}%"));

            var order = request.SortOrdering ?? SortOrdering.Ascending;
            var sort = request.SortFeedbacksBy ?? SortFeedbacksBy.Date;

            query = sort switch
            {
                SortFeedbacksBy.Date => order == SortOrdering.Ascending ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date),
                SortFeedbacksBy.UserName => order == SortOrdering.Ascending ? query.OrderBy(x => x.User.Login) : query.OrderByDescending(x => x.User.Login),
                _ => throw new ArgumentOutOfRangeException()
            };

            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
