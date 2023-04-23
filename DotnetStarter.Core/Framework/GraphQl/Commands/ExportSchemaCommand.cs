using DotnetStarter.Core.Framework.Cli;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DotnetStarter.Core.Framework.GraphQl.Commands;

[Command("export", "schema")]
internal sealed class ExportSchemaCommand : Command<ExportSchemaCommand.Settings>
{
	private readonly ISchema _schema;

	public ExportSchemaCommand (IRequestExecutorBuilder executorBuilder)
	{
		_schema = executorBuilder.BuildSchemaAsync().Result;
	}

	public override int Execute (CommandContext context, Settings settings)
	{
		AnsiConsole.MarkupLine($"[yellow]Exporting schema to [blue bold]{settings.File}[/][/]");
		File.WriteAllText(settings.File, _schema.Print());
		AnsiConsole.MarkupLine($"[green]Successfully exported schema to [blue bold]{settings.File}[/][/]");
		return 0;
	}

	public class Settings : CommandSettings
	{
		[CommandArgument(0, "[FILE]")]
		public string File { get; set; } = "schema.graphql";
	}
}