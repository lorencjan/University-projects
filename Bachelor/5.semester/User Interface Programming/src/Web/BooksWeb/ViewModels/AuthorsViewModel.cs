using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWeb.DAL.Services;
using BooksWeb.Resources;

namespace BooksWeb.ViewModels
{
    public class AuthorsViewModel : MasterPageViewModel
    {
        private readonly AuthorsService _authorsService;

        public List<AuthorLight> Authors { get; set; }
        public AuthorFilter Filter { get; set; } = new AuthorFilter();
        public string FilterFirstName { get; set; }
        public string FilterLastName { get; set; }
        public int? FilterYearFrom { get; set; }
        public int? FilterYearTo { get; set; }
        public float? FilterRatingFrom { get; set; }
        public float? FilterRatingTo { get; set; }
        public bool FilterVisible { get; set; }
        public List<string> Nationalities { get; set; }
        public SortAuthorsBy SortBy { get; set; } = SortAuthorsBy.LastName;
        public SortOrdering SortOrder { get; set; } = SortOrdering.Ascending;

        public AuthorsViewModel(AuthorsService authorsService) => _authorsService = authorsService;

        public override async Task Load()
        {
            Authors = await _authorsService.GetAuthorsLight(null, SortBy, SortOrder);
            GetNationalities();
            await base.Load();
            HighlightedNavItem = Routes.Authors;
        }

        public void GetNationalities()
        {
            Nationalities = new List<string>();
            foreach (var author in Authors)
            {
                if(!Nationalities.Contains(author.Country))
                {
                    Nationalities.Add(author.Country);
                }
            }
        }

        public async Task ApplyFilter()
        {
            Filter.Name = FilterFirstName + " " + FilterLastName;
            var yearFrom = FilterYearFrom != null ? new DateTimeOffset(new DateTime((int)FilterYearFrom, 1, 1)) : DateTimeOffset.MinValue;
            var yearTo = FilterYearTo != null ? new DateTimeOffset(new DateTime((int)FilterYearTo, 1, 1)) : DateTimeOffset.MaxValue;
            Authors = await _authorsService.GetAuthorsLight(Filter, SortBy, SortOrder);
            Authors.RemoveAll(x => x.BirthDate < yearFrom || x.BirthDate > yearTo || x.Rating < FilterRatingFrom || x.Rating > FilterRatingTo);
        }

        public async Task ResetFilter()
        {
            Authors = await _authorsService.GetAuthorsLight(null, SortBy, SortOrder);
            Filter = new AuthorFilter();
            FilterFirstName = null;
            FilterLastName = null;
            FilterYearFrom = 0;
            FilterYearTo = DateTime.Today.Year;
            FilterRatingFrom = 0;
            FilterRatingTo = 10;
        }

        public async Task Sort(SortAuthorsBy sortBy, SortOrdering order)
        {
            SortBy = sortBy;
            SortOrder = order;
            await ApplyFilter();
        }
    }
}