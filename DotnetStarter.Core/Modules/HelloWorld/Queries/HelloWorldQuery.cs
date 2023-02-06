namespace DotnetStarter.Core.Modules.HelloWorld.Queries;

[QueryType]
public class HelloWorldQuery
{
	public HelloWorldPayload HelloWorld (HelloWorldInput input)
	{
		return new()
			{ Message = $"Hello {input.Name}!" };
	}

	public record HelloWorldInput(string Name);

	public record HelloWorldPayload
	{
		public string Message { get; init; } = string.Empty;
	}
}