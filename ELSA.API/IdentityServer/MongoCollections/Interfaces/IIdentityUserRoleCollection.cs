using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using IdentityServer.Models.Wrapper;

namespace IdentityServer.MongoCollections
{
    public interface IIdentityUserRoleCollection
    {
        IMongoCollection<MongoObjectWrapper<IdentityUserRole<string>>> Value { get; }
    }
}
