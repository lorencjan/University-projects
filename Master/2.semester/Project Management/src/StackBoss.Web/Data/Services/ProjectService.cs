using Microsoft.EntityFrameworkCore;
using StackBoss.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBoss.Web.Data.Services
{
    public class ProjectService
    {
         private readonly ApplicationDbContext _appDBContext;
    
        public ProjectService(ApplicationDbContext appDBContext)
        {
            _appDBContext = appDBContext;
        }


        public async Task<List<ProjectEntity>> GetAllProjectsAsync()
        {
            return await _appDBContext.ProjectTable.ToListAsync();
        }


        public async Task<bool> InsertProjectAsync(ProjectEntity project)
        {
            await _appDBContext.ProjectTable.AddAsync(project);
            await _appDBContext.SaveChangesAsync();
            return true;
        }


        public async Task<ProjectEntity> GetProjectAsync(int Id)
        {
            ProjectEntity project = await _appDBContext.ProjectTable.
                Include(r => r.RiskList).
                FirstOrDefaultAsync(c => c.Id.Equals(Id));
            return project;
        }


        public async Task<bool> UpdateProjectAsync(ProjectEntity project)
        {
             _appDBContext.ProjectTable.Update(project);
            await _appDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProjectAsync(ProjectEntity project)
        {
            _appDBContext.Remove(project);
            await _appDBContext.SaveChangesAsync();
            return true;
        }
    }
}

