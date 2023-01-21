using Hotel.Command.Persistence.Sql.Entities;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Command.Persistence.Sql;

internal class HotelContextFactory : IDesignTimeDbContextFactory<HotelContext>
{
    HotelContext IDesignTimeDbContextFactory<HotelContext>.CreateDbContext(string[] args)
    {
        var ctxConf = new DbContextOptionsBuilder<HotelContext>()
            .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB; Initial Catalog = BooksDb; MultipleActiveResultSets = True; Integrated Security = True;");

        return new HotelContext(ctxConf.Options);
    }
}

public class HotelContext : DbContext
{
    public HotelContext(DbContextOptions<HotelContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>().HasOne(x => x.Client);
        modelBuilder.Entity<Reservation>().HasMany(x => x.Rooms).WithOne(x => x.Reservation).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RoomReservation>().HasOne(x => x.Reservation);
        modelBuilder.Entity<RoomReservation>().HasOne(x => x.Room);
    }
}