namespace DotnetStarter.Core.Framework.Cli;

public class CommandAttribute : Attribute
{
	public CommandAttribute (string name, string group = "")
	{
		Name = name;
		Group = group;
	}

	public string Name { get; }
	public string Group { get; }
}