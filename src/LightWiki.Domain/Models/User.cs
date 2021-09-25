using System;
using LightWiki.Domain.Interfaces;

namespace LightWiki.Domain.Models
{
    public class User : ITrackable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}