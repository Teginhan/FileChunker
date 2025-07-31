using FileChunker.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileChunker.Business.Services
{
    public class DataCleanupService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DataCleanupService> _logger;
        private readonly string _chunkBasePath;

        public DataCleanupService(AppDbContext context, ILogger<DataCleanupService> logger, string chunkBasePath)
        {
            _context = context;
            _logger = logger;
            _chunkBasePath = chunkBasePath;
        }

        public async Task ClearAllDataAsync()
        {
            _logger.LogWarning("Tüm veri ve chunk dosyaları siliniyor...");

            // Veritabanını temizle
            _context.Chunks.RemoveRange(_context.Chunks);
            _context.Files.RemoveRange(_context.Files);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Veritabanı temizlendi.");

            // Chunk klasörünü temizle
            if (Directory.Exists(_chunkBasePath))
            {
                Directory.Delete(_chunkBasePath, recursive: true);
                _logger.LogInformation("Chunk klasörü silindi: {Path}", _chunkBasePath);
            }
            else
            {
                _logger.LogInformation("Chunk klasörü zaten mevcut değil: {Path}", _chunkBasePath);
            }

            _logger.LogWarning("Silme işlemi tamamlandı.");
        }
    }
}
