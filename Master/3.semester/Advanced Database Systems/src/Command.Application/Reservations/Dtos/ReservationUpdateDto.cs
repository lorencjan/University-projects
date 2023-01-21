using Hotel.Command.Persistence.Sql.Entities.Enums;

namespace Hotel.Command.Application.Reservations.Dtos;

public class ReservationUpdateDto
{
    public int Paid { get; set; }
    public ReservationState State { get; set; }
}