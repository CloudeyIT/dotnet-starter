using System.Security.Claims;
using DotnetStarter.Core.Framework.Identity.Attributes;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Extensions;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Queries;

[QueryType]
public class MyRolesQuery
{
	[Guard]
	public async Task<IList<string>> MyRoles (ClaimsPrincipal claimsPrincipal, [Service] UserManager<User> userManager)
	{
		var user = await userManager.FindByIdAsync(claimsPrincipal.GetId().ToString()!);
		
		if (user is null)
		{
			return new List<string>();
		}
		
		return await userManager.GetRolesAsync(user);
	}
}