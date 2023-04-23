using System.Reflection;
using Cloudey.Reflex.GraphQL;
using DotnetStarter.Core.Framework.Database;
using HotChocolate.Execution.Configuration;

namespace DotnetStarter.Core.Framework.GraphQl;

public static class GraphQlSetupExtensions
{
	public static IRequestExecutorBuilder AddGraphQl (
		this IServiceCollection services,
		IEnumerable<Assembly>? assemblies = default
	)
	{
		var builder = services.AddGraphQLServer();

		builder.AddQueryType()
			.AddMutationType();
		// Uncomment to enable subscriptions
		// .AddInMemorySubscriptions()
		// .AddSubscriptionType();

		builder
			.AddReflexGraphQL(assemblies?.ToArray())
			.AddAuthorization()
			.RegisterDbContext<MainDb>(DbContextKind.Pooled);

		return builder;
	}
}