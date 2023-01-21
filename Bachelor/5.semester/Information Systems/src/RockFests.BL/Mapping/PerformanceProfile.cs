using AutoMapper;
using RockFests.BL.Model;
using RockFests.DAL.Entities;
using RockFests.DAL.Types;

namespace RockFests.BL.Mapping
{
    public class PerformanceProfile : Profile
    {
        public PerformanceProfile()
        {
            CreateMap<Performance, PerformanceDto>()
                .ForMember(x => x.Time, expr => expr.MapFrom(p => new DateTimeInterval(p.Start, p.End)))
                .ForMember(x => x.IsBand, expr => expr.MapFrom(p => p.BandId.HasValue))
                .ForMember(x => x.Interpret, expr => expr.MapFrom(p => new InterpretDto
                {
                    Id = p.BandId ?? p.MusicianId.Value,
                    Name = p.BandId.HasValue ? p.Band.Name : $"{p.Musician.FirstName} {p.Musician.LastName}"
                }));

            CreateMap<PerformanceDto, Performance>()
                .ForMember(x => x.Start, expr => expr.MapFrom(p => p.Time.Start))
                .ForMember(x => x.End, expr => expr.MapFrom(p => p.Time.End))
                .ForMember(x => x.MusicianId, expr => expr.MapFrom(p => p.IsBand ? null : (int?)p.Interpret.Id))
                .ForMember(x => x.BandId, expr => expr.MapFrom(p => p.IsBand ? (int?)p.Interpret.Id : null));
        }
    }
}
