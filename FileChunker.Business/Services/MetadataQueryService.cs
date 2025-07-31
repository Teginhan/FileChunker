using FileChunker.Core.Entities;
using FileChunker.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileChunker.Business.Services
{
    public class MetadataQueryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MetadataQueryService> _logger;

        public MetadataQueryService(AppDbContext context, ILogger<MetadataQueryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ListFilesAsync()
        {
            var files = await _context.Files
                .OrderByDescending(f => f.FileId)
                .ToListAsync();

            Console.WriteLine("Kayıtlı Dosyalar:");
            if (files.Count==0)
            {
                Console.WriteLine("Kayıtlı Dosya Yok");
                return;
            }

            foreach (var file in files)
            {
                Console.WriteLine($"Id: {file.FileId} | Ad: {file.OriginalFileName} | Chunk: {file.TotalChunks}");
            }

            
        }

        public async Task ShowFileDetailsAsync(Guid fileId)
        {
            var file = await _context.Files
                .Include(f => f.Chunks)
                .FirstOrDefaultAsync(f => f.FileId == fileId);

            if (file == null)
            {
                Console.WriteLine("Dosya bulunamadı.");
                return;
            }

            Console.WriteLine($"Dosya: {file.OriginalFileName}");
            Console.WriteLine($"SHA256: {file.OriginalChecksum}");
            Console.WriteLine($"Chunk Sayısı: {file.TotalChunks}");

            foreach (var chunk in file.Chunks.OrderBy(c => c.Sequence))
            {
                Console.WriteLine($" - Chunk {chunk.Sequence}: {chunk.ChunkId}");
            }
        }

        public async Task<FileMetadata?> GetFileMetadataAsync(Guid fileId)
        {
            return await _context.Files
                .Include(f => f.Chunks)
                .FirstOrDefaultAsync(f => f.FileId == fileId);
        }
    }
}
