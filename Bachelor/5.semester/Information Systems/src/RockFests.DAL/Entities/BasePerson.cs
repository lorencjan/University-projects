using System.ComponentModel.DataAnnotations.Schema;

namespace RockFests.DAL.Entities
{
    public abstract class BasePerson : BaseEntity
    {
        [Column("First Name")]
        public string FirstName { get; set; }
        [Column("Last Name")]
        public string LastName { get; set; }
    }
}
