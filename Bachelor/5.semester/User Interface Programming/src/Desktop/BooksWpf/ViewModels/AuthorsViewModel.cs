using BooksService.Client;
using BooksService.Common;
using BooksWpf.Commands;
using BooksWpf.DAL.Services;
using BooksWpf.Model;
using BooksWpf.Resources.Img;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BooksWpf.ViewModels
{
    public class AuthorsViewModel : ViewModelBase
    {
        #region Servicies
        private readonly AuthorService _authorService;
        #endregion

        #region Key properties
        private ObservableCollection<AuthorLight> _authors;
        public ObservableCollection<AuthorLight> Authors
        {
            get => _authors;
            set => SetProperty(ref _authors, value);
        }
        private Author _detailAuthor;
        public Author DetailAuthor
        {
            get { return _detailAuthor; }
            set { SetProperty(ref _detailAuthor, value); }
        }
        private ObservableCollection<AuthorRating> _ratings;
        public ObservableCollection<AuthorRating> Ratings
        {
            get { return _ratings; }
            set { SetProperty(ref _ratings, value); }
        }
        #endregion

        #region list commands
        public IAsyncCommand LoadAuthorsCommand { get; set; }
        public ICommand ToggleFilterCommand { get; set; }
        public ICommand ResetFilterCommand { get; set; }
        public IAsyncCommand FilterCommand { get; set; }
        public ICommand SelectAuthorCommand { get; set; }
        public ICommand CreateAuthorCommand { get; set; }
        public ICommand SortByFirstNameCommand { get; set; }
        public ICommand SortByLastNameCommand { get; set; }
        public ICommand SortByBirthDateCommand { get; set; }
        public ICommand SortByCountryCommand { get; set; }


        #endregion

        #region Edit commands
        public ICommand LoadPhotoCommand { get; set; }
        public IAsyncCommand SaveAuthorCommand { get; set; }
        public ICommand ToggleEditAuthorCommand { get; set; }
        public ICommand DeleteRatingCommand { get; set; }
        public IAsyncCommand DeleteAuthorCommand { get; set; }

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
        public Visibility _listVisible, _detailVisible, _readAuthorVisible, _editAuthorVisible, _filterVisible, _newVisible;
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

        public Visibility ReadAuthorVisible
        {
            get { return _readAuthorVisible; }
            set { SetProperty(ref _readAuthorVisible, value); }
        }

        public Visibility EditAuthorVisible
        {
            get { return _editAuthorVisible; }
            set { SetProperty(ref _editAuthorVisible, value); }
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

        #region dynamic value
        private string _editButtonText;
        public string EditButtonText
        {
            get { return _editButtonText; }
            set { SetProperty(ref _editButtonText, value); }
        }
        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set { SetProperty(ref _birthDate, value); }
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
        private readonly byte[] emptyImage = Images.None;
        private byte[] _firstNameSortIcon, _lastNameSortIcon, _birthDateSortIcon, _countrySortIcon, _editIcon, _filterChevron, _photo;
        public byte[] BookIcon { get; } = Images.BookBlack;
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

        public byte[] FirstNameSortIcon
        {
            get => _firstNameSortIcon;
            set => SetProperty(ref _firstNameSortIcon, value);
        }

        public byte[] LastNameSortIcon
        {
            get => _lastNameSortIcon;
            set => SetProperty(ref _lastNameSortIcon, value);
        }

        public byte[] BirthDateSortIcon
        {
            get => _birthDateSortIcon;
            set => SetProperty(ref _birthDateSortIcon, value);
        }

        public byte[] CountrySortIcon
        {
            get => _countrySortIcon;
            set => SetProperty(ref _countrySortIcon, value);
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
        private string _nameFilter, _countryFilter;
        private SortAuthorsBy _sortingBy;
        private SortOrdering _sortingOrder;
        private AuthorFilter _bookFilter;
        public AuthorFilter AuthorFilter
        {
            get { return _bookFilter; }
            set { SetProperty(ref _bookFilter, value); }
        }
        public string NameFilter
        {
            get { return _nameFilter; }
            set { SetProperty(ref _nameFilter, value); }
        }

        public string CountryFilter
        {
            get { return _countryFilter; }
            set { SetProperty(ref _countryFilter, value); }
        }
        #endregion
        public AuthorsViewModel(AuthorService authorService)
        {
            #region Services
            _authorService = authorService;
            #endregion
            #region Empty initializations
            Pager = new Pager();
            AddedAuthors = new ObservableCollection<AuthorLight>();
            AddedGenres = new ObservableCollection<Genre>();
            AuthorFilter = new AuthorFilter();
            #endregion

            #region View commands
            LoadAuthorsCommand = new AsyncCommand(async () => await LoadAuthors());
            PageLeftCommand = new AsyncCommand(async () => await MovePage(true), () => Pager.Page != Pager.Options.First());
            PageRightCommand = new AsyncCommand(async () => await MovePage(false), () => Pager.Page != Pager.Options.Last());
            SelectAuthorCommand = new AsyncCommand<int>(async id => await SelectAuthor(id));
            CreateAuthorCommand = new AsyncCommand(async () => await SelectAuthor(0));
            ToggleFilterCommand = new RelayCommand(ToggleFilter);
            ResetFilterCommand = new AsyncCommand(ResetFilter);
            FilterCommand = new AsyncCommand(Filter);
            SortByFirstNameCommand = new AsyncCommand(SortByFirstName);
            SortByLastNameCommand = new AsyncCommand(SortByLastName);
            SortByBirthDateCommand = new AsyncCommand(SortByBirthDate);
            SortByCountryCommand = new AsyncCommand(SortByCountry);
            #endregion

            #region Detail commands
            DeleteAuthorCommand = new AsyncCommand(DeleteAuthor);
            ToggleEditAuthorCommand = new RelayCommand(ToggleEditAuthor);
            SaveAuthorCommand = new AsyncCommand(SaveAuthor);
            LoadPhotoCommand = new RelayCommand(LoadPhoto);
            DeleteRatingCommand = new AsyncCommand<int>(async id => await RemoveRating(id));
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
        public async Task LoadAuthors()
        {
            var authors = await _authorService.GetAuthorsLight(AuthorFilter, _sortingBy, _sortingOrder);
            Pager.Options = new ObservableCollection<int>(Enumerable.Range(1, authors.Count / 10 + 1).ToList());
            if (Pager.Options.Last() < Pager.Page)
            {
                Pager.Page = Pager.Options.Last();
            }
            PageLeftCommand.RaiseCanExecuteChanged();
            PageRightCommand.RaiseCanExecuteChanged();
            var paged = authors.Skip(Pager.PageSize * (Pager.Page - 1)).Take(Pager.PageSize).ToList();

            Authors = new ObservableCollection<AuthorLight>(paged);
        }
        public async Task MovePage(bool left)
        {
            Pager.Page += left ? -1 : 1;
            await LoadAuthors();
        }

        private void NullateSortIcons()
        {
            FirstNameSortIcon = emptyImage;
            LastNameSortIcon = emptyImage;
            BirthDateSortIcon = emptyImage;
            CountrySortIcon = emptyImage;
        }
        public async Task SortByFirstName()
        {
            NullateSortIcons();
            if (_sortingBy == SortAuthorsBy.FirstName && _sortingOrder == SortOrdering.Ascending)
            {
                _sortingOrder = SortOrdering.Descending;
                FirstNameSortIcon = Images.ArrowDown;
            }
            else
            {
                _sortingOrder = SortOrdering.Ascending;
                FirstNameSortIcon = Images.ArrowUp;
            }
            _sortingBy = SortAuthorsBy.FirstName;
            await LoadAuthors();
        }

        public async Task SortByLastName()
        {
            NullateSortIcons();
            if (_sortingBy == SortAuthorsBy.LastName && _sortingOrder == SortOrdering.Ascending) {
                _sortingOrder = SortOrdering.Descending;
                LastNameSortIcon = Images.ArrowDown;
            }
            else
            {
                _sortingOrder = SortOrdering.Ascending;
                LastNameSortIcon = Images.ArrowUp;
            }
            _sortingBy = SortAuthorsBy.LastName;
            await LoadAuthors();
        }

        public async Task SortByBirthDate()
        {
            NullateSortIcons();
            if (_sortingBy == SortAuthorsBy.BirthDate && _sortingOrder == SortOrdering.Ascending)
            {
                _sortingOrder = SortOrdering.Descending;
                BirthDateSortIcon = Images.ArrowDown;
            }
            else
            {
                _sortingOrder = SortOrdering.Ascending;
                BirthDateSortIcon = Images.ArrowUp;
            }
            _sortingBy = SortAuthorsBy.BirthDate;
            await LoadAuthors();
        }

        public async Task SortByCountry()
        {
            NullateSortIcons();
            if (_sortingBy == SortAuthorsBy.Country && _sortingOrder == SortOrdering.Ascending)
            {
                _sortingOrder = SortOrdering.Descending;
                CountrySortIcon = Images.ArrowDown;
            }
            else
            {
                _sortingOrder = SortOrdering.Ascending;
                CountrySortIcon = Images.ArrowUp;
            }
            _sortingBy = SortAuthorsBy.Country;
            await LoadAuthors();
        }
        public async Task Filter()
        {
            AuthorFilter = new AuthorFilter()
            {
                Name = NameFilter,
                Country = CountryFilter
            };
            await LoadAuthors();

        }
        public async Task ResetFilter()
        {
            NameFilter = "";
            CountryFilter = "";
            AuthorFilter = new AuthorFilter();
            await LoadAuthors();
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
        public async Task SelectAuthor(int Id)
        {
            if (Id != 0)
            {
                DetailAuthor = await _authorService.GetById(Id);
                Photo = DetailAuthor.Photo;
            }
            else
            {
                DetailAuthor = new Author();
                DetailAuthor.Books = new List<AuthorBook>();
                DetailAuthor.Ratings = new List<AuthorRating>();
                Photo = GreyImageIcon;
            }
            Ratings = new ObservableCollection<AuthorRating>(DetailAuthor.Ratings);
            ShowDetail();
            if (DetailAuthor.Id == 0)
            {
                DeleteVisible = Visibility.Visible;
                ToggleEditAuthor();
            }
        }
        public void ShowDetail()
        {
            ListVisible = Visibility.Collapsed;
            DetailVisible = Visibility.Visible;
            ReadAuthorVisible = Visibility.Visible;
            EditAuthorVisible = Visibility.Collapsed;
        }
        #endregion

        #region Edit methods
        public async Task DeleteAuthor()
        {
            await _authorService.Delete(DetailAuthor.Id);
            ToggleEditAuthor();
            await LoadAuthors();
            DetailVisible = Visibility.Collapsed;
            ListVisible = Visibility.Visible;
        }

        public async Task SaveAuthor()
        {
            DetailAuthor.BirthDate = new DateTimeOffset(BirthDate);
            if (DetailAuthor.Id != 0)
            {
                await _authorService.Update(DetailAuthor);
            }
            else
            {
                DetailAuthor.Id = await _authorService.Create(DetailAuthor);
            }
            if (!Validate())
            {
                return;
            }
            DetailAuthor = await _authorService.GetById(DetailAuthor.Id);
            ToggleEditAuthor();
        }

        public void ToggleEditAuthor()
        {
            if (EditAuthorVisible == Visibility.Visible)
            {
                if (DetailAuthor!=null && DetailAuthor.Id != 0)
                {
                    ReadAuthorVisible = Visibility.Visible;
                }
                else
                {
                    
                    EditAuthorVisible = Visibility.Collapsed;
                    DetailVisible = Visibility.Collapsed;
                    ListVisible = Visibility.Visible;
                }
                EditAuthorVisible = Visibility.Collapsed;
                DeleteVisible = Visibility.Collapsed;
                EditIcon = Images.Edit;
                EditButtonText = "Upravit";
            }
            else
            {    
                if (DetailAuthor != null && DetailAuthor.Id != 0)
                {
                    BirthDate = DetailAuthor.BirthDate.DateTime;
                    DeleteVisible = Visibility.Visible;
                }
                else
                {
                    BirthDate = DateTime.Now;

                    DeleteVisible = Visibility.Collapsed;
                }
                ReadAuthorVisible = Visibility.Collapsed;
                EditAuthorVisible = Visibility.Visible;
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
            await _authorService.DeleteRating(Id);
            AuthorRating rating = Ratings.First(x => x.Id == Id);
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

        private bool Validate()
        {
            return ValidateName();
        }
        
        private bool ValidateName()
        {
            if (string.IsNullOrEmpty(DetailAuthor.LastName))
            {
                _messageBoxService.Show("Příjmení autora je požadovaná položka!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

    }

}

