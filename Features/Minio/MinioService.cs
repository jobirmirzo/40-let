using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace FortyLet.Storage;

public sealed class MinioService(
    IMinioClient client,
    IOptions<MinioOptions> options,
    ILogger<MinioService> logger) : IMinioService
{
    private readonly MinioOptions _options = options.Value;
    private string Bucket => _options.BucketName;

    public async Task<string> UploadAsync(IFormFile file, string folder, CancellationToken ct = default)
    {
        // Bucket is provisioned server-side; skip the existence check because
        // MinIO's HeadBucket is broken under virtual-host-style addressing.
        var extension = Path.GetExtension(file.FileName);
        var objectName = string.IsNullOrEmpty(folder)
            ? $"{Guid.NewGuid():N}{extension}"
            : $"{folder.Trim('/')}/{Guid.NewGuid():N}{extension}";

        await using var stream = file.OpenReadStream();

        var args = new PutObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(string.IsNullOrEmpty(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType);

        await client.PutObjectAsync(args, ct);
        logger.LogInformation("Uploaded {Object} to bucket {Bucket}", objectName, Bucket);
        return objectName;
    }

    /// <summary>Downloads an object into a seekable MemoryStream positioned at 0.</summary>
    public async Task<Stream> DownloadAsync(
        string objectName,
        CancellationToken ct = default)
    {
        var memory = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectName)
            .WithCallbackStream(async (stream, token) =>
                await stream.CopyToAsync(memory, token));

        await client.GetObjectAsync(args, ct);
        memory.Position = 0;
        return memory;
    }

    public async Task DeleteAsync(
        string objectName,
        CancellationToken ct = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectName);

        await client.RemoveObjectAsync(args, ct);
        logger.LogInformation("Deleted {Object} from bucket {Bucket}", objectName, Bucket);
    }

    public async Task<bool> ExistsAsync(
        string objectName,
        CancellationToken ct = default)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(Bucket)
                .WithObject(objectName);

            await client.StatObjectAsync(args, ct);
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
    }

    public async Task<string> GetPresignedUrlAsync(
        string objectName,
        int expirySeconds = 3600,
        CancellationToken ct = default)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(Bucket)
            .WithObject(objectName)
            .WithExpiry(expirySeconds);

        return await client.PresignedGetObjectAsync(args);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        try
        {
            var exists = await client.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(Bucket), ct);
            if (!exists)
                await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(Bucket), ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MinIO failed. Endpoint={Endpoint} SSL={Ssl} Bucket={Bucket}",
                _options.Endpoint, _options.UseSSL, Bucket);
            throw;
        }
    }
}