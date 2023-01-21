using System;

namespace Hotel.Command.Persistence.Sql.Entities;

public class RoomReservation : EntityBase
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }

    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; }

    public int RoomId { get; set; }
    public Room Room { get; set; }
}