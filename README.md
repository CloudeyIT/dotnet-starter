# DotnetStarter

## Creating a new project

### Installation and creating the project

1. Make sure you have the required dependencies installed: Docker, .NET 8, dotnet CLI, PowerShell Core
2. Clone the repository: `git clone git@github.com:CloudeyIT/dotnet-starter.git`
3. **IMPORTANT!** Do not start developing or making changes inside the pulled repository. We will create a new project using this repository as a template.
4. Install the repository as a project template with the dotnet CLI tool: `dotnet new -i ./dotnet-starter`. If you cloned the repository in a different directory, replace `dotnet-starter` with the directory name.
5. Create a new project using the dotnet CLI tool: `dotnet new dotnetstarter -n YOURPROJECTNAME`. Replace YOURPROJECTNAME with the name of your project. **Do not use spaces!** This name will become the root namespace of your project. Eg. using the name `Cloudey` will result in a solution called `Cloudey` and projects called `Cloudey.Core`, `Cloudey.Api`, etc.
6. Congratulations! Your new project is now (almost) ready to go! Navigate to the project directory and follow the "First-time setup" instructions below! 

### First-time setup

1. Remove the starter git remote and add a new remote for your project's repository.
2. Replace the badge URLs and images in this README with the correct URLs for your new project.
3. Delete the example HelloWorld module (from `YOURPROJECTNAME.Core/Modules/HelloWorld`) and the corresponding test (from `YOURPROJECTNAME.Test/Modules/HelloWorld`).
4. Replace the default RSA private keys for JWT signing (Identity.PrivateKey) in environment-specific configuration files (appsettings.Testing.yml and appsettings.Development.yml). You can generate one [here](https://gchq.github.io/CyberChef/#recipe=Generate_RSA_Key_Pair('2048','PEM')Escape_string('Special%20chars','Single',false,true,false)Find_/_Replace(%7B'option':'Regex','string':'-----BEGIN%20PUBLIC%20KEY-----.%2B-----END%20PUBLIC%20KEY-----%5C%5C%5C%5Cr%5C%5C%5C%5Cn%5C%5C%5C%5Cn'%7D,'',true,false,true,true)).
5. Remove this section about setting up a new project from the README.
6. That's it! Follow "Local setup" instructions below to start developing your project.

## Local setup

1. Make sure you have the required dependencies installed: Docker, .NET 7, dotnet CLI, PowerShell Core
2. Run `dotnet tool restore` to install the CLI tools for the project (EF Core, GraphQL client, ...)
3. Run the databases with `docker-compose up -d postgres postgres-test`
4. Optional: Configure application settings by creating a file `appsettings.Local.yml` in `./DotnetStarter.Api` and overriding settings from `appsettings.yaml` and `appsettings.Development.yaml` as needed.
5. Run the migrations with `./ef database update`
6. Start the application from your IDE, or from the command line with `dotnet run --project DotnetStarter.Api`

Once the application is running, navigate to `https://localhost:7100/graphql` and you should see the
GraphQL playground.

## Local development

### IDE

Using Rider as the IDE is **strongly recommended**, but using VS Code is also possible with the proper extensions.

### About Docker

Unlike PHP projects, it is **not necessary** to run the application inside a Docker container when developing locally.   
In PHP, the need for using Docker stems from the PHP runtime configuration and extensions being machine-wide and therefore not consistent across development environments. 
With .NET, configuration is project-specific and co-located with the solution, and is therefore always consistent across development environments. As such, there is no benefit to be gained from using Docker for development, in fact it makes development slower and more cumbersome due to the need to rebuild the container after every change.  

In short, run the application **natively, outside Docker** when developing locally.

Docker is still useful for running other required services, such as PostgreSQL, Redis, etc.

## GraphQL

### Playground / client

When the application is running, go to `https://localhost:7100/graphql` to see the built-in GraphQL client.

### Creating queries and mutations

For an example of a Query, see `DotnetStarter.Core.Framework.Identity.Queries.RolesQuery`.  
For an example of a Mutation, see `DotnetStarter.Core.Framework.Identity.Mutations.RegisterUserMutation`.
Queries and mutations are defined by applying an `[QueryType]` or `[ExtendObjectType(typeof(Mutation))]` attribute to the containing class.

### Input validation

The standard way to validate the input to a query or mutation is to create an input DTO. This can be a simple record or class containing the input arguments:
```c#
public record CreateTodoInput (string Title, string Description)
```

You can then create a validator (from [FluentValidation](https://fluentvalidation.net/)) to validate the input object:
```c#
public class CreateTodoInputValidator : AbstractValidator<CreateTodoInput>
{
    public CreateTodoInputValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinLength(5);
        RuleFor(x => x.Description).NotEmpty();
    }
}
```

The validator will be picked up and applied automatically.
Some useful custom validation rules are included in the projects, such as `Exists` and `Unique`, which can be used to validate against the database. Example:
```c#
public class CreateUserInputValidator : AbstractValidator<CreateUserInput>, IRequiresOwnScopeValidator
{
    public CreateUserInputValidator(MainDb db)
    {
        RuleFor(x => x.Email).Unique(db, (User user) => user.Email);
    }
}
```
This checks if the given email is unique in the User table. The `Exists` rule syntax is identical, and checks if the value already exists in the database. Note that you can check against any table by simply passing the correct type into the expression. Validators that use a database instance **must** have the `IRequiresOwnScopeValidator` interface to make sure clashes don't occur in case of parallel requests.

## Authentication and authorization

To authenticate a given request, entity, or property, use the `[Guard]` attribute. The Guard attribute can be used to authenticate based on roles or a policy.

### Authorizing a query or mutation

Apply the `[Guard]` attribute to the query or mutation **method**, eg:
```c#
[QueryType]
public class MyQuery {
    ...
    [Guard(new[] { "Admin" })]
    public async string GetHello () {
        return "Hello";
    }
}
```
If no roles are specified, all **authenticated** users are allowed to access the route.

### Authorizing an entity

Apply the `[Guard]` attribute to the entity class, eg:
```c#
[Guard(new[] { "Admin" })]
public class SecretInformation : Entity {
    ...
}
```
This prevents anyone without the `Admin` role from accessing the entity in any query.

### Authorizing a property

You can also restrict access on a field-level. Apply the `[Guard]` attribute to the field, eg:
```c#
public class User : Entity {
    public Guid Id {get; set;}

    [Guard(new[] { "Admin" })]
    public string PasswordHash {get; set;}
}
```
This disallows access to the PasswordHash fields for everyone except Admins.

### Combining the authorization attributes

You can apply authorization attributes on multiple `levels`, and they will all be executed in order. Eg. you can allow access to the Role entity to all authenticated users with a `[Guard]` attribute on the Role class, but only allow access for Admins to a specific field in that entity by adding `[Guard(new[] { "Admin" })]` to that field.

### Using policies

When simple role-based authentication is not enough, you can also use policies to create more complex authorization logic. Read more about policy-base authorization here: [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0).

An example of a policy in the GraphQL context can be found below, with comments and explanations:
```c#
// Policies need to implement the IPolicy interface to be automatically registered in the DI
// The policy will be registered with the same name as the class, 
// so you can refer to it with e.g. nameof(UserPolicy)
public class UserPolicy : IPolicy
{
    public AuthorizationPolicy? Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new CanAccessUserRequirement())
        .Build();
}

// A requirement can have properties, but doesn't have to. These properties can only be set STATICALLY
// in the policy on creation, NOT where the policy is consumed (e.g. the resolver) or at runtime
public record CanAccessUserRequirement : IAuthorizationRequirement;


// A requirement can have multiple handlers. 
// NOTE: It is not mandatory to add IDirectiveContext as a type parameter if you do not require
// access to the GraphQL context. Simply implement AuthorizationHandler<Requirement> instead.
public class CanAccessUserRequirementHandler : AuthorizationHandler<CanAccessUserRequirement, IDirectiveContext>
{
    // NOTE: It is possible to use constructor injection here for DI, e.g. to use the database

    // Handler methods can be asynchronous, e.g. for database calls
    protected override Task HandleRequirementAsync (
        AuthorizationHandlerContext context,
        CanAccessUserRequirement requirement,
        IDirectiveContext directiveContext
    )
    {
        // You can access the parent element through the directive context
        // If the Guard directive is applied to a class, this will be the class it is applied to
        // If the Guard directive is applied to a property, this will be the containing class of the property
        // Note: This throws an exception at runtime if the parent is not the expected type, e.g.
        // when the policy is mistakenly applied to a different type
        var user = directiveContext.Parent<User>();

        // The currently authenticated user (ClaimsPrincipal) can be accessed at context.User
        if (context.User.GetId() == user.Id)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // User's roles are stored statically in the JWT,
        // and are therefore available in the ClaimsPrincipal without a database query needed
        if (context.User.IsInRole(Role.Admin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        // If the handler cannot definitively determine whether the user has access, simply return.
        // Another handler might be able to provide an answer. 
        // If no handler succeeds, access is denied.
        return Task.CompletedTask;
    }
}
```

#### Authorising GraphQL requests

**For convenience, various extension methods are included for authorising GraphQL entities and fields.**

#### Parent assertion

`RequireParentAssertion` enables you to assert that the _parent_ of a field fulfils a condition.


**NOTE**
When using projection, non-projected fields will _not_ be resolved in the delegate. Therefore, it is important that you mark any other fields that you need in your delegate with the `[IsProjected]` attribute to make sure they are always loaded.

The delegate receives the following arguments:  
- The _parent_ of type `T`
- The authorization context `AuthorizationHandlerContext`
- The GraphQL context `IMiddlewareContext`

Example:
```c#
// UserPolicy.cs

public class UserPolicy : IPolicy
{
    public AuthorizationPolicy? Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireParentAssertion<User>(
        // The logged in user can access his own user, and admin can access all users
            (user, context, directiveContext) => user.Id == context.User.GetId() || context.User.IsInRole(Role.Admin)
        )
        .Build();
}

// User.cs

[Guard<UserPolicy>]
public class User : Entity {
    // ...
}
```

#### Field assertion

`RequireResultAssertion` enables you to assert that the resolved entity or field.   

**Important**  
If the policy is applied to a field, the target is the value of the field.  
If the policy is applied to a class, the target is the class instance.  
If the policy is applied to a resolver, the target is the result of the resolver.  
If the result is an IEnumerable, then the assertion is applied to all elements.  

**NOTE**
When using projection, non-projected fields will _not_ be resolved in the delegate. Therefore, it is important that you mark any other fields that you need in your delegate with the `[IsProjected]` attribute to make sure they are always loaded.

The delegate receives the following arguments:
- The _field value_ `T`
- The authorization context `AuthorizationHandlerContext`
- The GraphQL context `IMiddlewareContext`

Example:
```c#
// UserPolicy.cs

public class AvatarPolicy : IPolicy
{
    public AuthorizationPolicy? Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireTargetAssertion<Avatar>(
        // The avatar can only be accessed if it is set as public
            (avatar, context, directiveContext) => avatar.IsPublic
        )
        .Build();
}

// User.cs

public class User : Entity {
    // ...
    [Guard<AvatarPolicy>] // Can be applied here to only have an effect when accessed through User
    public Avatar? Avatar { get; set; }
}

// Avatar.cs

[Guard<AvatarPolicy>] // Can also be applied here to always have an effect when Avatar is resolved, incl. through other types
public class Avatar : Entity {
    // ...
    public bool IsPublic { get; set; } 
}
```

#### Related assertion

`RequireRelatedAssertion` enables you to assert that the field or the entity containing the field fulfills a condition. This is useful for defining policies used on both fields and parents.

**Important**  
If the policy is applied to a field with type T, the target is the value of the field.  
If the policy is applied to a member of T which is not of type T, the target is an instance of the parent T.  
If the policy is applied to a class of type T, the target is the instance of the class.  
If the policy is applied to a resolver of return type T, the result is the result of the resolver.  
If the result is an IEnumerable, then the assertion is applied to all elements.  

**NOTE**
When using projection, non-projected fields will _not_ be resolved in the delegate. Therefore, it is important that you mark any other fields that you need in your delegate with the `[IsProjected]` attribute to make sure they are always loaded.

The delegate receives the following arguments:
- The _related type_ `T`
- The authorization context `AuthorizationHandlerContext`
- The GraphQL context `IMiddlewareContext`

Example:
```c#
// UserPolicy.cs

public class AvatarPolicy : IPolicy
{
    public AuthorizationPolicy? Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireRelatedAssertion<Avatar>(
        // The avatar can only be accessed if it is set as public
            (avatar, context, directiveContext) => avatar.IsPublic
        )
        .Build();
}

// User.cs

public class User : Entity {
    // ...
    [Guard<AvatarPolicy>] // Here, the assertion is applied to Avatar
    public Avatar? Avatar { get; set; }
}

// Avatar.cs

[Guard<AvatarPolicy>] // Here, the assertion is applied to Avatar
public class Avatar : Entity {
    // ...
    [Guard<AvatarPolicy>] // Here, the assertion is applied to Avatar (the parent)
    public bool IsPublic { get; set; } 
    
    [Guard<AvatarPolicy>] // Here, the assertion is applied to the FIELD of type Avatar (the field not the parent!)
    public Avatar AlternativeAvatar { get; set; } // Just an example
}
```

## Database, migrations, ORM, entities

### Models

All entities that should be persisted in the database should inherit from the `DotnetStarter.Core.Framework.Database.Entity` class, which contains common configuration and properties (such as ID).  
Every entity must also contain an internal `Configuration` class which inherits from `DotnetStarter.Core.Framework.Database.EntityConfiguration<T>` where T is the Entity class. This is necessary for the ORM to recognise the class as an entity. The entity type configuration class can be used to configure indexes, property mappings, and other aspects of the entity.

### Migrations

This project uses the EF Core ORM to handle databases and migrations.

To run EF Core commands, use the `ef` script in the project root directory. E.g. `./ef migrations add MyNewMigration`  
*The `ef` script exists to pass some required common arguments to the `dotnet ef` tool. Using `dotnet ef` directly will not work.*

Migrations are generated automatically from entities found in the project when you run the command for creating a new migration. In most cases, you do not have to edit the generated migrations. **However, always check the generated migration before deploying it to avoid unexpected actions or potential data loss!**

### Common actions & commands

#### Show help and available commands
`./ef`

#### Update the database / run migrations
`./ef database update`

#### Create a new migration
`./ef migrations add`

#### Remove the last migration
`./ef migrations remove`  
**Note** If the migration was already applied, this will not work. Create a new migration to reverse the previous migration instead.

## Testing

Tests are located in the `DotnetStarter.Test` project. Tests should be organised in the same way as the Core project. For example, test classes for Core.Framework.Identity should be in Test.Framework.Identity. If a module has a large amount of test classes, subdivide them into specific directories inside the module.  

To run the tests, either: 
- use the IDE functionality (recommended), or
- run `dotnet test`, or
- run `docker compose up --build test`

### Creating tests

When creating test fixtures, extend from the `IntegrationFixture` class. This provides you with some useful tools for testing, such as `Database` for the database context, and `GraphQlClient` for running GraphQL queries.
Mark test fixtures with the `[TestFixture]` and test methods with the `[Test]` attribute. See [NUnit documentation](https://docs.nunit.org/articles/nunit/intro.html) for more info.

### Naming tests

For the sake of clarity and consistency, name your test methods as follows:

```
Can_[Do what?]_[To what?]_As_[Who?]
Cannot_[Do what?]_[To what?]_As_[Who?]
```

For example:
```
Can_Get_MyRoles_As_User
Can_Create_Users_As_Admin
Cannot_Get_Roles_As_Guest
```

`[To what?]` should be the name of the query/mutation or entity being tested
`[Who?]` should be the role or authentication status of the user performing the action

In certain cases, it might not make sense to use the format above. In that case, try to stay as consistent as possible, but don't go overboard with trying to fit into the mold.

### Assertions

Use the [FluentAssertions](https://fluentassertions.com) extension methods to assert in tests.  
Example:
```c#
var result = new HelloWorldProvider().GetHelloWorld();
result.Should().Be("Hello World!");
```

### Snapshot testing

For GraphQL queries and mutations, the easiest way to test is to use snapshot testing. The project includes the [Snapshooter](https://swisslife-oss.github.io/snapshooter/) library for this.  

Example:
```c#
var request = new GraphQLRequest // Imported from GraphQL (using GraphQL;)
{
    Query = @"
        mutation register {
            registerUser(
                input: {
                    email: ""test@example.com"",
                    password: ""Password123!"",
                    firstName: ""Test"",
                    lastName: ""User""
                }
            ) {
                email
                firstName
                lastName
                fullName
            }
        }
    "
};

var client = GraphQlClient; // Use the GraphQL client provided by the shared integration fixture (IntegrationFixture)

// Execute the mutation, casting the result to object because we don't need to use its properties directly
var response = await client.SendMutationAsync<object>(request); 

response.Data.Should().MatchSnapshot(); // Check that the result matches the snapshot
```

When running the test for the first time, a snapshot will be generated of the result and stored in `__snapshots__` directory next to the test class. **Verify that the snapshot is what you expected**.
If the result is different from the snapshot next time the test is run, the test will fail. To update the snapshot, simply remove the existing one and run the test again.  

**Make sure to commit snapshots in git so that the CI pipeline tests check against valid snapshots**! If no snapshot is found for a snapshot test in the CI pipeline, the test will fail.

### Testing requirements

All functionality must at least have obvious tests that check for breakage or unexpected behaviour. It only takes a couple of minutes to write a couple of basic tests (for example a snapshot test) so basic tests should **always** be shipped with new functionality.  
Prioritise testing functionality that integrates with other parts of the system, to make sure it doesn't break when other parts of the system change.  

**Code reviewers should verify that a merge request includes at least basic tests**.

### GraphQL
When working with GraphQL in tests, it is useful to set up your IDE to use the GraphQL schema, so that you get syntax highlighting. Use the "GraphQL" plugin for Rider for this.  
Set up the GraphQL plugin to use the schema from `DotnetStarter.Test/schema.graphql`. To refresh the GraphQL schema, run `dotnet graphql update` in the `DotnetStarter.Test` directory.

The GraphQL plugin does not automatically understood that a string is supposed to be GraphQL, so when writing a new query, you need to inject the language reference:  
Right-click inside the string > `Show Context Actions` > `Mark as injected language or reference` > `GraphQL`.

To run GraphQL queries, use the `GraphQlClient` that is provided by the `IntegrationFixture`. See example above under `Snapshot testing`.

## Feature-specific documentation

### Framework
C#: [Microsoft Docs | C#](https://docs.microsoft.com/en-us/dotnet/csharp/)  
ASP.NET Core: [Microsoft Docs | ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0)

### GraphQL
HotChocolate: [HotChocolate Docs](https://chillicream.com/docs/hotchocolate)

### ORM / database (Entity Framework Core)
Documentation: [Microsoft Docs | EF Core](https://docs.microsoft.com/en-us/ef/core/)

### Object-to-object mapper (Mapster)
Documentation: [Mapster GitHub Wiki](https://github.com/MapsterMapper/Mapster/wiki)

### Validator (FluentValidation)
Documentation: [fluentvalidation.net](https://fluentvalidation.net/)

### Testing (NUnit)
Documentation: [nunit.org](https://docs.nunit.org/articles/nunit/intro.html)
