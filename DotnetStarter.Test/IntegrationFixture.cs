using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bogus;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Identity.Services;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using PasswordGenerator;

namespace DotnetStarter.Test;

public abstract class IntegrationFixture
{
	protected readonly FakeWebApplicationFactory Factory = new();

	protected HttpClient HttpClient => Factory.CreateClient(
		new WebApplicationFactoryClientOptions
		{
			BaseAddress = new Uri("https://localhost"),
		}
	);

	protected ILifetimeScope Scope => Factory.Services.GetAutofacRoot().BeginLifetimeScope();
	protected MainDb Database => Scope.Resolve<MainDb>();
	protected LinkGenerator LinkGenerator => Scope.Resolve<LinkGenerator>();
	protected static Faker Faker => new();

	protected static string RandomPassword => new Password(true, true, true, true, 16).Next();

	protected GraphQLHttpClient GraphQlClient => new(
		new GraphQLHttpClientOptions
		{
			EndPoint = new Uri("https://localhost/graphql"),
		},
		new NewtonsoftJsonSerializer(),
		HttpClient
	);

	protected async Task<string> GetTokenForUser (string email)
	{
		var user = await Scope.Resolve<UserManager<User>>().FindByEmailAsync(email);
		if (user is null) throw new Exception("User not found");
		return GetTokenForUser(user);
	}

	protected string GetTokenForUser (User user)
	{
		return Scope.Resolve<TokenService>().GenerateToken(user);
	}

	protected async Task<User> CreateUser (
		string? email = null,
		string? password = null,
		string? firstName = null,
		string? lastName = null,
		string[]? roles = null
	)
	{
		var username = email ?? Faker.Internet.Email();
		var user = new User
		{
			Email = username,
			UserName = username,
			FirstName = firstName ?? Faker.Name.FirstName(),
			LastName = lastName ?? Faker.Name.LastName(),
		};

		var userManager = Scope.Resolve<UserManager<User>>();
		var result = await userManager.CreateAsync(user, password ?? RandomPassword);
		if (!result.Succeeded) throw new Exception("Failed to create user");

		await userManager.AddToRolesAsync(user, roles ?? new[] { Role.User });

		return user;
	}
}