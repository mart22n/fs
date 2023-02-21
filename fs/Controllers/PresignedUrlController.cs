using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using Minio;
using Minio.Exceptions;
using Minio.DataModel;
using System.Configuration;

namespace fs.Controllers
{
    public class PresignedUrlController : ApiController
    {
        [Microsoft.AspNetCore.Mvc.Route("/PresignedUrl/name={fileName}")]
        public async Task<IHttpActionResult> Index(string fileName)
        {
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;

            this.Configuration = new HttpConfiguration();
            this.Request = new HttpRequestMessage();
            string? endpoint = appSettings["MinioApiAddr"];
            string bucketName = (appSettings["MinioBucketName"] != null ?
                appSettings["MinioBucketName"] : string.Empty);
            var objectName = fileName;
            string uploadUrl = "";

            try
            {
                MinioClient minio = new MinioClient()
                                 .WithEndpoint(endpoint)
                                 .WithCredentials(appSettings["MinioUser"],
                                    appSettings["MinioPwd"])
                                 .WithSSL()
                                 .Build();

                if (await bucketExists(minio, bucketName))
                {
                    var tuple = await setPolicy(minio, bucketName, objectName);
                    uploadUrl = await getUploadUrl(minio, bucketName, objectName);
                    return Ok(uploadUrl);
                }
                return (IHttpActionResult)Results.StatusCode(500);
            }
            catch(Exception)
            {
                return (IHttpActionResult)Results.StatusCode(500);
            }
        }

        async static Task<(Uri, Dictionary<string, string>)> setPolicy(IMinioClient client,
             string bucketName,
             string objectName)
        {
            // default value for expiration is 2 minutes
            var expiration = DateTime.Now.AddMinutes(2);

            var form = new PostPolicy();
            form.SetKey(objectName);
            form.SetBucket(bucketName);
            form.SetExpires(expiration);
            form.SetContentRange(1, 500 * 1024 * 1024);

            var args = new PresignedPostPolicyArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithPolicy(form);

            return await client.PresignedPostPolicyAsync(form);
        }

        async static Task<string> getUploadUrl(MinioClient minio, string bucketName, string objectName)
        {
            //var location = "us-east-1";
            var contentType = "text/html";
            string res = "";
            try
            {
                // Upload a file to bucket.
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(objectName)
                    .WithContentType(contentType);

                var presignedPutArgs = new PresignedPutObjectArgs()
                    .WithBucket(bucketName)
                    .WithExpiry(1000)
                    .WithObject(objectName);

                res = await minio.PresignedPutObjectAsync(presignedPutArgs);
            }
            catch (MinioException)
            {
            }
            return res;
        }

        public static async Task<bool> bucketExists(MinioClient minio, string bucketName)
        {
            bool ret = false;
            try
            {
                var args = new BucketExistsArgs()
                    .WithBucket(bucketName);
                var found = await minio.BucketExistsAsync(args);
                ret = found;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }
    }
}