using DotnetStarter.Core.Framework.Database;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Entities;

// TODO: Fix user authorisation
// [Guard(Policy = nameof(UserPolicy))]
public class User : IdentityUser<Guid>, IEntity
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
	public override Guid Id { get; set; }

	public DateTime Created { get; set; }
	public DateTime Updated { get; set; }
	public Guid Revision { get; set; }
}