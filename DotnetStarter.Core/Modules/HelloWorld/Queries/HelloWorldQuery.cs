using DotnetStarter.Core.Framework.GraphQl.Types;

namespace DotnetStarter.Core.Modules.HelloWorld.Queries;

[ExtendObjectType(typeof(Query))]
public class HelloWorldQuery
{
    public record HelloWorldInput(string Name);

    public record HelloWorldPayload
    {
        public string Message { get; init; } = string.Empty;
    }

    public HelloWorldPayload HelloWorld (HelloWorldInput input) =>
        new HelloWorldPayload { Message = $"Hello {input.Name}!" };
}