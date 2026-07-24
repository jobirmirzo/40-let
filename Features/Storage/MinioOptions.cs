namespace _40Let.Features;

public class MinioOptions
{
    public const string SectionName = "Minio";

    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public bool UseSSL { get; set; }

    /// <summary>Default lifetime of generated presigned GET URLs, in seconds (max 7 days).</summary>
    public int PresignedExpirySeconds { get; set; } = 3600;
}
