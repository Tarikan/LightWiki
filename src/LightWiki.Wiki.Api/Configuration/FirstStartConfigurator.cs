using System.Collections.Generic;
using System.Linq;
using LightWiki.Data;
using LightWiki.Domain.Enums;
using LightWiki.Domain.Models;
using Slugify;

namespace LightWiki.Wiki.Api.Configuration;

public class FirstStartConfigurator : IFirstStartConfigurator
{
    private readonly WikiContext _wikiContext;
    private readonly FirstStartConfiguration _firstStartConfiguration;
    private readonly ISlugHelper _slugHelper;

    public FirstStartConfigurator(
        WikiContext wikiContext,
        FirstStartConfiguration firstStartConfiguration,
        ISlugHelper slugHelper)
    {
        _wikiContext = wikiContext;
        _firstStartConfiguration = firstStartConfiguration;
        _slugHelper = slugHelper;
    }

    public void Configure()
    {
        if (_wikiContext.Parties.Any())
        {
            return;
        }

        var adminGroupParty = new Party
        {
            PartyType = PartyType.Group,
        };

        var defaultGroupParty = new Party
        {
            PartyType = PartyType.Group,
        };

        var adminUserParty = new Party
        {
            PartyType = PartyType.User,
        };

        _wikiContext.Parties.Add(adminGroupParty);
        _wikiContext.Parties.Add(defaultGroupParty);
        _wikiContext.Parties.Add(adminUserParty);

        _wikiContext.SaveChanges();

        var admin = new User
        {
            Email = _firstStartConfiguration.AdminEmail,
            Name = _firstStartConfiguration.AdminDefaultName,
            Party = adminUserParty,
        };

        var adminGroup = new Group
        {
            Name = "Admin",
            Slug = "Admin",
            GroupAccessRule = GroupAccessRule.None,
            Users = new List<User>
            {
                admin,
            },
            GroupType = GroupType.Admin,
            Party = adminGroupParty,
        };

        var defaultGroup = new Group
        {
            Name = _firstStartConfiguration.DefaultGroupName,
            Slug = _slugHelper.GenerateSlug(_firstStartConfiguration.DefaultGroupName),
            GroupAccessRule = GroupAccessRule.None,
            Users = new List<User>
            {
                admin,
            },
            GroupType = GroupType.Default,
            Party = defaultGroupParty,
        };

        _wikiContext.Users.Add(admin);
        _wikiContext.Groups.Add(adminGroup);
        _wikiContext.Groups.Add(defaultGroup);

        _wikiContext.SaveChanges();
    }
}