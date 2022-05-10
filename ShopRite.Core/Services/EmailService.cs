using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using ShopRite.Core.Configurations;
using ShopRite.Core.DTOs;
using ShopRite.Core.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace ShopRite.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly GlobalConfiguration _config;
        public EmailService(IFluentEmail fluentEmail, IConfiguration configuration)
        {
            _fluentEmail = fluentEmail;
            _config = configuration.Get<GlobalConfiguration>();
        }
        public async Task SendEmailOutOfStock(OrderDTO order)
        {
            _fluentEmail.To(_config.Emails.RetailMail)
                        .UsingTemplateFromFile(GetPathOfView("OutOfStock"), order);
            await _fluentEmail.SendAsync();
        }

        public async Task SendEmailSuccessfulOrder(OrderDTO order, string buyerEmail)
        {
            _fluentEmail.To(buyerEmail)
                        .UsingTemplateFromFile(GetPathOfView("SuccessfulOrder"), order);
            await _fluentEmail.SendAsync();
        }

        private string GetPathOfView(string viewName)
        {
            var path = Path.GetFullPath(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "..", "..", "ShopRite.Platform", "Views"));
            return @$"{path}\{viewName}.cshtml";
        }
    }
}
