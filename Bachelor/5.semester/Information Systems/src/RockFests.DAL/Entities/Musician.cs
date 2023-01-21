using System;
using System.Collections.Generic;

namespace RockFests.DAL.Entities
{
    public class Musician : BasePerson
    {
        public DateTime Born { get; set; }
        public byte[] Photo { get; set; }
        public string Country { get; set; }
        public string Role { get; set; }
        public string Biography { get; set; }

        public List<BandMusician> Bands { get; set; }
        public List<MusicianRating> Ratings { get; set; }
        public List<Performance> Performances { get; set; }
    }
}
