using System;

namespace LightWiki.Infrastructure.Auth;

public class UserWithSuchEmailAlreadyExists : Exception
{
    public UserWithSuchEmailAlreadyExists(string message) : base(message)
    {
    }

    public UserWithSuchEmailAlreadyExists()
    {
    }

    public UserWithSuchEmailAlreadyExists(string message, Exception innerException) : base(message, innerException)
    {
    }
}