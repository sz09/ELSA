using MongoDB.Bson;

public interface ILoggedInUserService 
{
    ObjectId UserId { get; set; }
    string Username { get; set; }
    string AccessToken {get; }
}
