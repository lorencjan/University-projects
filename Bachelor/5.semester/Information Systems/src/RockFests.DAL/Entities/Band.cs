using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockFests.DAL.Entities
{
    public class Band : BaseEntity
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public byte[] Photo { get; set; }
        public string Country { get; set; }
        [Column("Formation Year")]
        public int FormationYear { get; set; }
        public string Description { get; set; }

        public List<BandMusician> Members { get; set; }
        public List<BandRating> Ratings { get; set; }
        public List<Performance> Performances { get; set; }
    }
}
