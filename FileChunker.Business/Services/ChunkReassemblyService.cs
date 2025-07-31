using FileChunker.Core.Interfaces;

namespace FileChunker.Business.Services
{
    public class ChunkReassemblyService
    {
        private readonly IStorageProvider _storageProvider;

        public ChunkReassemblyService(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        
        // Verilen path listesine göre chunk dosyalarını sırayla okuyup hedef dosyaya birleştirir.
        
        public async Task ReassembleChunksAsync(List<string> chunkPaths, string outputFilePath)
        {
            if (File.Exists(outputFilePath))
                File.Delete(outputFilePath);

            using (var outputStream = new FileStream(outputFilePath, FileMode.CreateNew))
            {
                foreach (var path in chunkPaths)
                {
                    var chunkData = await _storageProvider.ReadChunkAsync(path);
                    await outputStream.WriteAsync(chunkData, 0, chunkData.Length);
                }
            }
        }
    }
}
