using Hotel.Command.Persistence.Cassandra;
using Hotel.Command.Persistence.Sql;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Hotel.ProjectionEngine.Application;

public class ProjectRepository
{
    public static string InsertClientQuery = "INSERT INTO hotel.clients (id, name, lastName, email, phone, identityCardNumber) VALUES(?, ?, ?, ?, ?, ?);";
    public static string InsertIntoReservationByClients = "INSERT INTO hotel.reservationsByClients (id, numberOfPersons, price, paid, reservationState, note, fromDate, toDate, roomId, clientId) " +
                                                          "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
    public static string InsertIntoReservationByRooms = "INSERT INTO hotel.reservationsByRooms (id, numberOfPersons, price, paid, reservationState, note, fromDate, toDate, roomId, clientId) " +
                                                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
    public static string InsertRoomQuery = "INSERT INTO hotel.rooms (id, numberOfBeds, numberOfSideBeds, roomSize, floor, roomNumber, isCleaned, costPerNight) VALUES(?, ?, ?, ?, ?, ?, ?, ?);";

    public static string DeleteClientQuery = @"DELETE FROM hotel.clients WHERE id = ?;";
    public static string DeleteReservationByClientsQuery = @"DELETE FROM hotel.reservationsByClients WHERE id = ?;";
    public static string DeleteReservationByRoomsQuery = @"DELETE FROM hotel.reservationsByRooms WHERE id = ?;";
    public static string DeleteRoomQuery = @"DELETE FROM hotel.rooms WHERE id = ?;";

    private HotelContext _dbContext;

    public ProjectRepository(HotelContext context)
    {
        _dbContext = context;
    }

    public void ProjectClients()
    {
        //take from SSQL
        var clientsDb = _dbContext.Clients;

        //INSERT into NO-SQL
        using (var dbCass = new HotelContextCassandra())
        {
            var prepared = dbCass.Prepare(InsertClientQuery);

            foreach (var c in clientsDb)
            {
                dbCass.ExecutePrepare(prepared, new object[] { c.Id, c.Name, c.LastName, c.Email, c.Phone, c.IdentityCardNumber });
            }
        }
    }

    public void ProjectReservations()
    {
        //take from SSQL
        var reservationsDb = _dbContext.Reservations.Include(e=>e.Rooms).ToList();

        //INSERT into NO-SQL
        using (var dbCass = new HotelContextCassandra())
        {
            var preparedByClients = dbCass.Prepare(InsertIntoReservationByClients);
            var preparedByRooms = dbCass.Prepare(InsertIntoReservationByRooms);

            foreach (var res in reservationsDb)
            {
                var reservationState = (int)res.State;
                foreach (var room in res?.Rooms)
                {
                    var fromLocal = new Cassandra.LocalDate(room.From.Year, room.From.Month, room.From.Day);
                    var toLocal = new Cassandra.LocalDate(room.To.Year, room.To.Month, room.To.Day);

                    dbCass.ExecutePrepare(preparedByClients, new object[] { res.Id, res.NumberOfPeople, res.Price, res.Paid, reservationState, res.Note, fromLocal, toLocal, room.RoomId, res.ClientId });
                    dbCass.ExecutePrepare(preparedByRooms, new object[] { res.Id, res.NumberOfPeople, res.Price, res.Paid, reservationState, res.Note, fromLocal, toLocal, room.RoomId, res.ClientId });
                }
            }
        }
    }

    public void ProjectRooms()
    {
        //take from SSQL
        var roomsDb = _dbContext.Rooms;

        //INSERT into NO-SQL
        using (var dbCass = new HotelContextCassandra())
        {
            var prepared = dbCass.Prepare(InsertRoomQuery);

            foreach (var r in roomsDb)
            {
                var IsCleaned = r.IsCleaned ? 1 : 0;
                dbCass.ExecutePrepare(prepared, new object[] { r.Id, r.NumberOfBeds, r.NumberOfSideBeds, (float)r.Size, r.Floor, r.RoomNumber, IsCleaned, r.CostPerNight });
            }
        }
    }

    public void ProjectRoomUpdate(int id)
    {
        //take from SSQL
        var roomDb = _dbContext.Rooms.Where(x=>x.Id == id).FirstOrDefault();
        if (roomDb is null)
            return;

        DeleteEntity(EntityNames.room, id);

        //INSERT into NO-SQL
        using (var dbCass = new HotelContextCassandra())
        {
            var prepared = dbCass.Prepare(InsertRoomQuery);
            
            var IsCleaned = roomDb.IsCleaned ? 1 : 0;
            dbCass.ExecutePrepare(prepared, new object[] { roomDb.Id, roomDb.NumberOfBeds, roomDb.NumberOfSideBeds, (float)roomDb.Size, roomDb.Floor, roomDb.RoomNumber, IsCleaned, roomDb.CostPerNight });
            
        }
    }

    public void DeleteEntity(EntityNames entityName, int id)
    {
        switch (entityName)
        {
            case EntityNames.client:
                Delete(DeleteClientQuery, id);
                break;
            case EntityNames.reservation:
                Delete(DeleteReservationByClientsQuery, id);
                Delete(DeleteReservationByRoomsQuery, id);
                break;
            case EntityNames.room:
                Delete(DeleteRoomQuery, id);
                break;
        }
    }

    private void Delete(string query, int id)
    {
        using (var dbCass = new HotelContextCassandra())
        {
            dbCass.ExecutePrepare(query, new object[] { id });
        }
    }

    public enum EntityNames
    {
        client = 1,
        reservation = 2,
        room = 3
    }

}