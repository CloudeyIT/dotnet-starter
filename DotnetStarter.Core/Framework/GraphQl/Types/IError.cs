namespace DotnetStarter.Core.Framework.GraphQl.Types;

public interface IError
{
	public string Message { get; init; }
	public string Code { get; init; }
}