using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using BooksService.Client;
using BooksService.Common;
using BooksWpf.DAL.Providers;

namespace BooksWpf.DAL.Services
{
    public class AuthorService
    {
        private readonly AuthorsClient _authorsClient;

        public AuthorService() => _authorsClient = new ClientProvider().GetAuthorsClient();

        public async Task<ObservableCollection<AuthorLight>> GetAuthorsLight(AuthorFilter filter = null, SortAuthorsBy? sortBy = null, SortOrdering? sortOrdering = null)
        {
            try
            {
                return new ObservableCollection<AuthorLight>(await _authorsClient.GetAuthorsLightAsync(filter?.Name, filter?.Country, sortBy, sortOrdering));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to load authors light via AuthorClient");
                return new ObservableCollection<AuthorLight>();
            }
        }
        public async Task<Author> GetById(int id, AuthorIncludeFeatures? includeFeatures = null)
        {
            try
            {
                return await _authorsClient.GetByIdAsync(id, AuthorIncludeFeatures.Books | AuthorIncludeFeatures.Ratings);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to load author by id via AuthorClient");
                return new Author();
            }
        }

        public async Task<int> Create(Author author)
        {
            try
            {
                return await _authorsClient.CreateAsync(author);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to create author via AuthorClient");
                return 0;
            }
        }

        public async Task Update(Author author)
        {
            try
            {
                await _authorsClient.UpdateAsync(author);
            }
            catch(Exception e)
            {
                Trace.WriteLine(e, "Failed to update author via AuthorClient");
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                await _authorsClient.DeleteAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to delete author via AuthorClient");
            }
        }

        public async Task DeleteRating(int id)
        {
            try
            {
                await _authorsClient.DeleteRatingAsync(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Failed to delete author rating via AuthorClient");
            }
        }

    }
}
