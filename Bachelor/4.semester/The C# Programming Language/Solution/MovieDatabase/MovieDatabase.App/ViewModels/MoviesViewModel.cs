using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MovieDatabase.App.Commands;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Repositories;
using MovieDatabase.App.Resources.Img;

namespace MovieDatabase.App.ViewModels
{
    public class MoviesViewModel : ViewModelBase
    {
        #region Attributes
        private bool _edit;
        private readonly MovieRepository _movieRepository;
        private readonly RatingRepository _ratingRepository;
        private ObservableCollection<MovieListDto> _listMovies;
        private MovieDto _detailMovie;
        private double _averageRating;
        private Visibility _listVisible, _detailVisible, _newMovieVisible, _newRatingVisible, _editMovieVisible, _readMovieVisible, _averageVisible;
        private byte[] _editIcon;
        public byte[] NewIcon{ get; set; }
        public byte[] EditIcon
        {
            get { return _editIcon; }
            set { SetProperty(ref _editIcon, value); }
        }
        public byte[] DeleteIcon{ get; set; }

        public NewRatingViewModel NewRatingViewModel { get; set; }
        public NewMovieViewModel NewMovieViewModel { get; set; }

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
        public Visibility NewMovieVisible
        {
            get { return _newMovieVisible; }
            set { SetProperty(ref _newMovieVisible, value); }
        }
        public Visibility NewRatingVisible
        {
            get { return _newRatingVisible; }
            set { SetProperty(ref _newRatingVisible, value); }
        }
        public Visibility ReadMovieVisible
        {
            get { return _readMovieVisible; }
            set { SetProperty(ref _readMovieVisible, value); }
        }
        public Visibility EditMovieVisible
        {
            get { return _editMovieVisible; }
            set { SetProperty(ref _editMovieVisible, value); }
        }
        public Visibility AverageVisible
        {
            get { return _averageVisible; }
            set { SetProperty(ref _averageVisible, value); }
        }
        public ObservableCollection<MovieListDto> ListMovies
        {
            get { return _listMovies; }
            set { SetProperty(ref _listMovies, value); }
        }
        public MovieDto DetailMovie
        {
            get { return _detailMovie; }
            set { SetProperty(ref _detailMovie, value); }
        }
        public double AverageRating
        {
            get { return _averageRating; }
            set { SetProperty(ref _averageRating, value); }
        }

        public ICommand SelectMovieCommand { get; set; }
        public ICommand DeleteMovieCommand { get; set; }
        public ICommand ShowNewMovieCommand { get; set; }
        public ICommand ShowNewRatingCommand { get; set; }
        public ICommand DeleteRatingCommand { get; set; }
        public ICommand ToggleEditMovieCommand { get; set; }
        public ICommand SaveEditCommand { get; set; }
        public ICommand SaveRatingCommand { get; set; }
        #endregion

        public MoviesViewModel(MovieRepository movieRepository, RatingRepository ratingRepository, NewRatingViewModel newRatingViewModel, NewMovieViewModel newMovieViewModel)
        {
            NewRatingViewModel = newRatingViewModel;
            NewMovieViewModel = newMovieViewModel;
            NewIcon = Images.New;
            EditIcon = Images.Edit;
            DeleteIcon = Images.Delete;
            ShowList();
            _movieRepository = movieRepository;
            _ratingRepository = ratingRepository;
            SelectMovieCommand = new RelayCommand<Guid>(SelectMovie);
            DeleteMovieCommand = new RelayCommand(DeleteMovie);
            DeleteRatingCommand = new RelayCommand<Guid>(DeleteRating);
            ShowNewMovieCommand = new RelayCommand(ShowNewMovie);
            ShowNewRatingCommand = new RelayCommand(ShowNewRating);
            ToggleEditMovieCommand = new RelayCommand(ToggleEditMovie);
            SaveEditCommand = new RelayCommand<Guid>(SaveEdit);
            SaveRatingCommand = new RelayCommand(SaveRating);
            _edit = false;
        }
        
        private void CollapseAll()
        {
            ListVisible = Visibility.Collapsed;
            DetailVisible = Visibility.Collapsed;
            NewMovieVisible = Visibility.Collapsed;
            NewRatingVisible = Visibility.Collapsed;
            EditMovieVisible = Visibility.Collapsed;
        }

        private void CalculateAverageRating()
        {
            if (_detailMovie.Ratings.Count == 0)
            {
                AverageVisible = Visibility.Collapsed;
                return;
            }

            var sum = 0;
            foreach(var r in _detailMovie.Ratings)
            {
                sum += r.Number;
            }

            //Rounding to two decimal places
            AverageRating = Math.Truncate(( (double) sum / _detailMovie.Ratings.Count) * 100) / 100;
            AverageVisible = Visibility.Visible;
        }

        private void ReloadMovie()
        {
            DetailMovie = _movieRepository.GetById(DetailMovie.Id);
            CalculateAverageRating();
        }
        
        private void ShowList()
        {
            CollapseAll();
            ListVisible = Visibility.Visible;
        }

        private void ShowNewMovie()
        {
            CollapseAll();
            NewMovieViewModel.InitializeValues(new MovieDto());
            NewMovieVisible = Visibility.Visible;
            NewMovieViewModel.NewVisible = Visibility.Visible;
        }
       
        private void ShowDetail()
        {
            CollapseAll();
            ReadMovieVisible = Visibility.Visible;
            DetailVisible = Visibility.Visible;
            EditIcon = Images.Edit;
            CalculateAverageRating();
        }

        private void ToggleEditMovie()
        {
            _edit = true;
            ReadMovieVisible = EditMovieVisible == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            EditMovieVisible = EditMovieVisible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            NewMovieViewModel.InitializeValues(DetailMovie, "Original Photo");
            NewMovieViewModel.NewVisible = Visibility.Collapsed;
            EditIcon = EditIcon.SequenceEqual(Images.Edit) ? Images.Back : Images.Edit;
        }
        private void ShowNewRating()
        {
            MovieListDto movie = new MovieListDto()
            {
                Id = DetailMovie.Id,
                DisplayName = DetailMovie.OriginalName 
            };
            NewRatingViewModel.InitializeValues(movie);
            NewRatingVisible = NewRatingVisible==Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void SaveEdit(Guid id)
        {
            bool success = NewMovieViewModel.Save();
            if(success)
            {
                if(_edit)
                {
                    SelectMovie(id);
                }
                else
                {
                    LoadMovies();
                }
                _edit = false;
                EditIcon = Images.Edit;
            }
        }

        public void DeleteMovie()
        {
            _movieRepository.Delete(_detailMovie.Id);
            LoadMovies();
        }

        private void SaveRating()
        {
            bool success = NewRatingViewModel.Save();
            if(success)
            {
                ReloadMovie();
                NewRatingVisible = Visibility.Collapsed; 
            }
        }

        public void DeleteRating(Guid id)
        {
            _ratingRepository.Delete(id);
            SelectMovie(_detailMovie.Id);
        }

        public void LoadMovies()
        {
            try
            {
                var movies = _movieRepository.GetAllAsList();
                ListMovies = new ObservableCollection<MovieListDto>(movies.OrderBy(x => x.DisplayName));
                _edit = false;
                ShowList();
            }
            catch
            {
                _messageBoxService.Show("Failed to load movies from the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SelectMovie(Guid Id)
        {
            try
            {
                DetailMovie = _movieRepository.GetById(Id);
                ShowDetail();
            }
            catch
            {
                _messageBoxService.Show("Error occured while fetching movie details from the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
