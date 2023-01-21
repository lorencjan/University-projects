using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using BooksWpf.Resources.Img;
using BooksWpf.Commands;

namespace BooksWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private int _sideBarWidth;
        public int SideBarWidth {
            get => _sideBarWidth;
            set => SetProperty(ref _sideBarWidth, value);
        }
        private string _searchString;
        public string SearchString
        {
            get => _searchString;
            set => SetProperty(ref _searchString, value);
        }

        public ICommand GoCollapseSideBar { get; set; }
        public ICommand SearchInBooksCommand { get; set; }
        public ICommand SearchInAuthorsCommand { get; set; }
        public ICommand SearchInNewsPostsCommand { get; set; }
        private byte[] _sideBarIcon;
        public byte[] SideBarIcon {
            get => _sideBarIcon;
            set => SetProperty(ref _sideBarIcon, value);
        }

        public byte[] BookIcon { get; set; }
        public byte[] AuthorIcon { get; set; }
        public byte[] NewsPostIcon { get; set; }
        public byte[] UserIcon { get; set; }
        public byte[] FeedbackIcon { get; set; }
        public byte[] SearchIcon { get; } = Images.Filter;
        public byte[] Logo { get; } = Images.Logo;

        private readonly BooksViewModel _booksViewModel;

        private readonly AuthorsViewModel _authorsViewModel;
        private readonly NewsPostsViewModel _newsPostsViewModel;
        private readonly UsersViewModel _usersViewModel;
        private readonly FeedbacksViewModel _feedbacksViewModel;

        public MainViewModel(BooksViewModel booksViewModel, AuthorsViewModel authorsViewModel, NewsPostsViewModel newsPostsViewModel, UsersViewModel usersViewModel, FeedbacksViewModel feedbacksViewModel)
        {
            _booksViewModel = booksViewModel;
            _authorsViewModel = authorsViewModel;
            _newsPostsViewModel = newsPostsViewModel;
            _usersViewModel = usersViewModel;
            _feedbacksViewModel = feedbacksViewModel;
            SideBarWidth = 190;

            SideBarIcon = Images.ChevronLeft;
            BookIcon = Images.Book;
            AuthorIcon = Images.AuthorWhite;
            NewsPostIcon = Images.Newspaper;
            UserIcon = Images.Users;
            FeedbackIcon = Images.Feedback;
            SearchIcon = Images.Search;

            GoCollapseSideBar = new RelayCommand(CollapseSideBar);
            GoBooksCommand = new AsyncCommand(ShowBooks);
            GoAuthorsCommand = new AsyncCommand(ShowAuthors);
            GoNewsPostsCommand = new AsyncCommand(ShowNewsPosts);
            GoUsersCommand = new AsyncCommand(ShowUsers);
            GoFeedbacksCommand = new AsyncCommand(ShowFeedbacks);
            SearchInBooksCommand = new AsyncCommand(GlobalSearchBooks);
            SearchInAuthorsCommand = new AsyncCommand(GlobalSearchAuthors);
            SearchInNewsPostsCommand = new AsyncCommand(GlobalSearchNewsPosts);
            CollapseAllPages();
        }

        public async Task GlobalSearchBooks()
        {
            await ShowBooks();
            _booksViewModel.NameFilter = SearchString;
            await _booksViewModel.Filter();
        }

        public async Task GlobalSearchAuthors()
        {
            await ShowAuthors();
            _authorsViewModel.NameFilter = SearchString;
            await _authorsViewModel.Filter();
        }

        public async Task GlobalSearchNewsPosts()
        {
            await ShowNewsPosts();
            _newsPostsViewModel.HeaderFilter = SearchString;
            await _newsPostsViewModel.Filter();
        }

        public void CollapseSideBar()
        {
            SideBarIcon = Enumerable.SequenceEqual(SideBarIcon, Images.ChevronRight) ? Images.ChevronLeft : Images.ChevronRight;
            SideBarWidth = SideBarWidth == 47 ? 190 : 47;
        }

        #region Visibility attributes, functions and commands

        private async Task CollapseAllPages()
        {
            BooksVisible = Visibility.Collapsed;
            BooksColor = "#379816";
            _booksViewModel.DetailVisible = Visibility.Collapsed;
            if (_booksViewModel.EditBookVisible == Visibility.Visible)
            {
                await _booksViewModel.ToggleEditBook();
            }
            AuthorsVisible = Visibility.Collapsed;
            AuthorsColor = "#379816";
            _authorsViewModel.DetailVisible = Visibility.Collapsed;
            if (_authorsViewModel.EditAuthorVisible == Visibility.Visible)
            {
                _authorsViewModel.ToggleEditAuthor();
            }
            NewsPostsVisible = Visibility.Collapsed;
            NewsPostsColor = "#379816";
            _newsPostsViewModel.DetailVisible = Visibility.Collapsed;
            if (_newsPostsViewModel.EditNewsPostVisible == Visibility.Visible)
            {
                _newsPostsViewModel.ToggleEditNewsPost();
            }
            UsersVisible = Visibility.Collapsed;
            UsersColor = "#379816";
            FeedbacksVisible = Visibility.Collapsed;
            FeedbacksColor = "#379816";
        }

        #region Books visibility
        private string _booksColor;
        public string BooksColor
        {
            get => _booksColor;
            set => SetProperty(ref _booksColor, value);
        }

        private Visibility _booksVisible;

        public Visibility BooksVisible
        {
            get => _booksVisible;
            set => SetProperty(ref _booksVisible, value);
        }

        public IAsyncCommand GoBooksCommand { get; set; }
        private async Task ShowBooks()
        {
            await _booksViewModel.ResetFilter();
            await _booksViewModel.LoadBooks();
            await CollapseAllPages();
            if (_booksViewModel.FilterVisible == Visibility.Visible)
            {
                _booksViewModel.ToggleFilter();
            }
            BooksVisible = Visibility.Visible;
            BooksColor = "#286b0d";
            _booksViewModel.ListVisible = Visibility.Visible;
        }
        #endregion

        #region Authors visibility
        private string _authorsColor;
        public string AuthorsColor
        {
            get => _authorsColor;
            set => SetProperty(ref _authorsColor, value);
        }

        private Visibility _authorsVisible;

        public Visibility AuthorsVisible
        {
            get => _authorsVisible;
            set => SetProperty(ref _authorsVisible, value);
        }

        public ICommand GoAuthorsCommand { get; set; }
        private async Task ShowAuthors()
        {
            await _authorsViewModel.ResetFilter();
            await _authorsViewModel.LoadAuthors();
            await CollapseAllPages();
            AuthorsVisible = Visibility.Visible;
            if (_authorsViewModel.FilterVisible == Visibility.Visible)
            {
                _authorsViewModel.ToggleFilter();
            }
            _authorsViewModel.ListVisible = Visibility.Visible;
            AuthorsColor = "#286b0d";
        }
        #endregion

        #region NewsPosts visibility
        private string _newsPostsColor;
        public string NewsPostsColor
        {
            get => _newsPostsColor;
            set => SetProperty(ref _newsPostsColor, value);
        }

        private Visibility _newsPostsVisible;

        public Visibility NewsPostsVisible
        {
            get => _newsPostsVisible;
            set => SetProperty(ref _newsPostsVisible, value);
        }

        public ICommand GoNewsPostsCommand { get; set; }
        private async Task ShowNewsPosts()
        {
            await _newsPostsViewModel.ResetFilter();
            await _newsPostsViewModel.LoadNewsPosts();
            await CollapseAllPages();
            NewsPostsVisible = Visibility.Visible;
            if (_newsPostsViewModel.FilterVisible == Visibility.Visible)
            {
                _newsPostsViewModel.ToggleFilter();
            }
            _newsPostsViewModel.ListVisible = Visibility.Visible;
            NewsPostsColor = "#286b0d";
     
        }
        #endregion

        #region Users visibility
        private string _usersColor;
        public string UsersColor
        {
            get => _usersColor;
            set => SetProperty(ref _usersColor, value);
        }

        private Visibility _usersVisible;

        public Visibility UsersVisible
        {
            get => _usersVisible;
            set => SetProperty(ref _usersVisible, value);
        }

        public ICommand GoUsersCommand { get; set; }
        private async Task ShowUsers()
        {
            await _usersViewModel.ResetFilter();
            await _usersViewModel.LoadUsers();
            await CollapseAllPages();
            if (_usersViewModel.FilterVisible == Visibility.Visible)
            {
                _usersViewModel.ToggleFilter();
            }
            UsersVisible = Visibility.Visible;
            UsersColor = "#286b0d";
        }
        #endregion

        #region Feebacks visibility
        private string _feedbacksColor;
        public string FeedbacksColor
        {
            get => _feedbacksColor;
            set => SetProperty(ref _feedbacksColor, value);
        }

        private Visibility _feedbacksVisible;

        public Visibility FeedbacksVisible
        {
            get => _feedbacksVisible;
            set => SetProperty(ref _feedbacksVisible, value);
        }

        public ICommand GoFeedbacksCommand { get; set; }
        private async Task ShowFeedbacks()
        {
            await _feedbacksViewModel.LoadFeedbacks();
            await CollapseAllPages();
            FeedbacksVisible = Visibility.Visible;
            FeedbacksColor = "#286b0d";
        }
        #endregion

        #endregion
    }
}
