using BooksWeb.DAL.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BooksWeb.Resources;
using System.ComponentModel.DataAnnotations;
using DotVVM.Framework.ViewModel.Validation;

namespace BooksWeb.ViewModels.Authentication
{
    public class ProfileViewModel : MasterPageViewModel
    {
        private readonly UsersService _usersService;
        private readonly ILogger<UsersService> _logger;

        [Required(ErrorMessage = "Zadejte nové heslo.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Zadejte znovu nové heslo.")]
        public string NewPasswordConfirm { get; set; }
        public bool PasswordsDontMatch { get; set; }

        public ProfileViewModel(UsersService usersService, ILogger<UsersService> logger)
        {
            _usersService = usersService;
            _logger = logger;
        }

        public async Task ChangePassword()
        {
            try
            {
                if (NewPassword != NewPasswordConfirm)
                {
                    this.AddModelError(x => x.PasswordsDontMatch, Errors.PasswordConfirm);
                    Context.FailOnInvalidModelState();
                }

                if (!SignedInId.HasValue)
                {
                    await GetSignedUserId(_usersService);
                }
                await _usersService.ChangePassword(SignedInId.Value, NewPassword);
                GoHome();
            }
            catch (Exception e)
            {
                SetError(e, Errors.PasswordChange);
                _logger.LogError(e, Errors.PasswordChange);
            }
        }
    }
}

