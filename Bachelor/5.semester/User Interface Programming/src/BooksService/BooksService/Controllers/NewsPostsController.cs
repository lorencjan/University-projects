using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Application.NewsPosts;
using BooksService.Common;
using BooksService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BooksService.Controllers
{    /// <inheritdoc />
    [ApiController, Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class NewsPosts : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsPosts(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets news posts
        /// </summary>
        /// <param name="count">Specifies how many last news posts should bew returned, method gets all if omitted</param>
        /// <param name="filter">Filter for news posts properties. If omitted, method gets all news posts</param>
        /// <param name="sortBy">Property by which to sort</param>
        /// <param name="sortOrder">Sort direction</param>
        /// <returns>List of latest news posts</returns>
        [HttpGet(nameof(GetNewsPosts))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NewsPost>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNewsPosts([FromQuery] int? count = null, [FromQuery] NewsPostFilter filter = null, [FromQuery] SortNewsPostsBy? sortBy = null, [FromQuery] SortOrdering? sortOrder = null)
        {
            var newsPosts = await _mediator.Send(new GetNewsPosts
            {
                Count = count,
                Filter = filter,
                SortNewsPostsBy = sortBy,
                SortOrdering = sortOrder
            });
            return Ok(newsPosts);
        }

        /// <summary>
        /// Gets a single news post by given Id
        /// </summary>
        /// <param name="id">Id of the news post</param>
        /// <returns>News post entity with specified id or null if not found</returns>
        [HttpGet(nameof(GetById) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NewsPost))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute, BindRequired] int id)
        {
            var newsPost = await _mediator.Send(new GetNewsPostById { Id = id });
            return Ok(newsPost);
        }

        /// <summary>
        /// Creates a new news post
        /// </summary>
        /// <param name="header">Header of the post</param>
        /// <param name="text">Content of the post</param>
        /// <returns>Id of the created news post in the database</returns>
        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(string header, string text)
        {
            var id = await _mediator.Send(new CreateNewsPost { Header = header, Text = text});
            return Ok(id);
        }

        /// <summary>
        /// Updates news post
        /// </summary>
        /// <param name="newsPost">News post to be updated with new values</param>
        /// <returns></returns>
        [HttpPatch(nameof(Update))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody, BindRequired] NewsPost newsPost)
        {
            await _mediator.Send(new UpdateNewsPost { NewsPost = newsPost });
            return Ok();
        }

        /// <summary>
        /// Deletes a news post with given id from the database
        /// </summary>
        /// <param name="id">Id of the news post to be deleted</param>
        /// <returns></returns>
        [HttpDelete(nameof(Delete) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute, BindRequired] int id)
        {
            await _mediator.Send(new DeleteNewsPost { Id = id });
            return Ok();
        }
    }
}
