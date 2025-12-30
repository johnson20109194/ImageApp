using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ImageApp.Application.Interfaces;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Application.Interfaces.Services;
using ImageApp.Infrastructure.Caching;
using ImageApp.Infrastructure.Identity;
using ImageApp.Infrastructure.Persistence.Repositories;
using ImageApp.Infrastructure.Storage;

namespace ImageApp.Infrastructure;

public static class Register
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<IMediaStorage, AzureBlobMediaStorage>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IRatingRepository, RatingRepository>();

        services.Configure<StorageOptions>(configuration.GetSection(nameof(StorageOptions)));

        var redisConnectionString = configuration.GetConnectionString(RedisCacheService.CacheKeyPrefix);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });

        services.AddSingleton<IRedisCacheService, RedisCacheService>();
        return services;
    }
}