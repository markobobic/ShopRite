using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using ShopRite.Core.Configurations;
using ShopRite.Core.DTOs;
using ShopRite.Core.Interfaces;
using System.Collections.Generic;
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
        public async Task SendEmailOutOfStock(List<OrderDTO> orders)
        {
            var path = GetPathOfViews("ShopRite.Platform");
            _fluentEmail.To(_config.Emails.RetailMail)
                        .UsingTemplateFromFile(@$"{path}\OutOfStock.cshtml", orders);
            await _fluentEmail.SendAsync();
        }

        private string GetPathOfViews(string projectName) => 
            Path.GetFullPath(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "..", "..", "ShopRite.Platform", "Views"));
    }
}
