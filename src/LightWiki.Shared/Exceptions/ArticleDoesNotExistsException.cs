namespace LightWiki.Shared.Exceptions;

using LightWiki.Infrastructure.Exceptions;

public class ArticleDoesNotExistsException : NotFoundException
{
    public ArticleDoesNotExistsException(string message) : base(message)
    {
    }

    public ArticleDoesNotExistsException()
    {
    }

    public ArticleDoesNotExistsException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }
}