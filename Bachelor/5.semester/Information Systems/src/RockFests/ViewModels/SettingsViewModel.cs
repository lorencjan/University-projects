using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RockFests.BL.Model;
using RockFests.BL.Services;
using RockFests.Model;
using RockFests.BL.Resources;

namespace RockFests.ViewModels
{
   public class SettingsViewModel : MasterPageViewModel
    {
        private readonly UserService _userService;
        private readonly ILogger<SettingsViewModel> _logger;

        public UserDto User { get; set; }
        public PasswordModel Password { get; set; } = new PasswordModel();

        public SettingsViewModel(UserService userService, ILogger<SettingsViewModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public override Task Load()
        {
            if (!Context.IsPostBack)
            {
                User = new UserDto(SignedInUser);
            }
            return base.Load();
        }

        public async Task SaveInformation()
        {
            try
            {
                await _userService.Update(User);

                SignedInUser = new UserDto(User);
                var currentPrincipal = Context.GetAspNetCoreContext().User;
                var identity = currentPrincipal.Identity as ClaimsIdentity;
                identity.RemoveClaim(currentPrincipal.Claims.Single(x => x.Type == ClaimTypes.UserData));
                identity.AddClaim(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(SignedInUser)));
                await Context.GetAuthentication().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                Context.RedirectToRoute(Routes.Dashboard);
            }
            catch (Exception e)
            {
                SetError(e, Errors.UserSettings500);
                _logger.LogError(e, Errors.UserSettings500);
            }
        }

        public async Task ChangePassword()
        {
            try
            {
                await _userService.UpdatePassword(SignedInUser.Id, Password.Password);
                Context.RedirectToRoute(Routes.Dashboard);
            }
            catch (Exception e)
            {
                SetError(e, Errors.UserSettings500);
                _logger.LogError(e, Errors.UserSettings500);
            }
        }
    }
}
