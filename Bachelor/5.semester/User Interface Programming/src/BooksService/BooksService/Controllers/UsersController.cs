using System.Collections.Generic;
using System.Threading.Tasks;
using BooksService.Application.Favorites;
using BooksService.Application.Feedbacks;
using BooksService.Application.Users;
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
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Gets users meeting specified filter
        /// </summary>
        /// <param name="filter">Filter for users login. If omitted, method gets all users</param>
        /// <param name="sortBy">Property by which to sort</param>
        /// <param name="sortOrder">Sort direction</param>
        /// <param name="includes">Which related entities should be included</param>
        /// <returns>List of user entities</returns>
        [HttpGet(nameof(GetUsers))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsers([FromQuery] string filter = null, [FromQuery] SortUsersBy? sortBy = null, [FromQuery] SortOrdering? sortOrder = null, [FromQuery] UserIncludeFeatures? includes = null)
        {
            var users = await _mediator.Send(new GetUsers { LoginFilter = filter, SortUsersBy = sortBy, SortOrdering = sortOrder, IncludeFeatures = includes });
            return Ok(users);
        }

        /// <summary>
        /// Gets light version of users meeting specified filter.
        /// Good to use for list views.
        /// </summary>
        /// <param name="filter">Filter for users login. If omitted, method gets all users</param>
        /// <param name="sortBy">Property by which to sort</param>
        /// <param name="sortOrder">Sort direction</param>
        /// <returns>List of lightweight user entities</returns>
        [HttpGet(nameof(GetUsersLight))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserLight>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsersLight([FromQuery] string filter = null, [FromQuery] SortUsersBy? sortBy = null, [FromQuery] SortOrdering? sortOrder = null)
        {
            var users = await _mediator.Send(new GetUsersLight { LoginFilter = filter, SortUsersBy = sortBy, SortOrdering = sortOrder });
            return Ok(users);
        }

        /// <summary>
        /// Gets a single user entity by given Id
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="includeFeatures">Which related entities should be included</param>
        /// <returns>User entity with specified id or null if not found</returns>
        [HttpGet(nameof(GetById) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute, BindRequired] int id, [FromQuery] UserIncludeFeatures? includeFeatures = null)
        {
            var user = await _mediator.Send(new GetUserById { Id = id, IncludeFeatures = includeFeatures });
            return Ok(user);
        }

        /// <summary>
        /// Creates a new database record of user
        /// </summary>
        /// <param name="login">User's login</param>
        /// <param name="password">User's password</param>
        /// <returns>Id of the created user in the database</returns>
        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(string login, string password)
        {
            var id = await _mediator.Send(new CreateUser { Login = login, Password = password});
            return Ok(id);
        }

        /// <summary>
        /// Verifies user's credential
        /// </summary>
        /// <param name="login">User's login</param>
        /// <param name="password">User's password</param>
        /// <returns>True if credentials are correct, false otherwise</returns>
        [HttpPost(nameof(VerifyCredentials))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyCredentials(string login, string password)
        {
            var response = await _mediator.Send(new VerifyCredentials { Login = login, Password = password});
            return Ok(response);
        }

        /// <summary>
        /// Changes password of given user to the specified new password
        /// </summary>
        /// <param name="id">Id of the user whose password is to be changed</param>
        /// <param name="password">New user's password</param>
        /// <returns></returns>
        [HttpPatch(nameof(ChangePassword))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword(int id, string password)
        {
            await _mediator.Send(new ChangePassword { UserId = id, NewPassword = password });
            return Ok();
        }

        /// <summary>
        /// Deletes an user with given id from the database
        /// </summary>
        /// <param name="id">Id of the user to be deleted</param>
        /// <returns></returns>
        [HttpDelete(nameof(Delete) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute, BindRequired] int id)
        {
            await _mediator.Send(new DeleteUser { Id = id });
            return Ok();
        }

        /// <summary>
        /// Adds given author to the users favorites
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <param name="authorId">Author's id</param>
        /// <returns></returns>
        [HttpPost(nameof(AddFavoriteAuthor))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFavoriteAuthor(int userId, int authorId)
        {
            await _mediator.Send(new AddFavoriteAuthor { UserId = userId, AuthorId = authorId });
            return Ok();
        }

        /// <summary>
        /// Adds given book to the users favorites
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <param name="bookId">Book's id</param>
        /// <returns></returns>
        [HttpPost(nameof(AddFavoriteBook))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFavoriteBook(int userId, int bookId)
        {
            await _mediator.Send(new AddFavoriteBook { UserId = userId, BookId = bookId });
            return Ok();
        }

        /// <summary>
        /// Gets users favorite authors
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        [HttpGet(nameof(GetFavoriteAuthors))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AuthorLight>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFavoriteAuthors(int userId)
        {
            var authors = await _mediator.Send(new GetFavoriteAuthors { UserId = userId });
            return Ok(authors);
        }

        /// <summary>
        /// Gets users favorite books
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        [HttpGet(nameof(GetFavoriteBooks))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BookLight>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFavoriteBooks(int userId)
        {
            var books = await _mediator.Send(new GetFavoriteBooks { UserId = userId });
            return Ok(books);
        }

        /// <summary>
        /// Removes given author from the users favorites
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <param name="authorId">Author's id</param>
        /// <returns></returns>
        [HttpDelete(nameof(RemoveFavoriteAuthor))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveFavoriteAuthor(int userId, int authorId)
        {
            await _mediator.Send(new RemoveFavoriteAuthor { UserId = userId, AuthorId = authorId });
            return Ok();
        }

        /// <summary>
        /// Removes given book from the users favorites
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <param name="bookId">Book's id</param>
        /// <returns></returns>
        [HttpDelete(nameof(RemoveFavoriteBook))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveFavoriteBook(int userId, int bookId)
        {
            await _mediator.Send(new RemoveFavoriteBook { UserId = userId, BookId = bookId });
            return Ok();
        }

        /// <summary>
        /// Creates user's feedback
        /// </summary>
        /// <param name="text">Text of user's feedback</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        [HttpPost(nameof(CreateFeedback))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateFeedback(string text, int userId)
        {
            var id = await _mediator.Send(new CreateFeedback { Text = text, UserId = userId });
            return Ok(id);
        }

        /// <summary>
        /// Gets user's feedbacks meeting specified filter
        /// </summary>
        /// <param name="userLogin">Filter for users login. If omitted, method gets all feedbacks</param>
        /// <param name="sortBy">Property by which to sort</param>
        /// <param name="sortOrder">Sort direction</param>
        /// <returns>List of feedback entities</returns>
        [HttpGet(nameof(GetFeedbacks))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Feedback>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFeedbacks([FromQuery] string userLogin = null, [FromQuery] SortFeedbacksBy? sortBy = null, [FromQuery] SortOrdering? sortOrder = null)
        {
            var feedbacks = await _mediator.Send(new GetFeedbacks { UserFilter = userLogin, SortFeedbacksBy = sortBy, SortOrdering = sortOrder });
            return Ok(feedbacks);
        }

        /// <summary>
        /// Get user's feedback by id
        /// </summary>
        /// <param name="id">Id of the feedback</param>
        /// <returns>Single feedback entity with given id or null if not found</returns>
        [HttpGet(nameof(GetFeedbackById) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Feedback))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFeedbackById([FromRoute, BindRequired] int id)
        {
            var feedback = await _mediator.Send(new GetFeedbackById { Id = id });
            return Ok(feedback);
        }

        /// <summary>
        /// Deletes user's feedback from the database
        /// </summary>
        /// <param name="id">Id of the feedback to be deleted</param>
        /// <returns></returns>
        [HttpDelete(nameof(DeleteFeedback) + "/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFeedback([FromRoute, BindRequired] int id)
        {
            await _mediator.Send(new DeleteFeedback { Id = id });
            return Ok();
        }
    }
}
