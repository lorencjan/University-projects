using System;
using System.Threading.Tasks;
using System.Security.Claims;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel.Validation;
using RockFests.BL.Resources;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Services;
using RockFests.DAL.Enums;
using RockFests.Model;

namespace RockFests.ViewModels.Authentication
{
   public class RegisterViewModel : MasterPageViewModel
    {
        private readonly UserService _userService;
        private readonly ILogger<RegisterViewModel> _logger;

        public UserDto User { get; set; } = new UserDto { AccessRole = AccessRole.Spectator };
        public PasswordModel Password { get; set; } = new PasswordModel();
        public bool LoginExists { get; set; }

        public RegisterViewModel(UserService userService, ILogger<RegisterViewModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task Register()
        {
            try
            {
                var identity = await _userService.Register(User, Password.Password);
                if (identity != null)
                {
                    await Context.GetAuthentication().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    Context.RedirectToRoute(Routes.Dashboard);
                }
            }
            catch (Exception e)
            {
                SetError(e, Errors.Register500);
                _logger.LogError(e, Errors.Register500);
            }
            FailOnAlreadyExistingLogin();
        }

        private void FailOnAlreadyExistingLogin()
        {
            this.AddModelError(x => x.LoginExists, Errors.ExistingLogin);
            Context.FailOnInvalidModelState();
        }
    }
}
