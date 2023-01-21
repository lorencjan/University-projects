namespace RockFests.DAL.Entities
{
    public abstract class BaseRating : BaseEntity
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public short Number { get; set; }
    }
}
