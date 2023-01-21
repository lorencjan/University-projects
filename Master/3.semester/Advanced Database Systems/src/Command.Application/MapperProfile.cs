using AutoMapper;
using Hotel.Command.Application.Clients.Dtos;
using Hotel.Command.Application.Reservations.Dtos;
using Hotel.Command.Application.Rooms.Dtos;
using Hotel.Command.Persistence.Sql.Entities;

namespace Hotel.Command.Application
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ClientDto, Client>();
            CreateMap<UpdateClientDto, Client>();
            CreateMap<RoomDto, Room>();
            CreateMap<RoomReservationDto, RoomReservation>();
            CreateMap<ReservationDto, Reservation>();
        }
    }
}