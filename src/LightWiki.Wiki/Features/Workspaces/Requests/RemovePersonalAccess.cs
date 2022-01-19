﻿using LightWiki.Infrastructure.Models;
using MediatR;
using OneOf;

namespace LightWiki.Features.Workspaces.Requests;

public class RemovePersonalAccess : IRequest<OneOf<Success, Fail>>
{
    public int WorkspaceId { get; set; }

    public int UserId { get; set; }
}