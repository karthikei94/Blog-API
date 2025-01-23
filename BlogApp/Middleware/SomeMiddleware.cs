using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BlogApp.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class SomeMiddleware
    {
        private readonly RequestDelegate _next;

        public SomeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class SomeMiddlewareExtensions
    {
        public static IApplicationBuilder UseSomeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SomeMiddleware>();
        }
    }
}
