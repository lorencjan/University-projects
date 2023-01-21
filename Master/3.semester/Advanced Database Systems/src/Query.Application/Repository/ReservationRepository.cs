using Hotel.Command.Persistence.Cassandra;
using Hotel.Query.Application.Model;
using Hotel.Query.Application.RowSetExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Hotel.Query.Application.Repository;

public static class ReservationRepository
{
    private static string GetAllReservationsQuery = "SELECT * FROM hotel.reservationsByRooms;";
    private static string GetReservationByIdQuery = "SELECT * FROM hotel.reservationsByRooms WHERE id = ? ALLOW FILTERING;";
    private static string GetAllClientReservationsQuery = "SELECT * FROM hotel.reservationsByClients WHERE clientId = ? ALLOW FILTERING;";
    private static string GetAllRoomReservationsQuery = "SELECT * FROM hotel.reservationsByRooms WHERE roomId = ? ALLOW FILTERING;";

    public static List<ReservationDTO> GetAllReservations()
    {
        using var db = new HotelContextCassandra();
        return RowSetMapper.MapToReservations(db.Execute(GetAllReservationsQuery));
    }

    public static ReservationDTO GetReservationById(int id)
    {
        using var db = new HotelContextCassandra();
        var result = RowSetMapper.MapToReservations(db.ExecutePrepare(GetReservationByIdQuery, new object[] { id }));
        return result.Count == 1 ? result.First() : null;
    }

    public static List<ReservationDTO> GetAllReservationsOfClient(int clientId)
    {
        using var db = new HotelContextCassandra();
        return RowSetMapper.MapToReservations(db.ExecutePrepare(GetAllClientReservationsQuery, new object[] { clientId }));
    }

    public static List<ReservationDTO> GetAllRoomReservations(int roomId)
    {
        using var db = new HotelContextCassandra();
        return RowSetMapper.MapToReservations(db.ExecutePrepare(GetAllRoomReservationsQuery, new object[] { roomId }));
    }
}