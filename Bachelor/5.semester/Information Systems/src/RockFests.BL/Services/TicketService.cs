using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RockFests.BL.Model;
using RockFests.DAL;
using RockFests.DAL.Attributes;
using RockFests.DAL.Entities;

namespace RockFests.BL.Services
{
    [RegisterService]
    public class TicketService
    {
        private static DateTime? _lastTicketDeletion;
        private readonly RockFestsDbContext _dbContext;

        public TicketService(RockFestsDbContext rockFestsDbContext)
        {
            _dbContext = rockFestsDbContext;

            //delete unpaid tickets if it's been more than an hour since last deletion
            if (_lastTicketDeletion == null || (DateTime.Now - _lastTicketDeletion).Value.Hours >= 1)
            {
                DeleteUnpaidTickets();
            }
        }

        private async Task<long> GenerateVariableSymbol()
        {
            var tickets = await _dbContext.Tickets.ToListAsync(); //cannot use MaxAsync due to No elements exceptions if there are no tickets
            var last = tickets.Count == 0 ? 0 : tickets.Max(x => x.VariableSymbol);
            return (last + 1) % 9_999_999_999; //variable symbol has 10 decimals - checking overflow
        }

        public void DeleteUnpaidTickets()
        {
            try
            {
                var ticketsToDelete = _dbContext.Tickets.Where(x => !x.IsPaid && x.PaymentDue < DateTime.Now).ToList();
                if (ticketsToDelete.Count > 0)
                {
                    _dbContext.Tickets.RemoveRange(ticketsToDelete);
                    _dbContext.SaveChanges();
                }
                _lastTicketDeletion = DateTime.Now;
            }
            catch {}
        }

        public async Task<int> CountAvailableTickets(int userId, int festivalId)
        {
            var festival = await _dbContext.Festivals
                .AsNoTracking()
                .Include(x => x.Tickets)
                .SingleAsync(x => x.Id == festivalId);

            if (festival.EndDate < DateTime.Today)
                return 0;

            var availableTickets = festival.MaxTickets - festival.Tickets.Sum(x => x.Count);
            if (availableTickets <= 0)
                return 0;

            var availableTicketsForUser = festival.MaxTicketsForUser - festival.Tickets.Where(x => x.UserId == userId).Sum(x => x.Count);
            if (availableTicketsForUser <= 0)
                return 0;

            return availableTickets > availableTicketsForUser ? availableTicketsForUser : availableTickets;
        }

        public async Task<int> ReserveTicket(int userId, int festivalId, int count)
        {
            var festival = await _dbContext.Festivals.SingleAsync(x => x.Id == festivalId);

            var ticket = await _dbContext.Tickets.AddAsync(new Ticket
            {
                VariableSymbol = await GenerateVariableSymbol(),
                PaymentDue = DateTime.Now.AddDays(festival.ReservationDays),
                IsPaid = false,
                FestivalId = festivalId,
                UserId = userId,
                Count = count
            });

            //check concurrency
            var availableTickets = await CountAvailableTickets(userId, festivalId);
            if (availableTickets < count)
                return 0;

            await _dbContext.SaveChangesAsync();
            return ticket.Entity.Id;
        }

        public async Task<int> Create(TicketDto ticket)
        {
            ticket.VariableSymbol = await GenerateVariableSymbol();
            var created = await _dbContext.AddAsync(Mapper.Map<Ticket>(ticket));
            await _dbContext.SaveChangesAsync();
            return created.Entity.Id;
        }

        public async Task Update(int id, bool? isPaid = null, DateTime? due = null, int? count = null)
        {
            var ticket = await _dbContext.Tickets.SingleAsync(x => x.Id == id);
            ticket.Count = count ?? ticket.Count;
            ticket.PaymentDue = due ?? ticket.PaymentDue;
            ticket.IsPaid = isPaid ?? ticket.IsPaid;
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var ticket = await _dbContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return;
            }

            _dbContext.Remove(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TicketDto> GetById(int id)
        {
            var ticket = await _dbContext.Tickets
                .Include(x => x.User)
                .Include(x => x.Festival)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
            return ticket == null ? null : Mapper.Map<TicketDto>(ticket);
        }

        public async Task<List<TicketDto>> GetAll(string filter = null)
            => await _dbContext.Tickets
                .Where(x => string.IsNullOrWhiteSpace(filter) || x.VariableSymbol.ToString().ToLower().Contains(filter.ToLower()) ||
                            x.Festival.Name.ToLower().Contains(filter.ToLower()) ||
                            x.User.FirstName.ToLower().Contains(filter.ToLower()) || x.User.LastName.ToLower().Contains(filter.ToLower()))
                .Include(x => x.User)
                .Include(x => x.Festival)
                .AsNoTracking()
                .ProjectTo<TicketDto>()
                .ToListAsync();
    }
}
