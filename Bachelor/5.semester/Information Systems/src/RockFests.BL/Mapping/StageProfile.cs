using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RockFests.BL.Model;
using RockFests.DAL.Entities;

namespace RockFests.BL.Mapping
{
    public class StageProfile : Profile
    {
        public StageProfile()
        {
            CreateMap<Stage, StageDto>()
                .ForMember(x => x.Performances, expr =>
                    expr.MapFrom(s => s.Performances == null ? new List<PerformanceDto>() : s.Performances.Select(Mapper.Map<PerformanceDto>)));

            CreateMap<StageDto, Stage>()
                .ForMember(x => x.Performances, expr =>
                    expr.MapFrom(s => s.Performances == null ? new List<Performance>() : s.Performances.Select(Mapper.Map<Performance>)));
        }
    }
}
