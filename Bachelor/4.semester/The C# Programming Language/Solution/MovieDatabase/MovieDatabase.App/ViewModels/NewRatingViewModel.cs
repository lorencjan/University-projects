using System;
using System.Windows;
using System.Collections.Generic;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Repositories;
using MovieDatabase.App.Resources.Img;

namespace MovieDatabase.App.ViewModels
{
    public class NewRatingViewModel : ViewModelBase
    {
        #region Attributes
        private readonly RatingRepository _ratingRepository;
        private readonly MovieRepository _movieRepository;
        public byte[] SaveIcon {get; set;}
        private string _number, _text;
        private MovieListDto _movie;
        public List<MovieListDto> Movies {get; set;}
        public MovieListDto Movie
        {
            get { return _movie; }
            set { SetProperty(ref _movie, value); }
        }
        public string Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        #endregion

        public NewRatingViewModel(RatingRepository ratingRepository, MovieRepository movieRepository)
        {
            _ratingRepository = ratingRepository;
            _movieRepository = movieRepository;
            SaveIcon = Images.Save;
            InitializeValues();
        }

        public void InitializeValues(MovieListDto movie = null, string number = "", string text = "")
        {
            Movie = movie;
            Number = number;
            Text = text;
            Movies = _movieRepository.GetAllAsList();
        }

        public bool Save()
        {
            if (!ValidateNumber())
                return false;

            RatingDto newRating = new RatingDto()
            {
                MovieId = Movie.Id,
                MovieName = Movie.DisplayName,
                Number = short.Parse(Number),
                Text = Text
            };
            
            try
            {
                _ratingRepository.InsertOrUpdate(newRating);
            }
            catch
            {
                _messageBoxService.Show("Failed to update the database!", "Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool ValidateNumber()
        {
            try
            {
                short ratingVal = short.Parse(Number);
                if (ratingVal < 0 || ratingVal > 10)
                {
                    throw new Exception();
                }
            }
            catch
            {
                _messageBoxService.Show("Rating numeric value has to be an integer in range from 0 to 10!", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}
