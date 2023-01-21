using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RockFests.BL.Model;
using RockFests.BL.Services;
using RockFests.DAL.Entities;
using RockFests.DAL.Enums;
using RockFests.DAL.StaticServices;

namespace RockFests.Specification.ServiceTests
{
    public class UserServiceTests : TestFixture
    {
        private UserService _userService;

        [SetUp]
        public void UserServiceSetup() => _userService = ServiceProvider.GetRequiredService<UserService>();

        [TestCase("login", "password", true)]
        [TestCase("wronglogin", "password", false)]
        [TestCase("login", "wrongpassword", false)]
        public async Task Successfully_sign_in(string login, string password, bool success)
        {
            var dbContext = GetContext();
            await dbContext.Users.AddAsync(new User
            {
                Login = "login",
                Password = Encryption.HashPassword("password")
            });
            await dbContext.SaveChangesAsync();
            
            var claimsIdentity = await _userService.SignIn(login, password);

            if (success)
            {
                var claims = claimsIdentity.Claims.ToList();
                claims.Should().HaveCount(4); 
                claims[0].Type.Should().Be(ClaimTypes.Name);
                claims[0].Value.Should().Be(login);
                claims[1].Type.Should().Be(ClaimTypes.Role);
                claims[1].Value.Should().Be(AccessRole.Admin.ToString());
                claims[2].Type.Should().Be(ClaimTypes.UserData);
                claims[2].Value.Should().Be("{\"Login\":\""+login+ "\",\"AccessRole\":0,\"FirstName\":null,\"LastName\":null,\"Email\":null,\"Phone\":null,\"EnableValidation\":true,\"Id\":1}");
                claimsIdentity.AuthenticationType.Should().Be(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            else
            {
                claimsIdentity.Should().BeNull();
            }
        }

        [TestCase("login")]
        [TestCase("user")]
        public async Task Successfully_register_user(string login)
        {
            var dbContext = GetContext();
            await dbContext.Users.AddAsync(new User { Login = "login" });
            await dbContext.SaveChangesAsync();

            if (login == "login")
            {
                (await _userService.Register(new UserDto { Login = login }, "password")).Should().BeNull();
                return;
            }

            var claimsIdentity = await _userService.Register(new UserDto
            {
                Login = login,
                AccessRole = AccessRole.Admin,
                FirstName = "Test",
                LastName = "Testovic"
            }, "password");

            var claims = claimsIdentity.Claims.ToList();
            claims.Should().HaveCount(4);
            claims[0].Type.Should().Be(ClaimTypes.Name);
            claims[0].Value.Should().Be(login);
            claims[1].Type.Should().Be(ClaimTypes.Role);
            claims[1].Value.Should().Be(AccessRole.Admin.ToString());
            claims[2].Type.Should().Be(ClaimTypes.UserData);
            claims[2].Value.Should().Be("{\"Login\":\""+login+ "\",\"AccessRole\":0,\"FirstName\":\"Test\",\"LastName\":\"Testovic\",\"Email\":null,\"Phone\":null,\"EnableValidation\":true,\"Id\":2}");
            claimsIdentity.AuthenticationType.Should().Be(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [TestCase("login")]
        [TestCase("user")]
        public async Task Successfully_create_user(string login)
        {
            var dbContext = GetContext();
            await dbContext.Users.AddAsync(new User {Login = "login"});
            await dbContext.SaveChangesAsync();

            if (login == "login")
            {
                (await _userService.Create(new UserDto { Login = login }, "password")).Should().BeNull();
                return;
            }

            var userDto = await _userService.Create(new UserDto
            {
                Login = login,
                FirstName = "Test",
                LastName = "Testovic"
            }, "password");

            userDto.Id.Should().Be(2);
            var user = await GetContext().Users.SingleAsync(x => x.Login == login);
            Assert.DoesNotThrow(() => Encryption.VerifyPasswords(user.Password, "password"));
            user.Password = null;
            user.Should().BeEquivalentTo(new User{Id = 2, Login = login, FirstName = "Test", LastName = "Testovic"});
        }

        [Test]
        public async Task Successfully_update_user()
        {
            var dbContext = GetContext();
            await dbContext.Users.AddAsync(new User { Login = "login" });
            await dbContext.SaveChangesAsync();

            await _userService.Update(new UserDto { Id = 1, Login = "NewLogin" });

            var user = await GetContext().Users.SingleAsync(x => x.Id == 1);
            user.Password = null;
            user.Should().BeEquivalentTo(new User { Id = 1, Login = "NewLogin" });
        }

        [Test]
        public async Task Successfully_update_password()
        {
            var dbContext = GetContext();
            await dbContext.Users.AddAsync(new User { Password = Encryption.HashPassword("password") });
            await dbContext.SaveChangesAsync();

            await _userService.UpdatePassword(1, "NewPassword");

            var user = await GetContext().Users.SingleAsync(x => x.Id == 1);
            Assert.DoesNotThrow(() => Encryption.VerifyPasswords(user.Password, "NewPassword"));
        }

        [Test]
        public async Task Get_existing_user()
        {
            var dbContext = GetContext();
            await dbContext.Users.AddAsync(new User { Login = "login" });
            await dbContext.SaveChangesAsync();

            var user = await _userService.GetById(1);

            user.Should().BeEquivalentTo(new UserDto{Id = 1, Login = "login"});
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var ticketDto = await _userService.GetById(1);

            ticketDto.Should().Be(null);
        }

        [Test]
        public async Task Get_all_users()
        {
            var dbContext = GetContext();
            await dbContext.Users.AddRangeAsync(new User { Login = "login1" }, new User { Login = "login2" });
            await dbContext.SaveChangesAsync();

            var users = await _userService.GetAll();

            users.Should().HaveCount(2);
            users[0].Should().BeEquivalentTo(new UserDto { Id = 1, Login = "login1" });
            users[1].Should().BeEquivalentTo(new UserDto { Id = 2, Login = "login2" });
        }

        [Test]
        public async Task Get_all_users_by_filter()
        {
            var dbContext = GetContext();
            await dbContext.Users.AddRangeAsync(
                new User { Login = "login1", Email = "", FirstName = "", LastName = "" },
                new User { Login = "login2", Email = "", FirstName = "", LastName = "" }
                );
            await dbContext.SaveChangesAsync();

            var users = await _userService.GetAll("in2");

            users.Should().HaveCount(1);
            users.Single().Should().BeEquivalentTo(new UserDto { Id = 2, Login = "login2", Email = "", FirstName = "", LastName = "" });
        }

        [Test]
        public async Task Successfully_delete_user()
        {
            var dbContext = GetContext();
            var user = await dbContext.Users.AddAsync(new User { Login = "login1" });
            await dbContext.SaveChangesAsync();

            (await dbContext.Users.CountAsync()).Should().Be(1);
            await _userService.Delete(user.Entity.Id);

            (await dbContext.Users.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await _userService.Delete(5));
    }
}
