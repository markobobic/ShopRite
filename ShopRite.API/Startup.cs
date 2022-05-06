using Amazon.S3;
using Coravel;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents;
using Raven.DependencyInjection;
using Raven.Identity;
using ShopRite.Core.Configurations;
using ShopRite.Core.Constants;
using ShopRite.Core.Interfaces;
using ShopRite.Core.Middleware;
using ShopRite.Core.Pipelines;
using ShopRite.Core.Services;
using ShopRite.Domain;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Extensions;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Integrations;
using System.Reflection;
using System.Text;

namespace ShopRite.API
{
    public class Startup
    {
        private const string Issuer = "Token:Issuer";
        private const string Key = "Token:Key";
        private readonly IConfiguration _configuration;
        private readonly GlobalConfiguration _globalConfig;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _globalConfig = _configuration.Get<GlobalConfiguration>();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDefaultAWSOptions(_configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddJsonMultipartFormDataSupport(JsonSerializerChoice.SystemText);

            services.AddSingleton<IDocumentStore>(provider =>
            {
                var store = new DocumentStore()
                {
                    Urls = _globalConfig.Database.Urls,
                    Database = _globalConfig.Database.RavenDatabaseName
                };
                store.Initialize();
                return store;
            })
            .AddRavenDbAsyncSession()
           .AddIdentity<AppUser, IdentityRole>()
           .AddRavenDbIdentityStores<AppUser, IdentityRole>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[Key])),
                    ValidIssuer = _configuration[Issuer],
                    ValidateIssuer = true,
                    ValidateAudience = false
                };
            });

            services.AddMediatR(Assembly.Load(Assemblies.ShopRitePlatform));
            Assembly core = Assembly.Load(Assemblies.ShopRiteCore);
            services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var config = ConfigurationOptions.Parse(_globalConfig.Database.RedisDatabaseName, true);
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAwsService, AwsService>();
            services.AddMailer(_configuration);

            AssemblyScanner.FindValidatorsInAssembly(core)
                .ForEach(x => services.AddTransient(typeof(IValidator), x.ValidatorType));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));
            services.Scan(x =>
            {
                x.FromAssemblies(Assembly.Load(Assemblies.ShopRitePlatform))
                    .AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSwaggerGen(swagger =>
            {

                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT Token Authentication API",
                    Description = "ASP.NET Core 3.1 Web API"
                });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                            new string[] {}
                    }
                });
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopRite.API v1"));

            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();

            app.UseRouting();
           
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
