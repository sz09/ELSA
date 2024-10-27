using Humanizer;
using MongoDB.Driver;
using ELSA.DAL.Models.Base;
using ELSA.Config;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using ELSA.CodeChallenge.Repositories.Caching;

namespace ELSA.Repositories.Base
{
    public abstract class BaseSimpleRepository<T> : IBaseSimpleRepository<T>
                    where T : BaseEntity
    {
        public IMongoClient Client => _client.Value;

        protected readonly Lazy<IMongoClient> _client;
        private readonly Lazy<IMongoCollection<T>> _collection;
        public BaseSimpleRepository(IApplicationConfig config)
        {
            _client = new Lazy<IMongoClient>(() => new MongoClient(config.MongoDbConfig.ConnectionString));
            _collection = new Lazy<IMongoCollection<T>>(() =>
            {
                var mongoClient = _client.Value;
                var mongoDatabase = mongoClient.GetDatabase(config.MongoDbConfig.DatabaseNamespace);
                return mongoDatabase.GetCollection<T>(GetCollectionName(typeof(T)));
            });
        }

        public async Task<UpdateResult> UpdateAsync(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions = null)
        {
            if (session != null)
            {
                return await _collection.Value.UpdateOneAsync(session, filter, update, updateOptions ?? new UpdateOptions());
            }
            return await _collection.Value.UpdateOneAsync(filter, update, updateOptions ?? new UpdateOptions());
        }

        private static string GetCollectionName(Type type)
        {
            return type.Name.Pluralize();
        }

        public async Task<T> FindOneAsync(FilterDefinition<T> filter)
        {
            return await(await _collection.Value.FindAsync(filter, new FindOptions<T, T>
            {
                Limit = 1,
                Sort = Builders<T>.Sort.Ascending(d => d.Id) // Get first value inserted
            })).FirstOrDefaultAsync();
        }
    }

    public abstract class BaseRepository<T> : IBaseRepository<T>
                    where T : BaseAuditEntity
    {
        private readonly Lazy<IMongoCollection<T>> _collection;
        private readonly ILoggedInUserService _loggedInUserService;
        private readonly InMemoryCache _memoryCache;

        public IMongoCollection<T> Collection => _collection.Value;

        public BaseRepository(IApplicationConfig config, ILoggedInUserService loggedInUserService, InMemoryCache memoryCache)
        {
            _collection = new Lazy<IMongoCollection<T>>(() =>
            {
                var mongoClient = new MongoClient(config.MongoDbConfig.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.MongoDbConfig.DatabaseNamespace);
                return mongoDatabase.GetCollection<T>(GetCollectionName(typeof(T)));
            });
            _loggedInUserService = loggedInUserService;
            _memoryCache = memoryCache;
        }

        public async Task<T> InsertOneAsync(T entity)
        {
            entity.CreatedAt = entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = entity.CreatedBy = _loggedInUserService.UserId;
            await _collection.Value.InsertOneAsync(entity, new InsertOneOptions { });
            _memoryCache.InvalidateCache(CacheGroup());
            return entity;
        }

        public async Task InsertManyAsync(IList<T> entities)
        {
            if (!entities.Any())
            {
                return;
            }
            foreach (var entity in entities)
            {
                entity.CreatedAt = entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = entity.CreatedBy = _loggedInUserService.UserId;
            }
            await _collection.Value.InsertManyAsync(entities);
            _memoryCache.InvalidateCache(CacheGroup());
        }

        public async Task<UpdateResult> UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions = null)
        {
            var now = DateTime.UtcNow;
            
            update = Builders<T>.Update.Combine(update,
                                Builders<T>.Update.Set(d => d.UpdatedAt, now))
                                                  .Set(d => d.UpdatedBy, _loggedInUserService.UserId);
            var result = await _collection.Value.UpdateOneAsync(GetFilterWithoutDeleted(filter), update, updateOptions ?? new UpdateOptions());
            _memoryCache.InvalidateCache(CacheGroup());
            return result;
        }


