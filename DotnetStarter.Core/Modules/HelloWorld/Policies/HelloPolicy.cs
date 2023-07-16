using Cloudey.Reflex.Authorization;
using Cloudey.Reflex.Authorization.HotChocolate;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Extensions;
using DotnetStarter.Core.Modules.HelloWorld.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DotnetStarter.Core.Modules.HelloWorld.Policies;

public class HelloPolicy : IPolicy
{
	public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.RequireRelatedAssertion<Hello>(
			(entity, context, _) => entity?.UserId == context.User.GetId() || context.User.IsInRole(Role.Admin)
		)
		.Build();
}