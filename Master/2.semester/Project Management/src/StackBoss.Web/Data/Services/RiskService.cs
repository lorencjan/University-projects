using Microsoft.EntityFrameworkCore;
using StackBoss.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBoss.Web.Data.Services
{
    public class RiskService
    {

        private readonly ApplicationDbContext _appDBContext;

        public RiskService(ApplicationDbContext appDBContext)
        {
            _appDBContext = appDBContext;
        }


        public async Task<List<RiskEntity>> GetAllRisksAsync()
        {
            return await _appDBContext.RiskTable.ToListAsync();
        }


        public async Task<bool> InsertRiskAsync(RiskEntity risk)
        {
            await _appDBContext.RiskTable.AddAsync(risk);
            await _appDBContext.SaveChangesAsync();
            return true;
        }


        public async Task<RiskEntity> GetRiskAsync(int Id)
        {
            RiskEntity risk = await _appDBContext.RiskTable
                .Include(r => r.Project)
                .FirstOrDefaultAsync(c => c.Id.Equals(Id));
            return risk;
        }


        public async Task<bool> UpdateRiskAsync(RiskEntity risk)
        {
            _appDBContext.RiskTable.Update(risk);
            await _appDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRiskAsync(RiskEntity risk)
        {
            _appDBContext.Remove(risk);
            await _appDBContext.SaveChangesAsync();
            return true;
        }
    }
}
