using RockFests.DAL.Types;

namespace RockFests.BL.Model
{
    public class PerformanceDto : BaseDto
    {
        public DateTimeInterval Time { get; set; }
        public int StageId { get; set; }
        public bool IsBand { get; set; }
        public InterpretDto Interpret { get; set; }
    }
}
