using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Swagger;

public sealed class RemoveVersionParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var versionParam = operation.Parameters!.FirstOrDefault(p => p.Name == "version");
        if (versionParam is not null)
        {
            operation.Parameters!.Remove(versionParam);
        }
    }
}

public sealed class ReplaceVersionInPathDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var version = swaggerDoc.Info.Version!.TrimStart('v');
        var updatedPaths = swaggerDoc.Paths
            .ToDictionary(
                p => p.Key.Replace("{version}", version),
                p => p.Value);

        swaggerDoc.Paths.Clear();
        foreach (var (path, item) in updatedPaths)
        {
            swaggerDoc.Paths.Add(path, item);
        }
    }
}
