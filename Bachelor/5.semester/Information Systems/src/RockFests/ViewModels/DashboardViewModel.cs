using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;

namespace RockFests.ViewModels
{
    public class DashboardViewModel : MasterPageViewModel
    {
        private readonly FestivalRepository _festivalRepository;
        private readonly ILogger<DashboardViewModel> _logger;

        public FestivalDto UpcomingFestival { get; set; }
        public FestivalDto UsersClosestFestival { get; set; }

        public DashboardViewModel(FestivalRepository festivalRepository, ILogger<DashboardViewModel> logger)
        {
            _festivalRepository = festivalRepository;
            _logger = logger;
        }

        public override async Task Load()
        {
            try
            {
                UpcomingFestival = await _festivalRepository.GetClosestFestival() ?? new FestivalDto();
                if (SignedInUser != null)
                {
                    UsersClosestFestival = await _festivalRepository.GetClosestUsersFestival(SignedInUser.Id) ?? new FestivalDto();
                }
            }
            catch (Exception e)
            {
                SetError(e, Errors.DashboardLoad);
                _logger.LogError(e, Errors.DashboardLoad);
            }
            await base.Load();
        }
    }
}
