using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace FreeCourse.Web.Services;

public class ClientCredentialTokenService : IClientCredentialTokenService
{
    private readonly HttpClient _httpClient;
    private readonly ClientSettings _clientSettings;
    private readonly ServiceApiSettings _serviceApiSettings;
    private readonly IClientAccessTokenCache _clientAccessTokenCache;

    public ClientCredentialTokenService(IOptions<ClientSettings> clientSettings, 
                                        IOptions<ServiceApiSettings> serviceApiSettings, 
                                        IClientAccessTokenCache clientAccessTokenCache, HttpClient httpClient)
    {
        _clientSettings = clientSettings.Value;
        _serviceApiSettings = serviceApiSettings.Value;
        _clientAccessTokenCache = clientAccessTokenCache;
        _httpClient = httpClient;
    }

    public async Task<string> GetToken()
    {
        var currentToken = await _clientAccessTokenCache.GetAsync("WebClientToken", new ClientAccessTokenParameters(){});

        if (currentToken != null) return currentToken.AccessToken;
        
        // 1. Signin and retrieve access token
        var discovery = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false },
        });

        if (discovery.IsError) throw discovery.Exception;

        var clientCredentialTokenRequest = new ClientCredentialsTokenRequest()
        {
            ClientId = _clientSettings.WebClient.ClientId,
            ClientSecret = _clientSettings.WebClient.ClientSecret,
            Address = discovery.TokenEndpoint
        };

        var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);

        if (newToken.IsError) throw newToken.Exception;

        await _clientAccessTokenCache.SetAsync("WebClientToken", newToken.AccessToken, newToken.ExpiresIn, new ClientAccessTokenParameters(){});

        return newToken.AccessToken;

    }
}