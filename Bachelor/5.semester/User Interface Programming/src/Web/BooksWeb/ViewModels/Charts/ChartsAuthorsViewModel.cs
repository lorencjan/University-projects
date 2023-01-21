using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWeb.DAL.Services;
using BooksWeb.Model;
using BooksWeb.Resources;
using Microsoft.Extensions.Logging;

namespace BooksWeb.ViewModels
{
    public class ChartsAuthorsViewModel : MasterPageViewModel
    {
        private readonly AuthorsService _authorsService;
        private readonly ILogger<AuthorsService> _logger;

        public FilterDataSet<AuthorLight> Authors { get; set; } = new FilterDataSet<AuthorLight>();

        public ChartsAuthorsViewModel(AuthorsService authorsService, ILogger<AuthorsService> logger)
        {
            _authorsService = authorsService;
            _logger = logger;
        }

        public override async Task Load()
        {
            Authors.Set(Query);
            await base.Load();
            HighlightedNavItem = Routes.Charts_authors;
        }

        private async Task<IQueryable<AuthorLight>> Query()
        {
            try
            {
                var authors = await _authorsService.GetAuthorsLight(null, SortAuthorsBy.Rating, SortOrdering.Descending);
                return authors.AsQueryable();
            }
            catch (Exception e)
            {
                SetError(e, Errors.AuthorsLoad);
                _logger.LogError(e, Errors.AuthorsLoad);
                return new List<AuthorLight>().AsQueryable();
            }
        }
    }
}