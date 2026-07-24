namespace _40Let.Features;

public interface IMinioService
{
    /// <summary>
    /// Uploads a file and returns the object key (store this on your entity, not the URL).
    /// </summary>
    /// <param name="file">The uploaded file.</param>
    /// <param name="folder">Optional logical folder/prefix, e.g. "foods".</param>
    Task<string> UploadAsync(IFormFile file, string? folder = null, CancellationToken ct = default);

    /// <summary>
    /// Builds a time-limited presigned GET URL for an object key.
    /// Returns <c>null</c> when the key is null/empty.
    /// </summary>
    Task<string?> GetPresignedUrlAsync(string? key, int? expirySeconds = null, CancellationToken ct = default);

    /// <summary>Deletes an object by key. No-op when the key is null/empty.</summary>
    Task DeleteAsync(string? key, CancellationToken ct = default);
}
