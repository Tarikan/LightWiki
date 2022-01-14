using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class GroupPersonalAccessRule
    {
        public int Id { get; set; }

        public Group Group { get; set; }

        public int GroupId { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public GroupAccessRule AccessRule { get; set; }
    }
}