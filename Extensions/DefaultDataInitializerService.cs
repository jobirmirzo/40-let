using _40Let.Data;
using Microsoft.EntityFrameworkCore;

namespace _40Let.Extensions;

public static class DefaultDataInitializerService
{
 
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
