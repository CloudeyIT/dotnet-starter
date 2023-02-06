using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Attributes;
using DotnetStarter.Core.Framework.Identity.Policies;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Entities;

[Guard(Policy = nameof(UserPolicy))]
public class User : IdentityUser<Guid>, IEntity
{
    [IsProjected]
    public override Guid Id { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public Guid Revision { get; set; }

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
}