        public async Task UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions updateOptions = null)
        {
            var now = DateTime.UtcNow;

            update = Builders<T>.Update.Combine(update,
                                Builders<T>.Update.Set(d => d.UpdatedAt, now))
                                                  .Set(d => d.UpdatedBy, _loggedInUserService.UserId);
            var result = await _collection.Value.UpdateManyAsync(GetFilterWithoutDeleted(filter), update, updateOptions ?? new UpdateOptions());
            _memoryCache.InvalidateCache(CacheGroup());
        }

        public Task<IAsyncCursor<T>> FindAsync(FilterDefinition<T> filter, FindOptions<T, T> findOptions = null)
        {
            return _collection.Value.FindAsync(GetFilterWithoutDeleted(filter), options: findOptions);
        }

        public Task<long> CountAsync(FilterDefinition<T> filter)
        {
            return _collection.Value.CountDocumentsAsync(GetFilterWithoutDeleted(filter));
        }

        public async Task<T> FindOneAsync(FilterDefinition<T> filter)
        {
            return await (await _collection.Value.FindAsync(GetFilterWithoutDeleted(filter), new FindOptions<T, T>
            {
                Limit = 1,
                Sort = Builders<T>.Sort.Ascending(d => d.Id) // Get first value inserted
            })).FirstOrDefaultAsync();
        }

        public async Task ReplaceOneAsync(T entity)
        {
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = _loggedInUserService.UserId;
            var filter = Builders<T>.Filter.Eq(d => d.Id, entity.Id);
            await _collection.Value.ReplaceOneAsync(GetFilterWithoutDeleted(filter), entity, new ReplaceOptions
            {
                IsUpsert = false
            });
            _memoryCache.InvalidateCache(CacheGroup());
        }
        public async Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter)
        {
            var result = await _collection.Value.DeleteOneAsync(GetFilterWithoutDeleted(filter));
            _memoryCache.InvalidateCache(CacheGroup());
            return result;
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter)
        {
            var result = await _collection.Value.DeleteManyAsync(GetFilterWithoutDeleted(filter));
            _memoryCache.InvalidateCache(CacheGroup());
            return result;
        }

        public async Task<DeleteResult> SoftDeleteOneAsync(FilterDefinition<T> filter)
        {
            var now = DateTime.UtcNow;
            var update = Builders<T>.Update.Set(d => d.IsDeleted, true)
                                           .Set(d => d.UpdatedAt, now)
                                           .Set(d => d.UpdatedBy, _loggedInUserService.UserId)
                                           ;
            var result = await _collection.Value.UpdateOneAsync(GetFilterWithoutDeleted(filter), update);
            _memoryCache.InvalidateCache(CacheGroup());
            return DeleteResult.Unacknowledged.Instance;
        }

        public async Task<DeleteResult> SoftDeleteManyAsync(FilterDefinition<T> filter)
        {
            var now = DateTime.UtcNow;

            var update = Builders<T>.Update.Set(d => d.IsDeleted, true)
                                           .Set(d => d.UpdatedAt, now)
                                           .Set(d => d.UpdatedBy, _loggedInUserService.UserId)
                                           ;
            var result = await _collection.Value.UpdateManyAsync(GetFilterWithoutDeleted(filter), update);
            _memoryCache.InvalidateCache(CacheGroup());
            return DeleteResult.Unacknowledged.Instance;
        }

        private static string GetCollectionName(Type type)
        {
            return type.Name.Pluralize();
        }

        public async Task<T> GetOrCacheByIdAsync(ObjectId id)
        {
            return await _memoryCache.GetOrCreateAsync(CacheGroup(), id.ToString(), () => FindOneAsync(Builders<T>.Filter.Eq(d => d.Id, id)));
        }

        private FilterDefinition<T> GetFilterWithoutDeleted(FilterDefinition<T> filter)
        {
            return filter & Builders<T>.Filter.Eq(d => d.IsDeleted, false);
        }
        private string CacheGroup()
        {
            return typeof(T).Name.Pluralize();
        }
    }
}

