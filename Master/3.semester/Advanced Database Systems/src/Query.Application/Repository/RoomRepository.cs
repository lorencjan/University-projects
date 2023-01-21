using Hotel.Command.Persistence.Cassandra;
using Hotel.Query.Application.Model;
using Hotel.Query.Application.RowSetExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hotel.Query.Application.Repository;

public static class RoomRepository
{
    private static string GetRoomByIdQuery = "SELECT * FROM hotel.rooms WHERE id = ? ALLOW FILTERING;";
    private static string GetUncleanedRoomsQuery = "SELECT * FROM hotel.uncleaned;";
    private static string GetUnavailableRoomsQuery = "SELECT roomId FROM hotel.reservationsByRooms WHERE fromDate <= ? AND toDate >= ? ALLOW FILTERING;";
    private static string FilterRoomsByParamsBaseQuery = "SELECT * FROM hotel.rooms";


    public static RoomDTO GetRoomById(int id)
    {
        using var db = new HotelContextCassandra();
        var result = RowSetMapper.MapToRooms(db.ExecutePrepare(GetRoomByIdQuery, new object[] { id }));
        return result.Count == 1 ? result.First() : null;
    }

    public static List<RoomDTO> GetUncleanedRooms()
    {
        using var db = new HotelContextCassandra();
        return RowSetMapper.MapToUncleanedRooms(db.Execute(GetUncleanedRoomsQuery));
    }

    public static List<RoomDTO> GetAllRooms(int? floor, int? costPerNight, int? numberOfBeds, int? numberOfSideBeds)
    {
        using var db = new HotelContextCassandra();
        var filterRoomsByParamsQuery = FilterRoomsByParamsBaseQuery;

        if (floor is not null || costPerNight is not null|| numberOfBeds is not null || numberOfSideBeds is not null)
            UseFilter(ref filterRoomsByParamsQuery, floor, costPerNight, numberOfBeds, numberOfSideBeds  );

        return RowSetMapper.MapToRooms(db.Execute(filterRoomsByParamsQuery));
    }

    private static void UseFilter(ref string filterRoomsByParamsQuery, int? floor, int? costPerNight, int? numberOfBeds, int? numberOfSideBeds)
    {
        filterRoomsByParamsQuery += " WHERE";

        var parameters = new Dictionary<string, int?>
        {
            { nameof(floor), floor },
            { nameof(costPerNight), costPerNight},
            { nameof(numberOfBeds), numberOfBeds},
            { nameof(numberOfSideBeds), numberOfSideBeds },
        };
        var insertAND = false;

        foreach (var p in parameters.Where(p => p.Value is not null)) {
            if(insertAND)
                filterRoomsByParamsQuery += $" AND";
            else
                insertAND = true;

            filterRoomsByParamsQuery += $" {p.Key} = {p.Value}";
        }

        filterRoomsByParamsQuery += " ALLOW FILTERING";
    }

    public static List<RoomDTO> AvailableRooms(DateTime? fromDate, DateTime? toDate)
    {
        var rooms = new List<RoomDTO>();
        using var db = new HotelContextCassandra();
        fromDate ??= DateTime.Now;
        toDate ??= DateTime.Now.AddDays(1);

        var fromLocal = new Cassandra.LocalDate(fromDate.Value.Year, fromDate.Value.Month, fromDate.Value.Day);
        var toLocal = new Cassandra.LocalDate(toDate.Value.Year, toDate.Value.Month, toDate.Value.Day);


        var unavailableRooms = RowSetMapper.MapIds(db.ExecutePrepare(GetUnavailableRoomsQuery, new object[] { fromLocal, toLocal }), "roomid");
        var allRooms = GetAllRooms(null, null, null, null);

        rooms = allRooms.Where(x => !unavailableRooms.Contains(x.Id)).ToList();
        return rooms;
    }
}