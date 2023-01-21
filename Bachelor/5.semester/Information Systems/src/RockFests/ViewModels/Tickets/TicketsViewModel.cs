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

namespace RockFests.ViewModels.Tickets
{
    public class TicketsViewModel : MasterPageViewModel
    {
        private readonly TicketService _ticketService;
        private readonly ILogger<TicketsViewModel> _logger;

        public FilterDataSet<TicketDto> Tickets { get; set; } = new FilterDataSet<TicketDto>();
        public bool IncludePaid { get; set; } = true;

        public TicketsViewModel(TicketService ticketService, ILogger<TicketsViewModel> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        public override Task Init()
        {
            Tickets.Set(Query);
            return base.Init();
        }

        public override Task Load()
        {
            if (SignedInUser == null)
                Context.RedirectToLocalUrl(Routes.Dashboard_Url);

            return base.Load();
        }

        private async Task<IQueryable<TicketDto>> Query()
        {
            try
            {
                var tickets = await _ticketService.GetAll(Tickets.Filter);

                if (SignedInUser.AccessRole == AccessRole.Spectator)
                    tickets = tickets.Where(x => x.User.Id == SignedInUser.Id).ToList();
                if(!IncludePaid)
                    tickets = tickets.Where(x => !x.IsPaid).ToList();

                return tickets.OrderBy(x => x.IsPaid).ThenByDescending(x => x.PaymentDue).AsQueryable();
            }
            catch (Exception e)
            {
                SetError(e, Errors.Tickets500);
                _logger.LogError(e, Errors.Tickets500);
                return new List<TicketDto>().AsQueryable();
            }
        }

        public async Task ApproveTicket(int id)
        {
            try
            {
                await _ticketService.Update(id, true);
                Tickets.RequestRefresh();
            }
            catch (Exception e)
            {
                SetError(e, Errors.Tickets500);
                _logger.LogError(e, Errors.Tickets500);
            }
        }
    }
}
