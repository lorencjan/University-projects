using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Application.Authors;
using BooksService.Application.Ratings;
using BooksService.Common;
using BooksService.Domain;
using BooksService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BooksService.Controllers
{    /// <inheritdoc />
    [ApiController, Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class AuthorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorsController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets authors meeting specified filter
        /// </summary>
        /// <param name="filter">Filter for authors properties. If omitted, method gets all authors</param>
        /// <param name="sortBy">Property by which to sort</param>
        /// <param name="sortOrder">Sort direction</param>
        /// <param name="includes">Which related entities should be included</param>
        /// <returns>List of author entities</returns>
        [HttpGet(nameof(GetAuthors))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Author>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors([FromQuery] AuthorFilter filter = null, [FromQuery] SortAuthorsBy? sortBy = null, [FromQuery] SortOrdering? sortOrder = null, [FromQuery] AuthorIncludeFeatures? includes = null)
        {
            var authors = await _mediator.Send(new GetAuthors { Filter = filter, SortAuthorsBy = sortBy, SortOrdering = sortOrder, IncludeFeatures = includes });
            return Ok(authors);
        }

        /// <summary>
        /// Gets light version of authors meeting specified filter.
        /// Good to use for list views.
        /// </summary>
        /// <param name="filter">Filter for authors properties. If omitted, method gets all authors</param>
        /// <param name="sortBy">Property by which to sort</param>
        /// <param name="sortOrder">Sort direction</param>
        /// <returns>List of lightweight author entities</returns>
        [HttpGet(nameof(GetAuthorsLight))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AuthorLight>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthorsLight([FromQuery] AuthorFilter filter = null, [FromQuery] SortAuthorsBy? sortBy = null, [FromQuery] SortOrdering? sortOrder = null)
        {
            var authors = await _mediator.Send(new GetAuthorsLight { Filter = filter, SortAuthorsBy = sortBy, SortOrdering = sortOrder });
            return Ok(authors);
        }

        /// <summary>
        /// Gets a single author entity by given Id
        /// </summary>
        /// <param name="id">Id of the author</param>
        /// <param name="includeFeatures">Which related entities should be included</param>
        /// <returns>Author entity with specified id or null if not found</returns>
        [HttpGet(nameof(GetById) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Author))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute, BindRequired] int id, [FromQuery] AuthorIncludeFeatures? includeFeatures = null)
        {
            var author = await _mediator.Send(new GetAuthorById { Id = id, IncludeFeatures = includeFeatures });
            return Ok(author);
        }

        /// <summary>
        /// Creates a new database record of author
        /// </summary>
        /// <param name="author">Author entity to be created</param>
        /// <returns>Id of the created author in the database</returns>
        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody, BindRequired] Author author)
        {
            var id = await _mediator.Send(new CreateAuthor { Author = author });
            return Ok(id);
        }

        /// <summary>
        /// Updates values of an existing author
        /// </summary>
        /// <param name="author">Author to be updated with new values</param>
        /// <returns></returns>
        [HttpPatch(nameof(Update))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody, BindRequired] Author author)
        {
            await _mediator.Send(new UpdateAuthor { Author = author });
            return Ok();
        }

        /// <summary>
        /// Deletes an author with given id from the database
        /// </summary>
        /// <param name="id">Id of the author to be deleted</param>
        /// <returns></returns>
        [HttpDelete(nameof(Delete) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute, BindRequired] int id)
        {
            await _mediator.Send(new DeleteAuthor { Id = id });
            return Ok();
        }

        /// <summary>
        /// Creates a rating for the author
        /// </summary>
        /// <param name="number">Numeric value of the rating</param>
        /// <param name="text">Text of the rating</param>
        /// <param name="userId">Id of the user who posted the rating</param>
        /// <param name="authorId">Id of the author whom the rating is for</param>
        /// <returns>Id of the created rating</returns>
        [HttpPost(nameof(CreateRating))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRating(int number, string text, int userId, int authorId)
        {
            var id = await _mediator.Send(new CreateAuthorRating { Number = number, Text = text, UserId = userId, AuthorId = authorId});
            return Ok(id);
        }

        /// <summary>
        /// Updates a rating for the author
        /// </summary>
        /// <param name="ratingId">Id of the rating to be updated</param>
        /// <param name="number">Numeric value of the rating</param>
        /// <param name="text">Text of the rating</param>
        /// <returns></returns>
        [HttpPatch(nameof(UpdateRating))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRating(int ratingId, int number, string text)
        {
            await _mediator.Send(new UpdateAuthorRating { Id = ratingId, Number = number, Text = text });
            return Ok();
        }

        /// <summary>
        /// Deletes an author's rating
        /// </summary>
        /// <param name="id">Rating's id</param>
        /// <returns></returns>
        [HttpDelete(nameof(DeleteRating) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRating([FromRoute, BindRequired] int id)
        {
            await _mediator.Send(new DeleteAuthorRating { Id = id });
            return Ok();
        }
    }
}
