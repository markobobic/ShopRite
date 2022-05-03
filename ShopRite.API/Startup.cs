using Amazon.S3;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IConfiguration _configuration;
        private readonly DatabaseConfig _dbConfig;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConfig = _configuration.Get<DatabaseConfig>();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDefaultAWSOptions(_configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
            services.AddJsonMultipartFormDataSupport(JsonSerializerChoice.SystemText);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"])),
                    ValidIssuer = _configuration["Token:Issuer"],
                    ValidateIssuer = true,
                };
            });
            services.AddSingleton<IDocumentStore>(provider =>
            {
                var store = new DocumentStore()
                {
                    Urls = _dbConfig.Database.Urls,
                    Database = _dbConfig.Database.RavenDatabaseName
                };
                store.Initialize();
                return store;
            })
            .AddRavenDbAsyncSession()
           .AddIdentity<AppUser, IdentityRole>()
           .AddRavenDbIdentityStores<AppUser, IdentityRole>();

            services.AddMediatR(Assembly.Load(Assemblies.ShopRitePlatform));
            Assembly core = Assembly.Load(Assemblies.ShopRiteCore);
            services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var config = ConfigurationOptions.Parse(_dbConfig.Database.RedisDatabaseName, true);
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddScoped<ITokenService, TokenService>();
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShopRite.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
