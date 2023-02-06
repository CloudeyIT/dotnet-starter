using System.Threading.Tasks;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Test.Testing;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using NUnit.Framework;
using static System.Array;

namespace DotnetStarter.Test.Framework.Identity;

[TestFixture]
public class UserTests : IntegrationFixture
{
    [Test]
    public async Task Can_Get_Me_As_Authenticated_User ()
    {
        var user = await CreateUser();

        var request = new GraphQLRequest
        {
            Query = @"
                query {
                    me {
                        id
                        email
                        firstName
                        lastName
                        fullName
                    }
                }
            ",
        };

        var response = await GraphQlClient.WithToken(GetTokenForUser(user))
            .SendQueryAsync(request, () => new { me = new User() });

        response.Errors.Should().BeNullOrEmpty();
        response.Data.Should().NotBeNull();
        response.Data.me.Should().NotBeNull();
        response.Data.me.Id.Should().Be(user.Id);
        response.Data.me.Email.Should().Be(user.Email);
        response.Data.me.FirstName.Should().Be(user.FirstName);
        response.Data.me.LastName.Should().Be(user.LastName);
        response.Data.me.FullName.Should().Be(user.FullName);
    }

    [Test]
    public async Task Can_Get_MyRoles_As_Authenticated_User ()
    {
        var user = await CreateUser();

        var request = new GraphQLRequest
        {
            Query = @"
                query {
                    myRoles
                }
            ",
        };

        var response = await GraphQlClient.WithToken(GetTokenForUser(user))
            .SendQueryAsync(request, () => new { myRoles = Empty<string>() });

        response.Errors.Should().BeNullOrEmpty();
        response.Data.Should().NotBeNull();
        response.Data.myRoles.Should().OnlyContain(role => role == Role.User);
    }
}