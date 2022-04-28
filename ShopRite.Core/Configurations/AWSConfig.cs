namespace ShopRite.Core.Configurations
{
    public class AWS
    {
        public string Profile { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
    }

    public class AWSConfig
    {
        public AWS AWS { get; set; }
    }
}
