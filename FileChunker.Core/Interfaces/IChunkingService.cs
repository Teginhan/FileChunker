namespace FileChunker.Core.Interfaces
{
    public interface IChunkingService
    {
        Task<List<byte[]>> ChunkFileAsync(string filePath, int chunkSize);
    }
}
