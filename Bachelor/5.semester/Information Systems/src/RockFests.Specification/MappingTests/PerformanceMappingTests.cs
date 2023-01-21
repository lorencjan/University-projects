using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.DAL.Entities;
using RockFests.DAL.Types;

namespace RockFests.Specification.MappingTests
{
    public class PerformanceMappingTests : TestFixture
    {
        public static Performance Performance() => new Performance
        {
            Id = 1,
            Start = new DateTime(2020,1,1,12,0,0),
            End = new DateTime(2020,1,1,14,0,0),
            StageId = 2,
            Stage = new Stage
            {
                Id = 2, 
                Name = "TestFest",
                FestivalId = 2
            }
        };

        public static PerformanceDto PerformanceDto() => new PerformanceDto
        {
            Id = 1,
            Time = new DateTimeInterval(new DateTime(2020, 1, 1, 12, 0, 0), new DateTime(2020, 1, 1, 14, 0, 0)),
            StageId = 2,
            Interpret = new InterpretDto{ Id = 1, Name = "test testic"}
        };

        [Test]
        public void Successful_band_map_to_dto_object()
        {
            var performance = Performance();
            performance.BandId = 1;
            performance.Band = new Band{Id = 1, Name = "Test Maiden"};
            var performanceDto = PerformanceDto();
            performanceDto.IsBand = true;
            performanceDto.Interpret.Name = "Test Maiden";

            var mappedPerformanceDto = Mapper.Map<PerformanceDto>(performance);

            mappedPerformanceDto.Should().BeEquivalentTo(performanceDto);
        }

        [Test]
        public void Successful_musician_map_to_dto_object()
        {
            var performance = Performance();
            performance.MusicianId = 1;
            performance.Musician = new Musician { Id = 1, FirstName = "Test", LastName = "Testovic"};
            var performanceDto = PerformanceDto();
            performanceDto.IsBand = false;
            performanceDto.Interpret.Name = "Test Testovic";

            var mappedPerformanceDto = Mapper.Map<PerformanceDto>(performance);

            mappedPerformanceDto.Should().BeEquivalentTo(performanceDto);
        }

        [Test]
        public void Successful_map_from_dto_to_band_entity()
        {
            var entity = Performance();
            entity.Stage = null;
            entity.BandId = 1;
            var dto = PerformanceDto();
            dto.IsBand = true;

            var mappedEntity = Mapper.Map<Performance>(dto);

            mappedEntity.Should().BeEquivalentTo(entity);
        }

        [Test]
        public void Successful_map_from_dto_to_musician_entity()
        {
            var entity = Performance();
            entity.Stage = null;
            entity.MusicianId = 1;
            var dto = PerformanceDto();
            dto.IsBand = false;

            var mappedEntity = Mapper.Map<Performance>(dto);

            mappedEntity.Should().BeEquivalentTo(entity);
        }
    }
}
