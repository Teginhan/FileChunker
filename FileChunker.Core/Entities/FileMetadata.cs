using System.ComponentModel.DataAnnotations;

namespace FileChunker.Core.Entities
{
    public class FileMetadata
    {
        [Key]
        public Guid FileId { get; set; }

        [Required]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        public string OriginalChecksum { get; set; } = string.Empty;

        public int TotalChunks { get; set; }
        public ICollection<ChunkMetadata> Chunks { get; set; } = new List<ChunkMetadata>();
    }
}
