using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Microsoft.AspNetCore.Builder;

public static class StaticFileHelper
{
    public static IApplicationBuilder AddCustomStaticFile(this IApplicationBuilder app, string path,string requestPath)
    {
        // Ensure directory exists
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        app.UseDirectoryBrowser(new DirectoryBrowserOptions
        {
            FileProvider = new PhysicalFileProvider(path),
            RequestPath = requestPath
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(path),
            RequestPath = requestPath,
            ContentTypeProvider = CustomFileTypeProvider(),
        });

        return app;
    }

    private static FileExtensionContentTypeProvider CustomFileTypeProvider()
    {
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".mp3"] = "audio/mpeg";
        provider.Mappings[".m4a"] = "audio/mp4";
        provider.Mappings[".flac"] = "audio/flac";
        provider.Mappings[".wav"] = "audio/wav";
        return provider;
    }
}