using Hotel.Command.Persistence.Sql.Entities.Enums;
using System.Collections.Generic;

namespace Hotel.Command.Application.Reservations.Dtos;

public class ReservationDto
{
    public int NumberOfPeople { get; set; }
    public int Price { get; set; }
    public int Paid { get; set; }
    public ReservationState State { get; set; }
    public string Note { get; set; }

    public int ClientId { get; set; }
    public List<RoomReservationDto> Rooms { get; set; }
}