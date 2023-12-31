﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("resource_catalog"){Scopes = {"catalog_fullpermission"}},
            new ApiResource("resource_photo_stock"){Scopes = {"photo_stock_fullpermission"}},
            new ApiResource("resource_basket"){Scopes = {"basket_fullpermission"}},
            new ApiResource("resource_discount"){Scopes = {"discount_fullpermission"}},
            new ApiResource("resource_order"){Scopes = {"order_fullpermission"}},
            new ApiResource("resource_fakepayment"){Scopes = {"fakepayment_fullpermission"}},
            new ApiResource("resource_gateway"){Scopes = {"gateway_fullpermission"}},
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
               new IdentityResource[]
               {
                    new IdentityResources.Email(),
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResource(){Name = "roles", DisplayName = "Roles", Description = "User Roles", UserClaims = new[] {"role"}}
               };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
              new ApiScope("catalog_fullpermission", "Full Access to Catalog"),
              new ApiScope("photo_stock_fullpermission", "Full Access to Photo Stock"),
              new ApiScope("basket_fullpermission", "Full Access to Basket"),
              new ApiScope("discount_fullpermission", "Full Access to Discount"),
              new ApiScope("order_fullpermission", "Full Access to Order"),
              new ApiScope("fakepayment_fullpermission", "Full Access to FakePayment"),
              new ApiScope("gateway_fullpermission", "Full Access to Gateway"),
              new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
              new Client()
              {
                  ClientName = "Asp.Net Core MVC",
                  ClientId = "WebMvcClient",
                  ClientSecrets = {new Secret("secret".Sha256())},
                  AllowedGrantTypes = { GrantType.ClientCredentials },
                  AllowedScopes = 
                  { 
                    "gateway_fullpermission", 
                    "catalog_fullpermission", 
                    "photo_stock_fullpermission",
                    IdentityServerConstants.LocalApi.ScopeName
                  } 
              }
              ,
              new Client()
              {
                  ClientName = "Asp.Net Core MVC",
                  ClientId = "WebMvcClientForUser",
                  AllowOfflineAccess = true,
                  ClientSecrets = {new Secret("secret".Sha256())},
                  AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                  AllowedScopes = {
                    "gateway_fullpermission",
                      "order_fullpermission",
                      "basket_fullpermission",
                      IdentityServerConstants.LocalApi.ScopeName,
                      IdentityServerConstants.StandardScopes.Email, 
                      IdentityServerConstants.StandardScopes.OpenId, 
                      IdentityServerConstants.StandardScopes.Profile,
                      IdentityServerConstants.StandardScopes.OfflineAccess,
                      "roles"
                  },
                  AccessTokenLifetime = 1 * 60 * 60,
                  RefreshTokenExpiration = TokenExpiration.Absolute,
                  AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds,
                  RefreshTokenUsage = TokenUsage.ReUse
              },
              new Client()
              {
                  ClientName = "Token Exchange Client",
                  ClientId = "TokenExchangeClient",
                  ClientSecrets = {new Secret("secret".Sha256())},
                  AllowedGrantTypes = { "urn:ietf:params:oauth:grant-type:token-exchange"},
                  AllowedScopes = 
                  { 
                      "discount_fullpermission",
                      "fakepayment_fullpermission",
                      IdentityServerConstants.StandardScopes.OpenId, 
                  } 
              }
            };
    }
}