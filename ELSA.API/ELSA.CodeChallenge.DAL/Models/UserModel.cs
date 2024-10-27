using ELSA.DAL.Models.Base;

namespace ELSA.Repositories.Models
{
    public class UserModel : BaseAuditEntity
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacebookProfile { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
