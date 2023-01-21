using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Seeds;

namespace MovieDatabase.DAL
{
    public class MovieDatabaseDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<MovieActor> MovieActor { get; set; }
        public DbSet<MovieDirector> MovieDirector { get; set; }

        /// <summary>Standard empty constructor for normal instantiation</summary>
        public MovieDatabaseDbContext()
        { }
        /// <summary>Constructor for implementation of IDesignTimeDbContextFactory</summary>
        public MovieDatabaseDbContext(DbContextOptions contextOptions) : base(contextOptions)
        { }

        /// <summary>Sets up mappings and seeds the database</summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Many to Many relationship mapping
            //movies and actors
            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.PersonId, ma.MovieId });
            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Person)
                .WithMany(person => person.MoviesPlayedIn)
                .HasForeignKey(ma => ma.PersonId);
            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(movie => movie.Actors)
                .HasForeignKey(ma => ma.MovieId);

            //movies and directors
            modelBuilder.Entity<MovieDirector>()
                .HasKey(md => new { md.PersonId, md.MovieId });
            modelBuilder.Entity<MovieDirector>()
                .HasOne(md => md.Person)
                .WithMany(person => person.MoviesDirected)
                .HasForeignKey(md => md.PersonId);
            modelBuilder.Entity<MovieDirector>()
                .HasOne(md => md.Movie)
                .WithMany(movie => movie.Directors)
                .HasForeignKey(md => md.MovieId);
            #endregion

            #region One to Many relationship mapping
            //movie and ratings
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Ratings)
                .HasForeignKey(r => r.MovieId);
            #endregion

            #region Db Seed
            MoviesSeed.Seed(modelBuilder);
            PeopleSeed.Seed(modelBuilder);
            RatingsSeed.Seed(modelBuilder);
            RelationshipsSeed.Seed(modelBuilder);
            #endregion
        }

        /// <summary>Connects to the database via appsettings connection string</summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(config.GetConnectionString("DbConnection"));
            }
        }
    }
}
