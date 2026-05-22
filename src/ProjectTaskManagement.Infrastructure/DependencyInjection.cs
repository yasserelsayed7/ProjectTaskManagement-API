using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Domain.Interfaces;
using ProjectTaskManagement.Infrastructure.Caching;
using ProjectTaskManagement.Infrastructure.Identity;
using ProjectTaskManagement.Infrastructure.Persistence;
using ProjectTaskManagement.Infrastructure.Persistence.Repositories;

namespace ProjectTaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
                }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
        services.AddScoped<ITaskReadRepository, TaskReadRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddHttpContextAccessor();

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "ProjectTaskManagement:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}
