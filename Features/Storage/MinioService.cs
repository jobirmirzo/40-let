using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace _40Let.Features;

public class MinioService(IMinioClient client, IOptions<MinioOptions> options) : IMinioService
{
    private readonly MinioOptions _options = options.Value;
    private bool _bucketChecked;

    public async Task<string> UploadAsync(IFormFile file, string? folder = null, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("File is empty.", nameof(file));

        await EnsureBucketAsync(ct);

        var key = BuildKey(folder, file.FileName);

        await using var stream = file.OpenReadStream();
        await client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(key)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType), ct);

        return key;
    }

    public async Task<string?> GetPresignedUrlAsync(string? key, int? expirySeconds = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        return await client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(key)
            .WithExpiry(expirySeconds ?? _options.PresignedExpirySeconds));
    }

    public async Task DeleteAsync(string? key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        await client.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(key), ct);
    }

    private async Task EnsureBucketAsync(CancellationToken ct)
    {
        if (_bucketChecked)
            return;

        var exists = await client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.BucketName), ct);
        if (!exists)
            await client.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_options.BucketName), ct);

        _bucketChecked = true;
    }

    // Produces keys like "40let/foods/8f3c...e1.jpg" so a single bucket can host
    // multiple projects/features without collisions.
    private string BuildKey(string? folder, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var unique = $"{Guid.NewGuid():N}{extension}";

        var segments = new[] { _options.ProjectName, folder?.Trim('/'), unique }
            .Where(s => !string.IsNullOrWhiteSpace(s));

        return string.Join('/', segments);
    }
}
