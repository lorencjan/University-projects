using System.Collections.Generic;

namespace RockFests.BL.Model
{
    public class StageDto : BaseDto
    {
        public string Name { get; set; }
        public int FestivalId { get; set; }
        public List<PerformanceDto> Performances { get; set; } = new List<PerformanceDto>();

        public StageDto Copy() => new StageDto
        {
            Id = Id,
            Name = Name,
            FestivalId = FestivalId,
            Performances = new List<PerformanceDto>(Performances)
        };
    }
}
