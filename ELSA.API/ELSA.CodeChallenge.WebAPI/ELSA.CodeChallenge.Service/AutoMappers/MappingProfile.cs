using AutoMapper;
using ELSA.Repositories.Models;
using ELSA.Services.Models.Users;
using IdentityRegisterUserModel = IdentityServer.Models.RegisterUserModel;

namespace ELSA.Services.AutoMappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            WriteToRead();
            MapToWrite();
        }


        void WriteToRead()
        {

        }

        void MapToWrite()
        {
            CreateMap<RegisterUserModel, UserModel>();
            CreateMap<RegisterUserModel, IdentityRegisterUserModel>();
        }
    }
}
