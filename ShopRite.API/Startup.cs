using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents;
using ShopRite.Core.Configurations;
using ShopRite.Core.Constants;
using ShopRite.Core.Middleware;
using ShopRite.Core.Pipelines;
using StackExchange.Redis;
using System.Reflection;

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

            services.AddSingleton<IDocumentStore>(provider =>
            {
                var store = new DocumentStore()
                {
                    Urls = _dbConfig.Database.Urls,
                    Database = _dbConfig.Database.RavenDatabaseName
                };
                store.Initialize();
                return store;
            });
            services.AddMediatR(Assembly.Load(Assemblies.ShopRitePlatform));
            Assembly core = Assembly.Load(Assemblies.ShopRiteCore);
            services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var config = ConfigurationOptions.Parse(_configuration.GetConnectionString(_dbConfig.Database.RedisDatabaseName), true);
                return ConnectionMultiplexer.Connect(config);
            });
            

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
           
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
