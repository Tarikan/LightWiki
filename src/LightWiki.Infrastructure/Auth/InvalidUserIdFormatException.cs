using LightWiki.Infrastructure.Exceptions;

namespace LightWiki.Infrastructure.Auth;

public class InvalidUserIdFormatException : UnauthorizedException
{
    public InvalidUserIdFormatException(string message) : base(message)
    {
    }

    public InvalidUserIdFormatException()
    {
    }

    public InvalidUserIdFormatException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
}