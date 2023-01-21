using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RockFests.DAL.Entities;
using RockFests.DAL.Seeds;

namespace RockFests.DAL
{
    public class RockFestsDbContext : DbContext, IDesignTimeDbContextFactory<RockFestsDbContext>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Band> Bands { get; set; }
        public DbSet<Musician> Musicians { get; set; }
        public DbSet<Festival> Festivals { get; set; }
        public DbSet<Performance> Performances { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<BandRating> BandRatings { get; set; }
        public DbSet<MusicianRating> MusicianRatings { get; set; }
        public DbSet<BandMusician> BandMusicians { get; set; }

        public RockFestsDbContext() {}

        public RockFestsDbContext(DbContextOptions contextOptions) : base(contextOptions) {}

        RockFestsDbContext IDesignTimeDbContextFactory<RockFestsDbContext>.CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath($"{Directory.GetParent(Directory.GetCurrentDirectory()).FullName}\\RockFests")
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<RockFestsDbContext>()
                .UseSqlServer(config.GetConnectionString("RockFestsDb"));

            return new RockFestsDbContext(builder.Options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Many to Many relationships
            //band has multiple members and a musician can belong to multiple bands
            modelBuilder.Entity<BandMusician>()
                .HasKey(bm => new { bm.BandId, bm.MusicianId });
            modelBuilder.Entity<BandMusician>()
                .HasOne(bm => bm.Band)
                .WithMany(b => b.Members)
                .HasForeignKey(bm => bm.BandId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BandMusician>()
                .HasOne(bm => bm.Musician)
                .WithMany(m => m.Bands)
                .HasForeignKey(bm => bm.MusicianId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region One to Many relationship mapping
            // band/musician can have multiple ratings
            modelBuilder.Entity<BandRating>()
                .HasOne(r => r.Band)
                .WithMany(b => b.Ratings)
                .HasForeignKey(r => r.BandId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MusicianRating>()
                .HasOne(r => r.Musician)
                .WithMany(m => m.Ratings)
                .HasForeignKey(r => r.MusicianId)
                .OnDelete(DeleteBehavior.Cascade);

            // a festival can have multiple stages
            modelBuilder.Entity<Stage>()
                .HasOne(s => s.Festival)
                .WithMany(f => f.Stages)
                .HasForeignKey(s => s.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);

            // a stage can host multiple performances
            modelBuilder.Entity<Performance>()
                .HasOne(p => p.Stage)
                .WithMany(s => s.Performances)
                .HasForeignKey(p => p.StageId)
                .OnDelete(DeleteBehavior.Cascade);
            // band/musician have performances
            modelBuilder.Entity<Performance>()
                .HasOne(p => p.Band)
                .WithMany(b => b.Performances)
                .HasForeignKey(p => p.BandId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Performance>()
                .HasOne(p => p.Musician)
                .WithMany(m => m.Performances)
                .HasForeignKey(p => p.MusicianId)
                .OnDelete(DeleteBehavior.Cascade);

            // festival can have multiple tickets
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(m => m.Tickets)
                .HasForeignKey(t => t.UserId);
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Festival)
                .WithMany(f => f.Tickets)
                .HasForeignKey(t => t.FestivalId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Db Seed
            UsersSeed.Seed(modelBuilder);
            BandsSeed.Seed(modelBuilder);
            MusiciansSeed.Seed(modelBuilder);
            BandMusicianSeed.Seed(modelBuilder);
            RatingsSeed.Seed(modelBuilder);
            FestivalsSeed.Seed(modelBuilder);
            StagesSeed.Seed(modelBuilder);
            PerformancesSeed.Seed(modelBuilder);
            TicketsSeed.Seed(modelBuilder);
            #endregion
        }
    }
}
