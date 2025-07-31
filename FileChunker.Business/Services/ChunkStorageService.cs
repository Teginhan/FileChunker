using FileChunker.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileChunker.Business.Services
{
   
    // Chunk'ları storage provider aracılığıyla saklayan servis.
    
    public class ChunkStorageService : IChunkStorageService
    {
        private readonly IStorageProvider _storageProvider;

        public ChunkStorageService(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public async Task<List<(Guid ChunkId, string StoragePath)>> StoreChunksAsync(List<byte[]> chunks, Guid fileId)
        {
            var result = new List<(Guid, string)>();

            foreach (var chunk in chunks)
            {
                Guid chunkId = Guid.NewGuid();
                string path = await _storageProvider.SaveChunkAsync(fileId, chunkId, chunk);
                result.Add((chunkId, path));
            }

            return result;
        }
    }
}
