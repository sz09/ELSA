using ELSA.Config;
using IdentityServer.Extensions;
using ELSA.IdentityServer.Seed;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
// Add services to the container.
builder.Services.AddControllers();
// ReSharper disable once ConvertToLocalFunction
Func<IServiceProvider, ApplicationConfig> serviceConfigFunc = x => builder.Configuration.Get<ApplicationConfig>()!;
builder.Services.AddSingleton<IApplicationConfig>(serviceConfigFunc);
builder.Services.UseIdentityServer();
builder.Services.AddTransient<ISeeder, Seeder>();

MongoDbExtentions.ConfigureBsonClassMap();
var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();

app.UseIdentityServer();
app.MapControllers();
app.Run();

