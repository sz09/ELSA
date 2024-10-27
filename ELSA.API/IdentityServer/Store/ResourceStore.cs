using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Driver;
using IdentityServer.Models.Wrapper;
using ELSA.Config;

namespace IdentityServer.Store
{
    public class ResourceStore: IResourceStore
    {
        private Lazy<IMongoCollection<MongoObjectWrapper<ApiResource>>> _apiResourceCollection;
        private Lazy<IMongoCollection<MongoObjectWrapper<ApiScope>>> _apiScopeCollection;
        private Lazy<IMongoCollection<MongoObjectWrapper<IdentityResource>>> _identityResourceCollection;
        public ResourceStore(IApplicationConfig config)
        {
            _apiResourceCollection = new Lazy<IMongoCollection<MongoObjectWrapper<ApiResource>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<ApiResource>>(MongoObjectWrapper<ApiResource>.GetCollectionName());
            });

            _apiScopeCollection = new Lazy<IMongoCollection<MongoObjectWrapper<ApiScope>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<ApiScope>>(MongoObjectWrapper<ApiScope>.GetCollectionName());
            });

            _identityResourceCollection = new Lazy<IMongoCollection<MongoObjectWrapper<IdentityResource>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<IdentityResource>>(MongoObjectWrapper<IdentityResource>.GetCollectionName());
            });
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var filter = Builders<MongoObjectWrapper<ApiResource>>.Filter.In(d => d.Object.Name, apiResourceNames);
            return  await (await _apiResourceCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<ApiResource>, ApiResource>{
                Projection = MongoObjectWrapper<ApiResource>.ToProjectionDefinition()
            })).ToListAsync();
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var filter = Builders<MongoObjectWrapper<ApiResource>>.Filter.AnyNin(d => d.Object.Scopes, scopeNames);
            return  await (await _apiResourceCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<ApiResource>, ApiResource>{
                Projection = MongoObjectWrapper<ApiResource>.ToProjectionDefinition()
            })).ToListAsync();
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var filter = Builders<MongoObjectWrapper<ApiScope>>.Filter.In(d => d.Object.Name, scopeNames);
            return  await (await _apiScopeCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<ApiScope>, ApiScope>{
                Projection = MongoObjectWrapper<ApiScope>.ToProjectionDefinition()
            })).ToListAsync();
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var filter = Builders<MongoObjectWrapper<IdentityResource>>.Filter.AnyIn(d => d.Object.UserClaims, scopeNames);
            return  await (await _identityResourceCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<IdentityResource>, IdentityResource>{
                Projection = MongoObjectWrapper<IdentityResource>.ToProjectionDefinition()
            })).ToListAsync();
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            List<ApiResource> apiResources = new List<ApiResource>();
            List<ApiScope> apiScopes = new List<ApiScope>();
            List<IdentityResource> identityResources = new List<IdentityResource>();
            List<Task> tasks = new List<Task> {
                Task.Run(async () => {
                    apiResources = await (await _apiResourceCollection.Value.FindAsync(Builders<MongoObjectWrapper<ApiResource>>.Filter.Empty, 
                        new FindOptions<MongoObjectWrapper<ApiResource>, ApiResource> {
                            Projection = MongoObjectWrapper<ApiResource>.ToProjectionDefinition()
                        }
                    )).ToListAsync();
                }),
                Task.Run(async () => {
                    apiScopes = await (await _apiScopeCollection.Value.FindAsync(Builders<MongoObjectWrapper<ApiScope>>.Filter.Empty, 
                        new FindOptions<MongoObjectWrapper<ApiScope>, ApiScope> {
                            Projection = MongoObjectWrapper<ApiScope>.ToProjectionDefinition()
                        }
                    )).ToListAsync();
                }),

                Task.Run(async () => {
                   identityResources = await (await _identityResourceCollection.Value.FindAsync(Builders<MongoObjectWrapper<IdentityResource>>.Filter.Empty, 
                        new FindOptions<MongoObjectWrapper<IdentityResource>, IdentityResource> {
                            Projection = MongoObjectWrapper<IdentityResource>.ToProjectionDefinition()
                        }
                    )).ToListAsync();
                }),
            };
            await Task.WhenAll(tasks);
            return new Resources {
                ApiResources = apiResources,
                ApiScopes = apiScopes,
                IdentityResources = identityResources
            };
        }
    }
}

