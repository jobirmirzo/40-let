using System.Text;
using _40Let.Data;
using _40Let.Extensions;
using _40Let.Features;
using _40Let.Handler;
using Kippo.Extensions;
using Kippo.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

#region services

services.AddScoped<IBotUserService, BotUserService>();

#endregion
builder.Services.AddServicesByConvention();

#region auth
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();
#endregion

builder.Services.AddKippo<KippoHandler>(builder.Configuration)
    .AddKippoMiddleware<SessionMiddleware>();
var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapAuthEndpoints();
app.MapFoodEndpoints();
app.MapBotUserEndpoints();
app.MapOrderEndpoints();
app.MapCheckEndpoints();


app.Run();
