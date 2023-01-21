using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RockFests.BL.Model;
using RockFests.DAL.Entities;
using RockFests.DAL.Types;

namespace RockFests.BL.Mapping
{
    public class FestivalProfile : Profile
    {
        public FestivalProfile()
        {
            CreateMap<Festival, FestivalDto>()
                .ForMember(x => x.Date, expr => expr.MapFrom(f => new DateTimeInterval(f.StartDate, f.EndDate)))
                .ForMember(x => x.Stages, expr => expr.MapFrom(f => f.Stages == null ? new List<StageDto>() : f.Stages.Select(Mapper.Map<StageDto>)))
                .ForMember(x => x.Tickets, expr => expr.MapFrom(f => f.Tickets == null ? new List<TicketDto>() : f.Tickets.Select(Mapper.Map<TicketDto>)));

            CreateMap<Festival, FestivalLightDto>()
                .ForMember(x => x.Date, expr => expr.MapFrom(f => new DateTimeInterval(f.StartDate, f.EndDate)));

            CreateMap<FestivalDto, Festival>()
                .ForMember(x => x.StartDate, expr => expr.MapFrom(p => p.Date.Start))
                .ForMember(x => x.EndDate, expr => expr.MapFrom(p => p.Date.End))
                .ForMember(x => x.Stages, expr => expr.Ignore())
                .ForMember(x => x.Tickets, expr => expr.Ignore());
        }
    }
}
