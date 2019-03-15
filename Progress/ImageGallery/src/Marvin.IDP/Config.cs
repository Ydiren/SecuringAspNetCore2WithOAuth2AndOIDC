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
                           SubjectId = "BFF0AF2A-6230-4408-99F0-8A28C203D711",
                           Username = "Frank",
                           Password = "password",

                           Claims = new List<Claim>
                                    {
                                        new Claim("given_name", "Frank"),
                                        new Claim("family_name", "Underwood"),
                                        new Claim("address", "Main Road 1"),
                                        new Claim("role", "FreeUser")
                                    }
                       },
                       new TestUser
                       {
                           SubjectId = "82B616B2-CB65-43F3-9F1B-D4EBBF27ED70",
                           Username = "Claire",
                           Password = "password",

                           Claims = new List<Claim>
                                    {
                                        new Claim("given_name", "Claire"),
                                        new Claim("family_name", "Underwood"),
                                        new Claim("address", "Big Street 2"),
                                        new Claim("role", "PayingUser")
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
                       new IdentityResource("roles", "Your role(s)", new List<string> { "role" })
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
                               "roles"
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