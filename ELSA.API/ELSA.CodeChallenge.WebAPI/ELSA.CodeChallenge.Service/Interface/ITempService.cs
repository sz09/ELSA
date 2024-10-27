namespace ELSA.Services.Interface
{
    public interface ITempService
    {
        Task SeedAsync(string adminUserName, string adminEmail, string adminPassword);
    }
}
