using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Client;
using BooksWeb.DAL.Services;
using BooksWeb.Resources;

namespace BooksWeb.ViewModels
{
    public class FavouritesAuthorsViewModel : MasterPageViewModel
    {
        private readonly UsersService _usersService;

        public List<AuthorLight> Authors { get; set; }

        public FavouritesAuthorsViewModel(UsersService usersService) => _usersService = usersService;

        public override async Task Load()
        {
            if (!SignedInId.HasValue)
            {
                await GetSignedUserId(_usersService);
            }
            Authors = await _usersService.GetFavouriteAuthors(SignedInId.Value);
            await base.Load();
            HighlightedNavItem = Routes.Favourites_authors;
        }
    }
}