using Microsoft.AspNetCore.Authorization;

namespace DotnetStarter.Core.Framework.Identity.Types;

public interface IPolicy
{
    public AuthorizationPolicy? Policy { get; }
}