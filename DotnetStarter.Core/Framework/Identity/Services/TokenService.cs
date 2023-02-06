using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotnetStarter.Core.Framework.Identity.Configuration;
using DotnetStarter.Core.Framework.Identity.Entities;
using Microsoft.IdentityModel.Tokens;

namespace DotnetStarter.Core.Framework.Identity.Services;

public class TokenService
{
	private readonly IdentityConfiguration _configuration;

	public TokenService (IdentityConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string GenerateToken (User user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(
				new[]
				{
					new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
					new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
					new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				}
			),
			Audience = _configuration.Audience,
			Issuer = _configuration.Issuer,
			IssuedAt = DateTime.UtcNow,
			Expires = DateTime.UtcNow.AddMinutes(_configuration.ExpirationMinutes),
			SigningCredentials = KeyService.GetSigningCredentials(_configuration.SigningKey),
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}