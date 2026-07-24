using Microsoft.AspNetCore.Mvc;

namespace _40Let.Features;

public static class FileEndpoints
{
    public static IEndpointRouteBuilder MapFileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/files").WithTags("Files");

        // Upload any file; returns the object key to persist on your entity.
        group.MapPost("/", async (IFormFile file, IMinioService storage, [FromQuery] string? folder) =>
        {
            var key = await storage.UploadAsync(file, folder);
            return Results.Ok(new { key });
        })
            .DisableAntiforgery()
            .WithName("UploadFile")
            .WithSummary("Upload a file and get back its object key")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(StatusCodes.Status200OK);

        // Turn a stored key into a time-limited presigned GET URL.
        group.MapGet("/presigned", async (string key, IMinioService storage, [FromQuery] int? expiry) =>
        {
            var url = await storage.GetPresignedUrlAsync(key, expiry);
            return url is null ? Results.NotFound() : Results.Ok(new { url });
        })
            .WithName("GetPresignedUrl")
            .WithSummary("Get a presigned GET URL for an object key")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // Delete an object by key.
        group.MapDelete("/", async (string key, IMinioService storage) =>
        {
            await storage.DeleteAsync(key);
            return Results.NoContent();
        })
            .WithName("DeleteFile")
            .WithSummary("Delete an object by key")
            .Produces(StatusCodes.Status204NoContent);

        return app;
    }
}
