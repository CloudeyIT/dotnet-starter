using Cloudey.Reflex.Authorization.HotChocolate;
using Cloudey.Reflex.Database;
using DotnetStarter.Core.Framework.Identity.Policies;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Entities;

[Guard<UserPolicy>]
public class User : IdentityUser<Ulid>, IEntity
{
	[IsProjected]
	public string? FirstName { get; set; }

	[IsProjected]
	public string? LastName { get; set; }

	public string FullName => !string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName)
		? string.Join(" ", FirstName, LastName)
		: string.Empty;

	[GraphQLIgnore]
	public override string? PasswordHash { get; set; }

	[GraphQLIgnore]
	public override string? SecurityStamp { get; set; }

	[IsProjected]
	public override Ulid Id { get; set; }

	public DateTime Created { get; set; }
	public DateTime? Updated { get; set; }
	public Ulid Revision { get; set; }
}