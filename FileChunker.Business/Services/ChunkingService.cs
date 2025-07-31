using FileChunker.Core.Interfaces;

namespace FileChunker.Business.Services
{
    public class ChunkingService : IChunkingService
    {
        public async Task<List<byte[]>> ChunkFileAsync(string filePath, int chunkSize)
        {
            var chunks = new List<byte[]>();

            using (var stream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[chunkSize];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, chunkSize)) > 0)
                {
                    if (bytesRead < chunkSize)
                        buffer = buffer.Take(bytesRead).ToArray();

                    chunks.Add(buffer.ToArray());
                    buffer = new byte[chunkSize];
                }
            }

            return chunks;
        }
    }
}
