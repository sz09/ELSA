using IdentityModel.Client;
using IdentityServer.Extensions;
using IdentityServer.Models;
using ELSA.Config;

namespace IdentityServer.Clients
{
    public class IdentityServerClient: IntegrationClient, IIdentityServerClient
    {
        private readonly IApplicationConfig _applicationConfig;
        public IdentityServerClient(IHttpClientFactory httpClientFactory, IApplicationConfig applicationConfig)
            :base(httpClientFactory)
        {
            _applicationConfig = applicationConfig;
        }

        public async Task<TokenResponse> RequestPasswordTokenAsync(string username, string password, AccessType accessType, bool includeUserInfo = false, bool includeRefreshToken = false)
		{
            var disco = await DocumentResponseAsync();
            var httpClient = _httpClientFactory.CreateClient();
            List<string> scopes = [];
            if (includeUserInfo)
            {
                scopes.AddRange(AccessType.IncludeUserProfileScopes);
            }

            if (includeRefreshToken)
            {
                scopes.Add(IdentityConfiguration.OFFLINE_ACCESS);
            }
            var response = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = IdentityConfiguration.CLIENT_ID,
                ClientSecret = IdentityConfiguration.SECRET_KEY,
                UserName = username,
                Password = password,
                Scope = string.Join(" ", scopes)
            });
            return response;
        }

        public async Task<TokenResponse> RequestRefreshTokenAsync(string refreshToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var disco = await DocumentResponseAsync();
            return await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                RefreshToken = refreshToken,
                ClientId = IdentityConfiguration.CLIENT_ID,
                ClientSecret = IdentityConfiguration.SECRET_KEY
            });
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var disco = await DocumentResponseAsync();
            var httpClient = _httpClientFactory.CreateClient();
            return await httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken,
                ClientId = IdentityConfiguration.CLIENT_ID,
                ClientSecret = IdentityConfiguration.SECRET_KEY
            });
        }

        public async Task<RefreshTokenResponse> GetRefreshTokenAsync(string refreshToken)
        {
            return await PostAsync<object, RefreshTokenResponse>(
                url: $"{_applicationConfig.JwtSettings.Authority}/token/retrieve",
                content: new
                {
                    RefreshToken = refreshToken
                }, () => new AppDomainUnloadedException("Identity server unavailable!"));
        }

        class TempIdentity
        {
            public string IdentityId { get; set; }
        }

        // Return IdentityUser.Id
        public async Task<string> RegisterUserAsync(RegisterUserModel userModel)
        {
            var result = await PostAsync<object, TempIdentity>(
                url: $"{_applicationConfig.JwtSettings.Authority}/users", 
                content: userModel
            );
            return result.IdentityId;
        }

        public async Task UpdateUserIdAsync(string identityId, string userId)
        {
            await PutAsync<object>(
                url: $"{_applicationConfig.JwtSettings.Authority}/users/update/{identityId}/userId/{userId}", 
                content: new {}, 
                () => new AppDomainUnloadedException("Identity server unavailable!")
            );
        }

        private async Task<DiscoveryDocumentResponse> DocumentResponseAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var disco = await httpClient.GetDiscoveryDocumentAsync(_applicationConfig.JwtSettings.Authority);
            if (disco.IsError)
            {
                throw new AppDomainUnloadedException($"Identity server unavailable!, {disco.Error}");
            }

            return disco;
        }
    }

    public class RefreshTokenResponse
    {
        public DateTime CreationTime { get; set; } 
        public int Lifetime { get; set; }
    }
}

