using System.Collections.Generic;

namespace RockFests.DAL.Entities
{
    public class Stage : BaseEntity
    {
        public string Name { get; set; }

        public int FestivalId { get; set; }
        public Festival Festival { get; set; }

        public List<Performance> Performances { get; set; }
    }
}
