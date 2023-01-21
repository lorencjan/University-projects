using System;
using System.Windows;
using System.Windows.Input;
using MovieDatabase.App.Commands;
using MovieDatabase.App.Resources.Img;

namespace MovieDatabase.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Attributes
        private readonly MoviesViewModel _moviesViewModel;
        private readonly PeopleViewModel _peopleViewModel;
        private readonly SearchResultViewModel _searchResultViewModel;
        private Visibility _homeVisible, _moviesVisible, _peopleVisible, _searchResultsVisible;
        
        public Visibility HomePageVisible
        {
            get { return _homeVisible; }
            set { SetProperty(ref _homeVisible, value); }
        }
        public Visibility MoviePageVisible
        {
            get { return _moviesVisible; }
            set { SetProperty(ref _moviesVisible, value); }
        }
        public Visibility PeoplePageVisible
        {
            get { return _peopleVisible; }
            set { SetProperty(ref _peopleVisible, value); }
        }
        public Visibility SearchResultsVisible
        {
            get { return _searchResultsVisible; }
            set { SetProperty(ref _searchResultsVisible, value); }
        }

        public ICommand GoHomeCommand { get; set; }
        public ICommand GoToMoviesCommand { get; set; }
        public ICommand ShowMovieDetail { get; set; }
        public ICommand ShowMovieOfRating { get; set; }
        public ICommand GoToPeopleCommand { get; set; }
        public ICommand ShowPersonDetail { get; set; }
        public ICommand SearchCommand { get; set; }
        public byte[] HomeIcon { get; set; }
        public byte[] SearchIcon { get; set; }
        public byte[] NewIcon { get; set; }
        public string Filter { get; set; }
        #endregion
        
        public MainViewModel(MoviesViewModel moviesViewModel, PeopleViewModel peopleViewModel, SearchResultViewModel searchResultViewModel)
        {
            GoHomeCommand = new RelayCommand(GoHome);
            GoToMoviesCommand = new RelayCommand(GoToMovies);
            ShowMovieDetail = new RelayCommand<Guid>(GoToMovieDetail);
            ShowMovieOfRating = new RelayCommand<Guid>(GoToMovieDetailOfRating);
            GoToPeopleCommand = new RelayCommand(GoToPeople);
            ShowPersonDetail = new RelayCommand<Guid>(GoToPersonDetail);
            SearchCommand = new RelayCommand(Search, CanSearch);
            HomeIcon = Images.Home;
            NewIcon = Images.New;
            SearchIcon = Images.Search;
            ShowHomePage();
            _moviesViewModel = moviesViewModel;
            _peopleViewModel = peopleViewModel;
            _searchResultViewModel = searchResultViewModel;
        }

        private void CollapseAllPages()
        {
            HomePageVisible = Visibility.Collapsed;
            MoviePageVisible = Visibility.Collapsed;
            PeoplePageVisible = Visibility.Collapsed;
            SearchResultsVisible = Visibility.Collapsed;
        }

        private void ShowHomePage()
        {
            CollapseAllPages();
            HomePageVisible = Visibility.Visible;
        }

        private void ShowMoviePage()
        {
            CollapseAllPages();
            MoviePageVisible = Visibility.Visible;
        }

        private void ShowPeoplePage()
        {
            CollapseAllPages();
            PeoplePageVisible = Visibility.Visible;
        }

        private void ShowSearchResultPage()
        {
            CollapseAllPages();
            SearchResultsVisible = Visibility.Visible;
        }

        public void GoHome()
        {
            ShowHomePage();
        }

        public void GoToMovies()
        {
            ShowMoviePage();
            _moviesViewModel.LoadMovies();
        }

        public void GoToPeople()
        {
            ShowPeoplePage();
            _peopleViewModel.LoadPeople();
        }

        public bool CanSearch() => !string.IsNullOrEmpty(Filter);

        public void Search()
        {
            ShowSearchResultPage();
            _searchResultViewModel.SearchAll(Filter);
        }

        public void GoToMovieDetail(Guid id)
        {
            ShowMoviePage();
            _moviesViewModel.SelectMovie(id);
        }

        public void GoToMovieDetailOfRating(Guid id)
        {
            ShowMoviePage();
            var movieid = _searchResultViewModel.GetMovieByRating(id);
            _moviesViewModel.SelectMovie(movieid);
        }

        public void GoToPersonDetail(Guid id)
        {
            ShowPeoplePage();
            _peopleViewModel.SelectPerson(id);
        }
    }
}
