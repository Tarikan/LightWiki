using System;

namespace LightWiki.Features.Users.Responses.Models;

public class UserInfoModel
{
    public string CountryCode { get; set; }

    public string Location { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string Bio { get; set; }

    public string ContactEmail { get; set; }
}