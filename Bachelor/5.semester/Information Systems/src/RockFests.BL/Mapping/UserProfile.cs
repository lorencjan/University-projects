using AutoMapper;
using RockFests.BL.Model;
using RockFests.DAL.Entities;

namespace RockFests.BL.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
