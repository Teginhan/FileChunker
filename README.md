FileChunker

FileChunker, .NET 8 tabanlı, modüler ve test edilebilir bir dosya parçalama altyapısıdır. Büyük boyutlu dosyaları otomatik olarak parçalar (chunk'lar), farklı storage provider'lara dağıtır ve istenirse tekrar birleştirir. Tüm işlemler loglanır ve dosya bütünlüğü SHA256 checksum ile doğrulanır.

🌐 Mimari Yapı

Proje klasik katmanlı mimariye dayanır:

Core: Tüm arayüzler ve modeller (entities)

Business: İş mantığını barındıran servisler ve yöneticiler

DataAccess: EF Core kullanılarak SQLite veritabanı erisimi

ConsoleUI: CLI arayüz ve girilen komutların koordine edilmesi

✅ Desteklenen Özellikler

Tekli ya da çoklu dosya chunk'lama

Dinamik chunk boyutu (MB bazlı)

Chunk dosyalarını dosya bazlı klasörlerde saklama

Farklı storage provider'larla çalışma (FileSystem desteği hazır)

Dosya bütünlüğü için SHA256 checksum doğrulama

Metadata'nın SQLite'da saklanması

İsteğe bağlı olarak tüm verileri temizleme işlemi

Detaylı Serilog loglama

📂 Kullanılan Design Pattern'ler

# Design Pattern

Amaç    /    Kullanım

1 - Dependency Injection : Gevşek bağlantı ve test edilebilirlik

2 - Interface Segregation : Her servisin soyutlanması

3 - Strategy Pattern : IStorageProvider ile farklı storage stratejileri

4 - Coordinator : FileChunkCoordinator ile iş akışını yönetme

5 - Logging (Cross-cutting) : Serilog ile tüm işlemleri izlenebilir hale getirme

🔧 Kurulum

.NET 8 SDK yükleyin

Projeyi klonlayın: git clone https://github.com/kullanici/FileChunker.git

dotnet restore komutunu çalıştırın

Migration için:

dotnet ef migrations add InitialCreate --project FileChunker.DataAccess
dotnet ef database update

FileChunker.ConsoleUI projesini başlatın: dotnet run --project FileChunker.ConsoleUI

🎮 Kullanım

Konsolda menüden aşağıdaki işlemler yapılabilir:

Dosya chunk'lama

Chunk birleştirme ve dosya doğrulama

Metadata listeleme

Tüm verileri ve dosyaları temizleme
