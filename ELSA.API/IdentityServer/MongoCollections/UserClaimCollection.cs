using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using IdentityServer.Models.Wrapper;
using ELSA.Config;

namespace IdentityServer.MongoCollections
{
    internal class UserClaimCollection(IApplicationConfig config) : IUserClaimCollection
    {
        private Lazy<IMongoCollection<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>> mongoCollection = new Lazy<IMongoCollection<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>>(() =>
        {
            var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
            return mongoDatabase.GetCollection<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>(MongoUserObjectWrapper<List<IdentityUserClaim<string>>>.GetCollectionName());
        });
        public IMongoCollection<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>> Value => mongoCollection.Value;
    }

    internal static partial class MongoCollectionExtentions
    {
        internal static IServiceCollection UseUserClaimCollection(this IServiceCollection serviceCollection)
        {
           return serviceCollection.AddScoped<IUserClaimCollection, UserClaimCollection>();
        }
    }
}
