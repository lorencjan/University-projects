using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using BooksWpf.DAL.Services;
using BooksWpf.Commands;
using BooksService.Client;
using BooksService.Common;
using BooksWpf.Model;
using BooksWpf.Resources.Img;


namespace BooksWpf.ViewModels
{
    public class BooksViewModel : ViewModelBase
    {
        #region Servicies
        private readonly BookService _booksService;
        private readonly AuthorService _authorService;
        #endregion

        #region Key properties
        private ObservableCollection<BookLight> _books;
        public ObservableCollection<BookLight> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }
        private Book _detailBook;
        public Book DetailBook
        {
            get { return _detailBook; }
            set { SetProperty(ref _detailBook, value); }
        }
        private ObservableCollection<BookRating> _ratings;
        public ObservableCollection<BookRating> Ratings
        {
            get { return _ratings; }
            set { SetProperty(ref _ratings, value); }
        }
        #endregion

        #region list commands
        public IAsyncCommand LoadBooksCommand { get; set; }
        public ICommand ToggleFilterCommand { get; set; }
        public ICommand ResetFilterCommand { get; set; }
        public IAsyncCommand FilterCommand { get; set; }
        public ICommand SelectBookCommand { get; set; }
        public ICommand CreateBookCommand { get; set; }
        public ICommand SortByNameCommand { get; set; }
        #endregion

        #region Edit commands
        public ICommand RemoveSingleAuthor { get; set; }
        public ICommand RemoveSingleGenre { get; set; }
        public ICommand AddAuthorCommand { get; set; }
        public ICommand AddGenreCommand { get; set; }
        public ICommand LoadPhotoCommand { get; set; }
        public IAsyncCommand SaveBookCommand { get; set; }
        public ICommand ToggleEditBookCommand { get; set; }
        public ICommand DeleteRatingCommand { get; set; }
        public IAsyncCommand DeleteBookCommand { get; set; }

        #endregion

        #region relationships edit properities
        private ObservableCollection<AuthorLight> _availableAuthors, _addedAuthors;
        private ObservableCollection<Genre> _availableGenres, _addedGenres;
        private AuthorLight _chosenAuthor;
        private Genre _chosenGenre;
        public AuthorLight ChosenAuthor
        {
            get { return _chosenAuthor; }
            set { SetProperty(ref _chosenAuthor, value); }
        }
        public Genre ChosenGenre
        {
            get { return _chosenGenre; }
            set { SetProperty(ref _chosenGenre, value); }
        }
        public ObservableCollection<AuthorLight> AvailableAuthors
        {
            get { return _availableAuthors; }
            set { SetProperty(ref _availableAuthors, value); }
        }
        public ObservableCollection<AuthorLight> AddedAuthors
        {
            get { return _addedAuthors; }
            set { SetProperty(ref _addedAuthors, value); }
        }
        public ObservableCollection<Genre> AvailableGenres
        {
            get { return _availableGenres; }
            set { SetProperty(ref _availableGenres, value); }
        }
        public ObservableCollection<Genre> AddedGenres
        {
            get { return _addedGenres; }
            set { SetProperty(ref _addedGenres, value); }
        }
        #endregion

        #region Visibilities
        public Visibility _listVisible, _detailVisible, _readBookVisible, _editBookVisible, _filterVisible, _newVisible;
        public Visibility ListVisible
        {
            get { return _listVisible; }
            set { SetProperty(ref _listVisible, value); }
        }

        public Visibility DetailVisible
        {
            get { return _detailVisible; }
            set { SetProperty(ref _detailVisible, value); }
        }

        public Visibility ReadBookVisible
        {
            get { return _readBookVisible; }
            set { SetProperty(ref _readBookVisible, value); }
        }

        public Visibility EditBookVisible
        {
            get { return _editBookVisible; }
            set { SetProperty(ref _editBookVisible, value); }
        }
        public Visibility FilterVisible
        {
            get { return _filterVisible; }
            set { SetProperty(ref _filterVisible, value); }
        }
        public Visibility DeleteVisible
        {
            get { return _newVisible; }
            set { SetProperty(ref _newVisible, value); }
        }
        #endregion

        #region dynamic strings
        private string _editButtonText, _genreString;
        public string EditButtonText
        {
            get { return _editButtonText; }
            set { SetProperty(ref _editButtonText, value); }
        }
        public string GenreString
        {
            get { return _genreString; }
            set { SetProperty(ref _genreString, value); }
        }
        #endregion

