using MongoDB.Bson;

namespace ELSA.WebAPI.Utitlities
{
    public class LoggedInUserService : ILoggedInUserService
    {
        public ObjectId UserId { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
    }

    internal static class LoggedInUserExtensions {
        internal static void UseLoggedInUserPassToOtherLayer(this IServiceCollection serviceDescriptors) {
            serviceDescriptors.AddScoped<ILoggedInUserService, LoggedInUserService>();
        }
    }
}