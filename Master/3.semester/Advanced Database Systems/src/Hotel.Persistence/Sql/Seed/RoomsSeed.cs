using System.Collections.Generic;
using Hotel.Command.Persistence.Sql.Entities;

namespace Hotel.Command.Persistence.Sql.Seed;

public class RoomsSeed
{
    public static IEnumerable<Room> Data => new[]
    {
        new Room {
            NumberOfBeds = 2,
            NumberOfSideBeds = 0,
            Size = 16.7,
            Floor = 1,
            RoomNumber = 101,
            CostPerNight = 390,
            IsCleaned = true
        },
        new Room {
            NumberOfBeds = 3,
            NumberOfSideBeds = 1,
            Size = 21.2,
            Floor = 1,
            RoomNumber = 102,
            CostPerNight = 450,
            IsCleaned = true
        },
        new Room {
            NumberOfBeds = 4,
            NumberOfSideBeds = 0,
            Size = 22.4,
            Floor = 1,
            RoomNumber = 103,
            CostPerNight = 490,
            IsCleaned = false
        },
        new Room {
            NumberOfBeds = 2,
            NumberOfSideBeds = 0,
            Size = 16.7,
            Floor = 2,
            RoomNumber = 201,
            CostPerNight = 390,
            IsCleaned = false
        },
        new Room {
            NumberOfBeds = 4,
            NumberOfSideBeds = 2,
            Size = 28.1,
            Floor = 2,
            RoomNumber = 202,
            CostPerNight = 590,
            IsCleaned = true
        }
    };
}
