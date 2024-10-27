using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using IdentityServer.Models.Wrapper;
using ELSA.Config;

namespace IdentityServer.MongoCollections
{
    internal class IdentityUserRoleCollection(IApplicationConfig config) : IIdentityUserRoleCollection
    {
        private Lazy<IMongoCollection<MongoObjectWrapper<IdentityUserRole<string>>>> mongoCollection = new Lazy<IMongoCollection<MongoObjectWrapper<IdentityUserRole<string>>>>(() =>
        {
            var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
            return mongoDatabase.GetCollection<MongoObjectWrapper<IdentityUserRole<string>>>(MongoObjectWrapper<IdentityUserRole<string>>.GetCollectionName());
        });
        public IMongoCollection<MongoObjectWrapper<IdentityUserRole<string>>> Value => mongoCollection.Value;
    }

    internal static partial class MongoCollectionExtentions
    {
        internal static IServiceCollection UseIdentityUserRoleCollection(this IServiceCollection serviceCollection)
        {
           return serviceCollection.AddScoped<IIdentityUserRoleCollection, IdentityUserRoleCollection>();
        }
    }
}
