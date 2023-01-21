using System;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.ViewModel.Validation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Resources;
using RockFests.BL.Services;
using RockFests.DAL.Enums;
using RockFests.Model;

namespace RockFests.ViewModels.Festivals
{
    public class UnregisteredFormViewModel : DotvvmViewModelBase
    {
        private readonly UserService _userService;
        private readonly TicketService _ticketService;
        private readonly ILogger<FestivalDetailViewModel> _logger;

        public UserDto User { get; set; } = new UserDto { Login = Texts.TrashValueToPassValidation, AccessRole = AccessRole.Spectator };
        public PasswordModel Password { get; set; } = new PasswordModel { DontValidate = true };
        public string Error { get; set; }
        public bool ShowForm { get; set; }
        public bool ShowRegisterForm { get; set; }

        public UnregisteredFormViewModel(UserService userService, TicketService ticketService, ILogger<FestivalDetailViewModel> logger)
        {
            _userService = userService;
            _ticketService = ticketService;
            _logger = logger;
        }

        public async Task ReserveTickets(int festivalId, int ticketsCount)
        {
            UserDto user = null;
            if (!ShowRegisterForm)
            {
                User.Login = null;
                user = await _userService.Create(User, null);
            }
            else
            {
                try
                {
                    user = await _userService.Create(User, Password.Password);
                    if (user == null)
                    {
                        SetError(new Exception("Login already exists"), Errors.ExistingLogin);
                    }
                    await Context.GetAuthentication().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(UserService.CreateIdentity(user)));
                }
                catch (Exception e)
                {
                    SetError(e, Errors.Register500);
                }
            }

            try
            {
                var id = await _ticketService.ReserveTicket(user.Id, festivalId, ticketsCount);

                if (id == 0)
                {
                    SetError(new DBConcurrencyException(), Errors.ReservationConcurrency);
                }

                if (ShowRegisterForm)
                {
                    Context.RedirectToLocalUrl(Routes.Tickets_Detail_Url + id);
                }
                else
                {
                    Context.RedirectToLocalUrl(Routes.Festivals_Url);
                }
            }
            catch (Exception e)
            {
                SetError(e, Errors.ReserveTicket);
            }
        }

        private void SetError(Exception e, string msg)
        {
            if (e is DotvvmInterruptRequestExecutionException)
                throw e;
            this.AddModelError(x => x.Error, msg);
            Context.FailOnInvalidModelState();
            _logger.LogError(e, Errors.ReserveTicket);
        }
    }
}
