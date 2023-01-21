using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RockFests.BL.Model;
using RockFests.DAL.Entities;

namespace RockFests.BL.Mapping
{
    public class BandProfile : Profile
    {
        public BandProfile()
        {
            CreateMap<Band, BandDto>()
                .ForMember(x => x.Members, expr =>
                    expr.MapFrom(x => x.Members == null ? new List<MusicianLightDto>() : x.Members.Select(bm => Mapper.Map<MusicianLightDto>(bm.Musician))))
                .ForMember(x => x.Ratings, expr =>
                    expr.MapFrom(x => x.Ratings == null ? new List<RatingDto>() : x.Ratings.Select(Mapper.Map<RatingDto>)))
                .ForMember(x => x.Performances, expr =>
                    expr.MapFrom(x => x.Performances == null ? new List<PerformanceDto>() : x.Performances.Select(Mapper.Map<PerformanceDto>)));
            
            CreateMap<Band, BandLightDto>()
                .ForMember(x => x.Rating,
                    expr => expr.MapFrom(x => x.Ratings == null ? 0 : (double)x.Ratings.Sum(r => r.Number) / x.Ratings.Count));

            CreateMap<BandDto, Band>()
                .ForMember(x => x.Members, expr => expr.Ignore())
                .ForMember(x => x.Ratings, expr => expr.Ignore())
                .ForMember(x => x.Performances, expr => expr.Ignore());
        }
    }
}
