using IdentityModel;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using IdentityServer.Models;
using IdentityServer.Models.Wrapper;
using IdentityServer.MongoCollections;
using System.Security.Claims;
using ELSA.Config;

namespace IdentityServer.Store
{
    public class MongoUserStore :
        IUserStore<User>,
        IUserLoginStore<User>,
        IUserRoleStore<User>,
        IUserClaimStore<User>,
        IUserPasswordStore<User>,
        IUserSecurityStampStore<User>,
        IUserEmailStore<User>,
        IUserLockoutStore<User>,
        IUserPhoneNumberStore<User>
    {
        private Lazy<IMongoCollection<MongoObjectWrapper<Claim>>> _claimCollection;
        private Lazy<IMongoCollection<MongoObjectWrapper<UserLoginInfo>>> _userLoginInfoCollection;
        private Lazy<IMongoCollection<MongoObjectWrapper<User>>> _userCollection;
        private IIdentityUserRoleCollection _userRoleCollection;
        private IUserClaimCollection _userClaimCollection;
        public MongoUserStore(IApplicationConfig config, 
            IUserClaimCollection userClaimCollection,
            IIdentityUserRoleCollection userRoleCollection
            )
        {
            _claimCollection = new Lazy<IMongoCollection<MongoObjectWrapper<Claim>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<Claim>>(MongoObjectWrapper<Claim>.GetCollectionName());
            });
            _userLoginInfoCollection = new Lazy<IMongoCollection<MongoObjectWrapper<UserLoginInfo>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<UserLoginInfo>>(MongoObjectWrapper<UserLoginInfo>.GetCollectionName());
            });
            _userCollection = new Lazy<IMongoCollection<MongoObjectWrapper<User>>>(() =>
            {
                var mongoClient = new MongoClient(config.IdentityServer.ConnectionString);
                var mongoDatabase = mongoClient.GetDatabase(config.IdentityServer.DatabaseNamespace);
                return mongoDatabase.GetCollection<MongoObjectWrapper<User>>(MongoObjectWrapper<User>.GetCollectionName());
            });
            _userRoleCollection = userRoleCollection;
            _userClaimCollection = userClaimCollection;
        }
        public async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            await _claimCollection.Value.InsertManyAsync(claims.Select(d => new MongoObjectWrapper<Claim>(d)).ToArray());
        }

        public  async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            await _userLoginInfoCollection.Value.InsertOneAsync(new MongoObjectWrapper<UserLoginInfo>(login));
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            await _userRoleCollection.Value.InsertOneAsync(new MongoObjectWrapper<IdentityUserRole<string>>(new IdentityUserRole<string>
            {
                RoleId = roleName,
                UserId = user.Id
            }));
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var userWrapper = MongoObjectWrapper.Wrap(user);
            user.Id = userWrapper.Object.Id = BsonExtensions.GenerateId();
            await _userCollection.Value.InsertOneAsync(userWrapper);
            await _userClaimCollection.Value.InsertOneAsync(ExtractUserClaims(user));
            //TODO: 
            return IdentityResult.Success;
        }

        private static MongoUserObjectWrapper<List<IdentityUserClaim<string>>> ExtractUserClaims(User user)
        {
            var list = new List<IdentityUserClaim<string>>
            {
                new IdentityUserClaim<string>()
                {
                    UserId = user.Id,
                    ClaimType = JwtClaimTypes.Name,
                    ClaimValue = $"{user.FirstName} {user.LastName}"
                },
                new IdentityUserClaim<string>()
                {
                    UserId = user.Id,
                    ClaimType = JwtClaimTypes.Id,
                    ClaimValue = user.Id
                },
                new IdentityUserClaim<string>()
                {
                    UserId = user.Id,
                    ClaimType = JwtClaimTypes.Email,
                    ClaimValue = user.Email
                },
                new IdentityUserClaim<string>()
                {
                    UserId = user.Id,
                    ClaimType = JwtClaimTypes.GivenName,
                    ClaimValue = user.LastName
                },
                new IdentityUserClaim<string>()
                {
                    UserId = user.Id,
                    ClaimType = JwtClaimTypes.FamilyName,
                    ClaimValue = user.FirstName
                }
            };

            var items = MongoUserObjectWrapper.Wrap(user.Id, list);
            return items;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            await _userCollection.Value.DeleteManyAsync(filter);
            //TODO: 
            return IdentityResult.Success;
        }

        public void Dispose()
        {

        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.NormalizedEmail, normalizedEmail);
            return await (await _userCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<User>, User>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<User>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, userId);
            return await (await _userCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<User>, User>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<User>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
        }

        public Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            //TODO: 
            return null;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.NormalizedUserName, normalizedUserName);
            return await (await _userCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<User>, User>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<User>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
        }

        public async Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var foundUser = await (await _userCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<User>, User>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<User>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
            return foundUser?.AccessFailedCount ?? 0;
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>.Filter.Eq(d => d.UserId, user.Id);
            var result = await (await _userClaimCollection.Value.FindAsync(filter, new FindOptions<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>, List<IdentityUserClaim<string>>>
            {
                Projection = MongoUserObjectWrapper<List<IdentityUserClaim<string>>>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
            var claims = result.Select(d => new Claim(d.ClaimType, d.ClaimValue ?? "")).ToList();
            return claims;
        }

        public async Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.EmailConfirmed ?? false;
        }

        public async Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.LockoutEnabled ?? false;
        }

        public async Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.LockoutEnd;
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            //var filter = Builders<MongoObjectWrapper<UserLoginInfo>>.Filter.Eq(d => d.Object., normalizedEmail);
            //return await(await _userLoginInfoCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<UserLoginInfo>, UserLoginInfo>
            //{
            //    Limit = 1,
            //    Projection = MongoObjectWrapper<User>.ToProjectionDefinition()
            //})).FirstOrDefaultAsync();

            // TODO:
            IList<UserLoginInfo> list = new List<UserLoginInfo>();
            return Task.FromResult(list);
        }

        public async Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.NormalizedEmail;
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.NormalizedUserName;
        }

        public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.PasswordHash;
        }

        public async Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.PhoneNumber;
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.PhoneNumberConfirmed ?? false;
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<IdentityUserRole<string>>>.Filter.Eq(d => d.Object.UserId, user.Id);
            var result = await (await _userRoleCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<IdentityUserRole<string>>, IdentityUserRole<string>>
            {
                Projection = MongoObjectWrapper<IdentityUserRole<string>>.ToProjectionDefinition()
            })).ToListAsync();

            return result.Select(d => d.RoleId).ToList();
        }

        public async Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            var x = await GetUserFromDatabaseAsync(user);
            return x?.SecurityStamp;
        }

        public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.Id;
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return (await GetUserFromDatabaseAsync(user))?.UserName;
        }

        private async Task<User> GetUserFromDatabaseAsync(User user)
        {
            return user;
            var userFilter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            return await (await _userCollection.Value.FindAsync(userFilter, new FindOptions<MongoObjectWrapper<User>, User>
            {
                Limit = 1,
                Projection = MongoObjectWrapper<User>.ToProjectionDefinition()
            })).FirstOrDefaultAsync();
        }
        public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            //var builder = Builders<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>.Filter;
            //var filter = builder.eq(d => d.Object.ClaimValue, claim.Value);
            //var result = await (await _userClaimCollection.Value.FindAsync(filter, new FindOptions<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>, List<IdentityUserClaim<string>>>
            //{
            //    Projection = MongoUserObjectWrapper<List<IdentityUserClaim<string>>>.ToProjectionDefinition()
            //})).ToListAsync();

            //var userFilter = Builders<MongoObjectWrapper<User>>.Filter.In(d => d.Object.Id, result.Select(d => d.UserId));
            //return await (await _userCollection.Value.FindAsync(userFilter, new FindOptions<MongoObjectWrapper<User>, User>
            //{
            //    Projection = MongoObjectWrapper<User>.ToProjectionDefinition()
            //})).ToListAsync();

            // TODO: xxx
            return new List<User>();
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<IdentityUserRole<string>>>.Filter.Eq(d => d.Object.RoleId, roleName);
            var result = await (await _userRoleCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<IdentityUserRole<string>>, IdentityUserRole<string>>
            {
                Projection = MongoObjectWrapper<IdentityUserRole<string>>.ToProjectionDefinition()
            })).ToListAsync();

            var userFilter = Builders<MongoObjectWrapper<User>>.Filter.In(d => d.Object.Id, result.Select(d => d.UserId));
            return await (await _userCollection.Value.FindAsync(userFilter, new FindOptions<MongoObjectWrapper<User>, User>
            {
                Projection = MongoObjectWrapper<User>.ToProjectionDefinition()
            })).ToListAsync();
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public async Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Inc(d => d.Object.AccessFailedCount, 1);
            var afterUser = await _userCollection.Value.FindOneAndUpdateAsync(filter, update, 
                new FindOneAndUpdateOptions<MongoObjectWrapper<User>, MongoObjectWrapper<User>>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                }
            );
            return afterUser.Object.AccessFailedCount;
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var builder = Builders<MongoObjectWrapper<IdentityUserRole<string>>>.Filter;
            var filter = builder.Eq(d => d.Object.RoleId, roleName) &
                         builder.Eq(d => d.Object.UserId, user.Id);
            var a= await (await _userRoleCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<IdentityUserRole<string>>, IdentityUserRole<string>>
            {
                Projection = MongoObjectWrapper<IdentityUserRole<string>>.ToProjectionDefinition()
            })).AnyAsync();
            return a;
        }

        public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var builder = Builders<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>.Filter;
            var userFilter = builder.Eq(d => d.UserId, user.Id);
            //TODO: xxxx
            //var filerDefinitionInClaims = claims.Select(claim => builder.Eq(d => d.Object.ClaimType, claim.Type) & builder.Eq(d => d.Object.ClaimValue, claim.Value)).ToList();
            //var claimsFilter = builder.Or(filerDefinitionInClaims);
            //var filter = builder.And(userFilter, claimsFilter);
            //await _userClaimCollection.Value.DeleteManyAsync(filter);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<IdentityUserRole<string>>>.Filter.Eq(d => d.Object.RoleId, roleName);
            var result = await(await _userRoleCollection.Value.FindAsync(filter, new FindOptions<MongoObjectWrapper<IdentityUserRole<string>>, IdentityUserRole<string>>
            {
                Projection = MongoObjectWrapper<IdentityUserRole<string>>.ToProjectionDefinition()
            })).ToListAsync();
            await _userRoleCollection.Value.DeleteManyAsync(filter);
            var userFilter = Builders<MongoObjectWrapper<User>>.Filter.In(d => d.Object.Id, result.Select(s => s.UserId));
            await _userCollection.Value.DeleteManyAsync(userFilter);
        }

        public Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();

            return Task.CompletedTask;
        }

        public async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            // TODO: xxx
            //var builder = Builders<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>.Filter;
            //var filter = builder.Eq(d => d.UserId, user.Id);
            //var update = Builders<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>.Update
            //            .Set(d => d.Object.ClaimType, newClaim.ValueType)
            //            .Set(d => d.Object.ClaimValue, newClaim.Value);
            //await _userClaimCollection.Value.UpdateOneAsync(filter, update);
        }

        public async Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.AccessFailedCount, 0);
            await _userCollection.Value.FindOneAndUpdateAsync(filter, update);
        }

        public async Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.Email, email);
            await _userCollection.Value.UpdateOneAsync(filter, update);
        }

        public async Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.EmailConfirmed, confirmed);
            await _userCollection.Value.UpdateOneAsync(filter, update);
        }

        public async Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.LockoutEnabled, enabled);
            await _userCollection.Value.UpdateOneAsync(filter, update);
        }

        public async Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.LockoutEnd, lockoutEnd);
            await _userCollection.Value.UpdateOneAsync(filter, update);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            //var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            //var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.NormalizedEmail, normalizedEmail);
            //await _userCollection.Value.UpdateOneAsync(filter, update);

            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            //var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            //var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.NormalizedUserName, normalizedName);
            //await _userCollection.Value.UpdateOneAsync(filter, update);

            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.PasswordHash, passwordHash);
            await _userCollection.Value.UpdateOneAsync(filter, update);
        }

        public async Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.PhoneNumber, phoneNumber);
            await _userCollection.Value.UpdateOneAsync(filter, update);
        }

        public async Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object.PhoneNumberConfirmed, confirmed);
            await _userCollection.Value.UpdateOneAsync(filter, update);
        }

        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp ?? DateTime.UtcNow.Ticks.ToString();
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<MongoObjectWrapper<User>>.Filter.Eq(d => d.Object.Id, user.Id);
            var update = Builders<MongoObjectWrapper<User>>.Update.Set(d => d.Object, user);
            await _userCollection.Value.UpdateOneAsync(filter, update);

            //TODO: 
            return IdentityResult.Success;
        }
    }
}
