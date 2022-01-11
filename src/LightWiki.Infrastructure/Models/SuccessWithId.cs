namespace LightWiki.Infrastructure.Models;

public class SuccessWithId<TId> : Success
{
    public TId Id { get; set; }

    public SuccessWithId(TId id)
    {
        Id = id;
    }
}