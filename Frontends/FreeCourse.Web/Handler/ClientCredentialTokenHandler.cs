﻿using System.Net;
using System.Net.Http.Headers;
using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Handler;

public class ClientCredentialTokenHandler : DelegatingHandler
{
    private readonly IClientCredentialTokenService _clientCredentialTokenService;

    public ClientCredentialTokenHandler(IClientCredentialTokenService clientCredentialTokenService)
    {
        _clientCredentialTokenService = clientCredentialTokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", await _clientCredentialTokenService.GetToken());

        var response = await base.SendAsync(request, cancellationToken); 

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedException();

        return response;
    }
}