using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Resources;
using RockFests.BL.Services;
using RockFests.DAL.Enums;
using RockFests.Model;

namespace RockFests.ViewModels.Users
{
    public class UsersViewModel : MasterPageViewModel
    {
        private readonly UserService _userService;
        private readonly ILogger<UsersViewModel> _logger;

        public FilterDataSet<UserDto> Users { get; set; } = new FilterDataSet<UserDto>();

        public UsersViewModel(UserService userService, ILogger<UsersViewModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public override Task Init()
        {
            Users.Set(Query);
            return base.Init();
        }

        public override Task Load()
        {
            if (SignedInUser?.AccessRole != AccessRole.Admin)
                Context.RedirectToLocalUrl(Routes.Dashboard_Url);

            return base.Load();
        }

        private async Task<IQueryable<UserDto>> Query()
        {
            try
            {
                var users = await _userService.GetAll(Users.Filter);
                return users.OrderBy(x => x.Id).AsQueryable();
            }
            catch (Exception e)
            {
                SetError(e, Errors.Users500);
                _logger.LogError(e, Errors.Users500);
                return new List<UserDto>().AsQueryable();
            }
        }
    }
}
