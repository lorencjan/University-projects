using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.DAL.Entities;
using RockFests.DAL.Types;

namespace RockFests.Specification.MappingTests
{
    public class FestivalMappingTests : TestFixture
    {
        public static Festival Festival() => new Festival
        {
            Id = 2,
            Name = "TestFest",
            Location = "Florida",
            StartDate = new DateTime(2020, 2, 1, 12, 0, 5),
            EndDate = new DateTime(2020, 2, 2, 13, 6, 5),
            MaxTickets = 4000,
            MaxTicketsForUser = 5,
            ReservationDays = 7,
            Price = 400,
            Currency = "USD",
            Description = "Test",
            Stages = new List<Stage>{ StageMappingTests.Stage() },
            Tickets = new List<Ticket>{ TicketMappingTests.Ticket() }
            
        };

        public static FestivalDto FestivalDto() => new FestivalDto
        {
            Id = 2,
            Name = "TestFest",
            Location = "Florida",
            Date = new DateTimeInterval(new DateTime(2020, 2, 1, 12, 0, 5), new DateTime(2020, 2, 2, 13, 6, 5)),
            MaxTickets = 4000,
            MaxTicketsForUser = 5,
            ReservationDays = 7,
            Price = 400,
            Currency = "USD",
            Description = "Test",
            Stages = new List<StageDto> { StageMappingTests.StageDto() },
            Tickets = new List<TicketDto> { TicketMappingTests.TicketDto() }
        };

        public static FestivalLightDto FestivalLightDto() => new FestivalLightDto
        {
            Id = 2,
            Name = "TestFest",
            Location = "Florida",
            Date = new DateTimeInterval(new DateTime(2020, 2, 1, 12, 0, 5), new DateTime(2020, 2, 2, 13, 6, 5)),
            Price = 400,
            Currency = "USD"
        };

        [Test]
        public void Successful_map_to_dto_object()
        {
            var dto = Mapper.Map<FestivalDto>(Festival());
            dto.Should().BeEquivalentTo(FestivalDto());
        }

        [Test]
        public void Successful_map_to_light_dto_object()
        {
            var dto = Mapper.Map<FestivalLightDto>(Festival());
            dto.Should().BeEquivalentTo(FestivalLightDto());
        }

        [Test]
        public void Successful_map_from_dto_to_entity()
        {
            var entity = Festival();
            entity.Stages = null;
            entity.Tickets = null;

            var mappedEntity = Mapper.Map<Festival>(FestivalDto());

            mappedEntity.Should().BeEquivalentTo(entity);
        }
    }
}
