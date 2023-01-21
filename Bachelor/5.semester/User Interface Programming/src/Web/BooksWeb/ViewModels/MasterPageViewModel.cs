using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.ViewModel.Validation;
using Microsoft.AspNetCore.Authentication.Cookies;
using BooksWeb.Resources;
using BooksWeb.DAL.Services;

namespace BooksWeb.ViewModels
{
    public class MasterPageViewModel : DotvvmViewModelBase
    { 
        public int? SignedInId { get; set; }
        public string SignedInUser { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool CredentialsError { get; set; }
        public bool FeedbackShowed { get; set; } = false;
        public string FeedbackText { get; set; }
        public string Title { get; set; } = "KDFit";
        public string ErrorModalMessage { get; set; }
        public bool IsErrorModalShowed { get; set; }
        public bool IsDeleteModalShowed { get; set; }
        public string HighlightedNavItem { get; set; }
        
        public override async Task Init()
        {
            if (!Context.IsPostBack)
            {
                await SetSignedUser();
            }
            await base.Init();
        }

        private async Task SetSignedUser()
        {
            try
            {
                var user = Context.GetAspNetCoreContext().User;
                var login = user.Claims.Single(x => x.Type == ClaimTypes.Name);
                SignedInUser = login.Value;
            }
            catch (Exception)
            {
                await SignOut();
            }
        }
        
        public async Task SignIn(ClaimsIdentity identity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
                {
                    this.AddModelError(x => x.CredentialsError, Errors.EmptyLoginOrPassword);
                    Context.FailOnInvalidModelState();
                }
                if (identity == null)
                {
                    this.AddModelError(x => x.CredentialsError, Errors.SignIn);
                    Context.FailOnInvalidModelState();
                }
                await Context.GetAuthentication().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                if (Context.Route.RouteName == Routes.Register) GoHome();
                Context.RedirectToRoute(Context.Route.RouteName);
            }
            catch (Exception e)
            {
                if (e is DotvvmInterruptRequestExecutionException)
                    throw;
                SetError(e, Errors.SignIn500);
            }
        }

        public async Task SignOut()
        {
            await Context.GetAuthentication().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            SignedInUser = null;
            SignedInId = null;
            FeedbackShowed = false;
            var route = Context.Route?.RouteName;
            if (route == Routes.Profile || route == Routes.Favourites_authors || route == Routes.Favourites_books)
            {
                GoHome();
            }
        }

        public void GoHome() => Context.RedirectToLocalUrl("/Home");

        public void SetError(Exception e, string message)
        {
            if (e is DotvvmInterruptRequestExecutionException)
                throw e;
            ErrorModalMessage = $"<p>{message}</p><p>Please, try the action again. If the problem persists, try later.</p>";
            IsErrorModalShowed = true;
        }

        public async Task GetSignedUserId(UsersService usersService)
        {
            try
            {
                var userList = await usersService.GetUsersLight(SignedInUser);
                SignedInId = userList.Single(x => x.Login == SignedInUser).Id;
            }
            catch (Exception e)
            {
                SetError(e, Errors.Internal500UserId);
            }
        }
    }
}
