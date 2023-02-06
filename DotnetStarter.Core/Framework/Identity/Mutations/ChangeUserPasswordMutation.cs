using System.Security.Claims;
using DotnetStarter.Core.Framework.Identity.Attributes;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Exceptions;
using DotnetStarter.Core.Framework.Identity.Extensions;
using DotnetStarter.Core.Framework.Identity.Rules;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Mutations;

[MutationType]
public class ChangeUserPasswordMutation
{
	[Guard]
	[Error(typeof(InvalidCredentialsException))]
	public async Task<IdentityResult> ChangeUserPassword (
		ChangeUserPasswordInput input,
		[Service] UserManager<User> userManager,
		ClaimsPrincipal claimsPrincipal
	)
	{
		var user = await userManager.FindByIdAsync(claimsPrincipal.GetId().ToString()!);

		if (user is null)
		{
			throw new InvalidCredentialsException("User not found");
		}
		
		if (!await userManager.CheckPasswordAsync(user, input.CurrentPassword))
		{
			throw new InvalidCredentialsException();
		}

		return await userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
	}

	public record ChangeUserPasswordInput(string CurrentPassword, string NewPassword);

	public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordInput>
	{
		public ChangeUserPasswordValidator ()
		{
			RuleFor(_ => _.NewPassword)
				.StrongPassword();
		}
	}
}