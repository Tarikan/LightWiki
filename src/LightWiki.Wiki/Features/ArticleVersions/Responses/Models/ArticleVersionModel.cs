using System;
using LightWiki.Domain.Models;
using LightWiki.Features.Users.Responses.Models;

namespace LightWiki.Features.ArticleVersions.Responses.Models;

public class ArticleVersionModel
{
    public int Id { get; set; }

    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public UserModel User { get; set; }

    public DateTime CreationDate { get; set; }
}