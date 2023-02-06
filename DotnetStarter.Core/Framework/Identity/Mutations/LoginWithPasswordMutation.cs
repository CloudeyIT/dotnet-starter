using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Exceptions;
using DotnetStarter.Core.Framework.Identity.ResultTypes;
using DotnetStarter.Core.Framework.Identity.Services;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Mutations;

[MutationType]
public class LoginWithPasswordMutation
{
	private readonly TokenService _tokenService;

	public LoginWithPasswordMutation (TokenService tokenService)
	{
		_tokenService = tokenService;
	}

	[Error(typeof(InvalidCredentialsException))]
	public async Task<LoginResult> LoginWithPassword (
		LoginWithPasswordInput input,
		[Service] UserManager<User> userManager
	)
	{
		var user = await userManager.FindByEmailAsync(input.Email);
		if (user is null)
		{
			throw new InvalidCredentialsException();
		}

		var result = await userManager.CheckPasswordAsync(user, input.Password);
		if (!result)
		{
			throw new InvalidCredentialsException();
		}

		var token = _tokenService.GenerateToken(user);
		return new LoginResult { Token = token };
	}

	public record LoginWithPasswordInput(string Email, string Password);
}