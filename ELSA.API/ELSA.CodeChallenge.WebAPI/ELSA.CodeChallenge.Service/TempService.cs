using IdentityServer.Clients;
using ELSA.Config;
using ELSA.Services.Interface;

namespace ELSA.Services
{
    public class TempService(IHttpClientFactory httpClientFactory, IUserService userService, IApplicationConfig applicationConfig) : IntegrationClient(httpClientFactory), ITempService
    {
        public async Task SeedAsync(string adminUserName, string adminEmail, string adminPassword)
        {
            var response = await PostAsync<dynamic>($"{applicationConfig.JwtSettings.Authority}/temp/seed", new { });
            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            await userService.RegisterUserAsync(new Models.Users.RegisterUserModel
            {
                Email = adminEmail,
                Password = adminPassword,
                Username = adminUserName,
                FirstName = "Admin",
                LastName = "Admin"
            }, AccessType.AdminUserFullAccess);
        }
    }
}
