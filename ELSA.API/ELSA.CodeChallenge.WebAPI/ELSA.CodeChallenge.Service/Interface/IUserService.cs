using IdentityServer.Clients;
using ELSA.Repositories.Models;
using ELSA.Services.Models.Users;
using MongoDB.Bson;

namespace ELSA.Services.Interface
{
    public interface IUserService : IBaseService<UserModel>
    {
        Task<UserModel> RegisterUserAsync(RegisterUserModel model, AccessType? accessType = null);
        Task<ObjectId> RegisterUserAnonymousAsync(string email, string username);
    }
}

