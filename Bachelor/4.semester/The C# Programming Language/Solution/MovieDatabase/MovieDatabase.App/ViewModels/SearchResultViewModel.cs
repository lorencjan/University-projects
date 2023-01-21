using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Repositories;

namespace MovieDatabase.App.ViewModels
{
    public class SearchResultViewModel : ViewModelBase
    {
        private readonly PersonRepository _personRepository;
        private readonly MovieRepository _movieRepository;
        private readonly RatingRepository _ratingRepository;
        private ObservableCollection<PersonListDto> _listPeople;
        private ObservableCollection<MovieListDto> _listMovies;
        private ObservableCollection<RatingDto> _listRatings;
        public ObservableCollection<PersonListDto> ListPeople
        {
            get { return _listPeople; }
            set { SetProperty(ref _listPeople, value); }
        }
        public ObservableCollection<MovieListDto> ListMovies
        {
            get { return _listMovies; }
            set { SetProperty(ref _listMovies, value); }
        }
        public ObservableCollection<RatingDto> ListRatings
        {
            get { return _listRatings; }
            set { SetProperty(ref _listRatings, value); }
        }

        public SearchResultViewModel(MovieRepository movieRepository, PersonRepository personRepository, RatingRepository ratingRepository)
        {
            _personRepository = personRepository;
            _movieRepository = movieRepository;
            _ratingRepository = ratingRepository;
        }

        public void SearchAll(string filter)
        {
            SearchPeople(filter);
            SearchMovies(filter);
            SearchRatings(filter);
        }

        public void SearchPeople(string filter)
        {
            try
            {
                var people = _personRepository.GetAllAsList(filter);
                ListPeople = new ObservableCollection<PersonListDto>(people.OrderBy(x => x.DisplayName));
            }
            catch
            {
                _messageBoxService.Show("Failed to load people from the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SearchMovies(string filter)
        {
            try
            {
                var movies = _movieRepository.GetAllAsList(filter);
                ListMovies = new ObservableCollection<MovieListDto>(movies.OrderBy(x => x.DisplayName));
            }
            catch
            {
                _messageBoxService.Show("Failed to load movies from the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SearchRatings(string filter)
        {
            try
            {
                var ratings = _ratingRepository.GetAll(filter);
                ListRatings = new ObservableCollection<RatingDto>(ratings.OrderBy(x => x.MovieName));
            }
            catch
            {
                _messageBoxService.Show("Failed to load ratings from the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Guid GetMovieByRating(Guid id)
        {
            return _ratingRepository.GetById(id).MovieId;
        }
    }
}
