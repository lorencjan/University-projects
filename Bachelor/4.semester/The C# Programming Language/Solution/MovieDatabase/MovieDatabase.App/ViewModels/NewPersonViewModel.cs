using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MovieDatabase.App.Commands;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Repositories;
using MovieDatabase.App.Resources.Img;

namespace MovieDatabase.App.ViewModels
{
    public class NewPersonViewModel : ViewModelBase
    {
        #region Attributes
        private Visibility _newVisible;
        private string _firstName, _lastName, _age, _country, _fileName;
        private MovieListDto _chosenMoviePlayedIn, _chosenMovieDirected;
        private ObservableCollection<MovieListDto> _availableMovies, _moviesPlayedIn, _moviesDirected;
        private readonly PersonRepository _personRepository;
        private readonly MovieRepository _movieRepository;
        private byte[] _photo;
        private Guid _id;

        public byte[] NewIcon{ get; set; }
        public byte[] AddImage{ get; set; }
        public byte[] ResetIcon{ get; set; }
        public byte[] SaveIcon{ get; set; }

        public Guid Id 
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        public string FirstName 
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }
        public string LastName 
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }
        public string Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }
        public string Country 
        {
            get { return _country; }
            set { SetProperty(ref _country, value); }
        }
        public ObservableCollection<MovieListDto> AvailableMovies
        {
            get { return _availableMovies; }
            set { SetProperty(ref  _availableMovies, value); }
        }
        public ObservableCollection<MovieListDto> MoviesPlayedIn    
        {
            get { return _moviesPlayedIn; }
            set { SetProperty(ref  _moviesPlayedIn, value); }
        }
        public MovieListDto ChosenMoviePlayedIn 
        {
            get { return _chosenMoviePlayedIn; }
            set { SetProperty(ref  _chosenMoviePlayedIn, value); }
        }
        public ObservableCollection<MovieListDto> MoviesDirected
        {
            get { return _moviesDirected; }
            set { SetProperty(ref  _moviesDirected, value); }
        }

        public MovieListDto ChosenMovieDirected
        {
            get { return _chosenMovieDirected; }
            set { SetProperty(ref  _chosenMovieDirected, value); }
        }
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }
        public byte[] Photo
        {
            get { return _photo; }
            set { SetProperty(ref _photo, value); }
        }
        public Visibility NewVisible
        {
            get { return _newVisible; }
            set { SetProperty(ref _newVisible, value); }
        }

        public ICommand LoadPhotoCommand { get; set; }
        public ICommand AddMoviePlayedInCommand { get; set; }
        public ICommand ResetMoviesPlayedInCommand { get; set; }
        public ICommand AddMovieDirectedCommand { get; set; }
        public ICommand ResetMoviesDirectedCommand { get; set; }
        #endregion

        public NewPersonViewModel(PersonRepository personRepository, MovieRepository movieRepository)
        {
            _personRepository = personRepository;
            _movieRepository = movieRepository;
            NewIcon = Images.New;
            ResetIcon = Images.Reset;
            AddImage = Images.AddImage;
            SaveIcon = Images.Save;
            AddMoviePlayedInCommand = new RelayCommand(AddMoviePlayedIn);
            ResetMoviesPlayedInCommand = new RelayCommand(ResetMoviesPlayedIn);
            AddMovieDirectedCommand = new RelayCommand(AddMovieDirected);
            ResetMoviesDirectedCommand = new RelayCommand(ResetMoviesDirected);
            LoadPhotoCommand = new RelayCommand(LoadPhoto);
        }

        public void InitializeValues(PersonDto p, string filename = "")
        {
            Id = p.Id;
            FirstName = p.FirstName;
            LastName = p.LastName;
            Age = p.Age.ToString();
            Country = p.Country;
            FileName = filename;
            Photo = p.Photo;
            MoviesPlayedIn = p.MoviesPlayedIn == null ? new ObservableCollection<MovieListDto>() : new ObservableCollection<MovieListDto>(p.MoviesPlayedIn);
            MoviesDirected = p.MoviesDirected == null ? new ObservableCollection<MovieListDto>() : new ObservableCollection<MovieListDto>(p.MoviesDirected);
            ChosenMoviePlayedIn = null;
            ChosenMovieDirected = null;
            AvailableMovies = new ObservableCollection<MovieListDto>(_movieRepository.GetAllAsList());
        }

        private void AddMoviePlayedIn()
        {
            if(ChosenMoviePlayedIn!=null && !MoviesPlayedIn.Contains(ChosenMoviePlayedIn))
            {
                MoviesPlayedIn.Add(ChosenMoviePlayedIn);
            }
        }

        private void ResetMoviesPlayedIn()
        {
            MoviesPlayedIn.Clear();
        }

        private void AddMovieDirected()
        {
            if(ChosenMovieDirected != null && !MoviesDirected.Contains(ChosenMovieDirected))
            {
                MoviesDirected.Add(ChosenMovieDirected);
            }
        }

        private void ResetMoviesDirected()
        {
            MoviesDirected.Clear();
        }

        public void LoadPhoto()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if(openFileDialog.ShowDialog() == true)
            {
				Photo = File.ReadAllBytes(openFileDialog.FileName);
                FileName = openFileDialog.FileName;
            }
        }

        public bool Save()
        {
            if (!ValidateName() || !ValidateAge())
                return false;

            PersonDto newPerson = new PersonDto()
            {
                Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                Age = short.Parse(Age),
                Country = Country,
                Photo = Photo,
                MoviesPlayedIn = new List<MovieListDto>(MoviesPlayedIn),
                MoviesDirected = new List<MovieListDto>(MoviesDirected)
            };
            
            try
            {
                _personRepository.InsertOrUpdate(newPerson);
            }
            catch
            {
                _messageBoxService.Show("Failed to update the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool ValidateName()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                _messageBoxService.Show("Full name of the person is required!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private bool ValidateAge()
        {
            try
            {
                if (!string.IsNullOrEmpty(Age))
                {
                    var parsedAge = short.Parse(Age);
                    if (parsedAge < 0)
                    {
                        _messageBoxService.Show("Age cannot be a negative number!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                else
                {
                    Age = "0";
                }
            }
            catch
            {
                _messageBoxService.Show("Age has to be an integer!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}
