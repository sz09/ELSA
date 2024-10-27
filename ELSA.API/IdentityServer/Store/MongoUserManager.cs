using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using IdentityServer.Models;
using System.Security.Claims;

namespace IdentityServer.Store
{
    internal class MongoUserManager : UserManager<User>
    {
        private readonly IdentityStore _identityStore;
        public MongoUserManager(IdentityStore identityStore, 
            IUserStore<User> userStore,
            IPasswordHasher<User> passwordHasher,
            IOptions<IdentityOptions> optionsAccessor,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<MongoUserManager> logger
            ) : base(store: userStore, 
                passwordHasher: passwordHasher,
                optionsAccessor: optionsAccessor,
                userValidators: userValidators, 
                passwordValidators: passwordValidators,
                keyNormalizer: keyNormalizer,
                errors: errors,
                services: services,
                logger: logger)
        {
            _identityStore = identityStore;
        }

        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return base.AddClaimsAsync(user, claims);
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            return await Store.CreateAsync(user, cancellationToken);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            return Store.DeleteAsync(user, cancellationToken);
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Store.FindByIdAsync(userId, cancellationToken);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
           return Store.FindByNameAsync(normalizedUserName, cancellationToken);  
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            var userClaims = await _identityStore.IdentityUserClaims.Where(d => d.UserId == user.Id).ToListAsync();
            return userClaims.Select(d => d.ToClaim()).ToList();        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Store.GetNormalizedUserNameAsync(user, cancellationToken);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Store.GetUserIdAsync(user, cancellationToken);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Store.GetUserNameAsync(user, cancellationToken);
        }

        public Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return base.GetUsersForClaimAsync(claim);
        }

        public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return base.RemoveClaimsAsync(user, claims);
        }

        public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            return base.ReplaceClaimAsync(user, claim, newClaim);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            return Store.SetNormalizedUserNameAsync(user, normalizedName, cancellationToken);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            return Store.SetUserNameAsync(user, userName, cancellationToken);
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            return Store.UpdateAsync(user, cancellationToken);
        }
    }
}
