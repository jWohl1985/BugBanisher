namespace BugBanisher.Services.Interfaces;

public interface IFileService
{
    Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);
    string ConvertByteArrayToFile(byte[] fileData, string extension);
    string GetFileIcon(string file);
    string FormatFileSize(long bytes);
    string GetDefaultUserPicPath();
    string GetDefaultProjectPicPath();
}
