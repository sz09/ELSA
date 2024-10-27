
using AutoMapper;
using ELSA.CodeChallenge.Repositories.Caching;
using ELSA.Config;
using ELSA.Repositories;
using ELSA.Repositories.MongoExtensions;
using ELSA.Services;
using ELSA.Services.Interface;
using ELSA.Services.Utils;
using ELSA.WebAPI.Const;
using ELSA.WebAPI.Filters;
using ELSA.WebAPI.Helpers;
using ELSA.WebAPI.Middlewares;
using ELSA.WebAPI.ModelBinders;
using ELSA.WebAPI.Utitlities;
using IdentityServer.Clients;
using IdentityServer.Extensions;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Extensions;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Func<IServiceProvider, ApplicationConfig> serviceConfigFunc = x => builder.Configuration.Get<ApplicationConfig>();
        var services = builder.Services;
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
        services.AddControllers().AddNewtonsoftJson(d =>
        {
            d.SerializerSettings.Converters.Add(new CreateQuestionConverter());
        });
        services.AddSingleton<IApplicationConfig>(serviceConfigFunc);
        // Add services to the container.
        services.AddControllers(options =>
        {
            options.Filters.Add<ResponseLoggingFilter>();
            options.Filters.Add<GlobalExceptionFilter>();
        }).AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new ObjectIdConverter());
        });
        services.AddVersionedApiExplorer(o =>
        {
            o.GroupNameFormat = "'v'VVV";
            o.SubstituteApiVersionInUrl = true;
        });
        services.AddEndpointsApiExplorer();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            options.DocInclusionPredicate((documentName, apiDescription) =>
            {
                var actionApiVersionModel = apiDescription.ActionDescriptor.GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);
                var apiExplorerSettingsAttribute = (ApiExplorerSettingsAttribute)apiDescription.ActionDescriptor.EndpointMetadata.First(x => x.GetType().Equals(typeof(ApiExplorerSettingsAttribute)));
                var apiVersionAttribute = (ApiVersionAttribute)apiDescription.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x.GetType().Equals(typeof(ApiVersionAttribute)));

                var group = documentName.Split(' ');
                var dName = group.First();
                var isMatchVersion = true;
                if (apiVersionAttribute != null)
                {
                    var version = group.Last();
                    if (int.TryParse(version.Replace("v", ""), out var intVersion))
                        isMatchVersion = apiVersionAttribute.Versions.Where(d => d.MajorVersion == intVersion).Any();
                }

                return !apiExplorerSettingsAttribute.GroupName.IsNullOrEmpty()
                        && isMatchVersion
                        && dName.Equals(apiExplorerSettingsAttribute.GroupName);
            });
            var serviceProvider = services.BuildServiceProvider();
            var apiDescriptionGroupCollectionProvider = serviceProvider.GetService<IApiDescriptionGroupCollectionProvider>();
            var versionProvider = serviceProvider.GetService<IApiVersionDescriptionProvider>();
            foreach (var description in apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items)
            {
                var groupName = description.GroupName;
                if (groupName != "Authenticate")
                {
                    foreach (var versionDescription in versionProvider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc($"{groupName} {versionDescription.GroupName}", new OpenApiInfo
                        {
                            Title = $"{groupName} {versionDescription.GroupName}",
                            Description = $"ELSA API - {groupName} - {versionDescription.GroupName}",
                            Version = versionDescription.GroupName
                        });
                    }
                }
            }
        });
        RegisterAutoMapper(services);
        services.AddScoped<ResponseLoggingFilter>();
        services.UseLoggedInUserPassToOtherLayer();
        RegisterComponentsByPattern(services, typeof(IBaseService<>), "Service");
        RegisterComponentsByPattern(services, typeof(IBaseSimpleService<>), "Service");
        RegisterComponentsByPattern(services, typeof(IBaseRepository<>), "Repository");
        RegisterComponentsByPattern(services, typeof(IBaseSimpleRepository<>), "Repository");
        services.AddTransient<ITempService, TempService>();
        services.AddApiVersioning();
        services.AddTransient<IIdentityServerClient, IdentityServerClient>();
        services.AddIdentityServer()
            .AddInMemoryCaching()
            .AddClientStore<InMemoryClientStore>()
            .AddResourceStore<InMemoryResourcesStore>();
        services.AddLocalApiAuthentication();
        services.AddSingleton(d =>
        {
            return new InMemoryCache(d.GetRequiredService<IOptions<MemoryCacheOptions>>(), TimeSpan.FromHours(1));
        });
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
        {
            var applicationConfig = services.BuildServiceProvider().GetRequiredService<IApplicationConfig>();
            //options.SaveToken = true;
            options.Authority = applicationConfig.JwtSettings.Authority;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        });
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthConst.GROUP_ADMIN, policy =>
            {
                policy.RequireRole(IdentityConfiguration.API_ROLE_READ.ToUpper());
                policy.RequireRole(IdentityConfiguration.API_ROLE_WRITE.ToUpper());
                policy.RequireRole(IdentityConfiguration.API_ROLE_ADMIN.ToUpper());
                policy.RequireAuthenticatedUser();
            });
        });
        BsonClassMapExtensions.RegisterBsonClassMap();
        services.AddMvcCore(opts =>
        {
            opts.ModelBinderProviders.Insert(0, new CustomQuestionBinderProvider());
        });
        builder.Services.AddTransient<IFileService, FileService>();
        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI(swaggerOptions =>
        {
            var groupProvider = services.BuildServiceProvider().GetService<IApiDescriptionGroupCollectionProvider>();
            var versionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            foreach (var description in groupProvider.ApiDescriptionGroups.Items)
            {
                if (description.GroupName != "Authenticate")
                {
                    foreach (var version in versionProvider.ApiVersionDescriptions)
                    {
                        swaggerOptions.SwaggerEndpoint($"{description.GroupName} {version.GroupName}/swagger.json", $"{description.GroupName} {version.GroupName}");
                    }
                }
            }
        });
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }
        app.UseRouting();
        app.UseCors("AllowSpecificOrigins");
        app.UseAuthorization();
        app.UseMiddleware<PrepareUserDataMiddleware>();
        app.MapControllers();
        app.UseStaticFiles();
        app.Run();
    }

    private static void RegisterComponentsByPattern(IServiceCollection services, Type iBaseType,
        string pattern)
    {
        var classTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(p => p.GetTypes())
            .Where(t => t.Namespace == iBaseType.Namespace && t.IsClass && !t.IsGenericType && t.Name.EndsWith(pattern))
            .ToList();

        foreach (var classType in classTypes)
        {
            var interfaceTypes = classType.GetInterfaces().Where(d => d != iBaseType).ToList();
            foreach (var interfaceType in interfaceTypes)
            {
                services.AddScoped(interfaceType, classType);
            }
        }
    }

    private static void RegisterAutoMapper(IServiceCollection services)
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ELSA.WebAPI.AutoMappers.MappingProfile());
            mc.AddProfile(new ELSA.Services.AutoMappers.MappingProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
    }

}