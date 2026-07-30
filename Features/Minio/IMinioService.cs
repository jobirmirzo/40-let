namespace FortyLet.Storage;

public interface IMinioService
{
    Task<string> UploadAsync(IFormFile file, string folder, CancellationToken ct = default);

    Task<Stream> DownloadAsync(
        string objectName,
        CancellationToken ct = default);

    Task DeleteAsync(
        string objectName,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        string objectName,
        CancellationToken ct = default);

    Task<string> GetPresignedUrlAsync(
        string objectName,
        int expirySeconds = 3600,
        CancellationToken ct = default);
}