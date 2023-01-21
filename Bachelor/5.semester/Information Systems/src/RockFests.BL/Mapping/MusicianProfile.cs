using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RockFests.BL.Model;
using RockFests.DAL.Entities;

namespace RockFests.BL.Mapping
{
    public class MusicianProfile : Profile
    {
        public MusicianProfile()
        {
            CreateMap<Musician, MusicianDto>()
                .ForMember(x => x.Bands, expr =>
                    expr.MapFrom(x => x.Bands == null ? new List<BandLightDto>() : x.Bands.Select(bm => Mapper.Map<BandLightDto>(bm.Band))))
                .ForMember(x => x.Ratings, expr =>
                    expr.MapFrom(x => x.Ratings == null ? new List<RatingDto>() : x.Ratings.Select(Mapper.Map<RatingDto>)))
                .ForMember(x => x.Performances, expr =>
                    expr.MapFrom(x => x.Performances == null ? new List<PerformanceDto>() : x.Performances.Select(Mapper.Map<PerformanceDto>)));
            
            CreateMap<Musician, MusicianLightDto>()
                .ForMember(x => x.Name,
                    expr => expr.MapFrom(x => $"{x.FirstName} {x.LastName}"))
                .ForMember(x => x.Rating,
                    expr => expr.MapFrom(x => x.Ratings == null ? 0 : (double)x.Ratings.Sum(r => r.Number) / x.Ratings.Count));

            CreateMap<MusicianDto, Musician>()
                .ForMember(x => x.Bands, expr => expr.Ignore())
                .ForMember(x => x.Ratings, expr => expr.Ignore())
                .ForMember(x => x.Performances, expr => expr.Ignore());
        }
    }
}
