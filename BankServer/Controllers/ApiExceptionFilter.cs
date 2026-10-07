using BankServer.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankServer.Controllers
{
    /// <summary>
    /// Turns exceptions into an error body with a code, so a client – or a test –
    /// can tell *why* a call failed: { "error": "ACCOUNT_BLOCKED", "message": "..." }.
    /// </summary>
    public class ApiExceptionFilter : IExceptionFilter
    {
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var (status, code) = context.Exception switch
            {
                EntityNotFoundException => (StatusCodes.Status404NotFound, "NOT_FOUND"),
                AccountBlockedException => (StatusCodes.Status409Conflict, "ACCOUNT_BLOCKED"),
                ArgumentException => (StatusCodes.Status400BadRequest, "VALIDATION_ERROR"),
                _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR"),
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(context.Exception, "Unhandled exception");
            }

            context.Result = new ObjectResult(new { error = code, message = context.Exception.Message })
            {
                StatusCode = status,
            };
            context.ExceptionHandled = true;
        }

        private readonly ILogger<ApiExceptionFilter> _logger;
    }
}
