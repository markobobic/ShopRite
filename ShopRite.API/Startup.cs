using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents;
using ShopRite.Core.Middleware;
using ShopRite.Core.Pipelines;
using System.Reflection;

namespace ShopRite.API
{
    public class Startup
    {
        private const string DatabaseName = "ShopRite";
        private const string RavenURL = "http://127.0.0.1:8081/";
        private const string ShppRitePlatform = "ShopRite.Platform";
        private const string ShopRiteCore = "ShopRite.Core";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IDocumentStore>(provider =>
            {
                var store = new DocumentStore()
                {
                    Urls = new string[] { RavenURL },
                    Database = DatabaseName
                };
                store.Initialize();
                return store;
            });

            services.AddMediatR(Assembly.Load(ShppRitePlatform));
            Assembly core = Assembly.Load(ShopRiteCore);

            AssemblyScanner.FindValidatorsInAssembly(core)
                .ForEach(x => services.AddTransient(typeof(IValidator), x.ValidatorType));
            
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorPipelineBehavior<,>));
            services.Scan(
                x =>
                {
                    x.FromAssemblies(Assembly.Load(ShppRitePlatform))
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
