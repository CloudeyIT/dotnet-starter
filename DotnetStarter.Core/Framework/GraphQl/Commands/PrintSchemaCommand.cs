using DotnetStarter.Core.Framework.Cli;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Spectre.Console.Cli;

namespace DotnetStarter.Core.Framework.GraphQl.Commands;

[Command("print", "schema")]
internal sealed class PrintSchemaCommand : Command
{
	private readonly ISchema _schema;

	public PrintSchemaCommand (IRequestExecutorBuilder executorBuilder)
	{
		_schema = executorBuilder.BuildSchemaAsync().Result;
	}

	public override int Execute (CommandContext context)
	{
		Console.Write(_schema.Print());
		return 0;
	}
}
