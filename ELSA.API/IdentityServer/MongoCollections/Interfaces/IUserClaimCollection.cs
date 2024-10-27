using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using IdentityServer.Models.Wrapper;

namespace IdentityServer.MongoCollections
{
    public interface IUserClaimCollection
    {
        IMongoCollection<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>> Value { get; }
    }
}
