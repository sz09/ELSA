using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer.Models
{
    [BsonIgnoreExtraElements]
	public class Role: IdentityRole<string>
	{
		public Role()
		{
		}
	}
}

