using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using BooksService.Client;
using BooksWeb.DAL.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Linq;

namespace BooksWeb.DAL.Services
{
    [RegisterService]
    public class UsersService
    {
        private readonly IUsersClient _usersClient;
        private readonly ILogger<UsersService> _logger;

        public UsersService(IUsersClient usersClient, ILogger<UsersService> logger)
        {
            _usersClient = usersClient;
            _logger = logger;
        }
        
        public async Task<int> Create(string login, string password)
        {
            try
            {
                return await _usersClient.CreateAsync(login, password);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create user via UsersClient");
                return -1;
            }
        }

        public async Task<int> CreateFeedback(string text, string userLogin)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return 0;
                }
                var userId = GetUsersLight(userLogin).Result.Single().Id;
                return await _usersClient.CreateFeedbackAsync(text, userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create feedback via UsersClient");
                return -1;
            }
        }

        public async Task AddFavouriteAuthor(int userId, int authorId)
        {
            try
            {
                await _usersClient.AddFavoriteAuthorAsync(userId, authorId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add favourite author via UsersClient");
            }
        }

        public async Task AddFavouriteBook(int userId, int bookId)
        {
            try
            {
                await _usersClient.AddFavoriteBookAsync(userId, bookId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add favourite book via UsersClient");
            }
        }

        public async Task<User> GetById(int id)
        {
            try
            {
                return await _usersClient.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get user by id via UsersClient");
                return null;
            }
        }

        public async Task<List<UserLight>> GetUsersLight(string filter = null)
        {
            try
            {
                return await _usersClient.GetUsersLightAsync(filter);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get users light via UsersClient");
                return null;
            }
        }

        public async Task<List<AuthorLight>> GetFavouriteAuthors(int userId)
        {
            try
            {
                return await _usersClient.GetFavoriteAuthorsAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get favourite authors via UsersClient");
                return new List<AuthorLight>();
            }
        }

        public async Task<List<BookLight>> GetFavouriteBooks(int userId)
        {
            try
            {
                return await _usersClient.GetFavoriteBooksAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get favourite authors via UsersClient");
                return new List<BookLight>();
            }
        }

        public async Task RemoveFavouriteAuthor(int userId, int authorId)
        {
            try
            {
                await _usersClient.RemoveFavoriteAuthorAsync(userId, authorId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to remove favourite author via UsersClient");
            }
        }

        public async Task RemoveFavouriteBook(int userId, int bookId)
        {
            try
            {
                await _usersClient.RemoveFavoriteBookAsync(userId, bookId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to remove favourite book via UsersClient");
            }
        }

        public async Task Delete(int userId)
        {
            try
            {
                await _usersClient.DeleteAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to delete user via UsersClient");
            }
        }
        
        public async Task ChangePassword(int userId, string password)
        {
            try
            {
                await _usersClient.ChangePasswordAsync(userId, password);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to change password via UsersClient");
            }
        }

        private async Task<bool> VerifyCredentials(string login, string password)
        {
            try
            {
                return await _usersClient.VerifyCredentialsAsync(login, password);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to login (verify credentials) via UsersClient");
                return false;
            }
        }
        
        private static ClaimsIdentity CreateIdentity(string login)
        {
            return new ClaimsIdentity(
                new[] {
                    new Claim(ClaimTypes.Name, login)
                },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<ClaimsIdentity> SignIn(string login, string password)
            => await VerifyCredentials(login, password) ? CreateIdentity(login) : null;

        public async Task<ClaimsIdentity> Register(string login, string password)
        {
            var users = await GetUsersLight(login);
            if(users.Count != 0) //if user already exists
            {
                return null;
            }
            var userId = await Create(login, password);
            return userId == -1 ? null : CreateIdentity(login);
        }
    }
}