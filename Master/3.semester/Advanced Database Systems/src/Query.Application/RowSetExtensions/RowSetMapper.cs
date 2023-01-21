using Cassandra;
using Hotel.Query.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hotel.Query.Application.RowSetExtensions;

public static class RowSetMapper
{
    public static List<ClientDTO> MapToClients(RowSet rowSet) =>
        rowSet.Select(
            row => new ClientDTO {
                Id = row.GetValue<int>("id"),
                Name = row.GetValue<string>("name"),
                LastName = row.GetValue<string>("lastname"),
                Email = row.GetValue<string>("email"),
                Phone = row.GetValue<string>("phone"),
                IdentityCardNumber = row.GetValue<string>("identitycardnumber"),
            }).ToList();

    public static List<ReservationDTO> MapToReservations(RowSet rowSet)
    {
        var result = new List<ReservationDTO>();
        foreach (var row in rowSet)
        {
            var reservation = new ReservationDTO
            {
                Id = row.GetValue<int>("id"),
                NumberOfPersons = row.GetValue<int>("numberofpersons"),
                Price = row.GetValue<int>("price"),
                Paid = row.GetValue<int>("paid"),
                ReservationState = row.GetValue<int>("reservationstate"),
                Note = row.GetValue<string>("note"),
                ClientId = row.GetValue<int>("clientid")
            };

            var localDateFrom = row.GetValue<LocalDate>("fromdate");
            var localDateTo = row.GetValue<LocalDate>("todate");
            reservation.From = new DateTime(localDateFrom.Year, localDateFrom.Month, localDateFrom.Day);
            reservation.To = new DateTime(localDateTo.Year, localDateTo.Month, localDateTo.Day);

            result.Add(reservation);
        }


        return result;
    }

    public static List<RoomDTO> MapToRooms(RowSet rowSet) =>
        rowSet.Select(
            row => new RoomDTO {
                Id = row.GetValue<int>("id"),
                NumberOfBeds = row.GetValue<int>("numberofbeds"),
                NumberOfSideBeds = row.GetValue<int>("numberofsidebeds"),
                RoomSize = row.GetValue<float>("roomsize"),
                Floor = row.GetValue<int>("floor"),
                RoomNumber = row.GetValue<int>("roomnumber"),
                IsCleaned = row.GetValue<int>("iscleaned"),
                CostPerNight = row.GetValue<int>("costpernight")
            }).ToList();

    public static List<RoomDTO> MapToUncleanedRooms(RowSet rowSet) =>
        rowSet.Select(row => new RoomDTO { Id = row.GetValue<int>("id"), Floor = row.GetValue<int>("floor"), RoomNumber = row.GetValue<int>("roomnumber"), }).ToList();

    public static List<int> MapIds(RowSet rowSet, string columnName) => rowSet.Select(row => row.GetValue<int>(columnName)).ToList();
}