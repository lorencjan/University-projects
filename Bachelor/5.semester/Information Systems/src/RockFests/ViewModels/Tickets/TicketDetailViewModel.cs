using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.ViewModel;
using Microsoft.Extensions.Logging;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.BL.Resources;
using RockFests.BL.Services;
using RockFests.DAL.Enums;
using RockFests.DAL.Types;

namespace RockFests.ViewModels.Tickets
{
    public class TicketDetailViewModel : MasterPageViewModel
    {
        private readonly TicketService _ticketService;
        private readonly UserService _userService;
        private readonly FestivalRepository _festivalRepository;
        private readonly ILogger<TicketDetailViewModel> _logger;

        public TicketDto Ticket { get; set; }
        public TicketDto EditTicket { get; set; }

        public List<KeyValue<int, string>> Festivals { get; set; }
        public List<KeyValue<int, string>> Users { get; set; }

        [FromRoute("Id")]
        public int TicketId { get; set; }

        public string ModalDeleteMessage => string.Format(Texts.ModalDeleteBodyFormat, Texts.ThisTicket);

        public TicketDetailViewModel(TicketService ticketService, UserService userService, FestivalRepository festivalRepository, ILogger<TicketDetailViewModel> logger)
        {
            _ticketService = ticketService;
            _userService = userService;
            _festivalRepository = festivalRepository;
            _logger = logger;
        }

        public override async Task Load()
        {
            if (SignedInUser == null)
                Context.RedirectToLocalUrl(Routes.Dashboard_Url);

            if(!Context.IsPostBack)
                await LoadTicket();

            await base.Load();
        }

        private async Task LoadTicket()
        {
            if (TicketId == 0)
            {
                if(SignedInUser.AccessRole == AccessRole.Spectator)
                    Context.RedirectToRoute(Routes.Tickets);

                EditTicket = new TicketDto();
                EditTicket.User.EnableValidation = false;
                Festivals = (await _festivalRepository.GetAllLight()).Select(x => new KeyValue<int, string>(x.Id, x.Name)).ToList();
                Users = (await _userService.GetAll()).Select(x => new KeyValue<int, string>(x.Id, x.Login)).ToList();
                return;
            }

            try
            {
                Ticket = await _ticketService.GetById(TicketId);
                if (Ticket == null)
                {
                    Context.RedirectToRoute(Routes.Tickets);
                }
            }
            catch (Exception e)
            {
                SetError(e, Errors.TicketLoad);
                _logger.LogError(e, Errors.TicketLoad);
            }
        }

        public async Task CreateTicket()
        {
            try
            {
                await _ticketService.Create(EditTicket);
                Context.RedirectToRoute(Routes.Tickets);
            }
            catch (Exception e)
            {
                SetError(e, Errors.TicketCreate);
                _logger.LogError(e, Errors.TicketCreate);
            }
        }

        public async Task UpdateTicket()
        {
            try
            {
                await _ticketService.Update(TicketId, EditTicket.IsPaid, EditTicket.PaymentDue, EditTicket.Count);
                Context.RedirectToRoute(Routes.Tickets);
            }
            catch (Exception e)
            {
                SetError(e, Errors.TicketUpdate);
                _logger.LogError(e, Errors.TicketUpdate);
            }
        }

        public async Task DeleteTicket()
        {
            try
            {
                await _ticketService.Delete(TicketId);
                Context.RedirectToRoute(Routes.Tickets);
            }
            catch (Exception e)
            {
                SetError(e, Errors.TicketDelete);
                _logger.LogError(e, Errors.TicketDelete);
            }
        }
    }
}
