using IdentityServer.Models;
using IdentityServer.Models.Wrapper;
using IdentityServer4.Models;
using MongoDB.Bson.Serialization;

namespace IdentityServer.Extensions
{
    public static class MongoDbExtentions
    {
        public static void ConfigureBsonClassMap()
        {
            BsonClassMap.RegisterClassMap<User>();
            BsonClassMap.RegisterClassMap<Role>();
            BsonClassMap.RegisterClassMap<MongoObjectWrapper<Client>>();
            BsonClassMap.RegisterClassMap<MongoObjectWrapper<PersistedGrant>>();
            BsonClassMap.RegisterClassMap<MongoObjectWrapper<IdentityResource>>();
            BsonClassMap.RegisterClassMap<MongoObjectWrapper<ApiResource>>();
            BsonClassMap.RegisterClassMap<MongoObjectWrapper<ApiScope>>();
        }
    }
}
