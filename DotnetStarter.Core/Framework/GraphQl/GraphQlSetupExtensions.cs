using System.Reflection;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.GraphQl.Middleware;
using DotnetStarter.Core.Framework.GraphQl.Types;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Pagination;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IError = DotnetStarter.Core.Framework.GraphQl.Types.IError;

namespace DotnetStarter.Core.Framework.GraphQl;

public static class GraphQlSetupExtensions
{
    public static IRequestExecutorBuilder AddGraphQl (
        this IServiceCollection services,
        IEnumerable<Assembly>? assemblies = default
    )
    {
        var builder = services.AddGraphQLServer();

        builder.UseQueries();
        builder.UseMutations();

        // Uncomment to enable subscriptions
        // builder.UseSubscriptions();

        builder
            .AddTypeExtensionsFromAssemblies(assemblies ?? AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
            .UseExceptions()
            .AddFairyBread(_ => _.ThrowIfNoValidatorsFound = false)
            .AddAuthorization()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .SetPagingOptions(
                new PagingOptions
                {
                    IncludeTotalCount = true,
                    MaxPageSize = 100,
                    DefaultPageSize = 20,
                }
            )
            .AddErrorFilter<LoggingErrorFilter>()
            .UseAutomaticPersistedQueryPipeline()
            .AddInMemoryQueryStorage()
            .AddDefaultTransactionScopeHandler()
            .RegisterDbContext<MainDb>(DbContextKind.Pooled)
            .AddMutationConventions(
                new MutationConventionOptions
                {
                    ApplyToAllMutations = true,
                    PayloadTypeNamePattern = "{MutationName}Result",
                    PayloadErrorTypeNamePattern = "{MutationName}Error",
                    PayloadErrorsFieldName = "errors",
                    InputArgumentName = "input",
                    InputTypeNamePattern = "{MutationName}Input",
                }
            )
            .AddErrorInterfaceType<Types.IError>()
            .InitializeOnStartup();

        services.RemoveAll<IHttpResponseFormatter>();
        services.AddSingleton<IHttpResponseFormatter>(new CustomHttpResultFormatter());

        return builder;
    }

    public static IRequestExecutorBuilder AddTypeExtensionsFromAssemblies (
        this IRequestExecutorBuilder builder,
        IEnumerable<Assembly> assemblies
    )
    {
        assemblies.ForEach(
            assembly =>
            {
                assembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<ExtendObjectTypeAttribute>() is not null)
                    .ForEach(type => builder.AddTypeExtension(type));
            }
        );


        return builder;
    }

    public static IRequestExecutorBuilder UseQueries (this IRequestExecutorBuilder builder)
    {
        return builder.AddQueryType<Query>();
    }

    public static IRequestExecutorBuilder UseMutations (this IRequestExecutorBuilder builder)
    {
        return builder.AddMutationType<Mutation>();
    }

    public static IRequestExecutorBuilder UseSubscriptions (this IRequestExecutorBuilder builder)
    {
        return builder.AddSubscriptionType<Subscription>().AddInMemorySubscriptions();
    }
}