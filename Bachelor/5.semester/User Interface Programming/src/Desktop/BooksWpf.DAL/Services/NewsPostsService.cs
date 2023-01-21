using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWpf.DAL.Providers;

namespace BooksWpf.DAL.Services
{
    public class NewsPostsService
    {
        private readonly NewsPostsClient _newsPostClient;

        public NewsPostsService() => _newsPostClient = new ClientProvider().GetNewsPostsClient();


        public async Task<ObservableCollection<NewsPost>> GetNewsPosts(NewsPostFilter postFilter, BooksService.Client.SortNewsPostsBy sortBy, SortOrdering sortOrdering)
        {
            try
            {
                return new ObservableCollection<NewsPost>(await _newsPostClient.GetNewsPostsAsync(null, postFilter.Header, sortBy, sortOrdering));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to load newsPosts light via NewsPostClient");
                return new ObservableCollection<NewsPost>();
            }
        }

        public async Task<NewsPost> GetById(int id)
        {
            try
            {
                return await _newsPostClient.GetByIdAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to load newsPost by id via NewsPostClient");
                return new NewsPost();
            }
        }

        public async Task<int> Create(NewsPost newsPost)
        {
            try
            {
                return await _newsPostClient.CreateAsync(newsPost.Header, newsPost.Text);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to create newsPost via NewsPostClient");
                return 0;
            }
        }

        public async Task Update(NewsPost newsPost)
        {
            try
            {
                await _newsPostClient.UpdateAsync(newsPost);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to update newsPost via NewsPostClient");
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                await _newsPostClient.DeleteAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to delete newsPost via NewsPostClient");
            }
        }
    }
}
