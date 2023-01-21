using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.DAL.Entities;

namespace RockFests.Specification.MappingTests
{
    public class StageMappingTests : TestFixture
    {
        public static Stage Stage() => new Stage
        {
            Id = 1,
            Name = "Stage",
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
            Performances = new List<Performance>{ new Performance
            {
                Id = 1,
                Start = new DateTime(2020,1,1,12,0,0),
                End = new DateTime(2020,1,1,14,0,0),
                StageId = 2,
                MusicianId = 1,
                Musician = new Musician{FirstName = "test", LastName = "testic"}
            }}
        };

        public static StageDto StageDto() => new StageDto
        {
            Id = 1,
            Name = "Stage",
            FestivalId = 2,
            Performances = new List<PerformanceDto> { PerformanceMappingTests.PerformanceDto() }
        };

        [Test]
        public void Successful_map_to_dto_object()
        {
            var dto = Mapper.Map<StageDto>(Stage());
            dto.Should().BeEquivalentTo(StageDto());
        }

        [Test]
        public void Successful_map_from_dto_to_entity()
        {
            var entity = Stage();
            entity.Festival = null;

            var mappedEntity = Mapper.Map<Stage>(StageDto());

            entity.Performances.ForEach(x => x.Musician = null);
            mappedEntity.Should().BeEquivalentTo(entity);
        }
    }
}
