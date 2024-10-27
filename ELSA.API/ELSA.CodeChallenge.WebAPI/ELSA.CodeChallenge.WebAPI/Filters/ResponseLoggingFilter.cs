using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace ELSA.WebAPI.Filters
{
    public class ResponseLoggingFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Stopwatch sw = Stopwatch.StartNew();
            await next(); // Proceed to the next filter or action method
            Console.WriteLine($"{context.ActionDescriptor.DisplayName}.{context.ActionDescriptor} took {sw.ElapsedMilliseconds}ms");
        }
    }
}