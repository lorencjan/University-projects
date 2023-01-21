using System.Collections.Generic;
using RockFests.DAL.Enums;

namespace RockFests.DAL.Entities
{
    public class User : BasePerson
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public AccessRole AccessRole { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public List<Ticket> Tickets { get; set; }
    }
}
