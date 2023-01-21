namespace RockFests.DAL.Entities
{
    public class BandMusician
    {
        public int BandId { get; set; }
        public Band Band { get; set; }

        public int MusicianId { get; set; }
        public Musician Musician { get; set; }
    }
}
