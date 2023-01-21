using StackBoss.Web.Data;
using StackBoss.Web.Data.Entities;
using StackBoss.Web.Data.Services;
using System;
using Xunit;

namespace StackBoss.Tests
{
    public class StackbossDbContextTests
    {
        private readonly ApplicationDbContext _testContext;
        private readonly ProjectService _projectService;
        private readonly RiskService _riskService;
        public StackbossDbContextTests()
        {
            var setup = new StackbossDbContextSetup("testDb");
            _testContext = setup.DbContextFactory.CreateDbContext();
            _projectService = new ProjectService(_testContext);
            _riskService = new RiskService(_testContext);
            setup.PrepareDatabase();
        }
        [Fact]
        public async void Projects_GetAll()
        {
            int numberOfInterprets = 4;


            var projectList = await _projectService.GetAllProjectsAsync();

            Assert.Equal(numberOfInterprets, projectList.Count);

        }

        [Fact]
        public async void InsertProject()
        {
            ProjectEntity Project = new ProjectEntity();


            var result = await _projectService.InsertProjectAsync(Project);

            Assert.True(result);

        }

        [Fact]
        public async void GetProject()
        {
            var name = "Medical IS";


            var project = await _projectService.GetProjectAsync(1);

            Assert.Equal(name, project.Name);

        }

        [Fact]
        public async void UpdateProject()
        {
            ProjectEntity Project = new ProjectEntity();


            var result = await _projectService.InsertProjectAsync(Project);

            Assert.True(result);
            Project.Name = "Test Name";


            result = await _projectService.UpdateProjectAsync(Project);
            Assert.True(result);

        }

        [Fact]
        public async void DeleteProject()
        {
            ProjectEntity Project = new ProjectEntity();


            var result = await _projectService.InsertProjectAsync(Project);

            Assert.True(result);


            result = await _projectService.DeleteProjectAsync(Project);
            Assert.True(result);

        }

        [Fact]
        public async void Risks_GetAll()
        {
            int numberOfInterprets = 18;


            var riskList = await _riskService.GetAllRisksAsync();

            Assert.Equal(numberOfInterprets, riskList.Count);

        }

        [Fact]
        public async void InsertRisk()
        {
            RiskEntity Risk = new RiskEntity();


            var result = await _riskService.InsertRiskAsync(Risk);

            Assert.True(result);

        }

        [Fact]
        public async void GetRisk()
        {
            var name = "Risk of Losing customers";


            var risk = await _riskService.GetRiskAsync(1);

            Assert.Equal(name, risk.Name);

        }

        [Fact]
        public async void UpdateRisk()
        {
            RiskEntity Risk = new RiskEntity();


            var result = await _riskService.InsertRiskAsync(Risk);

            Assert.True(result);

            Risk.Name = "Test Name";


            result = await _riskService.UpdateRiskAsync(Risk);
            Assert.True(result);

        }

        [Fact]
        public async void DeleteRisk()
        {
            RiskEntity Risk = new RiskEntity();


            var result = await _riskService.InsertRiskAsync(Risk);

            Assert.True(result);


            result = await _riskService.DeleteRiskAsync(Risk);
            Assert.True(result);

        }
    }
}
