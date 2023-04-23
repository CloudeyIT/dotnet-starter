using Cloudey.FluentValidation.Rules;
using Cloudey.Reflex.Authorization.HotChocolate;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Mutations;

[MutationType]
public class SetUserRolesMutation
{
	[Guard(Roles = new[] { Role.Admin })]
	[UseMutationConvention(PayloadFieldName = "roles")]
	public async Task<IList<string>> SetUserRoles (
		SetUserRolesInput input,
		[Service] UserManager<User> userManager
	)
	{
		var user = await userManager.FindByIdAsync(input.Id.ToString());

		if (user is null)
		{
			return new List<string>();
		}

		var currentRoles = await userManager.GetRolesAsync(user);

		var rolesToAdd = input.Roles.Except(currentRoles);
		var rolesToRemove = currentRoles.Except(input.Roles).Except(new[] { Role.Admin });

		await userManager.RemoveFromRolesAsync(user, rolesToRemove);
		await userManager.AddToRolesAsync(user, rolesToAdd);

		var userRoles = await userManager.GetRolesAsync(user);

		return userRoles;
	}

	public record SetUserRolesInput(Ulid Id, string[] Roles);

	public class SetUserRolesValidator : AbstractValidator<SetUserRolesInput>
	{
		public SetUserRolesValidator (MainDb db)
		{
			RuleFor(_ => _.Id)
				.Exists(db, (User _) => _.Id);

			RuleFor(_ => _.Roles)
				.ForEach(role => role.Exists(db, (Role _) => _.Name));
		}
	}
}