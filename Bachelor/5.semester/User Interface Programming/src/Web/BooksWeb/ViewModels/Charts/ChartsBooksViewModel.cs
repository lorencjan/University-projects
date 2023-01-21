using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWeb.Model;
using BooksWeb.Resources;
using Microsoft.Extensions.Logging;

namespace BooksWeb.ViewModels
{
    public class ChartsBooksViewModel : MasterPageViewModel
    {
        private readonly DAL.Services.BooksService _booksService;
        private readonly ILogger<DAL.Services.BooksService> _logger;

        public FilterDataSet<BookLight> Books { get; set; } = new FilterDataSet<BookLight>();

        public ChartsBooksViewModel(DAL.Services.BooksService booksService, ILogger<DAL.Services.BooksService> logger)
        {
            _booksService = booksService;
            _logger = logger;
        }

        public override async Task Load()
        {
            Books.Set(Query);
            await base.Load();
            HighlightedNavItem = Routes.Charts_books;
        }

        private async Task<IQueryable<BookLight>> Query()
        {
            try
            {
                var books = await _booksService.GetBooksLight(null, SortBooksBy.Rating, SortOrdering.Descending);
                return books.AsQueryable();
            }
            catch (Exception e)
            {
                SetError(e, Errors.BooksLoad);
                _logger.LogError(e, Errors.BooksLoad);
                return new List<BookLight>().AsQueryable();
            }
        }
    }
}