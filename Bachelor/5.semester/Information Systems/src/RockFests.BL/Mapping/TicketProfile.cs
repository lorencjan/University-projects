using AutoMapper;
using RockFests.BL.Model;
using RockFests.DAL.Entities;

namespace RockFests.BL.Mapping
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket, TicketDto>()
                .ForMember(x => x.Festival, expr => expr.MapFrom(p => Mapper.Map<FestivalLightDto>(p.Festival)))
                .ForMember(x => x.User, expr => expr.MapFrom(p => Mapper.Map<UserDto>(p.User)));

            CreateMap<TicketDto, Ticket>()
                .ForMember(x => x.FestivalId, expr => expr.MapFrom(p => p.Festival.Id))
                .ForMember(x => x.UserId, expr => expr.MapFrom(p => p.User.Id))
                .ForMember(x => x.Festival, expr => expr.Ignore())
                .ForMember(x => x.User, expr => expr.Ignore());
        }
    }
}
