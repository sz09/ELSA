using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer.Models;
using IdentityServer.MongoCollections;
using IdentityServer.Store;
using ELSA.Config;
using ELSA.IdentityServer;
using System.Security.Claims;

namespace IdentityServer.Extensions
{
    public static class IdentityConfiguration
    {
        public const string CLIENT_ID = "elsa.web.client";
        public const string API_ROLE_READ = "elsa.api.read";
        public const string API_ROLE_WRITE = "elsa.api.write";
        public const string API_ROLE_ADMIN = "elsa.api.admin";
        public const string API_SCOPE_OPENID = "openid";
        public const string API_SCOPE_PROFILE = "profile";
        public const string OFFLINE_ACCESS = "offline_access";
        public const string SECRET_KEY = "this_is_secret_key";

        public static IServiceCollection UseIdentityServer(this IServiceCollection serviceCollection)
        {
            serviceCollection.UseIdentityUserRoleCollection();
            serviceCollection.UseUserClaimCollection();
            serviceCollection.AddScoped(d =>
            {
                return new UserOptions { RequireUniqueEmail = true };
            });

            serviceCollection.AddScoped(d =>
            {
                var applicationConfig = d.GetRequiredService<IApplicationConfig>();
                return new IdentityStore(applicationConfig.IdentityServer.ConnectionString, applicationConfig.IdentityServer.DatabaseNamespace);
            });

            serviceCollection.AddScoped<IRoleStore<Role>, MongoRoleStore>();
            serviceCollection.AddScoped<IUserClaimsPrincipalFactory<User>, MongoUserClaimsPrincipalFactory>();
            serviceCollection.AddScoped<UserManager<User>, MongoUserManager>();
            serviceCollection.AddScoped<MongoUserStore>();
            serviceCollection.AddIdentity<User, Role>()
                             .AddUserStore<MongoUserStore>()
                             .AddRoleStore<MongoRoleStore>()
                             .AddDefaultTokenProviders();
            var builder = serviceCollection.AddIdentityServer(options =>
            {
                options.Discovery.ShowIdentityScopes = false;
                options.Discovery.ShowApiScopes = false;
                options.Discovery.ShowClaims = false;
                options.Discovery.ShowExtensionGrantTypes = false;
            })
                                           .AddClientStore<ClientStore>()
                                           .AddResourceStore<ResourceStore>()
                                           .AddPersistedGrantStore<PersistedGrantStore>()
                                           //.AddProfileService<CustomProfileService>()
                                           .AddAspNetIdentity<User>();
            builder.AddDeveloperSigningCredential();
            return serviceCollection;
        }

        #region Data
        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope(OFFLINE_ACCESS),
                new ApiScope(API_SCOPE_OPENID),
                new ApiScope(API_SCOPE_PROFILE, "profile", 
                [
                    JwtClaimTypes.Id,
                    JwtClaimTypes.Email,
                    ELSAClaims.USER_ID,
                    JwtClaimTypes.Name,
                    JwtClaimTypes.GivenName,
                    JwtClaimTypes.FamilyName,
                    JwtClaimTypes.Role,
                    ClaimTypes.Role
                ])
            ];

        public static IEnumerable<ApiResource> ApiResources =>
            [
                new ApiResource("ELSAApi", "ELSA API", [OFFLINE_ACCESS])
                {
                    Scopes = new List<string>{ API_ROLE_ADMIN, API_ROLE_READ, API_ROLE_WRITE, OFFLINE_ACCESS },
                    ApiSecrets = new List<Secret> {
                        new Secret(SECRET_KEY.Sha256())
                    },
                    UserClaims =
                    {
                        JwtClaimTypes.Id,
                        JwtClaimTypes.Email,
                        ELSAClaims.USER_ID,
                        JwtClaimTypes.Name,
                        JwtClaimTypes.GivenName,
                        JwtClaimTypes.FamilyName,
                        JwtClaimTypes.Role,
                        ClaimTypes.Role
                    }
                }
            ];

        public static IEnumerable<Client> Clients =>
            [
                new Client
                {
                    ClientId = CLIENT_ID,
                    ClientName = "ELSA.API",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    ClientSecrets = { new Secret(SECRET_KEY.Sha256()) },
                    AllowedScopes = {
                        API_SCOPE_OPENID,
                        API_SCOPE_PROFILE,
                        OFFLINE_ACCESS
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600,// 3600, 
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AbsoluteRefreshTokenLifetime = 2592000,
                    SlidingRefreshTokenLifetime = 1296000
                }
            ];

        public static IEnumerable<IdentityResource> IdentityResources =>
            Array.Empty<IdentityResource>();
        #endregion
    }

    public static class ELSAClaims {
        public const string USER_ID = "UserId";
    }
}

