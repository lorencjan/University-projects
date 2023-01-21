using System;
using System.Linq;
using System.Threading.Tasks;
using BooksService.Application.Feedbacks;
using BooksService.Common;
using BooksService.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BooksService.Specification
{
    public class FeedbackTests : TestFixture
    {
        private User User() => new User {Id = 1, Login = "login", Password = "password"};

        [SetUp]
        public async Task FeedbackTestsSetup()
        {
            var ctx = GetContext();
            await ctx.Users.AddAsync(User());
            await ctx.SaveChangesAsync();
        }

        private Feedback CreateFeedback(int id = 1, string text = "Test") => new Feedback
        {
            Id = id,
            Text = text,
            UserId = 1
        };

        [Test]
        public async Task Successfully_create_feedback()
        {
            var feedback = CreateFeedback();

            var id = await GetMediator().Send(new CreateFeedback { Text = feedback.Text, UserId = feedback.UserId });
            var created = await GetContext().Feedbacks.SingleAsync();

            id.Should().Be(1);
            created.Date.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 1, 0));
            created.Date = new DateTime();
            created.Should().BeEquivalentTo(feedback);
        }

        [Test]
        public async Task Get_existing_feedback()
        {
            var feedback = CreateFeedback();
            var ctx = GetContext();
            await ctx.Feedbacks.AddAsync(feedback);
            await ctx.SaveChangesAsync();

            var response = await GetMediator().Send(new GetFeedbackById { Id = 1 });

            feedback.User = User();
            response.User.Feedbacks = null; //cyclic dependency
            response.Should().BeEquivalentTo(feedback);
        }

        [Test]
        public async Task Get_not_existing_should_return_null()
        {
            var book = await GetMediator().Send(new GetFeedbackById { Id = 1 });

            book.Should().Be(null);
        }

        [Test]
        public async Task Get_all_feedbacks()
        {
            var feedback1 = CreateFeedback(); 
            var feedback2 = CreateFeedback(2, "Test2");
            var feedback3 = CreateFeedback(3, "Test3");
            var ctx = GetContext();
            await ctx.Feedbacks.AddRangeAsync(feedback1, feedback2, feedback3);
            await ctx.SaveChangesAsync();

            var newsPosts = await GetMediator().Send(new GetFeedbacks());
            feedback1.User = User();
            feedback2.User = User();
            feedback3.User = User();

            newsPosts.ForEach(x => x.User.Feedbacks = null); //cyclic dependency
            newsPosts.Should().HaveCount(3);
            newsPosts.Should().ContainEquivalentOf(feedback1);
            newsPosts.Should().ContainEquivalentOf(feedback2);
            newsPosts.Should().ContainEquivalentOf(feedback3);
        }

        [Test]
        public async Task Get_filtered_feedbacks()
        {
            var feedback1 = CreateFeedback();
            var feedback2 = CreateFeedback(2, "Test2");
            var feedback3 = CreateFeedback(3, "Test3");
            feedback3.UserId = 2;
            var ctx = GetContext();
            await ctx.Users.AddAsync(new User {Login = "test"});
            await ctx.Feedbacks.AddRangeAsync(feedback1, feedback2, feedback3);
            await ctx.SaveChangesAsync();

            var newsPosts = await GetMediator().Send(new GetFeedbacks { UserFilter = "gin" });
            feedback1.User = User();
            feedback2.User = User();

            newsPosts.ForEach(x => x.User.Feedbacks = null); //cyclic dependency
            newsPosts.Should().HaveCount(2);
            newsPosts.Should().ContainEquivalentOf(feedback1);
            newsPosts.Should().ContainEquivalentOf(feedback2);
        }

        [TestCase(1, SortOrdering.Ascending, SortFeedbacksBy.UserName)]
        [TestCase(2, SortOrdering.Descending, SortFeedbacksBy.Date)]
        public async Task Get_sorted_feedbacks(int testRun, SortOrdering order, SortFeedbacksBy sort)
        {
            var feedback1 = CreateFeedback();
            var feedback2 = CreateFeedback(2, "Test2");
            feedback2.UserId = 2;
            var ctx = GetContext();
            await ctx.Users.AddAsync(new User { Login = "abc" });
            await ctx.Feedbacks.AddRangeAsync(feedback1, feedback2);
            await ctx.SaveChangesAsync();

            var newsPosts = await GetMediator().Send(new GetFeedbacks { SortOrdering = order, SortFeedbacksBy = sort });

            newsPosts.Should().HaveCount(2);

            if(testRun == 1)
                newsPosts.Select(x => x.User.Login).Should().BeInAscendingOrder();
            else
                newsPosts.Select(x => x.Date).Should().BeInDescendingOrder();
        }

        [Test]
        public async Task Successfully_delete_feedback()
        {
            var ctx = GetContext();
            var feedback = await ctx.Feedbacks.AddAsync(CreateFeedback());
            await ctx.SaveChangesAsync();

            (await ctx.Feedbacks.CountAsync()).Should().Be(1);
            await GetMediator().Send(new DeleteFeedback { Id = feedback.Entity.Id });

            (await ctx.Feedbacks.CountAsync()).Should().Be(0);
        }

        [Test]
        public void Delete_not_existing_should_do_nothing()
            => Assert.DoesNotThrowAsync(async () => await GetMediator().Send(new DeleteFeedback { Id = 1 }));
    }
}
