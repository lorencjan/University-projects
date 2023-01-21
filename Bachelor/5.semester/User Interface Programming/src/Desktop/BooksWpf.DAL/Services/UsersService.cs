using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWpf.DAL.Providers;

namespace BooksWpf.DAL.Services
{
    public class UsersService
    {
        private readonly UsersClient _userClient;

        public UsersService() => _userClient = new ClientProvider().GetUsersClient();

        public async Task<ObservableCollection<UserLight>> GetUsersLight(string userFilter, SortUsersBy? sortUsersBy=null, SortOrdering? sortingOrder = null)
        {
            try
            {
                return new ObservableCollection<UserLight>(await _userClient.GetUsersLightAsync(userFilter, sortUsersBy, sortingOrder));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to load users light via UserClient");
                return new ObservableCollection<UserLight>();
            }
        }

        public async Task<User> GetById(int id)
        {
            try
            {
                return await _userClient.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to load user by id via UserClient");
                return new User();
            }
        }

        public async Task Create(string text)
        {
            try
            {
                await _userClient.CreateAsync(text);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to create user via UserClient");
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _userClient.DeleteAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to delete user via UserClient");
            }
        }

        public async Task<ObservableCollection<Feedback>> GetFeedbacks()
        {
            try
            {
                return new ObservableCollection<Feedback>(await _userClient.GetFeedbacksAsync());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to get feedbacks via UserClient");
                return new ObservableCollection<Feedback>();
            }
        }

        public async Task DeleteFeedback(int id)
        {
            try
            {
                await _userClient.DeleteFeedbackAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to delete feedback via UserClient");
            }
        }


    }
}
