using System.Text;
using System.Text.Json.Serialization;
using _40Let.Data;
using _40Let.Enum;
using _40Let.Extensions;
using _40Let.Features;
using _40Let.Handler;
using FortyLet.Storage;
using Kippo.Extensions;
using Kippo.Middleware;
using Minio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var cfg = builder.Configuration;

// Kestrel's default (~28.6MB) is usually enough for a phone photo, but iPhone
// HEIC "Live Photo" uploads can run larger — give food image uploads headroom.
builder.WebHost.ConfigureKestrel(options =>
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

#region services

services.AddScoped<IBotUserService, BotUserService>();

#endregion

builder.Services.AddServicesByConvention();

// EF fixes up navigations both ways, so an Order -> Items -> Order cycle would
// otherwise serialise until MaxDepth. Emit null for the back-reference instead.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

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
            ValidateLifetime = true,
            // Inbound claims aren't mapped, so point role checks at the short
            // "role" claim AuthService writes.
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("SuperAdmin", policy => policy.RequireRole(nameof(Role.SuperAdmin)));
});
#endregion

builder.Services.Configure<SuperAdminOptions>(builder.Configuration.GetSection(SuperAdminOptions.SectionName));

#region swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "40Let API",
        Version = "v1"
    });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT returned by POST /auth/token (no \"Bearer \" prefix needed)."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region CORS

var configuredOrigins = cfg.GetSection("Cors:AllowedOrigins").Get<string[]>();
var defaultOrigins = new[]
{
    "https://tough-actually-imp.ngrok-free.app/",
    "https://tough-actually-imp.ngrok-free.app",
    "http://localhost:3003",
    "http://localhost:3003/"
};

var allowedOrigins = (configuredOrigins is { Length: > 0 } ? configuredOrigins : defaultOrigins)
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => x.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();


services.AddCors(cors => cors.AddDefaultPolicy(
    policy => policy
        .WithOrigins(allowedOrigins)
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders(HeaderNames.ContentDisposition)
));
services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

#endregion

#region Minio

services.AddOptions<MinioOptions>()
    .Bind(builder.Configuration.GetSection(MinioOptions.SectionName))
    .ValidateDataAnnotations();

services.AddSingleton<IMinioClient>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
    return new MinioClient()
        .WithEndpoint(opts.Endpoint)
        .WithCredentials(opts.AccessKey, opts.SecretKey)
        .WithSSL(opts.UseSSL)
        .Build();
});

services.AddScoped<IMinioService, MinioService>();
#endregion
builder.Services.Configure<WebAppOptions>(builder.Configuration.GetSection(WebAppOptions.SectionName));

builder.Services.AddKippo<KippoHandler>(builder.Configuration)
    .AddKippoMiddleware<SessionMiddleware>();
var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "40Let API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "Hello World!").AllowAnonymous();

app.MapAuthEndpoints();
app.MapFoodEndpoints();
app.MapBotUserEndpoints();
app.MapOrderEndpoints();
app.MapCheckEndpoints();


app.Run();
