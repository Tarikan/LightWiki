using System;

namespace LightWiki.Domain.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public string CountryCode { get; set; }

        public string Location { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Bio { get; set; }

        public string ContactEmail { get; set; }
    }
}