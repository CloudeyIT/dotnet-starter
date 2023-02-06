using DotnetStarter.Core.Framework.Identity.Services;
using Microsoft.IdentityModel.Tokens;

namespace DotnetStarter.Core.Framework.Identity.Configuration;

public class IdentityConfiguration
{
    public const string Key = IdentityModule.Name;
    public string PrivateKey { get; set; } = null!;
    public RsaSecurityKey SigningKey => KeyService.GetRsaKey(PrivateKey);
    public string? Audience { get; set; }
    public string? Issuer { get; set; }
    public int ExpirationMinutes { get; set; }
}