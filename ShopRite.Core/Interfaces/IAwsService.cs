using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShopRite.Core.Interfaces
{
    public interface IAwsService
    {
        Task UploadImageToS3Bucket(IFormFile file);
        string ReturnPreSignedURLOfUploadedImage(IFormFile file);
        string CreateUrlOfFile(IFormFile file);
    }
}