        #region Paging
        private Pager _pager;
        public Pager Pager
        {
            get => _pager;
            set => SetProperty(ref _pager, value);
        }

        public byte[] PageLeftIcon { get; set; } = Images.ChevronLeft___green;
        public byte[] PageRightIcon { get; set; } = Images.ChevronRight___green;

        public AsyncCommand PageLeftCommand { get; set; }
        public AsyncCommand PageRightCommand { get; set; }
        #endregion

        #region Icons and photos
        private byte[] _nameSortIcon, _editIcon, _filterChevron, _photo;
        public byte[] BookIcon { get; } = Images.Book;
        public byte[] NewIcon { get; } = Images.New;
        public byte[] DeleteIcon { get; } = Images.TrashBin;
        public byte[] FilterIcon { get; } = Images.Filter;
        public byte[] GreenImageIcon { get; } = Images.ImageGreen;
        public byte[] GreyImageIcon { get; } = Images.ImageGrey;

        public byte[] AuthorIcon { get; } = Images.AuthorGrey;
        public byte[] SaveIcon { get; } = Images.Save;

        public byte[] Photo
        {
            get { return _photo; }
            set { SetProperty(ref _photo, value); }
        }

        public byte[] NameSortIcon
        {
            get => _nameSortIcon;
            set => SetProperty(ref _nameSortIcon, value);
        }

        public byte[] EditIcon
        {
            get { return _editIcon; }
            set { SetProperty(ref _editIcon, value); }
        }

        public byte[] FilterChevron
        {
            get { return _filterChevron; }
            set { SetProperty(ref _filterChevron, value); }
        }
        #endregion

        #region Sort and filter
        private string _nameFilter, _authorFilter, _genreFilter, _isbnFilter;
        private SortBooksBy _sortingBy;
        private SortOrdering _sortingOrder;
        private BookFilter _bookFilter;
        public BookFilter BookFilter
        {
            get { return _bookFilter; }
            set { SetProperty(ref _bookFilter, value); }
        }
        public string NameFilter
        {
            get { return _nameFilter; }
            set { SetProperty(ref _nameFilter, value); }
        }

        public string AuthorFilter
        {
            get { return _authorFilter; }
            set { SetProperty(ref _authorFilter, value); }
        }
        public string GenreFilter
        {
            get { return _genreFilter; }
            set { SetProperty(ref _genreFilter, value); }
        }
        public string ISBNFilter
        {
            get { return _isbnFilter; }
            set { SetProperty(ref _isbnFilter, value); }
        }
        #endregion
        public BooksViewModel(BookService booksService, AuthorService authorService)
        {
            #region Services
            _booksService = booksService;
            _authorService = authorService;
            #endregion

            #region Empty initializations
            Pager = new Pager();
            AddedAuthors = new ObservableCollection<AuthorLight>();
            AddedGenres = new ObservableCollection<Genre>();
            BookFilter = new BookFilter();
            #endregion

            #region View commands
            LoadBooksCommand = new AsyncCommand(async () => await LoadBooks());
            PageLeftCommand = new AsyncCommand(async () => await MovePage(true), () => Pager.Page != Pager.Options.First());
            PageRightCommand = new AsyncCommand(async () => await MovePage(false), () => Pager.Page != Pager.Options.Last());
            SelectBookCommand = new AsyncCommand<int>(async id => await SelectBook(id));
            CreateBookCommand = new AsyncCommand(async () => await SelectBook(0));
            ToggleFilterCommand = new RelayCommand(ToggleFilter);
            ResetFilterCommand = new AsyncCommand(ResetFilter);
            FilterCommand = new AsyncCommand(Filter);
            SortByNameCommand = new AsyncCommand(SortByName);
            #endregion

            #region Detail commands
            DeleteBookCommand = new AsyncCommand(DeleteBook);
            ToggleEditBookCommand = new AsyncCommand(ToggleEditBook);
            SaveBookCommand = new AsyncCommand(SaveBook);
            LoadPhotoCommand = new RelayCommand(LoadPhoto);
            RemoveSingleAuthor = new RelayCommand<int>(id => RemoveAuthor(id));
            RemoveSingleGenre = new RelayCommand<int>(id => RemoveGenre(id));
            DeleteRatingCommand = new AsyncCommand<int>(async id => await RemoveRating(id));
            AddAuthorCommand = new RelayCommand(AddAuthor);
            AddGenreCommand = new RelayCommand(AddGenre);
            #endregion

            #region dynamic icons and strings
            EditIcon = Images.Edit;
            EditButtonText = "Upravit";
            FilterChevron = Images.ChevronDown;
            #endregion

            #region Visibilities
            ListVisible = Visibility.Visible;
            DetailVisible = Visibility.Collapsed;
            FilterVisible = Visibility.Collapsed;
            DeleteVisible = Visibility.Collapsed;
            #endregion
        }

