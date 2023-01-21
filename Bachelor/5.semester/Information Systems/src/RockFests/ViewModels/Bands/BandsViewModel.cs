using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;
using RockFests.Model;

namespace RockFests.ViewModels.Bands
{
    public class BandsViewModel : MasterPageViewModel
    {
        private readonly BandRepository _bandRepository;
        private readonly ILogger<BandsViewModel> _logger;

        public FilterDataSet<BandLightDto> Bands { get; set; } = new FilterDataSet<BandLightDto>();

        public BandsViewModel(BandRepository bandRepository, ILogger<BandsViewModel> logger)
        {
            _bandRepository = bandRepository;
            _logger = logger;
        }

        public override Task Init()
        {
            Bands.Set(Query);
            return base.Init();
        }

        private async Task<IQueryable<BandLightDto>> Query()
        {
            try
            {
                var bands = await _bandRepository.GetAllLight(Bands.Filter);
                return bands.OrderBy(x => x.Name).ThenBy(x => x.Rating).AsQueryable();
            }
            catch (Exception e)
            {
                SetError(e, Errors.Bands500);
                _logger.LogError(e, Errors.Bands500);
                return new List<BandLightDto>().AsQueryable();
            }
        }
    }
}
