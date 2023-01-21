using System.Collections.Generic;
using Hotel.Command.Persistence.Sql.Entities.Enums;

namespace Hotel.Command.Persistence.Sql.Entities;

public class Reservation : EntityBase
{
    public int NumberOfPeople { get; set; }
    public int Price { get; set; }
    public int Paid { get; set; }
    public ReservationState State { get; set; }
    public string Note { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; }

    public List<RoomReservation> Rooms { get; set; }
}