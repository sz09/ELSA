using AutoMapper;
using ELSA.Repositories.Interface;
using ELSA.Repositories.Models;
using ELSA.Services.Interface;
using ELSA.Services.Models.Users;
using IdentityServer.Clients;
using MongoDB.Bson;
using MongoDB.Driver;
using IdentityRegisterUserModel = IdentityServer.Models.RegisterUserModel;

namespace ELSA.Services
{
    public class UserService(IUserRepository userRepository, IIdentityServerClient identityServerClient, IMapper mapper) : BaseService<UserModel>(userRepository), IUserService
    {
        public async Task<ObjectId> RegisterUserAnonymousAsync(string email, string username)
        {
            var filterByEmail = Builders<UserModel>.Filter.Eq(d => d.Email, email);
            var existingUser = await userRepository.FindOneAsync(filterByEmail);
            if (existingUser != null)
            {
                throw new Exception($"User already exist with {email}");
            }

            var newUser = await userRepository.InsertOneAsync(new UserModel
            {
                Email = email,
                Nickname = username,
            });

            return newUser.Id;
        }

        public async Task<UserModel> RegisterUserAsync(RegisterUserModel model, AccessType? accessType = null)
        {
            var identityRegisterUserModel = mapper.Map<IdentityRegisterUserModel>(model);
            identityRegisterUserModel.AccessType = accessType ?? AccessType.NormalUserFullAccess;
            var identityId = await identityServerClient.RegisterUserAsync(identityRegisterUserModel);
            var userModel = mapper.Map<UserModel>(model);
            var result = await CreateAsync(userModel);
            await identityServerClient.UpdateUserIdAsync(identityId, result.Id.ToString());
            return result;
        }
    }
}