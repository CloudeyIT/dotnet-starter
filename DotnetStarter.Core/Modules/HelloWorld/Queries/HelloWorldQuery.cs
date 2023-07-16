using Cloudey.Reflex.Authorization;
using Cloudey.Reflex.Authorization.HotChocolate;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace DotnetStarter.Core.Modules.HelloWorld.Queries;

[QueryType]
public class HelloWorldQuery
{
	
	[Guard<HelloWorldQueryPolicy>(ApplyPolicy.AfterResolver)]
	public HelloWorldPayload HelloWorld (HelloWorldInput input)
	{
		return new HelloWorldPayload
			{ Message = $"Hello {input.Name}!" };
	}

	public record HelloWorldInput(string Name);

	public record HelloWorldPayload
	{
		public string Message { get; init; } = string.Empty;
	}

	public class HelloWorldQueryPolicy : IPolicy
	{
		public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
			.RequireAuthenticatedUser()
			.Build();
	}
}