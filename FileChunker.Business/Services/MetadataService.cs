using FileChunker.Core.Entities;
using FileChunker.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileChunker.Business.Services
{
    /// <summary>
    /// Dosya ve chunk metadata'larını veritabanına kaydeden servis.
    /// </summary>
    public class MetadataService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<MetadataService> _logger;

        public MetadataService(AppDbContext dbContext, ILogger<MetadataService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

       
        // Dosya ve ilişkili chunk'ları veritabanına kaydeder.

        public async Task SaveFileMetadataAsync(Guid fileId, string originalFileName, string checksum, List<(Guid ChunkId, string Path)> chunkInfos)
        {
            var fileMetadata = new FileMetadata
            {
                FileId = fileId,
                OriginalFileName = originalFileName,
                OriginalChecksum = checksum,
                TotalChunks = chunkInfos.Count,
                Chunks = chunkInfos.Select((info, index) => new ChunkMetadata
                {
                    ChunkId = info.ChunkId,
                    Sequence = index,
                    StoragePath = info.Path
                }).ToList()
            };

            _dbContext.Files.Add(fileMetadata);
            await _dbContext.SaveChangesAsync();
        }

    }
}
