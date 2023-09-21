using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using FreeCourse.Shared.DTOs;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace FreeCourse.Web.Services;

public class IdentityService: IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClientSettings _clientSettings;
    private readonly ServiceApiSettings _serviceApiSettings;

    public IdentityService(HttpClient client,
                            IHttpContextAccessor httpContextAccessor, 
                            IOptions<ClientSettings> clientSettings,
                            IOptions<ServiceApiSettings> serviceApiSettings)
    {
        _httpClient = client;
        _httpContextAccessor = httpContextAccessor;
        _clientSettings = clientSettings.Value;
        _serviceApiSettings = serviceApiSettings.Value;
    }

    /*
     * 1- Access identityserver4 endpoints via discovery
     *      a- sign in with the particular endpoints
     *      b- get jwt access token from the request
     * 2- Access identityserver4 userInfo endpoint via discovery
     *     a- read userInfo by giving access token
     * 3- Write AccessToken, UserInfo(Claims), RefreshToken to cookie
     */
    public async Task<Response<bool>> SignIn(SigninInput signInInput)
    {
        
        // 1. Signin and retrieve access token
        var discovery = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false },
        });

        if (discovery.IsError) throw discovery.Exception;

        var passwordTokenRequest = new PasswordTokenRequest()
        {
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            UserName = signInInput.Email,
            Password = signInInput.Password,
            Address = discovery.TokenEndpoint
        };

        var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

        if (token.IsError)
        {
            var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
            var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});

            return Response<bool>.Fail(errorDto.Errors, 400);
        }

        // 2. Access User Claims
        var userInfoRequest = new UserInfoRequest()
        {
            Token = token.AccessToken,
            Address = discovery.UserInfoEndpoint,
        };

        var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);

        if (userInfo.IsError) throw userInfo.Exception;

        // 3. Write claims to Cookie
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, 
                                            CookieAuthenticationDefaults.AuthenticationScheme,
                                            "name", "role");

        ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.StoreTokens(new List<AuthenticationToken>()
        {
            new AuthenticationToken(){Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken},
            new AuthenticationToken(){Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken},
            new AuthenticationToken(){Name = OpenIdConnectParameterNames.ExpiresIn, 
                                      Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O", CultureInfo.InvariantCulture)}
        });

        authenticationProperties.IsPersistent = signInInput.RememberMe;

        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            authenticationProperties);

        return Response<bool>.Success(200);

    }

    public async Task<TokenResponse> GetAccessTokenByRefreshToken()
    {
        // 1. Signin and retrieve access token
        var discovery = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false },
        });

        if (discovery.IsError) throw discovery.Exception;

        var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Resource);

        RefreshTokenRequest refreshTokenRequest = new()
        {
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            RefreshToken = refreshToken,
            Address = discovery.TokenEndpoint
        };

        var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

        if (token.IsError) return null;

        var authenticationTokens = new List<AuthenticationToken>()
        {
            new AuthenticationToken() { Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken },
            new AuthenticationToken() { Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken },
            new AuthenticationToken()
            {
                Name = OpenIdConnectParameterNames.ExpiresIn,
                Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O", CultureInfo.InvariantCulture)
            }
        };

        var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

        var properties = authenticationResult.Properties;
        properties.StoreTokens(authenticationTokens);

        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            authenticationResult.Principal, properties);
        
        return token;
    }

    public async Task RevokeRefreshToken()
    {
        // 1. Signin and retrieve access token
        var discovery = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false },
        });

        if (discovery.IsError) throw discovery.Exception;

        var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Resource);

        TokenRevocationRequest tokenRevocationRequest = new()
        {
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            Address = discovery.RevocationEndpoint,
            Token = refreshToken,
            TokenTypeHint = "refresh_token"
        };

        await _httpClient.RevokeTokenAsync(tokenRevocationRequest);
    }
}