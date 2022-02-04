using System;
using Amazon.S3;
using LightWiki.Infrastructure.Configuration.Aws;
using Microsoft.Extensions.DependencyInjection;

namespace LightWiki.Infrastructure.Aws.S3;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAwsS3(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddAWSService<IAmazonS3>(lifetime);
        var descriptor = new ServiceDescriptor(
            typeof(IAwsS3Helper),
            provider => new AwsS3Helper(
                provider.GetService<IAmazonS3>(), provider.GetService<S3Configuration>()),
            lifetime);
        services.Add(descriptor);
        return services;
    }
}