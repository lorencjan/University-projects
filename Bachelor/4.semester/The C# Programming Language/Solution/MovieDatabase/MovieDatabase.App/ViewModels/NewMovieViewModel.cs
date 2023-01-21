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
using MovieDatabase.DAL.Enums;
using MovieDatabase.App.Resources.Img;

namespace MovieDatabase.App.ViewModels
{
    public class NewMovieViewModel : ViewModelBase
    {
        #region Attributes
        private readonly MovieRepository _movieRepository;
        private readonly PersonRepository _personRepository;
        private Visibility _newVisible;
        private string _originalName, _czechName, _description, _country, _duration, _year, _fileName;
        private ObservableCollection<PersonListDto> _availablePeople, _addedActors, _addedDirectors;
        private PersonListDto _chosenActor, _chosenDirector;
        private List<RatingDto> _ratings;
        private Guid _id;
        private Genre _genre;

        public byte[] NewIcon{ get; set; }
        public byte[] AddImage{ get; set; }
        public byte[] ResetIcon{ get; set; }
        public byte[] SaveIcon{ get; set; }

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        public string OriginalName 
        {
            get { return _originalName; }
            set { SetProperty(ref _originalName, value); }
        }
        public string CzechName
        {
            get { return _czechName; }
            set { SetProperty(ref _czechName, value); }
        }
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        public string Country
        {
            get { return _country; }
            set { SetProperty(ref _country, value); }
        }
        public string Duration       
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }
        public string Year
        {
            get { return _year; }
            set { SetProperty(ref _year, value); }
        }
        public ObservableCollection<PersonListDto> AvailablePeople
        {
            get { return _availablePeople; }
            set { SetProperty(ref _availablePeople, value); }
        }
        public ObservableCollection<PersonListDto> AddedActors
        {
            get { return _addedActors; }
            set { SetProperty(ref _addedActors, value); }
        }
        public PersonListDto ChosenActor        
        {
            get { return _chosenActor; }
            set { SetProperty(ref _chosenActor, value); }
        }
        public ObservableCollection<PersonListDto> AddedDirectors
        {
            get { return _addedDirectors; }
            set { SetProperty(ref _addedDirectors, value); }
        }
        public PersonListDto ChosenDirector
        {
            get { return _chosenDirector; }
            set { SetProperty(ref _chosenDirector, value); }
        }
        public Genre MovieGenre
        {
            get { return _genre; }
            set { SetProperty(ref _genre, value); }
        }
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }
        public byte[] Photo { get; set; }
        public Visibility NewVisible
        {
            get { return _newVisible; }
            set { SetProperty(ref _newVisible, value); }
        }

        public ICommand LoadPhotoCommand { get; set; }
        public ICommand AddActorCommand { get; set; }
        public ICommand ResetActorsCommand { get; set; }
        public ICommand AddDirectorCommand { get; set; }
        public ICommand ResetDirectorsCommand { get; set; }
        #endregion

        public NewMovieViewModel(MovieRepository movieRepository, PersonRepository personRepository)
        {
            _movieRepository = movieRepository;
            _personRepository = personRepository;
            NewIcon = Images.New;
            ResetIcon = Images.Reset;
            AddImage = Images.AddImage;
            SaveIcon = Images.Save;
            LoadPhotoCommand = new RelayCommand(LoadPhoto);
            AddActorCommand = new RelayCommand(AddActor);
            ResetActorsCommand = new RelayCommand(ResetActors);
            AddDirectorCommand = new RelayCommand(AddDirector);
            ResetDirectorsCommand = new RelayCommand(ResetDirectors);
        }

        public void InitializeValues(MovieDto m, string filename = "")
        {
            Id = m.Id;
            OriginalName = m.OriginalName;
            CzechName = m.CzechName;
            Description= m.Description;
            MovieGenre = m.Genre;
            Country = m.Country;
            Duration = m.Duration.ToString();
            Year = m.Year.ToString();
            FileName = filename;
            Photo = m.TitlePhoto;
            AddedActors = m.Actors == null ? new ObservableCollection<PersonListDto>() : new ObservableCollection<PersonListDto>(m.Actors);
            AddedDirectors = m.Directors == null ? new ObservableCollection<PersonListDto>() : new ObservableCollection<PersonListDto>(m.Directors);
            ChosenActor = null;
            ChosenDirector = null;
            AvailablePeople = new ObservableCollection<PersonListDto>(_personRepository.GetAllAsList());
            _ratings = m.Ratings;
        }
        
        private void AddActor()
        {
            if(ChosenActor != null && !AddedActors.Contains(ChosenActor))
            {
                AddedActors.Add(ChosenActor);
            }
        }

        private void ResetActors()
        {
            AddedActors.Clear();
        }

        private void AddDirector()
        {
            if(ChosenDirector != null && !AddedDirectors.Contains(ChosenDirector))
            {
                AddedDirectors.Add(ChosenDirector);
            }
        }

        private void ResetDirectors()
        {
            AddedDirectors.Clear();
        }

		private void LoadPhoto()
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
            if (!ValidateName() || !ValidateDuration() || !ValidateYear())
                return false;

            MovieDto newMovie = new MovieDto()
            {
                Id = Id,
                OriginalName = OriginalName,
                CzechName = CzechName,
                Description = Description,
                Country = Country,
                Genre = MovieGenre,
                Duration = int.Parse(Duration),
                Year = int.Parse(Year),
                TitlePhoto = Photo,
                Actors = new List<PersonListDto>(AddedActors),
                Directors = new List<PersonListDto>(AddedDirectors),
                Ratings = _ratings
            };
            
            try
            {
                _movieRepository.InsertOrUpdate(newMovie);
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
            if (string.IsNullOrEmpty(OriginalName))
            {
                _messageBoxService.Show("Name of the movie is required!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private bool ValidateDuration()
        {
            try
            {
                if (!string.IsNullOrEmpty(Duration))
                {
                    var parsedDuration = int.Parse(Duration);
                    if (parsedDuration < 0)
                    {
                        _messageBoxService.Show("Duration cannot be a negative number!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                else
                {
                    Duration = "0";
                }
            }
            catch
            {
                _messageBoxService.Show("Duration has to be an integer!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private bool ValidateYear()
        {
            try
            {
                if (!string.IsNullOrEmpty(Year))
                {
                    var parsedYear = int.Parse(Year);
                    if (parsedYear < 0)
                    {
                        _messageBoxService.Show("Year cannot be a negative number!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                else
                {
                    Year = "0";
                }
            }
            catch
            {
                _messageBoxService.Show("Year has to be an integer!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}
