using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient = new HttpClient();

        public ImageGalleryHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<HttpClient> GetClient()
        {
            var accessToken = string.Empty;

            // Get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;

            // Get the access token
            //accessToken = await currentContext.GetTokenAsync(
            //    OpenIdConnectParameterNames.AccessToken);

            // Should we renew access & refresh tokens?
            // Get expiresAt value
            var expiresAt = await currentContext.GetTokenAsync("expires_at");

            // compare to make sure to use the exact date formats for comparison
            // If current token is expired or about to expire, renew tokens
            if (string.IsNullOrWhiteSpace(expiresAt) ||
                DateTime.Parse(expiresAt).AddSeconds(-60).ToUniversalTime() < DateTime.UtcNow)
            {
                accessToken = await RenewTokens();
            }
            else
            {
                // Get access token
                accessToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }
            
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                // Set bearer token
                _httpClient.SetBearerToken(accessToken);
            }

            _httpClient.BaseAddress = new Uri("https://localhost:44366/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        private async Task<string> RenewTokens()
        {
            // Get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;

            // Get the metadata
            var discoveryClient = new DiscoveryClient("https://localhost:44343/");
            var metadataResponse = await discoveryClient.GetAsync();

            // Create a new token client to get new tokens
            var tokenClient = new TokenClient(metadataResponse.TokenEndpoint, "imagegalleryclient", "secret");

            // Get the saved refresh token
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // Refresh the tokens
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                // Update the tokens & expiration value
                var updatedTokens = new List<AuthenticationToken>();
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = tokenResult.IdentityToken
                });
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResult.AccessToken
                });
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResult.RefreshToken
                });

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.ExpiresIn,
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                });

                // Get authenticate result, containing the current principal & properties
                var currentAuthenticateResult = await currentContext.AuthenticateAsync("Cookies");

                // Store the updated tokens
                currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

                // Sign in
                await currentContext.SignInAsync("Cookies",
                                                 currentAuthenticateResult.Principal,
                                                 currentAuthenticateResult.Properties);

                // Return the new access token
                return tokenResult.AccessToken;
            }

            throw new Exception("Problem encountered while refreshing tokens.", tokenResult.Exception);
        }
    }
}

