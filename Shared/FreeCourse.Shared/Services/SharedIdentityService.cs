using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FreeCourse.Shared.Services
{
    public class SharedIdentityService : ISharedIdentityService
    {
        private IHttpContextAccessor _httpContext;

        public SharedIdentityService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetUserId => _httpContext.HttpContext.User.FindFirst("sub").Value;
    }
}