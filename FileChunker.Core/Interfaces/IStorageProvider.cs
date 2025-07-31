using System.Threading.Tasks;

namespace FileChunker.Core.Interfaces
{

    // Chunk verilerini kaydetme ve geri alma işlemleri için temel arayüz.

    public interface IStorageProvider
    {
        Task<string> SaveChunkAsync(Guid fileId, Guid chunkId, byte[] data);
        Task<byte[]> ReadChunkAsync(string path);
    }

}
