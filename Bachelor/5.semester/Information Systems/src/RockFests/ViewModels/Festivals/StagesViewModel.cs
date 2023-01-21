using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;
using RockFests.DAL.Types;
using RockFests.Model;

namespace RockFests.ViewModels.Festivals
{
    public class StagesViewModel : MasterPageViewModel
    {
        private readonly BandRepository _bandRepository;
        private readonly MusicianRepository _musicianRepository;
        private readonly StageRepository _stageRepository;
        private readonly ILogger<StagesViewModel> _logger;

        public List<StageModel> Stages { get; set; } = new List<StageModel>();
        public List<InterpretDto> Bands { get; set; }
        public List<InterpretDto> Musicians { get; set; }
        
        public StagesViewModel(BandRepository bandRepository, MusicianRepository musicianRepository, StageRepository stageRepository, ILogger<StagesViewModel> logger)
        {
            _bandRepository = bandRepository;
            _musicianRepository = musicianRepository;
            _stageRepository = stageRepository;
            _logger = logger;
        }

        public override async Task Load()
        {
            ClearErrors();
            if (!Context.IsPostBack)
            {
                Bands = (await _bandRepository.GetAllLight()).Select(x => new InterpretDto { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
                Musicians = (await _musicianRepository.GetAllLight()).Select(x => new InterpretDto { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
            }
            await base.Load();
        }

        [AllowStaticCommand]
        public List<StageModel> CreatePerformance(StageDto stage)
        {
            Stages.Single(x => x.Stage.Id == stage.Id).EditStage.Performances.Add(new PerformanceDto
            {
                Time = new DateTimeInterval(), 
                Interpret = new InterpretDto(),
                IsBand = true
            });
            return Stages;
        }

        [AllowStaticCommand]
        public List<StageModel> DeletePerformance(int stageId, PerformanceDto p)
        {
            var performances = Stages.Single(x => x.Stage.Id == stageId).EditStage.Performances;
            if (p.Id > 0)
            {
                performances.RemoveAll(x => x.Id == p.Id);
                return Stages;
            }
            //there can be several new but not filled
            var found = performances.FindAll(x => x.Id == p.Id && x.Time.Start == p.Time.Start && x.Time.End == p.Time.End && x.IsBand == p.IsBand && x.Interpret.Id == p.Interpret.Id);
            if (found.Count > 0)
                performances.Remove(found[0]);
            return Stages;
        }

        public async Task SaveStage(StageDto stage)
        {
            try
            {
                if (stage.Id > 0)
                {
                    if (!Validate(stage))
                        return;
                    await _stageRepository.Update(stage);
                }
                else
                {
                    stage.Id = await _stageRepository.Add(stage);
                }

                CopyEditBack(stage);
            }
            catch (Exception e)
            {
                SetError(e, Errors.StageSave);
                _logger.LogError(e, Errors.StageDelete);
            }
        }

        private void CopyEditBack(StageDto editStage)
        {
            //combobox binds only Ids, not displayed values -> need to map it
            foreach (var p in editStage.Performances)
            {
                p.Interpret.Name = p.IsBand
                    ? Bands.Single(x => x.Id == p.Interpret.Id).Name
                    : Musicians.Single(x => x.Id == p.Interpret.Id).Name;
            }

            var stageModel = Stages.SingleOrDefault(x => x.Stage.Id == editStage.Id);
            if (stageModel == null) //created
            {
                Stages.Add(new StageModel { Stage = editStage, EditStage = editStage});
                return;
            }
            stageModel.Stage = editStage;
            stageModel.EditStage = null;
        }

        public async Task DeleteStage(int id)
        {
            try
            {
                await _stageRepository.Delete(id);
                Stages.RemoveAll(x => x.Stage.Id == id);
            }
            catch (Exception e)
            {
                SetError(e, Errors.StageDelete);
                _logger.LogError(e, Errors.StageDelete);
            }
        }

        private bool Validate(StageDto stage)
        {
            var model = Stages.Single(x => x.Stage.Id == stage.Id);
            if (string.IsNullOrEmpty(stage.Name))
            {
                model.Error = Errors.RequiredName;
                return false;
            }
            if (Stages.Any(x => x.Stage.Id != stage.Id && x.Stage.Name.Equals(stage.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                model.Error = Errors.StageExists;
                return false;
            }
            foreach (var p in stage.Performances)
            {
                if (p.Time.Start >= p.Time.End)
                    model.Error = Errors.InvalidDateInterval;
                else if (stage.Performances.Any(x => x != p && x.Time.OverlapsWith(p.Time)))
                    model.Error = Errors.PerformanceOverlap;
                else if (p.Interpret.Id == 0)
                    model.Error = Errors.RequiredInterpret;

                if (model.Error != null)
                    return false;
            }
            return true;
        }

        [AllowStaticCommand]
        public List<StageModel> CopyToEdit(StageDto stage)
        {
            Stages.Single(x => x.Stage.Id == stage.Id).EditStage = stage.Copy();
            ClearErrors();
            return Stages;
        }

        private void ClearErrors() => Stages.ForEach(x => x.Error = null);
    }
}
