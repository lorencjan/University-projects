using Hotel.Command.Persistence.Sql.Entities.Enums;

namespace Hotel.Command.Persistence.Sql.Entities;

public class Client: EntityBase
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string IdentityCardNumber { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string City { get; set; }
    public int Zip { get; set; }
    public string Country { get; set; }
    public Sex Sex { get; set; }
}