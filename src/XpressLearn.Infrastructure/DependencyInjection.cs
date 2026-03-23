using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using XpressLearn.Application.Interfaces;
using XpressLearn.Infrastructure.Repositories;

namespace XpressLearn.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IAttemptRepository, AttemptRepository>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
