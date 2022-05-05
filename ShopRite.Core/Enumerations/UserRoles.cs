using Ardalis.SmartEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopRite.Core.Enumerations
{
    public sealed class UserRoles : SmartEnum<UserRoles, string>
    {
        public static readonly UserRoles Admin = new UserRoles(nameof(Admin), nameof(Admin));
        public static readonly UserRoles Buyer = new UserRoles(nameof(Buyer), nameof(Buyer));
        public UserRoles(string name, string value) : base(name, value)
        {
        }
    }
}
