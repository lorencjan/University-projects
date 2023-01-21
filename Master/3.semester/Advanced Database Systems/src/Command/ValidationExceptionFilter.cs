using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Hotel.Command;

public class ValidationExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is not ValidationException)
            return;

        context.ExceptionHandled = true;
        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Result = new ObjectResult(new { context.Exception.Message });
    }
}