        #region List methods
        public async Task LoadBooks()
        {
            var books = await _booksService.GetBooksLight(BookFilter, _sortingBy, _sortingOrder);
            Pager.Options = new ObservableCollection<int>(Enumerable.Range(1, books.Count / 10 + 1).ToList());
            if (Pager.Options.Last() < Pager.Page)
            {
                Pager.Page = Pager.Options.Last();
            }
            PageLeftCommand.RaiseCanExecuteChanged();
            PageRightCommand.RaiseCanExecuteChanged();
            var paged = books.Skip(Pager.PageSize * (Pager.Page - 1)).Take(Pager.PageSize).ToList();

            Books = new ObservableCollection<BookLight>(paged);
        }
        public async Task MovePage(bool left)
        {
            Pager.Page += left ? -1 : 1;
            await LoadBooks();
        }
        public async Task SortByName()
        {
            _sortingBy = SortBooksBy.Name;
            if (_sortingOrder == SortOrdering.Ascending)
            {
                _sortingOrder = SortOrdering.Descending;
                NameSortIcon = Images.ArrowDown;
                await LoadBooks();
            }
            else
            {
                _sortingOrder = SortOrdering.Ascending;
                NameSortIcon = Images.ArrowUp;
                await LoadBooks();
            }
        }
        public async Task Filter()
        {
            BookFilter = new BookFilter()
            {
                Name = NameFilter,
                Author = AuthorFilter,
                Genre = GenreFilter,
                Isbn = ISBNFilter
            };
            await LoadBooks();

        }
        public async Task ResetFilter()
        {
            NameFilter = "";
            AuthorFilter = "";
            GenreFilter = "";
            ISBNFilter = "";
            BookFilter = new BookFilter();
            await LoadBooks();
        }

        public void ToggleFilter()
        {
            if (FilterVisible == Visibility.Visible)
            {
                FilterVisible = Visibility.Collapsed;
                FilterChevron = Images.ChevronDown;
            }
            else
            {
                FilterVisible = Visibility.Visible;
                FilterChevron = Images.ChevronUp;
            }

        }
        public async Task SelectBook(int Id)
        {
            if (Id != 0)
            {
                DetailBook = await _booksService.GetById(Id);
                Photo = DetailBook.Photo;
            }
            else
            {
                DetailBook = new Book();
                DetailBook.Authors = new List<AuthorBook>();
                DetailBook.Genres = new List<GenreBook>();
                DetailBook.Ratings = new List<BookRating>();
                Photo = GreyImageIcon;
            }
            Ratings = new ObservableCollection<BookRating>(DetailBook.Ratings);
            GenreString = DetailBook.Genres.Count==0?"":string.Join(", ", DetailBook.Genres.Select(x => x.Genre.Name));
            ShowDetail();
            if (DetailBook.Id == 0)
            {
                DeleteVisible = Visibility.Visible;
                await ToggleEditBook();
            }
        }
        public void ShowDetail()
        {
            ListVisible = Visibility.Collapsed;
            DetailVisible = Visibility.Visible;
            ReadBookVisible = Visibility.Visible;
            EditBookVisible = Visibility.Collapsed;
        }
        #endregion

        #region Edit methods
        public async Task DeleteBook()
        {
            await _booksService.Delete(DetailBook.Id);
            await ToggleEditBook();
            await LoadBooks();
            DetailVisible = Visibility.Collapsed;
            ListVisible = Visibility.Visible;
        }

