using System;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;
using RockFests.BL.Services;
using RockFests.DAL.Enums;
using RockFests.Model;

namespace RockFests.ViewModels.Festivals
{
    public class FestivalDetailViewModel : MasterPageViewModel
    {
        private readonly FestivalRepository _festivalRepository;
        private readonly TicketService _ticketService;
        private readonly ILogger<FestivalDetailViewModel> _logger;

        public FestivalDto Festival { get; set; } = new FestivalDto();
        public FestivalDto EditFestival { get; set; }

        public StagesViewModel StagesViewModel { get; set; }
        public UnregisteredFormViewModel UnregisteredFormViewModel { get; set; }

        public int AvailableTickets { get; set; }
        public int TicketsCount { get; set; } = 1;

        [FromRoute("Id")]
        public int FestivalId { get; set; }

        public string ModalDeleteMessage => string.Format(Texts.ModalDeleteBodyFormat, Festival?.Name);

        public FestivalDetailViewModel(FestivalRepository festivalRepository, TicketService ticketService, ILogger<FestivalDetailViewModel> logger, StagesViewModel stagesViewModel, UnregisteredFormViewModel unregisteredFormViewModel)
        {
            _festivalRepository = festivalRepository;
            _ticketService = ticketService;
            _logger = logger;
            StagesViewModel = stagesViewModel;
            UnregisteredFormViewModel = unregisteredFormViewModel;
        }

        public override async Task Load()
        {
            if (!Context.IsPostBack)
            {
                await LoadFestival();
            }
            await base.Load();
        }

        private async Task LoadFestival()
        {
            if (FestivalId == 0)
            {
                if(SignedInUser?.AccessRole != AccessRole.Admin && SignedInUser?.AccessRole != AccessRole.Organizer)
                    Context.RedirectToRoute(Routes.Festivals);

                EditFestival = new FestivalDto();
                return;
            }

            try
            {
                Festival = await _festivalRepository.GetById(FestivalId);
                if (Festival == null)
                {
                    Context.RedirectToRoute(Routes.Festivals);
                }
                StagesViewModel.Stages = Festival.Stages.Select(x => new StageModel{Stage = x}).ToList();
                await CountAvailableTickets();
            }
            catch (Exception e)
            {
                SetError(e, Errors.FestivalLoad);
                _logger.LogError(e, Errors.FestivalLoad);
            }
        }

        public async Task SaveFestival()
        {
            try
            {
                if (EditFestival.Id > 0)
                {
                    await _festivalRepository.Update(EditFestival);
                    await CountAvailableTickets();
                }
                else
                {
                    EditFestival.MaxTicketsForUser ??= EditFestival.MaxTickets;
                    EditFestival.Id = await _festivalRepository.Add(EditFestival);
                    Context.RedirectToLocalUrl(Routes.Festivals_Detail_Url + EditFestival.Id);
                }

                Festival = EditFestival;
                EditFestival = null;
            }
            catch (Exception e)
            {
                SetError(e, Errors.FestivalSave);
                _logger.LogError(e, Errors.FestivalSave);
            }
        }

        public async Task DeleteFestival()
        {
            try
            {
                await _festivalRepository.Delete(FestivalId);
                Context.RedirectToRoute(Routes.Festivals);
            }
            catch (Exception e)
            {
                SetError(e, Errors.FestivalDelete);
                _logger.LogError(e, Errors.FestivalDelete);
            }
        }

        private async Task CountAvailableTickets()
        {
            try
            {
                AvailableTickets = await _ticketService.CountAvailableTickets(SignedInUser?.Id ?? 0, FestivalId);
            }
            catch (Exception e)
            {
                SetError(e, Errors.CountTickets);
                _logger.LogError(e, Errors.CountTickets);
            }
        }

        public async Task ReserveTickets()
        {
            try
            {
                var id = await _ticketService.ReserveTicket(SignedInUser.Id, FestivalId, TicketsCount);
                if (id == 0)
                {
                    SetError(new Exception(), Errors.ReservationConcurrency);
                    return;
                }
                Context.RedirectToLocalUrl(Routes.Tickets_Detail_Url + id);
            }
            catch (Exception e)
            {
                SetError(e, Errors.ReserveTicket);
                _logger.LogError(e, Errors.ReserveTicket);
            }
        }
        
        public async Task CreateStage()
        {
            try
            {
                var stage = new StageDto {Name = Texts.Stage, FestivalId = FestivalId};
                await StagesViewModel.SaveStage(stage);
            }
            catch (Exception e)
            {
                SetError(e, Errors.StageCreate);
                _logger.LogError(e, Errors.StageCreate);
            }
        }
    }
}
