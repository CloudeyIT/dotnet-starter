using System.Security.Claims;
using Cloudey.Reflex.Authorization.HotChocolate;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Extensions;
using DotnetStarter.Core.Modules.HelloWorld.Entities;
using DotnetStarter.Core.Modules.HelloWorld.Policies;

namespace DotnetStarter.Core.Modules.HelloWorld.Queries;

[QueryType]
public class HelloQuery
{
	[Guard<HelloPolicy>]
	[UseProjection]
	public IQueryable<Hello> Hellos (MainDb db, ClaimsPrincipal user)
	{
		return db.Set<Hello>().Where(h => h.UserId == user.GetId());
	}
}