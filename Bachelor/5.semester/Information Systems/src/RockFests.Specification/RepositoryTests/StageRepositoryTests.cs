using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.BL.Repositories;
using RockFests.DAL.Entities;
using RockFests.DAL.Types;

namespace RockFests.Specification.RepositoryTests
{
    public class StageRepositoryTests : TestFixture
    {
        private StageRepository _stageRepository;

        private Performance Performance1(int id = 0) => new Performance
        {
            Id = id,
            MusicianId = 1,
            Musician = new Musician{Id = 1},
            StageId = 1,
            Start = new DateTime(2020, 1, 1, 12, 0, 0),
            End = new DateTime(2020, 1, 1, 14, 0, 0)
        };
        private Performance Performance2(int id = 0) => new Performance
        {
            Id = id,
            BandId = 2,
            Band = new Band { Id = 1 },
            StageId = 1,
            Start = new DateTime(2020, 1, 1, 15, 0, 0),
            End = new DateTime(2020, 1, 1, 16, 0, 0)
        };
        private PerformanceDto PerformanceDto1(int id = 0) => Mapper.Map<PerformanceDto>(Performance1(id));
        private PerformanceDto PerformanceDto2(int id = 0) => Mapper.Map<PerformanceDto>(Performance2(id));

        [SetUp]
        public void StageSetup() => _stageRepository = ServiceProvider.GetRequiredService<StageRepository>();

        [Test]
        public async Task Successfully_create_stage()
        {
            var stageId = await _stageRepository.Add(new StageDto
            {
                Name = "Test",
                FestivalId = 1
            });

            var stages = await GetContext().Stages.ToListAsync();

            stageId.Should().Be(1);
            stages.Should().HaveCount(1);
            stages.Single().Should().BeEquivalentTo(new Stage
            {
                Id = 1,
                Name = "Test",
                FestivalId = 1
            });
        }

        [Test]
        public async Task Successfully_create_stage_with_performances()
        {
            var stageId = await _stageRepository.Add(new StageDto
            {
                Name = "Test",
                FestivalId = 1,
                Performances = new List<PerformanceDto> { PerformanceDto1(), PerformanceDto2() }
            });

            var dbContext = GetContext();
            var stages = await dbContext.Stages.ToListAsync();
            var performances = await dbContext.Performances.ToListAsync();

            stages.Should().HaveCount(1);
            stages.Single().Id.Should().Be(stageId);
            stages.Single().Performances = null; //it's loaded all even without Includes due to being in memory -> not real behaviour
            stages.Single().Should().BeEquivalentTo(new Stage
            {
                Id = 1,
                Name = "Test",
                FestivalId = 1
            });
            performances.Should().HaveCount(2);
            performances.ForEach(x => x.Stage = null);    //cyclic dependencies
            var p1 = Performance1(1);
            p1.Musician = null; //not mocked data
            var p2 = Performance2(2);
            p2.Band = null; //not mocked data
            performances.Should().ContainEquivalentOf(p1);
            performances.Should().ContainEquivalentOf(p2);
        }

        [Test]
        public async Task Successfully_create_multiple_stages()
        {
            await _stageRepository.AddRange(new []
            {
                new StageDto
                {
                    Name = "Test1",
                    FestivalId = 1,
                    Performances = new List<PerformanceDto> { PerformanceDto1() }
                },
                new StageDto
                {
                    Name = "Test2",
                    FestivalId = 1,
                    Performances = new List<PerformanceDto> { PerformanceDto2() }
                }
            });

            var stages = await GetContext().Stages.ToListAsync();
            stages.Should().HaveCount(2);
            stages[0].Name.Should().Be("Test1");
            stages[1].Name.Should().Be("Test2");
        }

        [Test]
        public async Task Successfully_update_stage()
        {
            var dbContext = GetContext();
            await dbContext.Stages.AddAsync(new Stage
            {
                Name = "Test",
                FestivalId = 1
            });
            await dbContext.SaveChangesAsync();

            var stageDto = new StageDto
            {
                Id = 1,
                Name = "NewValue",
                FestivalId = 2
            };
            await _stageRepository.Update(stageDto);

            var stage = await GetContext().Stages.SingleAsync(x => x.Id == 1);
            stage.Should().BeEquivalentTo(new Stage
            {
                Id = 1,
                Name = "NewValue",
                FestivalId = 2
            });
        }

