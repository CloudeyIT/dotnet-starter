using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Cloudey.Reflex.GraphQL.Types;
using DotnetStarter.Core.Framework.Identity.Exceptions;
using DotnetStarter.Core.Framework.Identity.Extensions;
using DotnetStarter.Core.Framework.Identity.ResultTypes;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace DotnetStarter.Test.Framework.Identity;

[TestFixture]
public class LoginTests : IntegrationFixture
{
	[Test]
	public async Task Can_Login_With_Correct_Password ()
	{
		var password = RandomPassword;
		var user = await CreateUser(password: password);

		var request = new GraphQLRequest
		{
			Query = @"
                mutation loginWithPassword($input: LoginWithPasswordInput!) {
                    loginWithPassword(input: $input) {
                        loginResult {
                            token
                        }
                    }
                }",
			Variables = new
			{
				input = new
				{
					email = user.Email,
					password,
				},
			},
		};

		var response = await GraphQlClient.SendMutationAsync(
			request,
			() => new { LoginWithPassword = new { LoginResult = new LoginResult() } }
		);

		response.Errors.Should().BeNullOrEmpty();
		var result = response.Data.LoginWithPassword;
		result.Should().NotBeNull();

		var token = result.LoginResult.Token;
		token.Should().NotBeNull();
		token.Should().NotBeEmpty();

		var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(
			token,
			Scope.Resolve<TokenValidationParameters>(),
			out var validatedToken
		);

		claimsPrincipal.Should().NotBeNull();
		claimsPrincipal.GetId().Should().Be(user.Id);
		validatedToken.Should().NotBeNull();
	}

	[Test]
	public async Task Cannot_Login_With_Incorrect_Password ()
	{
		var user = await CreateUser();

		var request = new GraphQLRequest
		{
			Query = @"
                mutation loginWithPassword($input: LoginWithPasswordInput!) {
                    loginWithPassword(input: $input) {
                        loginResult {
                            token
                        }
                        errors {
                            ... on IError {
                                message
                                code
                            }
                        }
                    }
                }",
			Variables = new
			{
				input = new
				{
					email = user.Email,
					password = "wrongPassword",
				},
			},
		};

		var response = await GraphQlClient.SendMutationAsync(
			request,
			() => new
			{
				LoginWithPassword = new
				{
					LoginResult = new LoginResult(),
					Errors = new List<Error>(),
				},
			}
		);

		response.Should().NotBeNull();
		response.Data.LoginWithPassword.Errors.Should().ContainItemsAssignableTo<IError>();
		response.Data.LoginWithPassword.Errors.Single().Code.Should().Be(new InvalidCredentialsException().Code);
	}

	[Test]
	public async Task Cannot_Login_With_Incorrect_Email ()
	{
		var password = RandomPassword;

		var request = new GraphQLRequest
		{
			Query = @"
                mutation loginWithPassword($input: LoginWithPasswordInput!) {
                    loginWithPassword(input: $input) {
                        loginResult {
                            token
                        }
                        errors {
                            ... on IError {
                                message
                                code
                            }
                        }
                    }
                }",
			Variables = new
			{
				input = new
				{
					email = Faker.Internet.Email(),
					password,
				},
			},
		};

		var response = await GraphQlClient.SendMutationAsync(
			request,
			() => new
			{
				LoginWithPassword = new
				{
					LoginResult = new LoginResult(),
					Errors = new List<Error>(),
				},
			}
		);

		response.Should().NotBeNull();
		response.Data.LoginWithPassword.Errors.Should().ContainItemsAssignableTo<IError>();
		response.Data.LoginWithPassword.Errors.Single().Code.Should().Be(new InvalidCredentialsException().Code);
	}
}