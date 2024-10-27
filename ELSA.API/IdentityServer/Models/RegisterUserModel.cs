using IdentityServer.Clients;

namespace IdentityServer.Models
{
    public class RegisterUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacebookProfile { get; set; }
        public string ProfileImageUrl { get; set; }
        public AccessType AccessType { get; set; }
    }
}
