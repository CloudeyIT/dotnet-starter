using System.Security.Claims;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Attributes;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Extensions;

namespace DotnetStarter.Core.Framework.Identity.Queries;

[QueryType]
public class MeQuery
{
	[Guard]
	[UseFirstOrDefault]
	[UseProjection]
	public IQueryable<User> Me (ClaimsPrincipal claimsPrincipal, MainDb db)
	{
		return db.Users.Where(u => u.Id == claimsPrincipal.GetId());
	}
}