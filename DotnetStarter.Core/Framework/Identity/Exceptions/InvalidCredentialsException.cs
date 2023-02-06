namespace DotnetStarter.Core.Framework.Identity.Exceptions;

public class InvalidCredentialsException : Exception
{
    public string Code => "CREDENTIALS_INVALID";

    public InvalidCredentialsException (string message = "Invalid email or password") : base(message)
    {}
}