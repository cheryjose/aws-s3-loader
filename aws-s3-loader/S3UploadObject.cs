using System;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken.Model;
using Amazon;
using System.Threading.Tasks;
using Amazon.Extensions.NETCore.Setup;

namespace aws_s3_loader
{
    class S3UploadObject
    {
        private const string _roleArn = "arn:aws:iam::484519562410:role/DataProviderRole";
        private const string _roleSessionName = "test-session";
        private static readonly AWSOptions _awsOptions = new AWSOptions()
        {
            Profile = "default",
            Region = RegionEndpoint.APSoutheast2
        };


        public static void Main()
        {
            var bucketProperties = new BucketProperties
            {
                BucketName = "test.eventz.com",
                BucketObjectKey = "test-file",
                BucketFilePath = @"C:\Code\sports-form\test-file-chery.txt",
                BucketRegion = RegionEndpoint.APSoutheast2
            };

            Utils.ReadInputAndAction(() => SuccessfulScenarioUploadingToS3(bucketProperties), 
                () => FailureScenarioUploadingToS3(bucketProperties));
        }


        private static void SuccessfulScenarioUploadingToS3(BucketProperties bucketProperties)
        {
            // Successfully uploads file, after assuming given role

            Action<AssumeRoleResponse> putObjectToBucket = (assumeRoleResponse) =>
            {
                PutObjectToS3Async(assumeRoleResponse, bucketProperties).Wait();
            };

            RoleManagement.AssumeGivenRole(_awsOptions, _roleArn, _roleSessionName, putObjectToBucket).Wait();
        }

        private static void FailureScenarioUploadingToS3(BucketProperties bucketProperties)
        {
            // Fails with following error
            // Error encountered ***. Message:'The provided token is malformed or otherwise invalid.' when writing an object


            var bucketCredentials = new BucketCredentials
            {
                AccessKeyId = "AKIAJ42OR63QAZ7REWNQ", // replace with the correct access key
                SecretAccessKey = "KjdlCjUf2Vag7sPtRGpqZjzvZfKbh5CRKbDBRxVy", // replace with correct secret key
                SessionToken = _roleSessionName
            };

            PutObjectToS3Async(bucketCredentials, bucketProperties).Wait();
        }

        
        static async Task PutObjectToS3Async(AssumeRoleResponse assumeRoleResponse, BucketProperties bucketProperties)
        {
            try
            {
                var client = new AmazonS3Client(assumeRoleResponse.Credentials, bucketProperties.BucketRegion);
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketProperties.BucketName,
                    Key = bucketProperties.BucketObjectKey,
                    FilePath = bucketProperties.BucketFilePath,
                };

                await client.PutObjectAsync(putRequest);
                Console.WriteLine($"Successfully put {bucketProperties.BucketObjectKey} to {bucketProperties.BucketName}"); 
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);       
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }


        static async Task PutObjectToS3Async(BucketCredentials credentials, BucketProperties bucketProperties)
        {
            try
            {
                var client = new AmazonS3Client(credentials.AccessKeyId, credentials.SecretAccessKey, credentials.SessionToken, bucketProperties.BucketRegion);
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketProperties.BucketName,
                    Key = bucketProperties.BucketObjectKey,
                    FilePath = bucketProperties.BucketFilePath,
                };

                await client.PutObjectAsync(putRequest);
                Console.WriteLine($"Successfully put {bucketProperties.BucketObjectKey} to {bucketProperties.BucketName}");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }


        private class BucketProperties
        {
            public RegionEndpoint BucketRegion { get; set; }
            public string BucketName { get; set; }
            public string BucketObjectKey { get; set; }
            public string BucketFilePath { get; set; }

        }

        private class BucketCredentials
        {
            public string AccessKeyId { get; set; }
            public string SecretAccessKey { get; set; }
            public string SessionToken { get; set; }

        }
    }
}
