using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksWeb.DAL.Services;
using BooksWeb.Resources;
using DotVVM.Framework.ViewModel;
using BooksService.Client;
using BooksService.Common;
using Microsoft.Extensions.Logging;
using System.Linq;
using BooksWeb.Model;

namespace BooksWeb.ViewModels.Authors
{
    public class AuthorDetailViewModel : MasterPageViewModel
    {
        private readonly AuthorsService _authorsService;
        private readonly UsersService _usersService;
        private readonly ILogger<AuthorDetailViewModel> _logger;

        [FromRoute("Id")]
        public int Id { get; set; }
        public Author AuthorDetail { get; set; }
        public string BirthDate { get; set; }
        public List<Rating> Ratings { get; set; }
        public double AverageRating { get; set; }
        public Rating MyRating { get; set; }
        public Rating EditRating { get; set; } = new Rating();
        public List<int> RatingValues { get; set; } = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public bool CreateRatingButtonEnabled { get; set; } = true;
        public bool Favourite { get; set; }

        public AuthorDetailViewModel(AuthorsService authorsService, UsersService usersService, ILogger<AuthorDetailViewModel> logger)
        {
            _authorsService = authorsService;
            _usersService = usersService;
            _logger = logger;
        }

        public override async Task Load()
        {
            if (Id == 0)
            {
                Context.RedirectToRoute(Routes.Authors);
            }
            await base.Load();
            if (!Context.IsPostBack)
            {
                AuthorDetail = await _authorsService.GetById(Id, AuthorIncludeFeatures.Books);
                BirthDate = AuthorDetail.BirthDate.ToString("d");
                await LoadRatings();
                GetAverageRating();
            }
            if (SignedInUser != null)
            {
                if (!SignedInId.HasValue)
                {
                    await GetSignedUserId(_usersService);
                }
                var favourites = await _usersService.GetFavouriteAuthors(SignedInId.Value);
                Favourite = favourites.Any(x => x.Id == Id);
            }
            else
            {
                Favourite = false;
            }
        }

        public async Task LoadRatings()
        {
            AuthorDetail.Ratings = (await _authorsService.GetById(Id, AuthorIncludeFeatures.Ratings)).Ratings;
            if (AuthorDetail.Ratings == null || AuthorDetail.Ratings.Count == 0)
            {
                return;
            }
            try
            {
                Ratings = new List<Rating>();
                foreach (var rating in AuthorDetail.Ratings)
                {
                    var date = rating.Date.ToString("d");
                    var ratingModel = new Rating
                    {
                        Id = rating.Id,
                        UserName = rating.User.Login,
                        Number = rating.Number,
                        Date = date,
                        Text = rating.Text
                    };
                    if (ratingModel.UserName == SignedInUser)
                    {
                        MyRating = ratingModel;
                    }
                    else
                    {
                        Ratings.Add(ratingModel);
                    }
                }
            }
            catch (Exception e)
            {
                Ratings = null;
                SetError(e, Errors.AuthorRating_load);
                _logger.LogError(e, Errors.AuthorRating_load);
            }

        }

        public void GetAverageRating()
        {
            AverageRating = 0;
            if (MyRating != null)
            {
                AverageRating = MyRating.Number;
            }

            if (Ratings == null || Ratings.Count == 0)
            {
                return;
            }

            foreach (var rating in Ratings)
            {
                AverageRating += rating.Number;
            }

            if (MyRating != null)
                AverageRating /= (Ratings.Count + 1);
            else
                AverageRating /= Ratings.Count;
        }

        public async Task CreateRating()
        {
            try
            {
                if (!SignedInId.HasValue)
                {
                    await GetSignedUserId(_usersService);
                }
                await _authorsService.CreateRating(EditRating.Number, EditRating.Text, SignedInId.Value, Id);
                await LoadRatings();
                GetAverageRating();
            }
            catch (Exception e)
            {
                SetError(e, Errors.AuthorRating_create);
                _logger.LogError(e, Errors.AuthorRating_create);
            }
            finally
            {
                EditRating = new Rating();
                CreateRatingButtonEnabled = true;
            }
        }

        public async Task DeleteRating()
        {
            try
            {
                await _authorsService.DeleteRating(MyRating.Id);
                MyRating = null;
                await LoadRatings();
                GetAverageRating();
            }
            catch (Exception e)
            {
                SetError(e, Errors.AuthorRating_delete);
                _logger.LogError(e, Errors.AuthorRating_delete);
            }
        }

        public void EditMyRating() => EditRating = MyRating;

        public void CancelEditRating() => EditRating = new Rating();

        public async Task UpdateRating()
        {
            try
            {
                await _authorsService.UpdateRating(EditRating.Id, EditRating.Number, EditRating.Text);
                await LoadRatings();
                GetAverageRating();
            }
            catch (Exception e)
            {
                SetError(e, Errors.AuthorRating_update);
                _logger.LogError(e, Errors.AuthorRating_update);
            }
            finally
            {
                EditRating = new Rating();
            }
        }

        public async Task AddToFavourites()
        {
            if (SignedInUser == null) Context.RedirectToRoute(Routes.Register);
            try
            {
                if (SignedInId == null) await GetSignedUserId(_usersService);
                await _usersService.AddFavouriteAuthor((int)SignedInId, Id);
            }
            catch (Exception e)
            {
                SetError(e, Errors.Author_add_favourite);
                _logger.LogError(e, Errors.Author_add_favourite);
            }
        }

        public async Task RemoveFromFavourites()
        {
            try
            {
                if (SignedInId == null) await GetSignedUserId(_usersService);
                await _usersService.RemoveFavouriteAuthor((int)SignedInId, Id);
            }
            catch (Exception e)
            {
                SetError(e, Errors.Author_delete_favourite);
                _logger.LogError(e, Errors.Author_delete_favourite);
            }
        }
    }
}
