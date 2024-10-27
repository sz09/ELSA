using IdentityModel.Client;
using IdentityServer.Models;

namespace IdentityServer.Clients
{
    public interface IIdentityServerClient
	{
        Task<TokenResponse> RequestPasswordTokenAsync(string username,
            string password,
            AccessType accessType,
            bool includeUserInfo = false,
            bool includeRefreshToken = false);
        Task<TokenResponse> RequestRefreshTokenAsync(string refreshToken);
        Task<RefreshTokenResponse> GetRefreshTokenAsync(string refreshToken);
        Task<UserInfoResponse> GetUserInfoAsync(string accessToken);
        Task<string> RegisterUserAsync(RegisterUserModel registerUserModel);
        Task UpdateUserIdAsync(string identityId, string userId);
    }
}

