using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;
using RockFests.Model;

namespace RockFests.ViewModels.Musicians
{
    public class MusiciansViewModel : MasterPageViewModel
    {
        private readonly MusicianRepository _musicianRepository;
        private readonly ILogger<MusiciansViewModel> _logger;

        public FilterDataSet<MusicianLightDto> Musicians{ get; set; } = new FilterDataSet<MusicianLightDto>();

        public MusiciansViewModel(MusicianRepository musicianRepository, ILogger<MusiciansViewModel> logger)
        {
            _musicianRepository = musicianRepository;
            _logger = logger;
        }

        public override Task Init()
        {
            Musicians.Set(Query);
            return base.Init();
        }

        private async Task<IQueryable<MusicianLightDto>> Query()
        {
            try
            {
                var musicians = await _musicianRepository.GetAllLight(Musicians.Filter);
                return musicians.OrderBy(x => x.Name).ThenBy(x => x.Rating).AsQueryable();
            }
            catch (Exception e)
            {
                SetError(e, Errors.Musicians500);
                _logger.LogError(e, Errors.Musicians500);
                return new List<MusicianLightDto>().AsQueryable();
            }
        }
    }
}
