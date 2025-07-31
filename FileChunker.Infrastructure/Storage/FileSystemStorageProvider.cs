using FileChunker.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileChunker.Infrastructure.Storage
{
    public class FileSystemStorageProvider : IStorageProvider
    {
        private readonly string _basePath;
        private readonly ILogger<FileSystemStorageProvider> _logger;

        public FileSystemStorageProvider(string basePath, ILogger<FileSystemStorageProvider> logger)
        {
            _basePath = basePath;
            _logger = logger;
        }

        public async Task<string> SaveChunkAsync(Guid fileId, Guid chunkId, byte[] data)
        {
            string dirPath = Path.Combine(_basePath, fileId.ToString());
            Directory.CreateDirectory(dirPath);

            string filePath = Path.Combine(dirPath, $"{chunkId}.bin");

            await File.WriteAllBytesAsync(filePath, data);

            _logger.LogInformation("Chunk {ChunkId} saved to {Path}", chunkId, filePath);

            return filePath;
        }

        public async Task<byte[]> ReadChunkAsync(string path)
        {
            _logger.LogInformation("Reading chunk from {Path}", path);
            return await File.ReadAllBytesAsync(path);
        }
    }
}