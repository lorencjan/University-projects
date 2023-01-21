using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackBoss.Web.Data.Entities;
using StackBoss.Web.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBoss.Web.Data.Seeds
{
    public static class Seed
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.HasData( new IdentityRole() 
                { 
                Name = "Admin", 
                NormalizedName = "Admin".ToUpper() 
                });
                entity.HasData( new IdentityRole() 
                { 
                Name = "ProjectManager", 
                NormalizedName = "ProjectManager".ToUpper() 
                });
                entity.HasData( new IdentityRole() 
                { 
                Name = "ProjectDirector", 
                NormalizedName = "ProjectDirector".ToUpper() 
                });
            });
        } 
        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (userManager.FindByEmailAsync("admin@admin.com").Result==null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "admin").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }       
        } 

         public static void SeedProjects(this ModelBuilder modelBuilder)
        {
             modelBuilder.Entity<ProjectEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasMany(x => x.RiskList);
                entity.HasData(new ProjectEntity()
                {
                     Id = 1,
                     Name = "Medical IS",
                     Description = "Information system for Hospital in Brno",
                     Manager = "Ing. Jan Honza",
                     Staff = "Lukas Kudlicka, Michal Kovac",
                     CustomId = "P_001"

                });
                entity.HasData(new ProjectEntity()
                {
                    Id = 2,
                    Name = "Engineering IS",
                    Description = "Information system for Andrews Constructions",
                    Manager = "Ing. ´Michal Slivka",
                    Staff = "Peter Janosik, Michal Kutil",
                    CustomId = "P_002"

                });
                entity.HasData(new ProjectEntity()
                {
                    Id = 3,
                    Name = "Hotel IS",
                    Description = "Information system for Hotel Yasmin",
                    Manager = "Ing. ´Michal Chrapko",
                    Staff = "Jakub Varga, Jakub Kulan",
                    CustomId = "P_003"

                });
            });
         }
        
        public static void SeedRisks(this ModelBuilder modelBuilder)
        {
             modelBuilder.Entity<RiskEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Project);
                //Medical IS
                entity.HasData(new RiskEntity()
                { 
                 Id = 1,
                 Name = "Risk of Losing customers",
                 Description = "Test bussiness risk",
                 Category = Category.BusinessRisks,
                 Threat = "The supplier will not supply the medication",
                 Starters= "Lack of medication",
                 Reaction = "Have 2 or more suppliers",
                 Owner = "Ing. Jozko Mrkvicka",
                 Probability = Probability.Three,
                 Consequences = Consequences.Eight,
                 RiskEvaluation = ((int)Probability.Three) * ((int)Consequences.Eight),
                 State = State.Potential,
                 CreatedDate = new DateTime(2021,7,10),
                 ModifiedStateDate = new DateTime(2021,7,10),
                 ReactionDate = new DateTime(2022,9,20),
                 ProjectId = 1,
                 CustomId = "P001_R01"
                 
                });
                entity.HasData(new RiskEntity()
                { 
                 Id = 2,
                 Name = "Risk of National Crisis",
                 Description = "Test extern risk",
                 Category = Category.ExternRisks,
                 Threat = "Loosing all of money",
                 Starters = "People stopped buying medication enough",
                 Reaction = "Create a reserve fund",
                 Owner = "Ing. Jozko Mrkvicka",
                 Probability = Probability.Three,
                 Consequences = Consequences.Seven,
                 RiskEvaluation = ((int)Probability.Six) * ((int)Consequences.Eight),
                 State = State.Potential,
                 CreatedDate = new DateTime(2022,2,5),
                 ModifiedStateDate = new DateTime(2022,3,11),
                 ReactionDate = new DateTime(2022,5,1),
                 ProjectId = 1,
                 CustomId = "P001_R02"
                 
                });
                entity.HasData(new RiskEntity()
                { 
                 Id = 3,
                 Name = "Risk of Lost Data",
                 Description = "Test technical risk",
                 Category = Category.TechnicalRisks,
                 Threat = "Loosing all of data",
                 Starters = "The system was not checked against hacking",
                 Reaction = "Buy and install antivirus",
                 Owner = "Ing. Jozko Mrkvicka",
                 Probability = Probability.Four,
                 Consequences = Consequences.Nine,
                 RiskEvaluation = ((int)Probability.Four) * ((int)Consequences.Nine),
                 State = State.Potential,
                 CreatedDate = new DateTime(2021,7,7),
                 ModifiedStateDate = new DateTime(2021,7,21),
                 ReactionDate = new DateTime(2021,11,28),
                 ProjectId = 1,
                 CustomId = "P001_R03"
                 
                });
                entity.HasData(new RiskEntity()
                {
                    Id = 4,
                    Name = "Risk of Planning",
                    Description = "Test project risk",
                    Category = Category.ProjectRisks,
                    Threat = "Loss of control of the project",
                    Starters = "Insufficient control of employees",
                    Reaction = "Increase employee control",
                    Owner = "Ing. Jozko Mrkvicka",
                    Probability = Probability.One,
                    Consequences = Consequences.Eight,
                    RiskEvaluation = ((int)Probability.One * ((int)Consequences.Eight)),
                    State = State.Occured,
                    CreatedDate = new DateTime(2021, 10, 21),
                    ModifiedStateDate = new DateTime(2021, 10, 23),
                    ReactionDate = new DateTime(2022, 8, 5),
                    ProjectId = 1,
                    CustomId = "P001_R04"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 5,
                    Name = "Risk of System Failure",
                    Description = "Test project risk",
                    Category = Category.ProjectRisks,
                    Threat = "The whole system will fall",
                    Starters = "Not every part of the system has been tested",
                    Reaction = "Test the system as a whole but also in parts",
                    Owner = "Ing. Jozko Mrkvicka",
                    Probability = Probability.Four,
                    Consequences = Consequences.Seven,
                    RiskEvaluation = ((int)Probability.Four) * ((int)Consequences.Seven),
                    State = State.Occured,
                    CreatedDate = new DateTime(2021, 9, 21),
                    ModifiedStateDate = new DateTime(2021, 8, 23),
                    ReactionDate = new DateTime(2021, 12, 6),
                    ProjectId = 1,
                    CustomId = "P001_R06"

                });
                //Engineering IS
                entity.HasData(new RiskEntity()
                {
                    Id = 6,
                    Name = "Risk of Lost Data",
                    Description = "Test technical risk",
                    Category = Category.TechnicalRisks,
                    Threat = "Loosing all of data",
                    Starters = "The system was not checked against hacking",
                    Reaction = "Buy and install antivirus",
                    Owner = "Ing. Jan Hrasko",
                    Probability = Probability.Four,
                    Consequences = Consequences.Eight,
                    RiskEvaluation = ((int)Probability.Four) * ((int)Consequences.Eight),
                    State = State.Potential,
                    CreatedDate = new DateTime(2021, 6, 7),
                    ModifiedStateDate = new DateTime(2021, 6, 22),
                    ReactionDate = new DateTime(2021, 10, 27),
                    ProjectId = 2,
                    CustomId = "P002_R06"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 7,
                    Name = "Risk of Losing market position",
                    Description = "Test business risk",
                    Category = Category.BusinessRisks,
                    Threat = "The foreign market will know nothing about us",
                    Starters = "Product manual only in national language",
                    Reaction = "translate the manual into several languages",
                    Owner = "Ing. Jan Hrasko",
                    Probability = Probability.Four,
                    Consequences = Consequences.Six,
                    RiskEvaluation = ((int)Probability.Four) * ((int)Consequences.Six),
                    State = State.Potential,
                    CreatedDate = new DateTime(2021, 5, 8),
                    ModifiedStateDate = new DateTime(2021, 5, 25),
                    ReactionDate = new DateTime(2021, 9, 24),
                    ProjectId = 2,
                    CustomId = "P002_R07"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 8,
                    Name = "Risk of National Crisis",
                    Description = "Test extern risk",
                    Category = Category.ExternRisks,
                    Threat = "Loosing all of money",
                    Starters = "People stopped buying medication enough",
                    Reaction = "Create a reserve fund",
                    Owner = "Ing. Jan Hrasko",
                    Probability = Probability.Five,
                    Consequences = Consequences.Nine,
                    RiskEvaluation = ((int)Probability.Five) * ((int)Consequences.Nine),
                    State = State.Potential,
                    CreatedDate = new DateTime(2021, 6, 19),
                    ModifiedStateDate = new DateTime(2021, 7, 12),
                    ReactionDate = new DateTime(2022, 1, 5),
                    ProjectId = 2,
                    CustomId = "P002_R08"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 9,
                    Name = "Risk of Fire",
                    Description = "Test technical risk",
                    Category = Category.TechnicalRisks,
                    Threat = "loss of production site",
                    Starters = "The factory does not have fire detectors",
                    Reaction = "Buy and inspect fire detectors regularly",
                    Owner = "Ing. Jan Hrasko",
                    Probability = Probability.Six,
                    Consequences = Consequences.Ten,
                    RiskEvaluation = ((int)Probability.Six) * ((int)Consequences.Ten),
                    State = State.Potential,
                    CreatedDate = new DateTime(2021, 7, 29),
                    ModifiedStateDate = new DateTime(2021, 8, 14),
                    ReactionDate = new DateTime(2021, 9, 15),
                    ProjectId = 2,
                    CustomId = "P002_R09"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 10,
                    Name = "Risk of Planning",
                    Description = "Test project risk",
                    Category = Category.ProjectRisks,
                    Threat = "Loss of control of the project",
                    Starters = "Insufficient control of employees",
                    Reaction = "Increase employee control",
                    Owner = "Ing. Jan Hrasko",
                    Probability = Probability.Two,
                    Consequences = Consequences.Six,
                    RiskEvaluation = ((int)Probability.Two * ((int)Consequences.Six)),
                    State = State.Closed,
                    CreatedDate = new DateTime(2021, 8, 18),
                    ModifiedStateDate = new DateTime(2021, 8, 26),
                    ReactionDate = new DateTime(2022, 10, 5),
                    ProjectId = 2,
                    CustomId = "P002_R10"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 11,
                    Name = "Risk of System Failure",
                    Description = "Test project risk",
                    Category = Category.ProjectRisks,
                    Threat = "The whole system will fall",
                    Starters = "Not every part of the system has been tested",
                    Reaction = "Test the system as a whole but also in parts",
                    Owner = "Ing. Jan Hrasko",
                    Probability = Probability.Four,
                    Consequences = Consequences.Seven,
                    RiskEvaluation = ((int)Probability.Four) * ((int)Consequences.Seven),
                    State = State.Occured,
                    CreatedDate = new DateTime(2021, 9, 21),
                    ModifiedStateDate = new DateTime(2021, 8, 23),
                    ReactionDate = new DateTime(2021, 12, 6),
                    ProjectId = 2,
                    CustomId = "P002_R11"

                });
                //Hotel IS
                entity.HasData(new RiskEntity()
                {
                    Id = 12,
                    Name = "Risk of losing information about the accommodated",
                    Description = "Test technical risk",
                    Category = Category.TechnicalRisks,
                    Threat = "Loosing all of data",
                    Starters = "The system was not checked against hacking",
                    Reaction = "Buy and install antivirus",
                    Owner = "Ing. Jakub Malik",
                    Probability = Probability.Five,
                    Consequences = Consequences.Five,
                    RiskEvaluation = ((int)Probability.Five) * ((int)Consequences.Five),
                    State = State.Potential,
                    CreatedDate = new DateTime(2021, 6, 7),
                    ModifiedStateDate = new DateTime(2021, 6, 22),
                    ReactionDate = new DateTime(2021, 10, 27),
                    ProjectId = 3,
                    CustomId = "P003_R12"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 13,
                    Name = "risk of Losing the position on the international market",
                    Description = "Test business risk",
                    Category = Category.BusinessRisks,
                    Threat = "The foreign market will know nothing about us",
                    Starters = "The hotel was not sufficiently presented online",
                    Reaction = "Create a page for online booking",
                    Owner = "Ing. Jakub Malik",
                    Probability = Probability.Three,
                    Consequences = Consequences.Six,
                    RiskEvaluation = ((int)Probability.Three) * ((int)Consequences.Six),
                    State = State.Potential,
                    CreatedDate = new DateTime(2021, 5, 8),
                    ModifiedStateDate = new DateTime(2021, 5, 25),
                    ReactionDate = new DateTime(2021, 9, 24),
                    ProjectId = 3,
                    CustomId = "P003_R13"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 14,
                    Name = "Risk of Fire",
                    Description = "Test technical risk",
                    Category = Category.TechnicalRisks,
                    Threat = "Loss of space for customer accommodation",
                    Starters = "The hotel does not have fire detectors",
                    Reaction = "Buy and inspect fire detectors regularly",
                    Owner = "Ing. Jakub Malik",
                    Probability = Probability.Six,
                    Consequences = Consequences.Ten,
                    RiskEvaluation = ((int)Probability.Six) * ((int)Consequences.Ten),
                    State = State.Potential,
                    CreatedDate = new DateTime(2021, 7, 29),
                    ModifiedStateDate = new DateTime(2021, 8, 14),
                    ReactionDate = new DateTime(2021, 9, 15),
                    ProjectId = 3,
                    CustomId = "P003_R14"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 15,
                    Name = "Risk of System Failure",
                    Description = "Test project risk",
                    Category = Category.ProjectRisks,
                    Threat = "Impossibility to accommodate anyone",
                    Starters = "Not every part of the system has been tested",
                    Reaction = "Test the system as a whole but also in parts",
                    Owner = "Ing. Jakub Malik",
                    Probability = Probability.Two,
                    Consequences = Consequences.Seven,
                    RiskEvaluation = ((int)Probability.Four) * ((int)Consequences.Seven),
                    State = State.Concept,
                    CreatedDate = new DateTime(2021, 9, 21),
                    ModifiedStateDate = new DateTime(2021, 8, 23),
                    ReactionDate = new DateTime(2021, 12, 6),
                    ProjectId = 3,
                    CustomId = "P003_R15"

                });
                entity.HasData(new RiskEntity()
                {
                    Id = 16,
                    Name = "Risk of National Crisis",
                    Description = "Test extern risk",
                    Category = Category.ExternRisks,
                    Threat = "Lost most of money",
                    Starters = "People stopped going on trips with accommodation",
                    Reaction = "Create a reserve fund",
                    Owner = "Ing. Jakub Malik",
                    Probability = Probability.Four,
                    Consequences = Consequences.Eight,
                    RiskEvaluation = ((int)Probability.Four) * ((int)Consequences.Eight),
                    State = State.Potential,
                    CreatedDate = new DateTime(2021, 6, 19),
                    ModifiedStateDate = new DateTime(2021, 7, 12),
                    ReactionDate = new DateTime(2022, 1, 5),
                    ProjectId = 3,
                    CustomId = "P003_R16"

                });

            });
        }

       
    }
}
