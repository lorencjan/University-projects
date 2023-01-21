using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.DAL.Entities;

namespace RockFests.Specification.MappingTests
{
    public class RatingMappingTests : TestFixture
    {
        public static BandRating BandRating() => new BandRating
        {
            Id = 1,
            UserName = "Tester",
            Text = "Test",
            Number = 8,
            BandId = 1,
            Band = new Band{ Id = 1, Name = "Test Maiden" }
        };

        public static MusicianRating MusicianRating() => new MusicianRating
        {
            Id = 1,
            UserName = "Tester",
            Text = "Test",
            Number = 8,
            MusicianId = 1,
            Musician = new Musician { Id = 1, FirstName = "Test", LastName = "Testovic" }
        };

        public static RatingDto RatingDto() => new RatingDto
        {
            Id = 1,
            UserName = "Tester",
            Text = "Test",
            Number = 8,
            InterpretId = 1
        };

        [Test]
        public void Successful_map_from_band_to_dto_object()
        {
            var entity = BandRating();
            var dto = RatingDto();
            dto.InterpretName = entity.Band.Name;

            var mappedDto = Mapper.Map<RatingDto>(entity);

            mappedDto.Should().BeEquivalentTo(dto);
        }

        [Test]
        public void Successful_map_from_musician_to_dto_object()
        {
            var entity = MusicianRating();
            var dto = RatingDto();
            dto.InterpretName = $"{entity.Musician.FirstName} {entity.Musician.LastName}";

            var mappedDto = Mapper.Map<RatingDto>(entity);

            mappedDto.Should().BeEquivalentTo(dto);
        }

        [Test]
        public void Successful_map_from_dto_to_band_entity()
        {
            var entity = BandRating();
            entity.Band = null;

            var mappedEntity = Mapper.Map<BandRating>(RatingDto());

            mappedEntity.Should().BeEquivalentTo(entity);
        }

        [Test]
        public void Successful_map_from_dto_to_musician_entity()
        {
            var entity = MusicianRating();
            entity.Musician = null;

            var mappedEntity = Mapper.Map<MusicianRating>(RatingDto());

            mappedEntity.Should().BeEquivalentTo(entity);
        }
    }
}
