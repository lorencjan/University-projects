using MovieDatabase.BL.Repositories;
using MovieDatabase.DAL.Factories;

namespace MovieDatabase.App.ViewModels
{
    public class ViewModelLocator
    {
        private readonly MovieRepository _movieRepository;
        private readonly PersonRepository _personRepository;
        private readonly RatingRepository _ratingRepository;

        public MainViewModel MainViewModel { get; private set; }
        public HomePageViewModel HomePageViewModel { get; private set; }
        public MoviesViewModel MoviesViewModel { get; private set; }
        public PeopleViewModel PeopleViewModel { get; private set; }
        public SearchResultViewModel SearchResultViewModel { get; private set; }
        public NewMovieViewModel NewMovieViewModel { get; private set; }
        public NewPersonViewModel NewPersonViewModel { get; private set; }
        public NewRatingViewModel NewRatingViewModel { get; private set; }
        
        public ViewModelLocator()
        {
            var dbContextFactory = new MovieDatabaseDbContextFactory();
            _movieRepository = new MovieRepository(dbContextFactory);
            _personRepository = new PersonRepository(dbContextFactory);
            _ratingRepository = new RatingRepository(dbContextFactory);

            NewMovieViewModel = new NewMovieViewModel(_movieRepository, _personRepository);
            NewPersonViewModel = new NewPersonViewModel(_personRepository, _movieRepository);
            NewRatingViewModel = new NewRatingViewModel(_ratingRepository, _movieRepository);
            HomePageViewModel = new HomePageViewModel();
            MoviesViewModel = new MoviesViewModel(_movieRepository, _ratingRepository, NewRatingViewModel, NewMovieViewModel);
            PeopleViewModel = new PeopleViewModel(_personRepository, NewPersonViewModel);
            SearchResultViewModel = new SearchResultViewModel(_movieRepository, _personRepository, _ratingRepository);

            MainViewModel = new MainViewModel(MoviesViewModel, PeopleViewModel, SearchResultViewModel);
        }
    }
}
