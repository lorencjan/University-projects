using System;

namespace RockFests.DAL.Entities
{
    public class Performance : BaseEntity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int StageId { get; set; }
        public Stage Stage { get; set; }

        /// <summary> Empty if Band is set </summary>
        public int? MusicianId { get; set; }
        public Musician Musician { get; set; }

        /// <summary> Empty if Musician is set </summary>
        public int? BandId { get; set; }
        public Band Band { get; set; }
    }
}
