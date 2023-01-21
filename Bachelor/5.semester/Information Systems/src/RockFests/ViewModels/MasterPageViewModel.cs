using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RockFests.BL.Resources;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using RockFests.BL.Model;

namespace RockFests.ViewModels
{
    public class MasterPageViewModel : DotvvmViewModelBase
    {
        [Protect(ProtectMode.SignData)]
        public UserDto SignedInUser { get; set; }
        public DateTime SignInTimeOut { get; set; }
        public int RemainingMinutes => (SignInTimeOut - DateTime.Now).Minutes + 1;
        public string Title { get; set; } = Texts.Title;
        public string ErrorModalMessage { get; set; }
        public bool IsErrorModalShowed { get; set; }
        public bool IsDeleteModalShowed { get; set; }

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
                var data = user.Claims.Single(x => x.Type == ClaimTypes.UserData);
                var expiration = user.Claims.Single(x => x.Type == ClaimTypes.Expiration);
                SignedInUser = JsonConvert.DeserializeObject<UserDto>(data.Value);
                SignedInUser.EnableValidation = false;
                SignInTimeOut = DateTime.Parse(expiration.Value);

                if (SignInTimeOut < DateTime.Now)
                    await SignOut();
            }
            catch (Exception)
            {
                await SignOut();
            }
        }

        public async Task SignOut()
        {
            await Context.GetAuthentication().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            SignedInUser = null;
        }

        public void GoHome() => Context.RedirectToLocalUrl(Routes.Dashboard_Url);
       
        public void SetError(Exception e, string message)
        {
            if (e is DotvvmInterruptRequestExecutionException)
                throw e;
            ErrorModalMessage = $"<p>{message}</p><p>{Errors.Modal500BodyExtension}</p>";
            IsErrorModalShowed = true;
        }
    }
}
