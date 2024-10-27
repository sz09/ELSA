using IdentityServer.Extensions;
using IdentityServer.Models;
using IdentityServer.Models.Wrapper;
using IdentityServer.MongoCollections;
using IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ELSA.IdentityServer.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController(UserManager<User> _userManager, IUserClaimCollection _userClaimCollection,
       IPasswordHasher<User> _passwordHasher, MongoUserStore _mongoUserStore) : BaseController
    {

       [HttpPost("")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var registerResponse = await _userManager.CreateAsync(user: user);
            if (registerResponse.Succeeded)
            {
                var userRegistered = await _userManager.FindByIdAsync(user.Id);
                await _userManager.AddToRolesAsync(userRegistered, model.AccessType.Roles);
                await _mongoUserStore.SetPasswordHashAsync(user, _passwordHasher.HashPassword(userRegistered, model.Password), default);

                return Ok(new
                {
                    IdentityId = userRegistered.Id
                });
            }

            return BadRequest(new { registerResponse.Errors });
        }

        [HttpPut("update/{identityId}/userId/{userId}")]
        public async Task<IActionResult> UpdateUserIdAsync(string identityId, ObjectId userId)
        {
            var user = await _userManager.FindByIdAsync(identityId);
            if (user != null)
            {
                user.UserId = userId;
                await _userManager.UpdateAsync(user);
                var filter = Builders<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>.Filter.Eq(d => d.UserId, user.Id);
                var currentUser = await (await _userClaimCollection.Value.FindAsync(filter, new FindOptions<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>, MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>
                {
                    Limit = 1
                })).FirstOrDefaultAsync();
                if (currentUser is not null)
                {
                    var update = Builders<MongoUserObjectWrapper<List<IdentityUserClaim<string>>>>.Update.AddToSet(d => d.Object, new()
                    {
                        UserId = user.Id,
                        ClaimType = ELSAClaims.USER_ID,
                        ClaimValue = userId.ToString()
                    });
                    await _userClaimCollection.Value.UpdateOneAsync(filter, update);
                }
                return Ok();
            }

            return BadRequest();
        }
    }
}
