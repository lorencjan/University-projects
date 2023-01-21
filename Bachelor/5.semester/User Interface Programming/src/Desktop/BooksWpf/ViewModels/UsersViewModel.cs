using BooksService.Client;
using BooksService.Common;
using BooksWpf.Commands;
using BooksWpf.DAL.Services;
using BooksWpf.Model;
using BooksWpf.Resources.Img;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BooksWpf.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        #region Servicies
        private readonly UsersService _userService;
        #endregion

        #region Key properties
        private ObservableCollection<UserLight> _users;
        public ObservableCollection<UserLight> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }
        #endregion

        #region list commands
        public IAsyncCommand LoadUsersCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }
        #endregion

        #region Visibilities
        public Visibility _listVisible, _filterVisible;
        public Visibility ListVisible
        {
            get { return _listVisible; }
            set { SetProperty(ref _listVisible, value); }
        }
        public Visibility FilterVisible
        {
            get { return _filterVisible; }
            set { SetProperty(ref _filterVisible, value); }
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

        #region Sort and filter
        public ICommand ToggleFilterCommand { get; set; }
        public ICommand ResetFilterCommand { get; set; }
        public IAsyncCommand FilterCommand { get; set; }
        public IAsyncCommand SortByLoginCommand { get; set; }
        private SortUsersBy _sortingBy;
        private SortOrdering _sortingOrder;
        private string _userFilter;
        public string UserFilter
        {
            get { return _userFilter; }
            set { SetProperty(ref _userFilter, value); }
        }

        private byte[] _loginSortIcon,_filterChevron;
        public byte[] LoginSortIcon
        {
            get => _loginSortIcon;
            set => SetProperty(ref _loginSortIcon, value);
        }

        #endregion

        public byte[] DeleteIcon { get; } = Images.TrashBin;
        public byte[] SaveIcon { get; } = Images.Save;
        public byte[] FilterIcon {get;} = Images.Filter;
        public byte[] FilterChevron
        {
            get { return _filterChevron; }
            set { SetProperty(ref _filterChevron, value); }
        }
        public UsersViewModel(UsersService userService)
        {
            #region Services
            _userService = userService;
            #endregion

            #region Empty initializations
            Pager = new Pager();
            #endregion

            #region View commands
            LoadUsersCommand = new AsyncCommand(async () => await LoadUsers());
            PageLeftCommand = new AsyncCommand(async () => await MovePage(true), () => Pager.Page != Pager.Options.First());
            PageRightCommand = new AsyncCommand(async () => await MovePage(false), () => Pager.Page != Pager.Options.Last());
            DeleteUserCommand = new AsyncCommand<int>(async id => await DeleteUser(id));
            #endregion
            FilterChevron = Images.ChevronDown;
            LoginSortIcon = Images.ArrowUp;
            ToggleFilterCommand = new RelayCommand(ToggleFilter);
            ResetFilterCommand = new AsyncCommand(ResetFilter);
            FilterCommand = new AsyncCommand(Filter);
            SortByLoginCommand = new AsyncCommand(SortByLogin);
            ListVisible = Visibility.Visible;
            FilterVisible = Visibility.Collapsed;
            UserFilter="";
            }

        public async Task LoadUsers()
        {
            var users = await _userService.GetUsersLight(UserFilter, _sortingBy, _sortingOrder);
            Pager.Options = new ObservableCollection<int>(Enumerable.Range(1, users.Count / 10 + 1).ToList());
            if (Pager.Options.Last() < Pager.Page)
            {
                Pager.Page = Pager.Options.Last();
            }
            PageLeftCommand.RaiseCanExecuteChanged();
            PageRightCommand.RaiseCanExecuteChanged();
            var paged = users.Skip(Pager.PageSize * (Pager.Page - 1)).Take(Pager.PageSize).ToList();

            Users = new ObservableCollection<UserLight>(paged);
        }
        public async Task MovePage(bool left)
        {
            Pager.Page += left ? -1 : 1;
            await LoadUsers();
        }

        public async Task DeleteUser(int Id)
        {
            await _userService.Delete(Id);
            Users = await _userService.GetUsersLight(UserFilter, _sortingBy, _sortingOrder);
        }
        public async Task SortByLogin()
        {
            if (_sortingBy == SortUsersBy.Login && _sortingOrder == SortOrdering.Ascending)
            {
                _sortingOrder = SortOrdering.Descending;
                LoginSortIcon = Images.ArrowDown;
            }
            else
            {
                _sortingOrder = SortOrdering.Ascending;
                LoginSortIcon = Images.ArrowUp;
            }
            _sortingBy = SortUsersBy.Login;
            await LoadUsers();
        }

        public async Task Filter()
        {
            await LoadUsers();
        }
        public async Task ResetFilter()
        {
            UserFilter = "";
            await LoadUsers();
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
    }
}
