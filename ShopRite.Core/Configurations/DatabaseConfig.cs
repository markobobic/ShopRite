
namespace ShopRite.Core.Configurations
{
    public class Database
    {
        public string[] Urls { get; set; }
        public string DatabaseName { get; set; }
    }

    public class DatabaseConfig
    {
        public Database Database { get; set; }
    }
}
