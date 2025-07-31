using FileChunker.Business.Services;
using FileChunker.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileChunker.Business.Orchestration
{
    public class FileChunkCoordinator
    {
        private readonly ChunkingService _chunkingService;
        private readonly IChunkStorageService _chunkStorageService;
        private readonly ChecksumService _checksumService;
        private readonly ChunkReassemblyService _reassemblyService;
        private readonly MetadataService _metadataService;
        private readonly MetadataQueryService _metadataQueryService;
        private readonly ILogger<FileChunkCoordinator> _logger;

        public FileChunkCoordinator(
            ChunkingService chunkingService,
            IChunkStorageService chunkStorageService,
            ChecksumService checksumService,
            ChunkReassemblyService reassemblyService,
            MetadataService metadataService,
            MetadataQueryService metadataQueryService,
            ILogger<FileChunkCoordinator> logger)
        {
            _chunkingService = chunkingService;
            _chunkStorageService = chunkStorageService;
            _checksumService = checksumService;
            _reassemblyService = reassemblyService;
            _metadataService = metadataService;
            _metadataQueryService = metadataQueryService;
            _logger = logger;
        }

        public async Task RunMenuModeAsync()
        {
            Console.Write("Parçalanacak dosyanın tam yolunu girin: ");
            string? filePath = Console.ReadLine();

            Console.Write("Chunk boyutunu MB cinsinden girin: ");
            if (!int.TryParse(Console.ReadLine(), out int chunkSizeInMb))
            {
                Console.WriteLine("Geçersiz boyut.");
                return;
            }

            await ProcessNewFileAsync(filePath!, chunkSizeInMb);
        }
        public async Task ProcessMultipleFilesAsync()
        {
            Console.WriteLine("Dosya yollunu girin. Birden fazla dosya girecekseniz virgülle ayırın:");
            var input = Console.ReadLine(); // Örn: C:\A.txt,C:\B.txt
            
            var filePaths = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(p => p.Trim())
                                 .ToList();

            Console.Write("Chunk boyutunu MB olarak girin: ");
            if (!int.TryParse(Console.ReadLine(), out int chunkSizeInMb))
            {
                Console.WriteLine("Geçersiz değer.");
                return;
            }

            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Dosya bulunamadı: {Path}", filePath);
                    continue;
                }

                Console.WriteLine($"\n==> {Path.GetFileName(filePath)} işleniyor...");
                await ProcessNewFileAsync(filePath, chunkSizeInMb);
            }

        }
        public async Task ListMetadataAsync()
        {
            await _metadataQueryService.ListFilesAsync();
        }

        public async Task ShowDetailsMenuAsync()
        {
            Console.Write("Dosya ID'sini girin: ");
            if (Guid.TryParse(Console.ReadLine(), out Guid fileId))
                await _metadataQueryService.ShowFileDetailsAsync(fileId);
            else
                Console.WriteLine("Geçersiz GUID.");
        }

        public async Task VerifyFileAsync()
        {
            Console.Write("Doğrulamak istediğiniz dosya ID'sini girin: ");
            if (!Guid.TryParse(Console.ReadLine(), out Guid fileId))
            {
                Console.WriteLine("Geçersiz ID.");
                return;
            }

            var file = await _metadataQueryService.GetFileMetadataAsync(fileId);

            if (file == null)
            {
                Console.WriteLine("Dosya bulunamadı.");
                return;
            }

            string tempFile = Path.Combine(Directory.GetCurrentDirectory(), $"verify_{file.OriginalFileName}");

            try
            {
                await _reassemblyService.ReassembleChunksAsync(file.Chunks.OrderBy(c => c.Sequence).Select(c => c.StoragePath).ToList(), tempFile);

                string newChecksum = _checksumService.ComputeChecksumFromFile(tempFile);

                Console.WriteLine("Orijinal SHA256   : " + file.OriginalChecksum);
                Console.WriteLine("Yeniden Hesaplanan: " + newChecksum);

                if (newChecksum == file.OriginalChecksum)
                {
                    Console.WriteLine("Dosya bütünlüğü doğrulandı.");
                }
                else
                {
                    Console.WriteLine("UYUŞMAZLIK! Dosya bozulmuş olabilir.");
                }
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        private async Task ProcessNewFileAsync(string filePath, int chunkSizeInMb)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError("Dosya bulunamadı: {Path}", filePath);
                return;
            }

            int chunkSizeInBytes = chunkSizeInMb * 1024 * 1024;

            var chunks = await _chunkingService.ChunkFileAsync(filePath, chunkSizeInBytes);
            _logger.LogInformation("Toplam chunk: {Count}", chunks.Count);

            var fileId = Guid.NewGuid(); // Dosya için klasör ID’si olarak da kullanılacak

            var storedChunkInfos = await _chunkStorageService.StoreChunksAsync(chunks, fileId);
            _logger.LogInformation("Toplam kaydedilen chunk: {Count}", storedChunkInfos.Count);

            string originalChecksum = _checksumService.ComputeChecksumFromFile(filePath);
            _logger.LogInformation("Orijinal dosya SHA256: {Checksum}", originalChecksum);

            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"reassembled_{Path.GetFileName(filePath)}");
            await _reassemblyService.ReassembleChunksAsync(storedChunkInfos.Select(x => x.StoragePath).ToList(), outputPath);

            string reassembledChecksum = _checksumService.ComputeChecksumFromFile(outputPath);
            _logger.LogInformation("Birleştirilen dosya SHA256: {Checksum}", reassembledChecksum);

            if (reassembledChecksum == originalChecksum)
                _logger.LogInformation("Checksum doğrulandı.");
            else
                _logger.LogWarning("Checksum uyuşmadı!");

            await _metadataService.SaveFileMetadataAsync(fileId, Path.GetFileName(filePath), originalChecksum, storedChunkInfos);
        }
    }
}