        public async Task SaveBook()
        {
            DetailBook.Authors = new List<AuthorBook>();
            foreach (AuthorLight authorLight in AddedAuthors)
            {
                Author author = await _authorService.GetById(authorLight.Id);
                AuthorBook relationship = new AuthorBook()
                {
                    AuthorId = author.Id,
                    BookId = DetailBook.Id,
                };
                DetailBook.Authors.Add(relationship);
            }
            DetailBook.Genres = new List<GenreBook>();
            foreach (Genre genre in AddedGenres)
            {
                GenreBook relationship = new GenreBook()
                {
                    GenreId = genre.Id,
                    BookId = DetailBook.Id,
                };
                DetailBook.Genres.Add(relationship);
            }
            if (!Validate())
            {
                return;
            }
            if (DetailBook.Id != 0)
            {
                await _booksService.Update(DetailBook);

            }
            else
            {
                DetailBook.Id = await _booksService.Create(DetailBook);
            }
            DetailBook = await _booksService.GetById(DetailBook.Id);
            GenreString = string.Join(", ", DetailBook.Genres.Select(x => x.Genre.Name));
            await ToggleEditBook();
        }

        public async Task ToggleEditBook()
        {
            if (EditBookVisible == Visibility.Visible)
            {
                if (DetailBook!=null && DetailBook.Id != 0)
                {
                    ReadBookVisible = Visibility.Visible;
                }
                else
                {
                    EditBookVisible = Visibility.Collapsed;
                    DetailVisible = Visibility.Collapsed;
                    ListVisible = Visibility.Visible;
                }
                EditBookVisible = Visibility.Collapsed;
                DeleteVisible = Visibility.Collapsed;
                EditIcon = Images.Edit;
                EditButtonText = "Upravit";
            }
            else
            {
                AddedAuthors = new ObservableCollection<AuthorLight>();
                AvailableAuthors = await _authorService.GetAuthorsLight();
                foreach (AuthorBook authorBook in DetailBook.Authors)
                {
                    AuthorLight authorLight = AvailableAuthors.First(x => x.Id == authorBook.AuthorId);
                    AddedAuthors.Add(authorLight);
                    AvailableAuthors.Remove(authorLight);
                };
                AddedGenres = new ObservableCollection<Genre>();
                AvailableGenres = new ObservableCollection<Genre>(await _booksService.GetGenres());
                foreach (GenreBook genreBook in DetailBook.Genres)
                {
                    Genre genre = AvailableGenres.First(x => x.Id == genreBook.GenreId);
                    AddedGenres.Add(genre);
                    AvailableGenres.Remove(genre);
                };
                if (DetailBook != null && DetailBook.Id != 0)
                {
                    DeleteVisible = Visibility.Visible;
                }
                else
                {
                    DeleteVisible = Visibility.Collapsed;
                }
                ReadBookVisible = Visibility.Collapsed;
                EditBookVisible = Visibility.Visible;
                EditIcon = Images.GoBack;
                EditButtonText = "Zpět";
            }
        }
        public void AddAuthor()
        {
            AddedAuthors.Add(ChosenAuthor);
            AvailableAuthors.Remove(ChosenAuthor);
        }

        public void AddGenre()
        {
            AddedGenres.Add(ChosenGenre);
            AvailableGenres.Remove(ChosenGenre);
        }
        public void RemoveAuthor(int Id)
        {
            AuthorLight remove = AddedAuthors.First(x => x.Id == Id);
            AddedAuthors.Remove(remove);
        }

        public void RemoveGenre(int Id)
        {
            Genre remove = AddedGenres.First(x => x.Id == Id);
            AddedGenres.Remove(remove);
        }

        public async Task RemoveRating(int Id)
        {
            await _booksService.DeleteRating(Id);
            BookRating rating = Ratings.First(x => x.Id == Id);
            Ratings.Remove(rating);
        }

        private void LoadPhoto()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Photo = File.ReadAllBytes(openFileDialog.FileName);
            }
        }
        #endregion

        #region Validation
        private bool Validate()
        {
            return ValidateName()&&ValidatePages();
        }
        
        private bool ValidateName()
        {
            if (string.IsNullOrEmpty(DetailBook.Name))
            {
                _messageBoxService.Show("Jméno knihy je požadovaná položka!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private bool ValidatePages()
        {
            if (DetailBook.Pages<=0)
            {
                _messageBoxService.Show("Počet stran nemůže být nula nebo méně!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        #endregion 
    }

}

