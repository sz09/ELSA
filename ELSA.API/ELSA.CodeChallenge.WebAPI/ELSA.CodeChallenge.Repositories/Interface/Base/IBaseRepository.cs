using MongoDB.Driver;
using ELSA.DAL.Models.Base;
using MongoDB.Bson;

namespace ELSA.Repositories
{
    public interface IBaseSimpleRepository<T> where T : BaseEntity
    {
        IMongoClient Client { get; }
        Task<T> FindOneAsync(FilterDefinition<T> filter);
        Task<UpdateResult> UpdateAsync(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions = null);
    }

    public interface IBaseRepository<T> where T: BaseAuditEntity
    {
        IMongoCollection<T> Collection { get; }
        Task<T> InsertOneAsync(T entity);
        Task InsertManyAsync(IList<T> entities);
        Task ReplaceOneAsync(T entity);
        Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions = null);
        Task<UpdateResult> UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions = null);
        Task<IAsyncCursor<T>> FindAsync(FilterDefinition<T> filter, FindOptions<T, T> findOptions = null);
        Task<long> CountAsync(FilterDefinition<T> filter);
        Task<T> FindOneAsync(FilterDefinition<T> filter);
        Task<T> GetOrCacheByIdAsync(ObjectId id);
        Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter);
        Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter);
        Task<DeleteResult> SoftDeleteOneAsync(FilterDefinition<T> filter);
        Task<DeleteResult> SoftDeleteManyAsync(FilterDefinition<T> filter);
    }
}

