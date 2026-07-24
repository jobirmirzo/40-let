using System.Text;
using System.Text.Json.Serialization;
using _40Let.Data;
using _40Let.Extensions;
using _40Let.Features;
using _40Let.Handler;
using Kippo.Extensions;
using Kippo.Middleware;
using Minio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

#region services

services.AddScoped<IBotUserService, BotUserService>();

#endregion

#region minio
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection(MinioOptions.SectionName));
var minio = builder.Configuration.GetSection(MinioOptions.SectionName).Get<MinioOptions>()!;

builder.Services.AddSingleton<IMinioClient>(_ => new MinioClient()
    .WithEndpoint(minio.Endpoint)
    .WithCredentials(minio.AccessKey, minio.SecretKey)
    .WithSSL(minio.UseSSL)
    .Build());
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
builder.Services.AddAuthorization();
#endregion

#region swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "40Let API",
        Version = "v1"
    });

    // JWT bearer support: lets you click "Authorize" and paste a token.
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT returned by POST /auth/token (no \"Bearer \" prefix needed).",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [scheme] = Array.Empty<string>()
    });
});
#endregion

builder.Services.AddKippo<KippoHandler>(builder.Configuration)
    .AddKippoMiddleware<SessionMiddleware>();
var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "40Let API v1");
    // Swagger UI available at /swagger (root "/" is taken by the Hello World endpoint).
    options.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapAuthEndpoints();
app.MapFoodEndpoints();
app.MapBotUserEndpoints();
app.MapOrderEndpoints();
app.MapCheckEndpoints();
app.MapFileEndpoints();


app.Run();
