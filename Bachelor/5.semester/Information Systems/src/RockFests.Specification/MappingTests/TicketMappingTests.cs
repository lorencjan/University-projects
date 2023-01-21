using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.DAL.Entities;
using RockFests.DAL.Types;

namespace RockFests.Specification.MappingTests
{
    public class TicketMappingTests : TestFixture
    {
        public static Ticket Ticket() => new Ticket
        {
            Id = 1,
            IsPaid = true,
            PaymentDue = new DateTime(2020,1,1),
            VariableSymbol = 666,
            Count = 2,
            FestivalId = 2,
            Festival = new Festival
            {
                Id = 2, 
                Name = "TestFest", 
                Location = "Florida", 
                StartDate = new DateTime(2020, 2, 1, 12, 0, 5),
                EndDate = new DateTime(2020, 2, 2, 13, 6, 5),
                Price = 400,
                Currency = "USD",
                Description = "Test"
            },
            UserId = 1,
            User = UserMappingTests.User()
        };

        public static TicketDto TicketDto() => new TicketDto
        {
            Id = 1,
            IsPaid = true,
            PaymentDue = new DateTime(2020, 1, 1),
            VariableSymbol = 666,
            Count = 2,
            Festival = new FestivalLightDto
            {
                Id = 2,
                Name = "TestFest",
                Location = "Florida",
                Date = new DateTimeInterval(new DateTime(2020, 2, 1, 12, 0, 5), new DateTime(2020, 2, 2, 13, 6, 5)),
                Price = 400,
                Currency = "USD"
            },
            User = UserMappingTests.UserDto()
        };

        [Test]
        public void Successful_map_to_dto_object()
        {
            var dto = Mapper.Map<TicketDto>(Ticket());
            dto.Should().BeEquivalentTo(TicketDto());
        }

        [Test]
        public void Successful_map_from_dto_to_entity()
        {
            var entity = Ticket();
            entity.Festival = null;
            entity.User = null;

            var mappedEntity = Mapper.Map<Ticket>(TicketDto());

            mappedEntity.Should().BeEquivalentTo(entity);
        }
    }
}
