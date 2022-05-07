﻿using ShopRite.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopRite.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailOutOfStock(List<OrderDTO> orders);
    }
}
