using BooksWeb.DAL.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BooksWeb.Resources;
using DotVVM.Framework.ViewModel.Validation;
using System.ComponentModel.DataAnnotations;

namespace BooksWeb.ViewModels.Authentication
{
   public class RegisterViewModel : MasterPageViewModel
   {
        private readonly UsersService _usersService;
        private readonly ILogger<UsersService> _logger;

        [Required(ErrorMessage = "Zvolte své přihlašovací jméno.")]
        public string RegisterLogin { get; set; }
        [Required(ErrorMessage = "Zadejte své heslo.")]
        public string RegisterPassword { get; set; }
        [Required(ErrorMessage = "Zadejte znovu své heslo.")]
        public string RegisterPasswordConfirm { get; set; }
        public bool LoginExists { get; set; }
        public bool PasswordsDontMatch { get; set; }

        public RegisterViewModel(UsersService usersService, ILogger<UsersService> logger)
        {
            _usersService = usersService;
            _logger = logger;
        }

        public async Task Register()
        {
            try
            {
                if (RegisterPassword != RegisterPasswordConfirm)
                {
                    this.AddModelError(x => x.PasswordsDontMatch, Errors.PasswordConfirm);
                    Context.FailOnInvalidModelState();
                }
                var newUserId = await _usersService.Register(RegisterLogin, RegisterPassword);
                if (newUserId == null)
                {
                    this.AddModelError(x => x.LoginExists, Errors.LoginExists);
                    Context.FailOnInvalidModelState();
                }
                UserName = RegisterLogin;
                Password = RegisterPassword;
                var identity = await _usersService.SignIn(RegisterLogin, RegisterPassword);
                await SignIn(identity);
                GoHome();
            }
            catch (Exception e)
            {
                SetError(e, Errors.Registration);
                _logger.LogError(e, Errors.Registration);
            }
        }
   }
}
