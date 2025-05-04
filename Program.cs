using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Services;
using MusiCloud.Interface;
using Scalar.AspNetCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Inject services to IoC container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDirectoryBrowser();

// Inject sqlite3 database service
builder.Services.AddDbContext<MusiCloudDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MusiCloud")
        ?? throw new NullReferenceException("Database ConnectionString Not Found!"))
);

// Inject service
builder.Services.AddScoped<IMusicService, MusicService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddScoped<IFileProcessService, FileProcessService>();

builder.Services.AddHostedService<FileWatchService>();

// Inject OpenApi service
builder.Services.AddOpenApi();



var app = builder.Build();

// Middleware pipe
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/MusiCloud.json");
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/MusiCloud.json");
    });
}

// app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseStaticFiles();

app.AddCustomStaticFile(app.Configuration.GetValue<string>("CoverCache") ??
    Path.Combine(Directory.GetCurrentDirectory(), "CoverCache"), "/api/cover");
app.AddCustomStaticFile(app.Configuration.GetValue<string>("MusicFolder") ??
        Path.Combine(Directory.GetCurrentDirectory(), "MusicFolder"), "/api/file");

app.MapControllers();

app.Run();
