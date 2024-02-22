using System.Net;

namespace ControlPlane.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpRequestException ex)
            {
                await HandleHttpRequestException(context, ex);
            }
        }

        private static Task HandleHttpRequestException(HttpContext context, HttpRequestException exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = exception.Message;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}