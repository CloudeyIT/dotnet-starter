using Cloudey.FluentValidation.Rules;
using Cloudey.Reflex.GraphQL.Exceptions;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Rules;
using FairyBread;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace DotnetStarter.Core.Framework.Identity.Mutations;

[MutationType]
public class RegisterUserMutation
{
	[UseMutationConvention(PayloadFieldName = "userId")]
	[Error(typeof(UserRegistrationFailedException))]
	public async Task<Ulid> RegisterUser (RegisterUserInput input, [Service] UserManager<User> userManager)
	{
		var user = new User
		{
			Email = input.Email,
			UserName = input.Email,
			FirstName = input.FirstName,
			LastName = input.LastName,
		};

		var result = await userManager.CreateAsync(user, input.Password);

		if (!result.Succeeded)
		{
			throw new UserRegistrationFailedException(
				$"Failed to register user: ({result.Errors.FirstOrDefault()?.Code}) {result.Errors.FirstOrDefault()?.Description}",
				"REGISTER_USER_FAILED"
			);
		}

		await userManager.AddToRoleAsync(user, Role.User);

		return user.Id;
	}

	public class UserRegistrationFailedException : BaseException
	{
		public UserRegistrationFailedException (string message, string code) : base(message, code) { }
	}

	/// <summary>
	///     Details of the new user
	/// </summary>
	/// <param name="Email">E-mail address, must be shorter than 60 characters and unique</param>
	/// <param name="FirstName">First name of the user, must be between 1 and 60 characters</param>
	/// <param name="LastName">Last name of the user, must be between 1 and 60 characters</param>
	/// <param name="Password">
	///     Password must be at least 10 characters long, and contain 1 lowercase, 1 uppercase, 1 special
	///     character, and 1 digit
	/// </param>
	public record RegisterUserInput(
		string Email,
		string FirstName,
		string LastName,
		string Password
	);

	public class RegisterUserValidator : AbstractValidator<RegisterUserInput>, IRequiresOwnScopeValidator
	{
		public RegisterUserValidator (MainDb db)
		{
			RuleFor(x => x.FirstName)
				.NotEmpty()
				.MinimumLength(3)
				.MaximumLength(60);

			RuleFor(x => x.LastName)
				.NotEmpty()
				.MinimumLength(3)
				.MaximumLength(60);

			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress()
				.MaximumLength(60)
				.Unique(db, (User user) => user.NormalizedEmail!, v => v.ToUpper().Trim())
				.WithMessage("User with this email already exists");

			RuleFor(x => x.Password)
				.StrongPassword();
		}
	}
}