using System;
using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Users.Requests;

public class UpdateUserInfo : IRequest<OneOf<Success, Fail>>
{
    public string CountryCode { get; set; }

    public string Location { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string Bio { get; set; }

    public string ContactEmail { get; set; }
}