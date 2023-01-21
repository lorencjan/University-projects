namespace RockFests.BL.Model
{
    public class RatingDto : BaseDto
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public short Number { get; set; }

        public int InterpretId { get; set; }
        public string InterpretName { get; set; }
    }
}
