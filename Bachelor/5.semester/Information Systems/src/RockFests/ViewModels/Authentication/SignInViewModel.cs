using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using RockFests.BL.Resources;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel.Validation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using RockFests.BL.Services;

namespace RockFests.ViewModels.Authentication
{
    public class SignInViewModel : MasterPageViewModel
    {
        private readonly UserService _userService;
        private readonly ILogger<SignInViewModel> _logger;

        public SignInViewModel(UserService userService, ILogger<SignInViewModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = "Required")]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        public bool CredentialsError { get; set; }

        public async Task SignIn()
        {
            try
            {
                var identity = await _userService.SignIn(UserName, Password);
                if (identity == null)
                {
                    this.AddModelError(x => x.CredentialsError, Errors.SignIn);
                    Context.FailOnInvalidModelState();
                }
                await Context.GetAuthentication().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                Context.RedirectToRoute(Routes.Dashboard);
            }
            catch (Exception e)
            {
                if (e is DotvvmInterruptRequestExecutionException)
                    throw;
                SetError(e, Errors.SignIn500);
                _logger.LogError(e, Errors.SignIn500);
            }
        }
    }
}