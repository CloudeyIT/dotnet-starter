using System.Net;
using System.Threading.Tasks;
using DotnetStarter.Test.Testing;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using NUnit.Framework;
using static DotnetStarter.Core.Modules.HelloWorld.Queries.HelloWorldQuery;

namespace DotnetStarter.Test.Modules;

[TestFixture]
public class HelloWorldTests : IntegrationFixture
{
	[Test]
	public async Task Cannot_Get_HelloWorld_As_Unauthenticated_User ()
	{
		var request = new GraphQLRequest
		{
			Query = @"
                query helloWorld($name: String!) {
                    helloWorld(input: {name: $name}) {
                        message
                    }
                }",
			Variables = new
			{
				name = "World",
			},
		};

		try
		{
			var response = await GraphQlClient.SendQueryAsync(
				request,
				() => new { helloWorld = new HelloWorldPayload() }
			);
			response.Errors.Should().NotBeEmpty();
		}
		catch (GraphQLHttpRequestException exception)
		{
			exception.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}
	}
	
	[Test]
	public async Task Can_Get_HelloWorld_As_Authenticated_User ()
	{
		var request = new GraphQLRequest
		{
			Query = @"
                query helloWorld($name: String!) {
                    helloWorld(input: {name: $name}) {
                        message
                    }
                }",
			Variables = new
			{
				name = "World",
			},
		};

		var user = await CreateUser();
		var token = GetTokenForUser(user);

		var response = await GraphQlClient
			.WithToken(token)
			.SendQueryAsync(request, () => new { helloWorld = new HelloWorldPayload() });

		response.Errors.Should().BeNullOrEmpty();
		response.Data.helloWorld.Message.Should().Be("Hello World!");
	}
}