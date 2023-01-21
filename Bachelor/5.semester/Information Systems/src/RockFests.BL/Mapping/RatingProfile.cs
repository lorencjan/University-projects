using AutoMapper;
using RockFests.BL.Model;
using RockFests.DAL.Entities;

namespace RockFests.BL.Mapping
{
    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMap<BandRating, RatingDto>()
                .ForMember(x => x.InterpretId, expr => expr.MapFrom(r => r.BandId))
                .ForMember(x => x.InterpretName, expr => expr.MapFrom(r => r.Band == null ? null : r.Band.Name));

            CreateMap<RatingDto, BandRating>()
                .ForMember(x => x.BandId, expr => expr.MapFrom(r => r.InterpretId));

            CreateMap<MusicianRating, RatingDto>()
                .ForMember(x => x.InterpretId, expr => expr.MapFrom(r => r.MusicianId))
                .ForMember(x => x.InterpretName, expr => expr.MapFrom(r => r.Musician == null ? null : $"{r.Musician.FirstName} {r.Musician.LastName}"));

            CreateMap<RatingDto, MusicianRating>()
                .ForMember(x => x.MusicianId, expr => expr.MapFrom(r => r.InterpretId));
        }
    }
}
