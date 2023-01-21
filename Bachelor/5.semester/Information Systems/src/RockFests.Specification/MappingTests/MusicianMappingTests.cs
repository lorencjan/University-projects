using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.DAL.Entities;
using RockFests.DAL.Seeds;
using RockFests.DAL.Types;

namespace RockFests.Specification.MappingTests
{
    public class MusicianMappingTests : TestFixture
    {
        public static Musician Musician() => new Musician
        {
            Id = 2,
            FirstName = "Test",
            LastName = "Testovic",
            Born = new DateTime(1995, 5, 11),
            Country = "USA",
            Role = "guitar",
            Bands = new List<BandMusician>
            {
                new BandMusician
                {
                    MusicianId = 1,
                    BandId = 2,
                    Band = new Band
                    {
                        Id = 1,
                        Name = "Test Maiden",
                        Genre = "rock",
                        Ratings = new List<BandRating>{new BandRating { Number = 8}, new BandRating { Number = 9}}
                    }
                },
            },
            Performances = new List<Performance>{new Performance
            {
                Id = 1,
                Start = new DateTime(2020, 1, 20, 15, 0, 0),
                End = new DateTime(2020, 1, 20, 17, 30, 0),
                MusicianId = 2,
                Musician = new Musician{FirstName = "Test", LastName = "Testovic"},
                StageId = 1
            }},
            Photo = SeedImages.bruce_dickinson,
            Ratings = new List<MusicianRating>
            {
                new MusicianRating
                {
                    Id = 1,
                    MusicianId = 1,
                    Musician = new Musician{ FirstName = "Test", LastName = "Testovic" },
                    Number = 9,
                    Text = "Test rating 1",
                    UserName = "User1"
                },
                new MusicianRating
                {
                    Id = 2,
                    MusicianId = 1,
                    Musician = new Musician{ FirstName = "Test", LastName = "Testovic" },
                    Number = 10,
                    Text = "Test rating 2",
                    UserName = "User2"
                }
            }
        };

        public static MusicianDto MusicianDto() => new MusicianDto
        {
            Id = 2,
            FirstName = "Test",
            LastName = "Testovic",
            Born = new DateTime(1995, 5, 11),
            Country = "USA",
            Role = "guitar",
            Bands = new List<BandLightDto>
            {
                new BandLightDto { Id = 1, Name = "Test Maiden", Genre = "rock", Rating = 8.5}
            },
            Performances = new List<PerformanceDto>{new PerformanceDto
            {
                Id = 1,
                Time = new DateTimeInterval(new DateTime(2020, 1, 20, 15, 0, 0), new DateTime(2020, 1, 20, 17, 30, 0)),
                Interpret = new InterpretDto{Id = 2, Name = "Test Testovic"},
                StageId = 1
            }},
            Photo = SeedImages.bruce_dickinson,
            Ratings = new List<RatingDto>
            {
                new RatingDto
                {
                    Id = 1,
                    UserName = "User1",
                    Number = 9,
                    Text = "Test rating 1",
                    InterpretId = 1,
                    InterpretName = "Test Testovic"
                },
                new RatingDto
                {
                    Id = 2,
                    UserName = "User2",
                    Number = 10,
                    Text = "Test rating 2",
                    InterpretId = 1,
                    InterpretName = "Test Testovic"
                }
            }
        };

        public static MusicianLightDto MusicianLightDto() => new MusicianLightDto
        {
            Id = 2,
            Name = "Test Testovic",
            Role = "guitar",
            Rating = 9.5
        };

        [Test]
        public void Successful_map_to_dto_object()
        {
            var dto = Mapper.Map<MusicianDto>(Musician());
            dto.Should().BeEquivalentTo(MusicianDto());
        }

        [Test]
        public void Successful_map_to_light_dto_object()
        {
            var dto = Mapper.Map<MusicianLightDto>(Musician());
            dto.Should().BeEquivalentTo(MusicianLightDto());
        }

        [Test]
        public void Successful_map_from_dto_to_entity()
        {
            var entity = Musician();
            entity.Bands = null;
            entity.Ratings = null;
            entity.Performances = null;

            var mappedEntity = Mapper.Map<Musician>(MusicianDto());

            mappedEntity.Should().BeEquivalentTo(entity);
        }
    }
}
