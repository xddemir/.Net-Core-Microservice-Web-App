using System.Net.Http.Headers;
using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace FreeCourse.Web.Handler;

public class ResourceOwnerPasswordTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _HttpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger;

    public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor httpContextAccessor, 
        IIdentityService identityService, ILogger<ResourceOwnerPasswordTokenHandler> logger)
    {
        _HttpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _HttpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var tokenResponse = await _identityService.GetAccessTokenByRefreshToken();
            if (tokenResponse != null)
            {
                request.Headers.Authorization = new BasicAuthenticationOAuthHeaderValue("Bearer", tokenResponse.AccessToken);
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // throws error
            throw new UnauthorizedException();
        }

        return response;
    }
}