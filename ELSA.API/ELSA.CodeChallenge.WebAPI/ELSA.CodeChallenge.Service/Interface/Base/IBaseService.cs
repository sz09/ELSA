using MongoDB.Bson;
using MongoDB.Driver;
using ELSA.DAL.Models.Base;
using ELSA.Services.Utils;

namespace ELSA.Services
{
    public interface IBaseSimpleService<T> where T : BaseEntity
    {
        Task UpdateAsync(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions);
    }

    public interface IBaseService<T> where T : BaseAuditEntity
    {
        Task<T> CreateAsync(T entity);
        Task CreateManyAsync(IList<T> entities);
        protected Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions);
        Task<IAsyncCursor<T>> FindAsync(FilterDefinition<T> filter, FindOptions<T, T> findOptions = null);
        Task<IAsyncCursor<T>> GetAllAsync();
        Task<long> CountAsync();
        Task<DeleteResult> DeleteOneAsync(ObjectId id);
        Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter);
        Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter);
        Task<PageResult<T>> FetchAsync(FetchRequest fetchRequest);
        Task<T> GetAsync(ObjectId id);
    }
}

