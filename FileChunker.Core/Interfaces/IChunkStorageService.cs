namespace FileChunker.Core.Interfaces
{
    public interface IChunkStorageService
    {
        Task<List<(Guid ChunkId, string StoragePath)>> StoreChunksAsync(List<byte[]> chunks, Guid fileId);
    }
}
