using _40Let.Data;
using _40Let.Handler;
using Kippo.Extensions;
using Kippo.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddKippo<KippoHandler>(builder.Configuration)
    .AddKippoMiddleware<SessionMiddleware>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.Run();
