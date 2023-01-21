using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Resources;
using RockFests.BL.Services;
using RockFests.DAL.Enums;
using RockFests.Model;

namespace RockFests.ViewModels.Users
{
    public class UserDetailViewModel : MasterPageViewModel
    {
        private readonly UserService _userService;
        private readonly ILogger<UserDetailViewModel> _logger;

        public UserDto User { get; set; } = new UserDto {AccessRole = AccessRole.Spectator};
        public PasswordModel Password { get; set; } = new PasswordModel();

        [FromRoute("Id")]
        public int UserId { get; set; }

        [Bind(Direction.ServerToClientFirstRequest)]
        public List<AccessRole> AccessRoles { get; set; } = ((AccessRole[])Enum.GetValues(typeof(AccessRole))).ToList();

        public string ModalDeleteMessage => string.Format(Texts.ModalDeleteBodyFormat, User?.Login);

        public UserDetailViewModel(UserService userService, ILogger<UserDetailViewModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public override async Task Load()
        {
            if (SignedInUser?.AccessRole != AccessRole.Admin)
                Context.RedirectToLocalUrl(Routes.Dashboard_Url);
            
            if(!Context.IsPostBack)
                await LoadUser();
            
            await base.Load();
        }

        private async Task LoadUser()
        {
            if (UserId == 0)
                return;

            try
            {
                User = await _userService.GetById(UserId);
                if (User == null)
                {
                    Context.RedirectToRoute(Routes.Users);
                }
            }
            catch (Exception e)
            {
                SetError(e, Errors.UserLoad);
                _logger.LogError(e, Errors.UserLoad);
            }
        }

        public async Task CreateUser()
        {
            try
            {
                await _userService.Create(User, Password.Password);
                Context.RedirectToRoute(Routes.Users);
            }
            catch (Exception e)
            {
                SetError(e, Errors.UserCreate);
                _logger.LogError(e, Errors.UserCreate);
            }
        }

        public async Task UpdateUser()
        {
            try
            {
                await _userService.Update(User);
                Context.RedirectToRoute(Routes.Users);
            }
            catch (Exception e)
            {
                SetError(e, Errors.UserUpdate);
                _logger.LogError(e, Errors.UserUpdate);
            }
        }

        public async Task UpdateUsersPassword()
        {
            try
            {
                await _userService.UpdatePassword(UserId, Password.Password);
                Context.RedirectToRoute(Routes.Users);
            }
            catch (Exception e)
            {
                SetError(e, Errors.UserUpdatePassword);
                _logger.LogError(e, Errors.UserUpdatePassword);
            }
        }

        public async Task DeleteUser()
        {
            try
            {
                await _userService.Delete(UserId);
                Context.RedirectToRoute(Routes.Users);
            }
            catch (Exception e)
            {
                SetError(e, Errors.UserDelete);
                _logger.LogError(e, Errors.UserDelete);
            }
        }
    }
}
