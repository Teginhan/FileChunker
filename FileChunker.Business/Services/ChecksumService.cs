using System.Security.Cryptography;
using System.Text;

namespace FileChunker.Business.Services
{
   
    // SHA256 checksum işlemlerini yöneten servis.
    
    public class ChecksumService
    {
       
        // Bir dosyanın SHA256 hash'ini üretir.
        
        public string ComputeChecksumFromFile(string filePath)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha.ComputeHash(stream);
            return Convert.ToHexString(hash);
        }

       
        // Byte dizisinden SHA256 checksum üretir.
        
        public string ComputeChecksumFromBytes(byte[] data)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(data);
            return Convert.ToHexString(hash);
        }
    }
}
