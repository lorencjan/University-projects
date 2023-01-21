using BooksWpf.DAL.Services;

namespace BooksWpf.ViewModels
{
    public class ViewModelLocator
    {
        public BooksViewModel BooksViewModel { get; }
        public AuthorsViewModel AuthorsViewModel { get; }
        public NewsPostsViewModel NewsPostsViewModel { get; }
        public UsersViewModel UsersViewModel { get; }
        public FeedbacksViewModel FeedbacksViewModel { get; }
        public MainViewModel MainViewModel { get; }
        public ViewModelLocator()
        {
            BooksViewModel = new BooksViewModel(new BookService(), new AuthorService());
            AuthorsViewModel = new AuthorsViewModel(new AuthorService());
            NewsPostsViewModel = new NewsPostsViewModel(new NewsPostsService());
            UsersViewModel = new UsersViewModel(new UsersService());
            FeedbacksViewModel = new FeedbacksViewModel(new UsersService());
            MainViewModel = new MainViewModel(BooksViewModel, AuthorsViewModel, NewsPostsViewModel, UsersViewModel, FeedbacksViewModel);
        }
    }
}
