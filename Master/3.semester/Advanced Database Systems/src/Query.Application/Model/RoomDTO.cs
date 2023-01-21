namespace Hotel.Query.Application.Model;

public class RoomDTO
{
    public int Id { get; set; }
    public int NumberOfBeds { get; set; }
    public int NumberOfSideBeds { get; set; }
    public float RoomSize { get; set; }
    public int Floor { get; set; }
    public int RoomNumber { get; set; }
    public int IsCleaned { get; set; }
    public int CostPerNight { get; set; }
}