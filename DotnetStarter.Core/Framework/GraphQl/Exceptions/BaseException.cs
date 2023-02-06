namespace DotnetStarter.Core.Framework.GraphQl.Exceptions;

public class BaseException : Exception
{
    public string Code { get; set; }

    public BaseException (string message, string code) : base(message)
    {
        Code = code;
    }
}