namespace RockFests.DAL.Entities
{
    public class BandRating : BaseRating
    {
        public int BandId { get; set; }
        public Band Band { get; set; }
    }
}
