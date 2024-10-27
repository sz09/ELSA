using IdentityServer.Extensions;

namespace IdentityServer.Clients
{
    public class AccessType
    {
        public string[] Roles { get; set; }
        public static AccessType READ => new()
        {
            Roles = [IdentityConfiguration.API_ROLE_READ]
        };
        public static AccessType Write => new()
        {
            Roles = [IdentityConfiguration.API_ROLE_WRITE]
        };

        public static AccessType NormalUserFullAccess => new()
        {
            Roles =
            [
                IdentityConfiguration.API_ROLE_READ,
                IdentityConfiguration.API_ROLE_WRITE
            ]
        };

        public static AccessType AdminUserFullAccess => new()
        {
            Roles =
            [
                IdentityConfiguration.API_ROLE_ADMIN,
                IdentityConfiguration.API_ROLE_READ,
                IdentityConfiguration.API_ROLE_WRITE
            ]
        };

        public static string[] IncludeUserProfileScopes =
        {
            IdentityConfiguration.API_SCOPE_PROFILE,
            IdentityConfiguration.API_SCOPE_OPENID
        };
    }
}