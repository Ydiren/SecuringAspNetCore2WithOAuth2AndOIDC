using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageGallery.Client.Services
{
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
            // Get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;

            // Get the access token
            var accessToken = await currentContext.GetTokenAsync(
                OpenIdConnectParameterNames.AccessToken);

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
    }
}

