using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.BL.Services;
using RockFests.DAL.Entities;
using RockFests.DAL.Types;

namespace RockFests.Specification.ServiceTests
{
    public class TicketServiceTests : TestFixture
    {
        private TicketService _ticketService;

        private TicketDto TicketDto() => new TicketDto
        {
            Id = 1,
            Festival = new FestivalLightDto
            {
                Id = 1, 
                Name = "Fest", 
                Date = new DateTimeInterval(new DateTime(), new DateTime())
            },
            User = new UserDto {Id = 1, Login = "Test"}
        };

        [SetUp]
        public void TicketServiceSetup() => _ticketService = ServiceProvider.GetRequiredService<TicketService>();

        [Test]
        public async Task Successfully_delete_unpaid_tickets()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival { Name = "Fest" });
            await dbContext.Users.AddAsync(new User { Login = "Test" });
            await dbContext.Tickets.AddAsync(new Ticket { FestivalId = 1, UserId = 1, PaymentDue = DateTime.Today.AddDays(-1) });
            await dbContext.Tickets.AddAsync(new Ticket { FestivalId = 1, UserId = 1, PaymentDue = DateTime.Today.AddDays(1) });
            await dbContext.Tickets.AddAsync(new Ticket { FestivalId = 1, UserId = 1, PaymentDue = DateTime.Today.AddDays(-2) });
            await dbContext.SaveChangesAsync();

            _ticketService.DeleteUnpaidTickets();

            var tickets = await dbContext.Tickets.ToListAsync();
            tickets.Should().HaveCount(1);
            tickets.Single().Id.Should().Be(2);
        }

        [TestCase(5, 2, 1, 1)]
        [TestCase(5, 7, 1, 4)]
        [TestCase(2, 1, 1, 0)]
        [TestCase(1, 2, 1, 0)]
        [TestCase(5, 4, 3, 1)]
        [TestCase(5, 4, 4, 0)]
        public async Task Checks_if_user_can_buy_another_ticket(int maxTickets, int maxTicketsForUser, int ticketForPeople, int result)
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival { Name = "Fest", MaxTickets = maxTickets, MaxTicketsForUser = maxTicketsForUser });
            await dbContext.Users.AddAsync(new User { Login = "Test" });
            await dbContext.Tickets.AddAsync(new Ticket { FestivalId = 1, UserId = 1, Count = ticketForPeople });
            await dbContext.SaveChangesAsync();

            (await _ticketService.CountAvailableTickets(1,1)).Should().Be(result);
        }

        [TestCase(11, 2)]
        [TestCase(9_999_999_999, 1)]
        public async Task Successfully_reserves_ticket(long lastVariableSymbol, int count)
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival { Name = "Fest", ReservationDays = 7, MaxTickets = 7, MaxTicketsForUser = 7 });
            await dbContext.Tickets.AddAsync(new Ticket { VariableSymbol = lastVariableSymbol });
            await dbContext.Users.AddAsync(new User { Login = "Test" });
            await dbContext.SaveChangesAsync();

            await _ticketService.ReserveTicket(1, 1, count);

            var tickets = await GetContext().Tickets.ToListAsync();
            tickets.Should().HaveCount(2);
            var ticket = tickets[1];
            ticket.PaymentDue.ToShortDateString().Should().Be(DateTime.Now.AddDays(7).ToShortDateString());
            ticket.PaymentDue = new DateTime();
            ticket.Should().BeEquivalentTo(new Ticket
            {
                Id = 2,
                VariableSymbol = (lastVariableSymbol + 1) % 9_999_999_999,
                PaymentDue = new DateTime(),
                IsPaid = false,
                FestivalId = 1,
                UserId = 1,
                Count = count
            });
        }

        [Test]
        public async Task Successfully_create_ticket()
        {
            var ticketDto = new TicketDto {Festival = new FestivalLightDto{Id = 1}, User = new UserDto{Id = 1}, Count = 1};
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival { Name = "Fest" });
            await dbContext.Users.AddAsync(new User { Login = "Test" });
            await dbContext.SaveChangesAsync();

            ticketDto.Id = await _ticketService.Create(ticketDto);

            var ticket = await GetContext().Tickets.SingleAsync();
            ticket.VariableSymbol = 1;
            ticket.Should().BeEquivalentTo(Mapper.Map<Ticket>(ticketDto));
        }

        [Test]
        public async Task Successfully_update_ticket()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival { Name = "Fest" });
            await dbContext.Users.AddAsync(new User { Login = "Test" });
            await dbContext.Tickets.AddAsync(new Ticket { FestivalId = 1, UserId = 1 });
            await dbContext.SaveChangesAsync();

            (await GetContext().Tickets.SingleAsync()).IsPaid.Should().BeFalse();
            await _ticketService.Update(1, true, count:5);

            var ticket = await GetContext().Tickets.SingleAsync();
            ticket.IsPaid.Should().BeTrue();
            ticket.PaymentDue.Should().Be(new DateTime());
            ticket.Count.Should().Be(5);
        }

        [Test]
        public async Task Get_existing_ticket()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival { Name = "Fest", ReservationDays = 7 });
            await dbContext.Users.AddAsync(new User { Login = "Test" });
            await dbContext.Tickets.AddAsync(new Ticket { FestivalId = 1, UserId = 1 });
            await dbContext.SaveChangesAsync();

            var ticket = await _ticketService.GetById(1);

            ticket.Should().BeEquivalentTo(TicketDto());
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var ticketDto = await _ticketService.GetById(1);

            ticketDto.Should().Be(null);
        }

        [Test]
        public async Task Get_all_tickets()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival { Name = "Fest", ReservationDays = 7 });
            await dbContext.Users.AddAsync(new User { Login = "Test" });
            await dbContext.Tickets.AddRangeAsync(new Ticket { FestivalId = 1, UserId = 1 }, new Ticket { FestivalId = 1, UserId = 1 });
            await dbContext.SaveChangesAsync();
            
            var tickets = await _ticketService.GetAll();

            tickets.Should().HaveCount(2);
            var ticket = TicketDto();
            tickets[0].Should().BeEquivalentTo(ticket);
            ticket.Id = 2;
            tickets[1].Should().BeEquivalentTo(ticket);
        }

        [Test]
        public async Task Get_all_tickets_by_filter()
        {
            var dbContext = GetContext();
            await dbContext.Festivals.AddAsync(new Festival { Name = "Fest", ReservationDays = 7 });
            await dbContext.Users.AddAsync(new User { Login = "Test" });
            await dbContext.Tickets.AddRangeAsync(
                new Ticket { FestivalId = 1, UserId = 1, VariableSymbol = 16981 },
                new Ticket { FestivalId = 1, UserId = 1 });
            await dbContext.SaveChangesAsync();

            var tickets = await _ticketService.GetAll("16981"); //variable symbol

            tickets.Should().HaveCount(1);
            var ticket = TicketDto();
            ticket.VariableSymbol = 16981;
            tickets.Single().Should().BeEquivalentTo(ticket);
        }

        [Test]
        public async Task Successfully_delete_ticket()
        {
            var dbContext = GetContext();
            var ticket = await dbContext.Tickets.AddAsync(new Ticket());
            await dbContext.SaveChangesAsync();

            (await dbContext.Tickets.CountAsync()).Should().Be(1);
            await _ticketService.Delete(ticket.Entity.Id);

            (await dbContext.Tickets.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await _ticketService.Delete(5));
    }
}
