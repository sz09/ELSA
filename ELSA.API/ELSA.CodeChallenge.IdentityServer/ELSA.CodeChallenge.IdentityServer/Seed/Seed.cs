using IdentityServer.Extensions;
using IdentityServer.Models.Wrapper;
using IdentityServer4.Models;
using MongoDB.Driver;
using ELSA.Config;

namespace ELSA.IdentityServer.Seed
{
    public interface ISeeder
    {
        Task SeedAsync();
    }

    public class Seeder : ISeeder
    {
        private readonly IApplicationConfig _config;

        public Seeder(IApplicationConfig applicationConfig)
        {
            _config = applicationConfig;
        }

        public async Task SeedAsync()
        {

            var clientCollection = GetCollection<Client>();
            foreach (var client in IdentityConfiguration.Clients)
            {
                if (!(await clientCollection.FindAsync(d => d.Object.ClientId == client.ClientId)).Any())
                {
                    await clientCollection.InsertOneAsync(new MongoObjectWrapper<Client>(client));
                }
            }

            var identityResourceCollection = GetCollection<IdentityResource>();
            foreach (var identityResource in IdentityConfiguration.IdentityResources)
            {
                if (!(await identityResourceCollection.FindAsync(d => d.Object.Name == identityResource.Name)).Any())
                {
                    await identityResourceCollection.InsertOneAsync(new MongoObjectWrapper<IdentityResource>(identityResource));
                }
            }

            var apiResourceResourceCollection = GetCollection<ApiResource>();
            foreach (var apiResource in IdentityConfiguration.ApiResources)
            {
                if (!(await apiResourceResourceCollection.FindAsync(d => d.Object.Name == apiResource.Name)).Any())
                {
                    await apiResourceResourceCollection.InsertOneAsync(new MongoObjectWrapper<ApiResource>(apiResource));
                }
            }

            var apiScopeResourceCollection = GetCollection<ApiScope>();
            foreach (var apiScope in IdentityConfiguration.ApiScopes)
            {
                if (!(await apiScopeResourceCollection.FindAsync(d => d.Object.Name == apiScope.Name)).Any())
                {
                    await apiScopeResourceCollection.InsertOneAsync(new MongoObjectWrapper<ApiScope>(apiScope));
                }
            }
        }

        private IMongoCollection<MongoObjectWrapper<T>> GetCollection<T>()
        {
            var mongoClient = new MongoClient(_config.IdentityServer.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(_config.IdentityServer.DatabaseNamespace);
            return mongoDatabase.GetCollection<MongoObjectWrapper<T>>(MongoObjectWrapper<T>.GetCollectionName());
        }
    }
}
