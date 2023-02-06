namespace DotnetStarter.Core.Framework.GraphQl.Types;

public record Error(string Message, string Code) : IError;