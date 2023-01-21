using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.DAL.Entities;
using RockFests.DAL.Enums;

namespace RockFests.Specification.MappingTests
{
    public class UserMappingTests : TestFixture
    {
        public static User User() => new User
        {
            Id = 1,
            FirstName = "Slim",
            LastName = "Shady",
            Login = "slimshady",
            Password = "passwd",
            AccessRole = AccessRole.Spectator,
            Email = "slim@gmail.com",
            Phone = "111222333",
            Tickets = new List<Ticket>{ new Ticket {Id = 1}, new Ticket{Id = 2}}
        };

        public static UserDto UserDto() => new UserDto
        {
            Id = 1,
            FirstName = "Slim",
            LastName = "Shady",
            Login = "slimshady",
            AccessRole = AccessRole.Spectator,
            Email = "slim@gmail.com",
            Phone = "111222333"
        };

        [Test]
        public void Successful_map_to_dto_object()
        {
            var dto = Mapper.Map<UserDto>(User());
            dto.Should().BeEquivalentTo(UserDto());
        }

        [Test]
        public void Successful_map_from_dto_to_entity()
        {
            var entity = User();
            entity.Password = null;
            entity.Tickets = null;

            var mappedEntity = Mapper.Map<User>(UserDto());

            mappedEntity.Should().BeEquivalentTo(entity);
        }
    }
}
