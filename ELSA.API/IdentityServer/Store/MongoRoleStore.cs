using IdentityServer.Models;
using IdentityServer.Models.Wrapper;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using ELSA.Config;

namespace IdentityServer.Store
{
    public class MongoRoleStore : IRoleStore<Role>
    {
        private Lazy<IMongoCollection<MongoObjectWrapper<Role>>> _roleCollection;
        public MongoRoleStore(IApplicationConfig config)
        {
            _roleCollection = new Lazy<IMongoCollection<MongoObjectWrapper<Role>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<Role>>(MongoObjectWrapper<Role>.GetCollectionName());
            });
        }
        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            await _roleCollection.Value.InsertOneAsync(new MongoObjectWrapper<Role>(role));
            //TODO: 
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, role.Id);
            await _roleCollection.Value.DeleteOneAsync(filter);
            //TODO: 
            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, roleId);
            return await (await _roleCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<Role>, Role>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<Role>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.NormalizedName, normalizedRoleName);
            return await(await _roleCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<Role>, Role>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<Role>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
        }

        public async Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, role.Id);
            return (await GetRoleFromDatabaseAsync(role))?.NormalizedName;
        }

        public async Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, role.Id);
            return (await GetRoleFromDatabaseAsync(role))?.Id;
        }

        public async Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, role.Id);
            return (await GetRoleFromDatabaseAsync(role))?.Name;
        }

        public async Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, role.Id);
            var update = Builders<MongoObjectWrapper<Role>>.Update.Set(d => d.Object.NormalizedName, normalizedName);
            await _roleCollection.Value.UpdateOneAsync(filter, update);
        }

        public async Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, role.Id);
            var update = Builders<MongoObjectWrapper<Role>>.Update.Set(d => d.Object.Name, roleName);
            await _roleCollection.Value.UpdateOneAsync(filter, update);
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, role.Id);
            var update = Builders<MongoObjectWrapper<Role>>.Update.Set(d => d.Object, role);
            await _roleCollection.Value.UpdateOneAsync(filter, update);

            //TODO:
            return IdentityResult.Success;
        }

        private async Task<Role> GetRoleFromDatabaseAsync(Role role)
        {
            return role;
            var userFilter = Builders<MongoObjectWrapper<Role>>.Filter.Eq(d => d.Object.Id, role.Id);
            return await (await _roleCollection.Value.FindAsync(userFilter, new FindOptions<MongoObjectWrapper<Role>, Role>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<Role>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
        }
    }
}
