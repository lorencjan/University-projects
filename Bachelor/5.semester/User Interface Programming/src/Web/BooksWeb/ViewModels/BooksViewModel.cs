using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWeb.Resources;

namespace BooksWeb.ViewModels
{
    public class BooksViewModel : MasterPageViewModel
    {
        private readonly DAL.Services.BooksService _booksService;

        public List<BookLight> Books { get; set; }
        public BookFilter Filter { get; set; } = new BookFilter();
        public int? FilterPagesFrom { get; set; }
        public int? FilterPagesTo { get; set; }
        public float? FilterRatingFrom { get; set; }
        public float? FilterRatingTo { get; set; }
        public bool FilterVisible { get; set; }
        public List<string> Genres { get; set; }
        public SortBooksBy SortBy { get; set; } = SortBooksBy.Name;
        public SortOrdering SortOrder { get; set; } = SortOrdering.Ascending;

        public BooksViewModel(DAL.Services.BooksService booksService)
            => _booksService = booksService;

        public override async Task Load()
        {
            Books = await _booksService.GetBooksLight();
            Genres = (await _booksService.GetGenres()).ConvertAll(x => x.Name);
            await base.Load();
            HighlightedNavItem = Routes.Books;
        }

        public async Task ApplyFilter()
        {
            Books = await _booksService.GetBooksLight(Filter, SortBy, SortOrder);
            Books.RemoveAll(x => (x.Pages < FilterPagesFrom || x.Pages > FilterPagesTo || x.Rating < FilterRatingFrom || x.Rating > FilterRatingTo));
        }

        public async Task ResetFilter()
        {
            Books = await _booksService.GetBooksLight(null, SortBy, SortOrder);
            Filter = new BookFilter();
            FilterPagesFrom = null;
            FilterPagesTo = null;
            FilterRatingFrom = null;
            FilterRatingTo = null;
        }

        public async Task Sort(SortBooksBy sortBy, SortOrdering order)
        {
            SortBy = sortBy;
            SortOrder = order;
            await ApplyFilter();
        }
    }
}