        [Test]
        public async Task Successfully_update_with_performances_stage()
        {
            var dbContext = GetContext();
            await dbContext.Stages.AddAsync(new Stage
            {
                Name = "Test",
                FestivalId = 1,
                Performances = new List<Performance>{Performance1(), Performance2()}
            });
            await dbContext.SaveChangesAsync();

            var stageDto = new StageDto
            {
                Id = 1,
                Name = "Modified",
                FestivalId = 1,
                Performances = new List<PerformanceDto>
                {
                    new PerformanceDto
                    {
                        Id = 1,
                        Interpret = new InterpretDto{Id = 2},
                        IsBand = true,
                        StageId = 1,
                        Time = new DateTimeInterval(new DateTime(2020, 1, 1, 12, 0, 0), new DateTime(2020, 1, 1, 15, 0, 0))
                    },
                    new PerformanceDto
                    {
                        Interpret = new InterpretDto{Id = 3},
                        IsBand = true,
                        StageId = 1,
                        Time = new DateTimeInterval(new DateTime(2020, 1, 1, 17, 0, 0), new DateTime(2020, 1, 1, 18, 30, 0))
                    }
                }
            };
            await _stageRepository.Update(stageDto);

            var stage = await GetContext().Stages.Include(x => x.Performances).SingleAsync(x => x.Id == 1);
            stage.Performances.ForEach(x => x.Stage = null); //again to remove the cyclic dependencies
            stage.Name.Should().Be("Modified");
            stage.Performances.Should().HaveCount(2);
            stage.Performances.Should().ContainEquivalentOf(new Performance
            {
                Id = 1,
                BandId = 2,
                StageId = 1,
                Start = new DateTime(2020, 1, 1, 12, 0, 0),
                End = new DateTime(2020, 1, 1, 15, 0, 0)
            });
            stage.Performances.Should().ContainEquivalentOf(new Performance
            {
                Id = 3,
                BandId = 3,
                StageId = 1,
                Start = new DateTime(2020, 1, 1, 17, 0, 0), 
                End = new DateTime(2020, 1, 1, 18, 30, 0)
            });
        }

        [Test]
        public async Task Get_existing_stage()
        {
            var dbContext = GetContext();
            await dbContext.Stages.AddAsync(new Stage
            {
                Name = "Test",
                FestivalId = 1,
                Festival = new Festival{Name = "TestFest"}
            });
            await dbContext.SaveChangesAsync();

            var stageDto = await _stageRepository.GetById(1);

            stageDto.Should().BeEquivalentTo(new StageDto
            {
                Id = 1,
                Name = "Test",
                FestivalId = 1,
                Performances = new List<PerformanceDto>()
            });
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var stageDto = await _stageRepository.GetById(1);

            stageDto.Should().Be(null);
        }

        [Test]
        public async Task Get_all_stages()
        {
            var stages = new List<Stage>
            {
                new Stage
                {
                    Id = 1,
                    Name = "Test1",
                    FestivalId = 1,
                    Festival = new Festival{Name = "TestFest"},
                    Performances = new List<Performance>()
                },
                new Stage
                {
                    Id = 2,
                    Name = "Test2",
                    FestivalId = 2,
                    Festival = new Festival{Name = "TestFest"},
                    Performances = new List<Performance>()
                },
                new Stage
                {
                    Id = 3,
                    Name = "Test3",
                    FestivalId = 3,
                    Festival = new Festival{Name = "TestFest2"},
                    Performances = new List<Performance>()
                }
            };
            var stagesDto = stages.AsQueryable().ProjectTo<StageDto>().ToList();
            var dbContext = GetContext();
            await dbContext.Stages.AddRangeAsync(stages);
            await dbContext.SaveChangesAsync();

            var allStagesDto = await _stageRepository.GetAll();

            allStagesDto.Should().HaveCount(3);
            allStagesDto.ForEach(x => stagesDto.Should().ContainEquivalentOf(x));
        }

        [Test]
        public async Task Successfully_delete_stage()
        {
            var dbContext = GetContext();
            var stage = await dbContext.Stages.AddAsync(new Stage
            {
                Name = "Test",
                FestivalId = 1
            });
            await dbContext.SaveChangesAsync();

            (await dbContext.Stages.CountAsync()).Should().Be(1);
            await _stageRepository.Delete(stage.Entity.Id);

            (await dbContext.Stages.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await _stageRepository.Delete(5));
    }
}
