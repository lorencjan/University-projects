using System;

namespace Hotel.Query.Application.Model;

public class ReservationDTO
{
    public int Id { get; set; }
    public int NumberOfPersons { get; set; }
    public int Price { get; set; }
    public int Paid { get; set; }
    public int ReservationState { get; set; } 
    public string Note { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int ClientId { get; set; }
}