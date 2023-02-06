using HotChocolate.Authorization;

namespace DotnetStarter.Core.Framework.Identity.Attributes;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Method,
    AllowMultiple = true
)]
public class Guard : AuthorizeAttribute { }