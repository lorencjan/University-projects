namespace Hotel.Command.Persistence.Sql.Entities;

public class Room : EntityBase
{
    public int NumberOfBeds { get; set; }
    public int NumberOfSideBeds { get; set; }
    public double Size { get; set; }
    public int Floor { get; set; }
    public int RoomNumber { get; set; }
    public int CostPerNight { get; set; }
    public bool IsCleaned { get; set; }
}