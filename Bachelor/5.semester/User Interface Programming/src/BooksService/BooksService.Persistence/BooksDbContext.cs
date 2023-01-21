using BooksService.Domain.Entities;
using BooksService.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BooksService.Persistence
{
    internal class BooksDbContextFactory : IDesignTimeDbContextFactory<BooksDbContext>
    {
        BooksDbContext IDesignTimeDbContextFactory<BooksDbContext>.CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BooksDbContext>()
                .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB; Initial Catalog = BooksDb; MultipleActiveResultSets = True; Integrated Security = True;");

            return new BooksDbContext(builder.Options);
        }
    }

    public class BooksDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookRating> BookRatings { get; set; }
        public DbSet<AuthorRating> AuthorRatings { get; set; }
        public DbSet<AuthorBook> AuthorBooks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GenreBook> GenreBooks { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<NewsPost> NewsPosts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFavoriteBook> UsersFavoriteBooks { get; set; }
        public DbSet<UserFavoriteAuthor> UsersFavoriteAuthors { get; set; }

        public BooksDbContext() { }

        public BooksDbContext(DbContextOptions<BooksDbContext> options): base(options)
        {}
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Many to Many relationships
            //author can write multiple books and a book can have multiple authors
            modelBuilder.Entity<AuthorBook>()
                .HasKey(ab => new { ab.BookId, ab.AuthorId });
            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Book)
                .WithMany(b => b.Authors)
                .HasForeignKey(ab => ab.BookId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(ab => ab.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            //book can belong to several genres and there are many books of one genre
            modelBuilder.Entity<GenreBook>()
                .HasKey(gb => new { gb.GenreId, gb.BookId });
            modelBuilder.Entity<GenreBook>()
                .HasOne(gb => gb.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(gb => gb.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GenreBook>()
                .HasOne(gb => gb.Book)
                .WithMany(b => b.Genres)
                .HasForeignKey(gb => gb.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            //user can have several favorite books and book can be favored by many users
            modelBuilder.Entity<UserFavoriteBook>()
                .HasKey(f => new { f.UserId, f.BookId });
            modelBuilder.Entity<UserFavoriteBook>()
                .HasOne(f => f.Book)
                .WithMany(b => b.FavoredBy)
                .HasForeignKey(f => f.BookId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserFavoriteBook>()
                .HasOne(f => f.User)
                .WithMany(u => u.FavoriteBooks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //user can have several favorite authors and an author can be favored by many users
            modelBuilder.Entity<UserFavoriteAuthor>()
                .HasKey(f => new { f.UserId, f.AuthorId });
            modelBuilder.Entity<UserFavoriteAuthor>()
                .HasOne(f => f.Author)
                .WithMany(b => b.FavoredBy)
                .HasForeignKey(f => f.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserFavoriteAuthor>()
                .HasOne(f => f.User)
                .WithMany(u => u.FavoriteAuthors)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region One to Many relationship mapping
            // author/book can have multiple ratings
            modelBuilder.Entity<AuthorRating>()
                .HasOne(r => r.Author)
                .WithMany(a => a.Ratings)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BookRating>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Ratings)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            //user can send several feedbacks
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId);

            //user can rate several authors/books
            modelBuilder.Entity<AuthorRating>()
                .HasOne(r => r.User)
                .WithMany(u => u.AuthorRatings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BookRating>()
                .HasOne(r => r.User)
                .WithMany(u => u.BookRatings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Seeds
            AuthorBooksSeed.Seed(modelBuilder);
            AuthorRatingsSeed.Seed(modelBuilder);
            AuthorsSeed.Seed(modelBuilder);
            BookRatingsSeed.Seed(modelBuilder);
            BooksSeed.Seed(modelBuilder);
            FeedbacksSeed.Seed(modelBuilder);
            GenreBooksSeed.Seed(modelBuilder);
            GenresSeed.Seed(modelBuilder);
            NewsPostsSeed.Seed(modelBuilder);
            UserFavoriteAuthorsSeed.Seed(modelBuilder);
            UserFavoriteBooksSeed.Seed(modelBuilder);
            UsersSeed.Seed(modelBuilder);
            #endregion
        }
    };
}
