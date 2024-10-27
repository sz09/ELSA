using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer.Extensions;
using System.Security.Claims;

namespace IdentityServer.Services
{
    public class MongoProfileService : IProfileService
    {
        private readonly IClientStore _clientStore;
        public MongoProfileService(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var apiResource = IdentityConfiguration.ApiResources.First();
            var claims = apiResource.UserClaims.Select(d => new Claim($"{apiResource.Name}/{d}", d)).ToArray();
            context.AddRequestedClaims(claims);
            return Task.CompletedTask;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var client = await _clientStore.FindClientByIdAsync(clientId: context.Client.ClientId);
            context.Client = client;
        }
    }
}
