using System;

namespace LightWiki.Infrastructure.Auth;

public class UserContext : ICloneable
{
    public int Id { get; set; }

    public string Email { get; set; }

    public object Clone()
    {
        return new UserContext
        {
            Id = Id,
            Email = Email,
        };
    }
}