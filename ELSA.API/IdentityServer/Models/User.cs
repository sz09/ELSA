using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer.Models
{
    [BsonIgnoreExtraElements]
    public class User: IdentityUser<string>
	{
        public ObjectId UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public User()
		{
		}
	}
}

