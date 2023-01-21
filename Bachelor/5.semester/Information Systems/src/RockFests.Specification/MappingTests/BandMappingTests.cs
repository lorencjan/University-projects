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
    public class BandMappingTests : TestFixture
    {
        public static Band Band() => new Band
        {
            Country = "USA",
            Description = "Test",
            FormationYear = 2020,
            Genre = "Rock",
            Id = 2,
            Members = new List<BandMusician>
            {
                new BandMusician
                {
                    BandId = 2,
                    MusicianId = 1,
                    Musician = new Musician
                    {
                        Id = 1,
                        FirstName = "firstname1",
                        LastName = "lastname1",
                        Role = "guitar",
                        Ratings = new List<MusicianRating>{new MusicianRating{Number = 8}, new MusicianRating{Number = 10}}
                    }
                },
                new BandMusician
                {
                    BandId = 2,
                    MusicianId = 2,
                    Musician = new Musician
                    {
                        Id = 2,
                        FirstName = "firstname2",
                        LastName = "lastname2",
                        Role = "vocals",
                        Ratings = new List<MusicianRating>{new MusicianRating{Number = 7}, new MusicianRating{Number = 10}}
                    }
                }
            },
            Name = "Test Maiden",
            Performances = new List<Performance>{new Performance
            {
                Id = 1,
                Start = new DateTime(2020, 1, 20, 15, 0, 0),
                End = new DateTime(2020, 1, 20, 17, 30, 0),
                BandId = 2,
                Band = new Band{Name = "Test Maiden"},
                StageId = 1
            }},
            Photo = SeedImages.iron_maiden_logo,
            Ratings = new List<BandRating>
            {
                new BandRating
                {
                    Id = 1, 
                    BandId = 1,
                    Band = new Band{Name = "Test Maiden"},
                    Number = 9,
                    Text = "Test rating 1",
                    UserName = "User1"
                },
                new BandRating
                {
                    Id = 2,
                    BandId = 1,
                    Band = new Band{Name = "Test Maiden"},
                    Number = 10,
                    Text = "Test rating 2",
                    UserName = "User2"
                }
            }
        };

        public static BandDto BandDto() => new BandDto
        {
            Country = "USA",
            Description = "Test",
            FormationYear = 2020,
            Genre = "Rock",
            Id = 2,
            Members = new List<MusicianLightDto>
            {
                new MusicianLightDto { Id = 1, Name = "firstname1 lastname1", Role = "guitar", Rating = 9}, 
                new MusicianLightDto { Id = 2, Name = "firstname2 lastname2", Role = "vocals", Rating = 8.5}
            },
            Name = "Test Maiden",
            Performances = new List<PerformanceDto>{new PerformanceDto
            {
                Id = 1,
                Time = new DateTimeInterval(new DateTime(2020, 1, 20, 15, 0, 0), new DateTime(2020, 1, 20, 17, 30, 0)),
                IsBand = true,
                Interpret = new InterpretDto{Id = 2, Name = "Test Maiden"},
                StageId = 1
            }},
            Photo = SeedImages.iron_maiden_logo,
            Ratings = new List<RatingDto>
            {
                new RatingDto
                {
                    Id = 1,
                    UserName = "User1",
                    Number = 9,
                    Text = "Test rating 1",
                    InterpretId = 1,
                    InterpretName = "Test Maiden"
                },
                new RatingDto
                {
                    Id = 2,
                    UserName = "User2",
                    Number = 10,
                    Text = "Test rating 2",
                    InterpretId = 1,
                    InterpretName = "Test Maiden"
                }
            }
        };

        public static BandLightDto BandLightDto() => new BandLightDto
        {
            Genre = "Rock",
            Id = 2,
            Name = "Test Maiden",
            Rating = 9.5
        };

        [Test]
        public void Successful_map_to_dto_object()
        {
            var dto = Mapper.Map<BandDto>(Band());
            dto.Should().BeEquivalentTo(BandDto());
        }

        [Test]
        public void Successful_map_to_light_dto_object()
        {
            var dto = Mapper.Map<BandLightDto>(Band());
            dto.Should().BeEquivalentTo(BandLightDto());
        }

        [Test]
        public void Successful_map_from_dto_to_entity()
        {
            var entity = Band();
            entity.Members = null;
            entity.Ratings = null;
            entity.Performances = null;

            var mappedEntity = Mapper.Map<Band>(BandDto());

            mappedEntity.Should().BeEquivalentTo(entity);
        }
    }
}
