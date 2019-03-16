using System.Collections.Generic;
using System.Numerics;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Http;

namespace Marvin.IDP
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
                   {
                       new TestUser
                       {
                           SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                           Username = "Frank",
                           Password = "password",

                           Claims = new List<Claim>
                                    {
                                        new Claim("given_name", "Frank"),
                                        new Claim("family_name", "Underwood"),
                                        new Claim("address", "Main Road 1"),
                                        new Claim("role", "FreeUser"),
                                        new Claim("subscriptionlevel", "FreeUser"),
                                        new Claim("country", "nl")
                                    }
                       },
                       new TestUser
                       {
                           SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                           Username = "Claire",
                           Password = "password",

                           Claims = new List<Claim>
                                    {
                                        new Claim("given_name", "Claire"),
                                        new Claim("family_name", "Underwood"),
                                        new Claim("address", "Big Street 2"),
                                        new Claim("role", "PayingUser"),
                                        new Claim("subscriptionlevel", "PayingUser"),
                                        new Claim("country", "be")
                                    }
                       }
                   };
        }

        /// <summary>
        /// Identity related resources (scopes)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles",
                                     "Your role(s)",
                                     // Claims related to this scope
                                     new List<string>
                                     {
                                         "role"
                                     }),
                // Add "country" scope
                new IdentityResource("country",
                                     "The country you're living in",
                                     // Claims related to this scope
                                     new List<string>
                                     {
                                         "country"
                                     }),
                // Add "subscription" scope
                new IdentityResource("subscriptionlevel",
                                     "Your subscription level",
                                     // Claims related to this scope
                                     new List<string>
                                     {
                                         "subscriptionlevel"
                                     })

            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                // Include the given claims when requesting the API scope
                new ApiResource("imagegalleryapi", "Image Gallery API", new List<string>
                {
                    "role"
                })
                {
                    ApiSecrets =
                    {
                        new Secret("apisecret".Sha256())
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
                   {
                       new Client
                       {
                           ClientName = "Image Gallery",
                           ClientId = "imagegalleryclient",
                           AllowedGrantTypes = GrantTypes.Hybrid,
                           AccessTokenType = AccessTokenType.Reference,
                           // IdentityTokenLifetime = 5 mins by default
                           // AuthorizationCodeLifetime = 5 mins by default
                           // AccessTokenLifetime = Defaults to 1 hour (number in seconds)
                           AccessTokenLifetime = 120,
                           AllowOfflineAccess = true,
                           // AbsoluteRefreshTokenLifetime = Default is 30 days
                           // RefreshTokenExpiration = TokenExpiration.Absolute (Sliding is also available)
                           UpdateAccessTokenClaimsOnRefresh = true,
                           RedirectUris = new List<string>
                                          {
                                              "https://localhost:44302/signin-oidc"
                                          },
                           PostLogoutRedirectUris = new List<string>
                                                    {
                                                        "https://localhost:44302/signout-callback-oidc"
                                                    },
                           AllowedScopes =
                           {
                               IdentityServerConstants.StandardScopes.OpenId,
                               IdentityServerConstants.StandardScopes.Profile,
                               IdentityServerConstants.StandardScopes.Address,
                               "roles",
                               "imagegalleryapi",
                               "country",
                               "subscriptionlevel"
                           },
                           ClientSecrets =
                           {
                               new Secret("secret".Sha256())
                           }
                       }
                   };
        }
    }
}