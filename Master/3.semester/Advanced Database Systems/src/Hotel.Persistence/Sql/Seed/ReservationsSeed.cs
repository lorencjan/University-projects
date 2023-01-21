using System;
using System.Collections.Generic;
using Hotel.Command.Persistence.Sql.Entities;
using Hotel.Command.Persistence.Sql.Entities.Enums;

namespace Hotel.Command.Persistence.Sql.Seed;

public class ReservationsSeed
{
    public static IEnumerable<Reservation> Data => new[]
    {
        new Reservation {
            NumberOfPeople = 1,
            Price = 390,
            Paid = 390,
            State = ReservationState.CheckIn,
            Note = "Something with a view",
            ClientId = 1,
            Rooms = new List<RoomReservation> {
                new RoomReservation {
                    RoomId = 1,
                    From = DateTime.Today.AddDays(-1),
                    To = DateTime.Today.AddDays(5)
                }
            }
        },
        new Reservation {
            NumberOfPeople = 2,
            Price = 780,
            Paid = 0,
            State = ReservationState.Reserved,
            Note = "",
            ClientId = 2,
            Rooms = new List<RoomReservation> {
                new RoomReservation {
                    RoomId = 4,
                    From = DateTime.Today.AddDays(1),
                    To = DateTime.Today.AddDays(7)
                }
            }
        },
        new Reservation {
            NumberOfPeople = 6,
            Price = 3140,
            Paid = 0,
            State = ReservationState.Reserved,
            Note = "Maybe louder during the night",
            ClientId = 2,
            Rooms = new List<RoomReservation> {
                new RoomReservation {
                    RoomId = 4,
                    From = DateTime.Today.AddDays(10),
                    To = DateTime.Today.AddDays(13)
                },
                new RoomReservation {
                    RoomId = 5,
                    From = DateTime.Today.AddDays(10),
                    To = DateTime.Today.AddDays(13)
                }
            }
        }
    };
}
