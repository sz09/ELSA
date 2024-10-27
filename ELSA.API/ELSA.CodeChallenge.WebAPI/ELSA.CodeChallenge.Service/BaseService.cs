using MongoDB.Bson;
using MongoDB.Driver;
using ELSA.DAL.Models.Base;
using ELSA.Repositories;
using ELSA.Services.Utils;

namespace ELSA.Services
{

    public abstract class BaseSimpleService<T> : IBaseSimpleService<T> where T : BaseEntity
    {

        protected readonly IBaseSimpleRepository<T> Repository;

        public BaseSimpleService(IBaseSimpleRepository<T> repository)
        {
            Repository = repository;
        }

        public Task UpdateAsync(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions)
        {
            return Repository.UpdateAsync(session, filter, update, updateOptions);
        }

    }
    public abstract class BaseService<T> : IBaseService<T> where T : BaseAuditEntity
	{
		protected readonly IBaseRepository<T> Repository;

        public BaseService(IBaseRepository<T> repository)
		{
			Repository = repository;
		}

        public virtual Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter)
        {
            return Repository.DeleteManyAsync(filter);
        }

        public virtual Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter)
        {
            return Repository.DeleteOneAsync(filter);
        }

        public virtual Task<IAsyncCursor<T>> FindAsync(FilterDefinition<T> filter, FindOptions<T, T> findOptions = null)
        {
            return Repository.FindAsync(filter);
        }

        public virtual Task<IAsyncCursor<T>> GetAllAsync()
        {
            return Repository.FindAsync(FilterDefinition<T>.Empty);
        }

        public virtual Task<long> CountAsync()
        {
            return Repository.CountAsync(FilterDefinition<T>.Empty);
        }

        public virtual Task CreateManyAsync(IList<T> entities)
        {
            return Repository.InsertManyAsync(entities);
        }

        public virtual Task<T> CreateAsync(T entity)
        {
            return Repository.InsertOneAsync(entity);
        }

        public virtual Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions)
        {
            return Repository.UpdateAsync(filter, update, updateOptions);
        }

        public virtual async Task<PageResult<T>> FetchAsync(FetchRequest fetchRequest)
        {
            var filter = Builders<T>.Filter.Empty;
            var count = await Repository.CountAsync(filter);
            if (count == 0)
            {
                return new PageResult<T>
                {
                    Data = [],
                    Total = 0
                };
            }

            var data = await (await Repository.FindAsync(filter, new FindOptions<T, T>
            {
                Limit = fetchRequest.PageSize,
                Skip = fetchRequest.Skip,
                Sort = fetchRequest.ToSort<T>()
            })).ToListAsync();

            return new PageResult<T>
            {
                Data = data,
                Total = count
            };
        }

        public virtual Task<T> GetAsync(ObjectId id)
        {
            return Repository.FindOneAsync(Builders<T>.Filter.Eq(d => d.Id, id));
        }

        public virtual Task<DeleteResult> DeleteOneAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(d => d.Id, id);
            return Repository.DeleteOneAsync(filter);
        }
    }
}

