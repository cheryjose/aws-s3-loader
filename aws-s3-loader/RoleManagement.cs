using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.Extensions.NETCore.Setup;

namespace aws_s3_loader
{
    class RoleManagement
    {
        public static async Task<AssumeRoleResponse> AssumeGivenRole(AWSOptions awsOptions, string roleArn, string roleSessionName)
        {
            var stsClient = awsOptions.CreateServiceClient<IAmazonSecurityTokenService>();
            var assumedRoleResponse = await stsClient.AssumeRoleAsync(new AssumeRoleRequest()
            {
                RoleArn = roleArn,
                RoleSessionName = roleSessionName
            });

            return assumedRoleResponse;
        }

        public static async Task AssumeGivenRole(AWSOptions awsOptions, string roleArn, string roleSessionName, Action<AssumeRoleResponse> execute)
        {
            var stsClient = awsOptions.CreateServiceClient<IAmazonSecurityTokenService>();
            var assumedRoleResponse = await stsClient.AssumeRoleAsync(new AssumeRoleRequest()
            {
                RoleArn = roleArn,
                RoleSessionName = roleSessionName
            });

            execute(assumedRoleResponse);
        }
    }
}
