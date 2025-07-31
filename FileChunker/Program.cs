using FileChunker.Business.Orchestration;
using FileChunker.Business.Services;
using FileChunker.Core.Interfaces;
using FileChunker.DataAccess.Context;
using FileChunker.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

try
{
    var services = new ServiceCollection();

    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog();
    });

    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=filechunker.db"));

    services.AddScoped<FileChunkCoordinator>();
    services.AddScoped<ChunkingService>();
    services.AddScoped<IChunkStorageService, ChunkStorageService>();
    services.AddScoped<ChecksumService>();
    services.AddScoped<ChunkReassemblyService>();
    services.AddScoped<MetadataService>();
    services.AddScoped<MetadataQueryService>();
    services.AddScoped<IStorageProvider>(provider =>
        new FileSystemStorageProvider("chunks", provider.GetRequiredService<ILogger<FileSystemStorageProvider>>()));
    services.AddScoped<DataCleanupService>(provider =>
    {
        var dbContext = provider.GetRequiredService<AppDbContext>();
        var logger = provider.GetRequiredService<ILogger<DataCleanupService>>();
        return new DataCleanupService(dbContext, logger, "chunks");
    });
    var provider = services.BuildServiceProvider();
    var coordinator = provider.GetRequiredService<FileChunkCoordinator>();

    while (true)
    {
        Console.WriteLine("--- FileChunker ---");
        Console.WriteLine("1 - Dosya Parçala");
        Console.WriteLine("2 - Dosya Listesi");
        Console.WriteLine("3 - Detay Göster");
        Console.WriteLine("4 - Dosya Bütünlüğünü Kontrol Et");
        Console.WriteLine("5 - Tüm Verileri ve Chunk Dosyalarını Sil");
        Console.WriteLine("0 - Çıkış");
        Console.Write("Seçim: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await coordinator.ProcessMultipleFilesAsync();
                break;
            case "2":
                await coordinator.ListMetadataAsync();
                break;
            case "3":
                await coordinator.ShowDetailsMenuAsync();
                break;
            case "4":
                await coordinator.VerifyFileAsync();
                break;
            case "5":
                Console.Write("Bu işlem geri alınamaz. Emin misiniz? (y/n): ");
                var confirm = Console.ReadLine()?.ToLower();
                if (confirm == "y")
                {
                    var cleanup = provider.GetRequiredService<DataCleanupService>();
                    await cleanup.ClearAllDataAsync();
                }
                break;
            case "0":
                return;
        }
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama başlatılırken hata oluştu.");
}
finally
{
    Log.CloseAndFlush();
}