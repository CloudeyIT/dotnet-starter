using System;
using System.Threading.Tasks;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using NUnit.Framework;
using static DotnetStarter.Core.Framework.Identity.Mutations.RegisterUserMutation;

namespace DotnetStarter.Test.Framework.Identity;

[TestFixture]
public class RegisterUserTests : IntegrationFixture
{
    [Test]
    public async Task Can_Register_User_As_Unauthenticated_User ()
    {
        var request = new GraphQLRequest
        {
            Query = @"
                mutation register($input: RegisterUserInput!) {
                    registerUser(input: $input) {
                        userId
                    }
                }
            ",
            Variables = new
            {
                Input = new RegisterUserInput("test@example.com", "Test", "User", RandomPassword),
            },
        };

        var client = GraphQlClient;

        var response = await client.SendMutationAsync(request, () => new { RegisterUser = new { UserId = Guid.Empty } });

        response.Data.RegisterUser.UserId.Should().NotBeEmpty();
        
        var user = await Database.Users.FindAsync(response.Data.RegisterUser.UserId);
        user.Should().NotBeNull();
        user!.Email.Should().Be("test@example.com");
        user.FirstName.Should().Be("Test");
        user.LastName.Should().Be("User");
    }
}