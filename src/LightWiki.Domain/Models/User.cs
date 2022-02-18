using System;
using System.Collections.Generic;
using LightWiki.Domain.Interfaces;

namespace LightWiki.Domain.Models
{
    public class User : ITrackable
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public List<Group> Groups { get; set; }

        public int PartyId { get; set; }

        public Party Party { get; set; }

        public List<GroupPersonalAccessRule> GroupPersonalAccessRules { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserInfo UserInfo { get; set; }
    }
}