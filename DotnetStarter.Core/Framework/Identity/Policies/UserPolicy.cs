using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Extensions;
using DotnetStarter.Core.Framework.Identity.Types;
using Microsoft.AspNetCore.Authorization;

namespace DotnetStarter.Core.Framework.Identity.Policies;

public class UserPolicy : IPolicy
{
	public AuthorizationPolicy? Policy { get; } = new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.RequireRelatedAssertion<User>(
			(user, context, _) => user.Id == context.User.GetId() || context.User.IsInRole(Role.Admin)
		)
		.Build();
}