using System.Security.Claims;
using DotnetStarter.Core.Framework.Identity.Extensions;
using DotnetStarter.Core.Framework.GraphQl.Types;
using DotnetStarter.Core.Framework.Identity.Attributes;
using DotnetStarter.Core.Framework.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Queries;

[ExtendObjectType(typeof(Query))]
public class MyRolesQuery
{
    [Guard]
    public async Task<IList<string>> MyRoles (ClaimsPrincipal claimsPrincipal, [Service] UserManager<User> userManager)
    {
        return await userManager.GetRolesAsync(await userManager.FindByIdAsync(claimsPrincipal.GetId().ToString()));
    }
}