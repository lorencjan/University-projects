using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RockFests.BL.Model;
using RockFests.DAL;
using RockFests.DAL.Attributes;
using RockFests.DAL.Entities;
using RockFests.DAL.StaticServices;

namespace RockFests.BL.Services
{
    [RegisterService]
    public class UserService
    {
        private readonly RockFestsDbContext _dbContext;

        public UserService(RockFestsDbContext rockFestsDbContext)
            => _dbContext = rockFestsDbContext;

        public async Task<ClaimsIdentity> SignIn(string userName, string password)
        {
            var user = await VerifyCredentials(userName, password);
            return user == null ? null : CreateIdentity(user);
        }

        public async Task<ClaimsIdentity> Register(UserDto userDto, string password)
        {
            userDto = await Create(userDto, password);
            return userDto == null ? null : CreateIdentity(userDto);
        }

        public static ClaimsIdentity CreateIdentity(UserDto user)
        {
            return new ClaimsIdentity(
                new[] {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.Role, user.AccessRole.ToString()),
                    new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(user)),
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddHours(1).ToString()) 
                },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private async Task<UserDto> VerifyCredentials(string userName, string password)
        {
            try
            {
                var user = await _dbContext.Users.SingleAsync(x => EF.Functions.Like(x.Login, userName));
                Encryption.VerifyPasswords(user.Password, password);
                return Mapper.Map<UserDto>(user);
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserDto> Create(UserDto userDto, string password)
        {
            if (await _dbContext.Users.AnyAsync(x => !string.IsNullOrEmpty(x.Login) && EF.Functions.Like(x.Login, userDto.Login)))
                return null;

            var user = Mapper.Map<User>(userDto);
            if (password != null)
            {
                user.Password = Encryption.HashPassword(password);
            }
            user = (await _dbContext.Users.AddAsync(user)).Entity;
            await _dbContext.SaveChangesAsync();
            return Mapper.Map<UserDto>(user);
        }

        public async Task Update(UserDto userDto)
        {
            var user = await _dbContext.Users.FindAsync(userDto.Id);
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Login = userDto.Login;
            user.Email = userDto.Email;
            user.Phone = userDto.Phone;
            user.AccessRole = userDto.AccessRole;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePassword(int userId, string password)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            user.Password = Encryption.HashPassword(password);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return;
            }

            _dbContext.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            return user == null ? null : Mapper.Map<UserDto>(user);
        }

        public async Task<List<UserDto>> GetAll(string filter = null)
        {
            var users = await _dbContext.Users
                .Where(x => !string.IsNullOrWhiteSpace(x.Login))
                .AsNoTracking().ProjectTo<UserDto>()
                .ToListAsync();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                users = users.Where(x => x.Login.ToLower().Contains(filter.ToLower()) ||
                                         x.FirstName.ToLower().Contains(filter.ToLower()) || x.LastName.ToLower().Contains(filter.ToLower()) ||
                                         x.Email.ToLower().Contains(filter.ToLower()) || x.AccessRole.ToString().ToLower().Contains(filter.ToLower()) ||
                                         !string.IsNullOrWhiteSpace(x.Phone) && x.Phone.ToLower().Contains(filter.ToLower())).ToList();
            }
            return users;
        }
    }
}
