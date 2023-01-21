using BooksService.Client;
using BooksService.Common;
using BooksWpf.Commands;
using BooksWpf.DAL.Services;
using BooksWpf.Model;
using BooksWpf.Resources.Img;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BooksWpf.ViewModels
{
    public class NewsPostsViewModel : ViewModelBase
    {
        #region Servicies
        private readonly NewsPostsService _newsPostService;
        #endregion

        #region Key properties
        private ObservableCollection<NewsPost> _newsPosts;
        public ObservableCollection<NewsPost> NewsPosts
        {
            get => _newsPosts;
            set => SetProperty(ref _newsPosts, value);
        }
        private NewsPost _detailNewsPost;
        public NewsPost DetailNewsPost
        {
            get { return _detailNewsPost; }
            set { SetProperty(ref _detailNewsPost, value); }
        }
        #endregion

        #region list commands
        public IAsyncCommand LoadNewsPostsCommand { get; set; }
        public ICommand ToggleFilterCommand { get; set; }
        public ICommand ResetFilterCommand { get; set; }
        public IAsyncCommand FilterCommand { get; set; }
        public ICommand SelectNewsPostCommand { get; set; }
        public ICommand CreateNewsPostCommand { get; set; }
        public ICommand SortByHeaderCommand { get; set; }
        public ICommand SortByDateCommand { get; set; }
        #endregion

        #region Edit commands
        public IAsyncCommand SaveNewsPostCommand { get; set; }
        public ICommand ToggleEditNewsPostCommand { get; set; }
        public IAsyncCommand DeleteNewsPostCommand { get; set; }
        #endregion

        #region Visibilities
        public Visibility _listVisible, _detailVisible, _readNewsPostVisible, _editNewsPostVisible, _filterVisible, _newVisible;
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

        public Visibility ReadNewsPostVisible
        {
            get { return _readNewsPostVisible; }
            set { SetProperty(ref _readNewsPostVisible, value); }
        }

        public Visibility EditNewsPostVisible
        {
            get { return _editNewsPostVisible; }
            set { SetProperty(ref _editNewsPostVisible, value); }
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
        private string _editButtonText;
        public string EditButtonText
        {
            get { return _editButtonText; }
            set { SetProperty(ref _editButtonText, value); }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
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

        private byte[] _headerSortIcon, _dateSortIcon, _editIcon, _filterChevron;
        public byte[] NewIcon { get; } = Images.New;
        public byte[] DeleteIcon { get; } = Images.TrashBin;
        public byte[] FilterIcon { get; } = Images.Filter;

        public byte[] SaveIcon { get; } = Images.Save;


        public byte[] HeaderSortIcon
        {
            get => _headerSortIcon;
            set => SetProperty(ref _headerSortIcon, value);
        }

        public byte[] DateSortIcon
        {
            get => _dateSortIcon;
            set => SetProperty(ref _dateSortIcon, value);
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
        private string _headerFilter;
        private BooksService.Client.SortNewsPostsBy _sortingBy;
        private SortOrdering _sortingOrder;
        private NewsPostFilter _bookFilter;
        public NewsPostFilter NewsPostFilter
        {
            get { return _bookFilter; }
            set { SetProperty(ref _bookFilter, value); }
        }
        public string HeaderFilter
        {
            get { return _headerFilter; }
            set { SetProperty(ref _headerFilter, value); }
        }
        #endregion
        public NewsPostsViewModel(NewsPostsService newsPostService)
        {
            #region Services
            _newsPostService = newsPostService;
            #endregion

            #region Empty initializations
            Pager = new Pager();
            NewsPostFilter = new NewsPostFilter();
            #endregion

            #region View commands
            LoadNewsPostsCommand = new AsyncCommand(async () => await LoadNewsPosts());
            PageLeftCommand = new AsyncCommand(async () => await MovePage(true), () => Pager.Page != Pager.Options.First());
            PageRightCommand = new AsyncCommand(async () => await MovePage(false), () => Pager.Page != Pager.Options.Last());
            SelectNewsPostCommand = new AsyncCommand<int>(async id => await SelectNewsPost(id));
            CreateNewsPostCommand = new AsyncCommand(async () => await SelectNewsPost(0));
            ToggleFilterCommand = new RelayCommand(ToggleFilter);
            ResetFilterCommand = new AsyncCommand(ResetFilter);
            FilterCommand = new AsyncCommand(Filter);
            SortByHeaderCommand = new AsyncCommand(SortByHeader);
            SortByDateCommand = new AsyncCommand(SortByDate);
            #endregion

            #region Detail commands
            DeleteNewsPostCommand = new AsyncCommand(DeleteNewsPost);
            ToggleEditNewsPostCommand = new RelayCommand(ToggleEditNewsPost);
            SaveNewsPostCommand = new AsyncCommand(SaveNewsPost);
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
        public async Task LoadNewsPosts()
        {
            var newsPosts = await _newsPostService.GetNewsPosts(NewsPostFilter, _sortingBy, _sortingOrder);
            Pager.Options = new ObservableCollection<int>(Enumerable.Range(1, newsPosts.Count / 10 + 1).ToList());
            if (Pager.Options.Last() < Pager.Page)
            {
                Pager.Page = Pager.Options.Last();
            }
            PageLeftCommand.RaiseCanExecuteChanged();
            PageRightCommand.RaiseCanExecuteChanged();
            var paged = newsPosts.Skip(Pager.PageSize * (Pager.Page - 1)).Take(Pager.PageSize).ToList();

            NewsPosts = new ObservableCollection<NewsPost>(paged);
        }
        public async Task MovePage(bool left)
        {
            Pager.Page += left ? -1 : 1;
            await LoadNewsPosts();
        }

        private void NullateSortIcons()
        {
            HeaderSortIcon =  emptyImage;
            DateSortIcon =  emptyImage;
        }
        public async Task SortByHeader()
        {
            NullateSortIcons();
            if (_sortingBy == BooksService.Client.SortNewsPostsBy.Header && _sortingOrder == SortOrdering.Ascending)
            {
                _sortingOrder = SortOrdering.Descending;
                HeaderSortIcon = Images.ArrowDown;
            }
            else
            {
                _sortingOrder = SortOrdering.Ascending;
                HeaderSortIcon = Images.ArrowUp;
            }
            _sortingBy = BooksService.Client.SortNewsPostsBy.Header;
            await LoadNewsPosts();
        }

        public async Task SortByDate()
        {
            NullateSortIcons();
            if (_sortingBy == BooksService.Client.SortNewsPostsBy.Date && _sortingOrder == SortOrdering.Ascending)
            {
                _sortingOrder = SortOrdering.Descending;
                DateSortIcon = Images.ArrowDown;
            }
            else
            {
                _sortingOrder = SortOrdering.Ascending;
                DateSortIcon = Images.ArrowUp;
            }
            _sortingBy = BooksService.Client.SortNewsPostsBy.Date;
            await LoadNewsPosts();
        }

        public async Task Filter()
        {
            NewsPostFilter = new NewsPostFilter()
            {
                Header = HeaderFilter,
            };
            await LoadNewsPosts();

        }
        public async Task ResetFilter()
        {
            HeaderFilter = "";
            NewsPostFilter = new NewsPostFilter();
            await LoadNewsPosts();
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
        public async Task SelectNewsPost(int Id)
        {
            if (Id != 0)
            {
                DetailNewsPost = await _newsPostService.GetById(Id);
            }
            else
            {
                DetailNewsPost = new NewsPost();
            }
            ShowDetail();
            if (DetailNewsPost.Id == 0)
            {
                DeleteVisible = Visibility.Visible;
                ToggleEditNewsPost();
            }
        }
        public void ShowDetail()
        {
            ListVisible = Visibility.Collapsed;
            DetailVisible = Visibility.Visible;
            ReadNewsPostVisible = Visibility.Visible;
            EditNewsPostVisible = Visibility.Collapsed;
        }
        #endregion

        #region Edit methods
        public async Task DeleteNewsPost()
        {
            await _newsPostService.Delete(DetailNewsPost.Id);
            ToggleEditNewsPost();
            await LoadNewsPosts();
            DetailVisible = Visibility.Collapsed;
            ListVisible = Visibility.Visible;
        }

        public async Task SaveNewsPost()
        {
            DetailNewsPost.Date = new DateTimeOffset(Date);
            if (!Validate())
            {
                return;
            }
            if (DetailNewsPost.Id != 0)
            {
                await _newsPostService.Update(DetailNewsPost);
            }
            else
            {
                DetailNewsPost.Id = await _newsPostService.Create(DetailNewsPost);
            }
            DetailNewsPost = await _newsPostService.GetById(DetailNewsPost.Id);
            ToggleEditNewsPost();
        }

        public void ToggleEditNewsPost()
        {
            if (EditNewsPostVisible == Visibility.Visible)
            {
                if (DetailNewsPost != null && DetailNewsPost.Id != 0)
                {
                    ReadNewsPostVisible = Visibility.Visible;
                }
                else
                {
                    EditNewsPostVisible = Visibility.Collapsed;
                    DetailVisible = Visibility.Collapsed;
                    ListVisible = Visibility.Visible;
                }
                EditNewsPostVisible = Visibility.Collapsed;
                DeleteVisible = Visibility.Collapsed;
                EditIcon = Images.Edit;
                EditButtonText = "Upravit";
            }
            else
            {
                if (DetailNewsPost != null && DetailNewsPost.Id != 0)
                {
                    Date = DetailNewsPost.Date.DateTime;
                    DeleteVisible = Visibility.Visible;
                }
                else
                {
                    Date = DateTime.Now;
                    DeleteVisible = Visibility.Collapsed;
                }
                ReadNewsPostVisible = Visibility.Collapsed;
                EditNewsPostVisible = Visibility.Visible;
                EditIcon = Images.GoBack;
                EditButtonText = "Zpět";
            }
        }
        #endregion

        private bool Validate()
        {
            return ValidateHeader() && ValidateText();

        }
        
        private bool ValidateHeader()
        {
            if (string.IsNullOrEmpty(DetailNewsPost.Header))
            {
                _messageBoxService.Show("Novinka musí mít nadpis!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private bool ValidateText()
        {
            if (string.IsNullOrEmpty(DetailNewsPost.Text))
            {
                _messageBoxService.Show("Novinka musí mít text!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }

}
