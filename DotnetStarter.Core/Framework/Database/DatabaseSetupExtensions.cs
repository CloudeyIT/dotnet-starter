using Npgsql;

namespace DotnetStarter.Core.Framework.Database;

public static class DatabaseSetupExtensions
{
    public static IServiceCollection AddDatabase (this IServiceCollection services, IConfiguration configuration)
    {
        var migrationsAssembly = configuration.GetValue<string>("MigrationsAssembly", "DotnetStarter.Migrations");

        var mainDbConfig = configuration.GetSection("Database").GetSection(nameof(MainDb));

        services.AddPooledDbContextFactory<MainDb>(
            builder => builder
                .UseNpgsql(
                    new NpgsqlConnectionStringBuilder
                    {
                        Host = mainDbConfig.GetValue<string>("Host"),
                        Port = mainDbConfig.GetValue<int>("Port"),
                        Username = mainDbConfig.GetValue<string>("User"),
                        Password = mainDbConfig.GetValue<string>("Password"),
                        Database = mainDbConfig.GetValue<string>("Database"),
                    }.ConnectionString,
                    npgsql =>
                    {
                        npgsql.MigrationsAssembly(migrationsAssembly);
                        npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    }
                )
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
        );

        return services;
    }
}