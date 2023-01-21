using System;

namespace Hotel.Command.Application.Reservations.Dtos;

public class RoomReservationDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int RoomId { get; set; }
}