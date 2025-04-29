using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Inject services to IoC container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Inject sqlite3 database service
builder.Services.AddDbContext<MusiCloudDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MusiCloud")
        ?? throw new NullReferenceException("Database ConnectionString Not Found!"))
);

// Inject OpenApi service
builder.Services.AddOpenApi();

// Inject MusiCloud service
builder.Services.AddScoped<IMusicService, MusicService>();

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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
