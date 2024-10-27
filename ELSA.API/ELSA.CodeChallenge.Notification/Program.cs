using ELSA.CodeChallenge.NotificationAPI.Services;
using ELSA.Config;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
Func<IServiceProvider, ApplicationConfig> serviceConfigFunc = x => builder.Configuration.Get<ApplicationConfig>();
var services = builder.Services;
services.AddSingleton<ISignalRService, SignalRService>();
services.AddSingleton<IApplicationConfig>(serviceConfigFunc);
services.AddHostedService<ELSABackgroundService>();
services.AddSignalR();
services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            
            builder.WithOrigins("http://localhost:4200") // Angular app URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});
var app = builder.Build();

app.MapGet("/", () => "Notification api");
app.MapHub<SingalRHub>("/ELSAClientHUB");
app.UseCors("AllowSpecificOrigins");
app.Run();
