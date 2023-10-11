using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using urlShortener.DBContext;

namespace urlShortener.Utilities
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyHeaderAttribute : Attribute, IAsyncActionFilter
    {
        private const string APIKEYNAME = "x-api-key";

        private readonly shortnerDBContext _context;
        public ApiKeyHeaderAttribute(shortnerDBContext context)
        {
            _context = context;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "API Key was not provided"
                };
                return;
            }

            var apiKey = _context.user.FirstOrDefault(u => u.apiKey == extractedApiKey.ToString());

            if (apiKey == null)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "API Key is not valid"
                };
                return;
            }
            await next();

        }
    }
}
