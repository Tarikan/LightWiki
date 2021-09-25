using System;

namespace LightWiki.Domain.Interfaces
{
    public interface ITrackable
    {
        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}