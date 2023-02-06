using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Configuration;
using DotnetStarter.Core.Framework.Identity.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DotnetStarter.Core.Framework.Identity;

public static class IdentitySetupExtensions
{
	public static IServiceCollection AddIdentityModule (
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		// TODO: Adapt identity configuration for your own project
		services.AddIdentity<User, Role>(
				options =>
				{
					options.SignIn.RequireConfirmedAccount = false;
					options.User.RequireUniqueEmail = true;
					options.Password.RequiredLength = 8;
					options.Password.RequiredUniqueChars = 3;
				}
			)
			.AddEntityFrameworkStores<MainDb>()
			.AddDefaultTokenProviders();

		var config = configuration.GetSection(IdentityConfiguration.Key).Get<IdentityConfiguration>();
		if (config is null) throw new Exception("Identity configuration is missing");
		
		services.AddSingleton(config);

		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidAudience = config.Audience,
			ValidIssuer = config.Issuer,
			ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha512 },
			IssuerSigningKey = config.SigningKey,
			ClockSkew = TimeSpan.Zero,
		};
		services.AddSingleton(tokenValidationParameters);

		services.AddAuthentication(
				options => { options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; }
			)
			.AddJwtBearer(
				options => { options.TokenValidationParameters = tokenValidationParameters; }
			);


		services.AddAuthorization(
			options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser()
					.Build();
			}
		);

		return services;
	}

	public static IApplicationBuilder UseIdentityModule (this IApplicationBuilder app)
	{
		app.UseAuthentication();
		app.UseAuthorization();

		return app;
	}
}