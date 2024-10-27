using IdentityServer4.Models;
using IdentityServer4.Stores;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using IdentityServer.Extensions;
using IdentityServer.Models.Wrapper;
using ELSA.Config;

namespace ELSA.IdentityServer
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private Lazy<IMongoCollection<MongoObjectWrapper<PersistedGrant>>> _collection;
        public PersistedGrantStore(IApplicationConfig config)
        {
            _collection = new Lazy<IMongoCollection<MongoObjectWrapper<PersistedGrant>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<PersistedGrant>>(MongoObjectWrapper<PersistedGrant>.GetCollectionName());
            });
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var builder = Builders<MongoObjectWrapper<PersistedGrant>>.Filter;
            var mongoFilter = builder.Empty;
            if (string.IsNullOrEmpty(filter.ClientId))
            {
                mongoFilter &= builder.Eq(d => d.Object.ClientId, filter.ClientId);
            }
            if (string.IsNullOrEmpty(filter.SessionId))
            {
                mongoFilter &= builder.Eq(d => d.Object.SessionId, filter.SessionId);
            }
            if (string.IsNullOrEmpty(filter.SubjectId))
            {
                mongoFilter &= builder.Eq(d => d.Object.SubjectId, filter.SubjectId);
            }
            if (string.IsNullOrEmpty(filter.Type))
            {
                mongoFilter &= builder.Eq(d => d.Object.Type, filter.Type);
            }
            return await (await _collection.Value.FindAsync(mongoFilter, new FindOptions<MongoObjectWrapper<PersistedGrant>, PersistedGrant> {
                Projection = MongoObjectWrapper<PersistedGrant>.ToProjectionDefinition()
            })).ToListAsync();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var builder = Builders<MongoObjectWrapper<PersistedGrant>>.Filter;
            var filter = builder.Eq(d => d.Object.ClientId, IdentityConfiguration.CLIENT_ID) &
                         builder.Eq(d => d.Object.Key, key);
            var findOptions = new FindOptions<MongoObjectWrapper<PersistedGrant>, PersistedGrant>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<PersistedGrant>.ToProjectionDefinition()
            };
            return await (await _collection.Value.FindAsync(filter, findOptions)).FirstOrDefaultAsync();
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            var builder = Builders<MongoObjectWrapper<PersistedGrant>>.Filter;
            var mongoFilter = builder.Empty;
            if (string.IsNullOrEmpty(filter.ClientId))
            {
                mongoFilter &= builder.Eq(d => d.Object.ClientId, filter.ClientId);
            }
            if (string.IsNullOrEmpty(filter.SessionId))
            {
                mongoFilter &= builder.Eq(d => d.Object.SessionId, filter.SessionId);
            }
            if (string.IsNullOrEmpty(filter.SubjectId))
            {
                mongoFilter &= builder.Eq(d => d.Object.SubjectId, filter.SubjectId);
            }
            if (string.IsNullOrEmpty(filter.Type))
            {
                mongoFilter &= builder.Eq(d => d.Object.Type, filter.Type);
            }

            await _collection.Value.DeleteManyAsync(mongoFilter);
        }

        public Task RemoveAsync(string key)
        {
            return _collection.Value.DeleteOneAsync(d => d.Object.ClientId == IdentityConfiguration.CLIENT_ID && d.Object.Key == key);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            return _collection.Value.InsertOneAsync(new MongoObjectWrapper<PersistedGrant>(grant));
        }
    }
}

namespace IdentityServer4.Models
{
    public class PersistedGrantWrapper : PersistedGrant
    {
        [BsonId]
        public string Id { get; set; }
    }
}
