using Cloudey.Reflex.Authorization;
using Cloudey.Reflex.Authorization.HotChocolate;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DotnetStarter.Core.Framework.Identity.Policies;

public class UserPolicy : IPolicy
{
	public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.RequireRelatedAssertion<User>(
			(user, context, _) => user?.Id == context.User.GetId() || context.User.IsInRole(Role.Admin)
		)
		.Build();
}