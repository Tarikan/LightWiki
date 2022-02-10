using LightWiki.Shared.Models;

namespace LightWiki.Features.Users.Responses.Models;

public class UserModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ImageModel Avatar { get; set; }
}