using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Application.Books;
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
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets books meeting specified filter
        /// </summary>
        /// <param name="filter">Filter for books properties. If omitted, method gets all books</param>
        /// <param name="sortBy">Property by which to sort</param>
        /// <param name="sortOrder">Sort direction</param>
        /// <param name="includes">Which related entities should be included</param>
        /// <returns>List of book entities</returns>
        [HttpGet(nameof(GetBooks))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Book>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks([FromQuery] BookFilter filter = null, [FromQuery] SortBooksBy? sortBy = null, [FromQuery] SortOrdering? sortOrder = null, [FromQuery] BookIncludeFeatures? includes = null)
        {
            var books = await _mediator.Send(new GetBooks { Filter = filter, SortBooksBy = sortBy, SortOrdering = sortOrder, IncludeFeatures = includes });
            return Ok(books);
        }

        /// <summary>
        /// Gets light version of books meeting specified filter.
        /// Good to use for list views.
        /// </summary>
        /// <param name="filter">Filter for books properties. If omitted, method gets all books</param>
        /// <param name="sortBy">Property by which to sort</param>
        /// <param name="sortOrder">Sort direction</param>
        /// <returns>List of lightweight book entities</returns>
        [HttpGet(nameof(GetBooksLight))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BookLight>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooksLight([FromQuery] BookFilter filter = null, [FromQuery] SortBooksBy? sortBy = null, [FromQuery] SortOrdering? sortOrder = null)
        {
            var books = await _mediator.Send(new GetBooksLight { Filter = filter, SortBooksBy = sortBy, SortOrdering = sortOrder });
            return Ok(books);
        }

        /// <summary>
        /// Gets a single book entity by given Id
        /// </summary>
        /// <param name="id">Id of the book</param>
        /// <param name="includeFeatures">Which related entities should be included</param>
        /// <returns>Book entity with specified id or null if not found</returns>
        [HttpGet(nameof(GetById) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Book))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute, BindRequired] int id, [FromQuery] BookIncludeFeatures? includeFeatures = null)
        {
            var book = await _mediator.Send(new GetBookById { Id = id, IncludeFeatures = includeFeatures });
            return Ok(book);
        }

        /// <summary>
        /// Creates a new database record of book
        /// </summary>
        /// <param name="book">Book entity to be created</param>
        /// <returns>Id of the created book in the database</returns>
        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody, BindRequired] Book book)
        {
            var id = await _mediator.Send(new CreateBook { Book = book });
            return Ok(id);
        }

        /// <summary>
        /// Updates values of an existing book
        /// </summary>
        /// <param name="book">Book to be updated with new values</param>
        /// <returns></returns>
        [HttpPatch(nameof(Update))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody, BindRequired] Book book)
        {
            await _mediator.Send(new UpdateBook { Book = book });
            return Ok();
        }

        /// <summary>
        /// Deletes a book with given id from the database
        /// </summary>
        /// <param name="id">Id of the book to be deleted</param>
        /// <returns></returns>
        [HttpDelete(nameof(Delete) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute, BindRequired] int id)
        {
            await _mediator.Send(new DeleteBook { Id = id });
            return Ok();
        }

        /// <summary>
        /// Creates a rating for the book
        /// </summary>
        /// <param name="number">Numeric value of the rating</param>
        /// <param name="text">Text of the rating</param>
        /// <param name="userId">Id of the user who posted the rating</param>
        /// <param name="bookId">Id of the book whom the rating is for</param>
        /// <returns>Id of the created rating</returns>
        [HttpPost(nameof(CreateRating))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRating(int number, string text, int userId, int bookId)
        {
            var id = await _mediator.Send(new CreateBookRating { Number = number, Text = text, UserId = userId, BookId = bookId});
            return Ok(id);
        }

        /// <summary>
        /// Updates a rating for the book
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
            await _mediator.Send(new UpdateBookRating { Id = ratingId, Number = number, Text = text });
            return Ok();
        }

        /// <summary>
        /// Deletes a book's rating
        /// </summary>
        /// <param name="id">Rating's id</param>
        /// <returns></returns>
        [HttpDelete(nameof(DeleteRating) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRating([FromRoute, BindRequired] int id)
        {
            await _mediator.Send(new DeleteBookRating { Id = id });
            return Ok();
        }

        /// <summary>
        /// Gets all genres in the database.
        /// </summary>
        /// <returns>List of genre names</returns>
        [HttpGet(nameof(GetGenres))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Genre>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _mediator.Send(new GetGenres());
            return Ok(genres);
        }
    }
}
