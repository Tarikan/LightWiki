using System;
using LightWiki.Infrastructure.Exceptions;

namespace LightWiki.Infrastructure.Auth;

public class UserNotFoundException : UnauthorizedException
{
    public UserNotFoundException(string message) : base(message)
    {
    }

    public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public UserNotFoundException()
    {
    }
}