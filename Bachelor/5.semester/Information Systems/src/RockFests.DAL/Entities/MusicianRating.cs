namespace RockFests.DAL.Entities
{
    public class MusicianRating : BaseRating
    {
        public int MusicianId { get; set; }
        public Musician Musician { get; set; }
    }
}
