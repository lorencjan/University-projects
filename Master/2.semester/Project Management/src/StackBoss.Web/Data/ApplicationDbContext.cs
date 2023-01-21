using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StackBoss.Web.Data.Entities;
using StackBoss.Web.Data.Seeds;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackBoss.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.SeedRisks();
            modelBuilder.SeedProjects();
            modelBuilder.SeedRoles();
            //modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() });
           
          
        }
        public DbSet<RiskEntity> RiskTable { get; set; }
        public DbSet<ProjectEntity> ProjectTable { get; set; }
    }
}
