using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileChunker.Core.Entities
{
    public class ChunkMetadata
    {
        [Key]
        public Guid ChunkId { get; set; }

        public int Sequence { get; set; }

        [Required]
        public string StoragePath { get; set; } = string.Empty;

        [Required]
        public Guid FileId { get; set; }
        [ForeignKey(nameof(FileId))]
        public FileMetadata File { get; set; } = null!;
    }
}
