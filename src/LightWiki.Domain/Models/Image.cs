using LightWiki.Domain.Enums;

namespace LightWiki.Domain.Models
{
    public class Image
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public OwnerType OwnerType { get; set; }

        public string Folder { get; set; }

        public string FileName { get; set; }

        public string Metadata { get; set; }
    }
}