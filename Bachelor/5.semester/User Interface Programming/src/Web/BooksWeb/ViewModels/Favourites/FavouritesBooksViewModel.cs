using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Client;
using BooksWeb.DAL.Services;
using BooksWeb.Resources;

namespace BooksWeb.ViewModels
{
    public class FavouritesBooksViewModel : MasterPageViewModel
    {
        private readonly UsersService _usersService;

        public List<BookLight> Books { get; set; }

        public FavouritesBooksViewModel(UsersService usersService) => _usersService = usersService;

        public override async Task Load()
        {
            if (!SignedInId.HasValue)
            {
                await GetSignedUserId(_usersService);
            }
            Books = await _usersService.GetFavouriteBooks(SignedInId.Value);
            await base.Load();
            HighlightedNavItem = Routes.Favourites_books;
        }
    }
}