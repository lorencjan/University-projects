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

namespace BooksWeb.ViewModels.Books
{
    public class BookDetailViewModel : MasterPageViewModel
    {

        private readonly DAL.Services.BooksService _booksService;
        private readonly UsersService _usersService;
        private readonly ILogger<BookDetailViewModel> _logger;

        [FromRoute("Id")]
        public int Id { get; set; }
        public Book BookDetail { get; set; }
        public List<Rating> Ratings { get; set; }
        public double AverageRating { get; set; }
        public Rating MyRating { get; set; }
        public Rating EditRating { get; set; } = new Rating();
        public List<int> RatingValues { get; set; } = new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public bool CreateRatingButtonEnabled { get; set; } = true;
        public bool Favourite { get; set; }

        public BookDetailViewModel(DAL.Services.BooksService booksService, UsersService usersService, ILogger<BookDetailViewModel> logger)
        {
            _booksService = booksService;
            _usersService = usersService;
            _logger = logger;
        }

        public override async Task Load()
        {
            if (Id == 0)
            {
                Context.RedirectToRoute(Routes.Books);
            }
            await base.Load();
            if (!Context.IsPostBack)
            {
                BookDetail = await _booksService.GetById(Id, BookIncludeFeatures.All);
                await LoadRatings();
                GetAverageRating();
            }
            if (SignedInUser != null)
            {
                if (!SignedInId.HasValue)
                {
                    await GetSignedUserId(_usersService);
                }

                var favourites = await _usersService.GetFavouriteBooks(SignedInId.Value);
                Favourite = favourites.Any(x => x.Id == Id);
            }
            else
            {
                Favourite = false;
            }
        }

        public async Task LoadRatings()
        {
            BookDetail = await _booksService.GetById(Id, BookIncludeFeatures.All);
            if (BookDetail.Ratings == null || BookDetail.Ratings.Count == 0)
            {
                return;
            }
            try
            {
                Ratings = new List<Rating>();
                foreach (var rating in BookDetail.Ratings)
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
                SetError(e, Errors.BookRating_load);
                _logger.LogError(e, Errors.BookRating_load);
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
                await _booksService.CreateRating(EditRating.Number, EditRating.Text, SignedInId.Value, Id);
                await LoadRatings();
                GetAverageRating();
            }
            catch (Exception e)
            {
                SetError(e, Errors.BookRating_create);
                _logger.LogError(e, Errors.BookRating_create);
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
                await _booksService.DeleteRating(MyRating.Id);
                MyRating = null;
                await LoadRatings();
                GetAverageRating();
            }
            catch (Exception e)
            {
                SetError(e, Errors.BookRating_delete);
                _logger.LogError(e, Errors.BookRating_delete);
            }
        }

        public void EditMyRating() => EditRating = MyRating;

        public void CancelEditRating() => EditRating = new Rating();

        public async Task UpdateRating()
        {
            try
            {
                await _booksService.UpdateRating(EditRating.Id, EditRating.Number, EditRating.Text);
                await LoadRatings();
                GetAverageRating();
            }
            catch (Exception e)
            {
                SetError(e, Errors.BookRating_update);
                _logger.LogError(e, Errors.BookRating_update);
            }
            finally
            {
                EditRating = new Rating();
            }
        }

        public async Task AddToFavourites()
        {
            if (SignedInUser == null)
            {
                Context.RedirectToRoute(Routes.Register);
            }
            try
            {
                if (!SignedInId.HasValue)
                {
                    await GetSignedUserId(_usersService);
                }
                await _usersService.AddFavouriteBook(SignedInId.Value, Id);
            }
            catch (Exception e)
            {
                SetError(e, Errors.Book_add_favourite);
                _logger.LogError(e, Errors.Book_add_favourite);
            }
        }

        public async Task RemoveFromFavourites()
        {
            try
            {
                if (!SignedInId.HasValue)
                {
                    await GetSignedUserId(_usersService);
                }
                await _usersService.RemoveFavouriteBook(SignedInId.Value, Id);
            }
            catch (Exception e)
            {
                SetError(e, Errors.Book_delete_favourite);
                _logger.LogError(e, Errors.Book_delete_favourite);
            }
        }
    }
}
