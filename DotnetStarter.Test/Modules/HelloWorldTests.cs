using System.Threading.Tasks;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using NUnit.Framework;
using static DotnetStarter.Core.Modules.HelloWorld.Queries.HelloWorldQuery;

namespace DotnetStarter.Test.Modules;

[TestFixture]
public class HelloWorldTests : IntegrationFixture
{
    [Test]
    public async Task Can_Get_HelloWorld_As_Unauthenticated_User ()
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

        var response = await GraphQlClient.SendQueryAsync(request, () => new { helloWorld = new HelloWorldPayload() });

        response.Errors.Should().BeNullOrEmpty();
        response.Data.helloWorld.Message.Should().Be("Hello World!");
    }
}