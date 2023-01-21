using System.Collections.Generic;
using Hotel.Command.Persistence.Sql.Entities;
using Hotel.Command.Persistence.Sql.Entities.Enums;

namespace Hotel.Command.Persistence.Sql.Seed;

public class ClientsSeed
{
    public static IEnumerable<Client> Data => new[]
    {
        new Client {
            Name = "Karel",
            LastName = "Novák",
            City = "Brno",
            Street = "Kotlářská",
            Country = "Czech republic",
            Email = "karel.novak@seznam.cz",
            HouseNumber = "405/36a",
            IdentityCardNumber = "9504197544",
            Phone = "+420605872941",
            Sex = Sex.Male,
            Zip = 60500
        },
        new Client {
            Name = "Karolína",
            LastName = "Světlá",
            City = "Lelekovice",
            Street = "Podélná",
            Country = "Czech republic",
            Email = "svetlakaja@gmail.com",
            HouseNumber = "54",
            IdentityCardNumber = "9901293665",
            Phone = "721651878",
            Sex = Sex.Female,
            Zip = 66432
        }
    };
}
