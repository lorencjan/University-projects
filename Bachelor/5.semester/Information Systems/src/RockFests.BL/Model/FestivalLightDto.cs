using RockFests.DAL.Types;

namespace RockFests.BL.Model
{
    public class FestivalLightDto : BaseDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTimeInterval Date { get; set; }
        public int Price { get; set; }
        public string Currency { get; set; }
    }
}
