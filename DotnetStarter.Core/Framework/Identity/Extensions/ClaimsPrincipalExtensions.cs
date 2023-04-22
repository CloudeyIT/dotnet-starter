using System.Security.Claims;
using IdentityModel;

namespace DotnetStarter.Core.Framework.Identity.Extensions;

public static class ClaimsPrincipalExtensions
{
	public static Ulid? GetId (this ClaimsPrincipal claimsPrincipal)
	{
		if (Ulid.TryParse(
			    claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier),
			    out var idFromNameIdentifierClaim
		    ))
		{
			return idFromNameIdentifierClaim;
		}

		if (Ulid.TryParse(claimsPrincipal.FindFirstValue(JwtClaimTypes.Subject), out var idFromSubClaim))
		{
			return idFromSubClaim;
		}

		return null;
	}
}