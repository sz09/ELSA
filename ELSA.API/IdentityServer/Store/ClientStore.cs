using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Driver;
using IdentityServer.Models.Wrapper;
using ELSA.Config;

namespace IdentityServer.Store
{
    public class ClientStore: IClientStore
	{
        private Lazy<IMongoCollection<MongoObjectWrapper<Client>>> _collection;
        public ClientStore(IApplicationConfig config)
        {
            _collection = new Lazy<IMongoCollection<MongoObjectWrapper<Client>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<Client>>(MongoObjectWrapper<Client>.GetCollectionName());
            });
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var filter = Builders<MongoObjectWrapper<Client>>.Filter.Eq(d => d.Object.ClientId, clientId);
            return await (await _collection.Value.FindAsync(filter, 
                new FindOptions<MongoObjectWrapper<Client>, Client>
                {
                    Limit = 1,
                    Projection = MongoObjectWrapper<Client>.ToProjectionDefinition()
                })).FirstOrDefaultAsync();
        }
    }
}

