using IdentityServer.Extensions;

public class AccessType
{
	public string[] Scopes { get; private set; }
	public static AccessType READ => new AccessType
    {
        Scopes = new string[] { IdentityConfiguration.API_ROLE_READ }
    };
    public static AccessType Write => new AccessType
    {
        Scopes = new string[] { IdentityConfiguration.API_ROLE_WRITE }
    };

    public static AccessType FullAccess => new AccessType
    {
        Scopes = new string[]
        {
            IdentityConfiguration.API_ROLE_READ,
            IdentityConfiguration.API_ROLE_WRITE
        }
    };

    public static string[] IncludeUserProfileScopes =
    {
        IdentityConfiguration.API_SCOPE_PROFILE,
        IdentityConfiguration.API_SCOPE_OPENID
    };
}

