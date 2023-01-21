using System;
using System.Threading.Tasks;
using BooksService.Application.NewsPosts;
using BooksService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class NewsPostTests : TestFixture
    {
        private NewsPost CreateNewsPost(int id = 1, string header = "Header", string text = "Test") => new NewsPost
        {
            Id = id,
            Header = header,
            Text = text
        };

        [Test]
        public async Task Successfully_create_newspost()
        {
            var newsPost = CreateNewsPost();

            var id = await GetMediator().Send(new CreateNewsPost { Header = newsPost.Header, Text = newsPost.Text });
            var created = await GetContext().NewsPosts.SingleAsync();

            id.Should().Be(1);
            created.Date.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 1, 0));
            created.Date = new DateTime();
            created.Should().BeEquivalentTo(newsPost);
        }

        [Test]
        public async Task Successfully_update_newspost()
        {
            var newsPost = CreateNewsPost();
            var ctx = GetContext();
            await ctx.NewsPosts.AddAsync(newsPost);
            await ctx.SaveChangesAsync();

            newsPost = await GetContext().NewsPosts.AsNoTracking().SingleAsync();
            newsPost.Header = "NewHeader";
            newsPost.Text = "NewText";
            await GetMediator().Send(new UpdateNewsPost { NewsPost = newsPost });

            var updated = await GetContext().NewsPosts.SingleAsync();
            updated.Should().BeEquivalentTo(newsPost);
        }

        [Test]
        public async Task Get_existing_newspost()
        {
            var newsPost = CreateNewsPost();
            var ctx = GetContext();
            await ctx.NewsPosts.AddAsync(newsPost);
            await ctx.SaveChangesAsync();

            var response = await GetMediator().Send(new GetNewsPostById { Id = 1 });

            response.Should().BeEquivalentTo(newsPost);
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var book = await GetMediator().Send(new GetNewsPostById { Id = 1 });

            book.Should().Be(null);
        }

        [Test]
        public async Task Get_all_newsposts()
        {
            var newsPost1 = CreateNewsPost(); 
            var newsPost2 = CreateNewsPost(2, "Header2", "Test2");
            var newsPost3 = CreateNewsPost(3, "header3", "Test3");
            var ctx = GetContext();
            await ctx.NewsPosts.AddRangeAsync(newsPost1, newsPost2, newsPost3);
            await ctx.SaveChangesAsync();

            var newsPosts = await GetMediator().Send(new GetNewsPosts());

            newsPosts.Should().HaveCount(3);
            newsPosts.Should().ContainEquivalentOf(newsPost1);
            newsPosts.Should().ContainEquivalentOf(newsPost2);
            newsPosts.Should().ContainEquivalentOf(newsPost3);
        }

        [Test]
        public async Task Get_only_several_newsposts()
        {
            var newsPost1 = CreateNewsPost();
            var newsPost2 = CreateNewsPost(2, "Header2", "Test2");
            var newsPost3 = CreateNewsPost(3, "header3", "Test3");
            var ctx = GetContext();
            await ctx.NewsPosts.AddRangeAsync(newsPost1, newsPost2, newsPost3);
            await ctx.SaveChangesAsync();

            var newsPosts = await GetMediator().Send(new GetNewsPosts {Count = 2});

            newsPosts.Should().HaveCount(2);
            newsPosts.Should().ContainEquivalentOf(newsPost1);
            newsPosts.Should().ContainEquivalentOf(newsPost2);
        }

        [Test]
        public async Task Successfully_delete_newspost()
        {
            var ctx = GetContext();
            var newsPost = await ctx.NewsPosts.AddAsync(CreateNewsPost());
            await ctx.SaveChangesAsync();

            (await ctx.NewsPosts.CountAsync()).Should().Be(1);
            await GetMediator().Send(new DeleteNewsPost { Id = newsPost.Entity.Id });

            (await ctx.NewsPosts.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new DeleteNewsPost { Id = 1 }));
    }
}
