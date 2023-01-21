using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksWeb.DAL.Services;
using BooksWeb.Resources;

namespace BooksWeb.ViewModels
{
    public class HomeViewModel : MasterPageViewModel
    {
        ///<summary>Nested class with formatted Date</summary>
        public class FormattedNewsPost
        {
            public string Header { get; set; }
            public string Date { get; set; }
            public string Text { get; set; }

            public FormattedNewsPost()
            {}

            public FormattedNewsPost(string header, string date, string text)
            {
                Header = header;
                Date = date;
                Text = text;
            }
        }

        private readonly NewsPostsService _newsService;
        public List<FormattedNewsPost> NewsPosts { get; set; }

        public HomeViewModel(NewsPostsService newsPostsService) => _newsService = newsPostsService;

        public override async Task Load()
        {
            try
            {
                NewsPosts = new List<FormattedNewsPost>();
                var unformattedNewsPosts = await _newsService.GetNewsPosts(5);
                foreach (var post in unformattedNewsPosts)
                {
                    NewsPosts.Add(new FormattedNewsPost(post.Header, post.Date.ToString("d"), post.Text));
                }
                await base.Load();
                HighlightedNavItem = Routes.Home;
            }
            catch (Exception e)
            {
                SetError(e, Errors.NewsLoad);
            }
        }
    }
}