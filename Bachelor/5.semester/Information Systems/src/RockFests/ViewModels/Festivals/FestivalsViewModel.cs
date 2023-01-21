using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;
using RockFests.Model;

namespace RockFests.ViewModels.Festivals
{
    public class FestivalsViewModel : MasterPageViewModel
    {
        private readonly FestivalRepository _festivalRepository;
        private readonly ILogger<FestivalsViewModel> _logger;

        public FilterDataSet<FestivalLightDto> Festivals { get; set; } = new FilterDataSet<FestivalLightDto>();
        public bool IncludePast { get; set; }

        public FestivalsViewModel(FestivalRepository festivalRepository, ILogger<FestivalsViewModel> logger)
        {
            _festivalRepository = festivalRepository;
            _logger = logger;
        }

        public override Task Init()
        {
            Festivals.Set(Query);
            return base.Init();
        }

        private async Task<IQueryable<FestivalLightDto>> Query()
        {
            try
            {
                var festivals = await _festivalRepository.GetAllLight(Festivals.Filter, IncludePast);
                return festivals.AsQueryable();
            }
            catch (Exception e)
            {
                SetError(e, Errors.Festivals500);
                _logger.LogError(e, Errors.Festivals500);
                return new List<FestivalLightDto>().AsQueryable();
            }
        }
    }
}
