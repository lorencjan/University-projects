namespace Hotel.Command.Application.Rooms.Dtos;

public class RoomDto
{
    public int NumberOfBeds { get; set; }
    public int NumberOfSideBeds { get; set; }
    public double Size { get; set; }
    public int Floor { get; set; }
    public int RoomNumber { get; set; }
    public int CostPerNight { get; set; }
    public bool IsCleaned { get; set; }
}