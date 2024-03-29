﻿using Amazon.S3;
using Amazon.S3.Model;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ShopRite.Core.Configurations;
using ShopRite.Core.Extensions;
using ShopRite.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace ShopRite.Core.Services
{
    public class AwsService : IAwsService
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly GlobalConfiguration _globalConfig;

        public AwsService(IConfiguration configuration, IAmazonS3 s3Client)
        {
            _configuration = configuration;
            _s3Client = s3Client;
            _globalConfig = _configuration.Get<GlobalConfiguration>();
        }
        public string ReturnPreSignedURLOfUploadedImage(IFormFile file)
        {
            GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest();
            urlRequest.BucketName = _globalConfig.AWS.BucketName;
            urlRequest.Key = file.FileName;
            urlRequest.Expires = DateTime.Now.AddHours(1);
            urlRequest.Protocol = Protocol.HTTP;
            string url = _s3Client.GetPreSignedURL(urlRequest);

            return url;
        }
        public string CreateUrlOfFile(IFormFile file) => 
            file is null ? string.Empty : $@"https://{_globalConfig.AWS.BucketName}.s3.amazonaws.com/{file.FileName}";

        public async Task UploadImageToS3Bucket(IFormFile file)
        {
            if (file is null) return;

            var awsRequest = new PutObjectRequest()
            {
                BucketName = _globalConfig.AWS.BucketName,
                Key = file.FileName,
                InputStream = file.OpenReadStream()
            };
            awsRequest.Metadata.Add("Content-Type", file.ContentType);
            var response = await _s3Client.PutObjectAsync(awsRequest);
            Guard.Against.False(response.HttpStatusCode.ToInt() == 200, "S3 bucket didn't upload file.");
        }
    }
}
