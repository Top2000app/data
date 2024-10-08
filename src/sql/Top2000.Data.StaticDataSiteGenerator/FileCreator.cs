﻿namespace Top2000.Data.StaticApiGenerator;

public interface IFileCreator
{
    Task CreateDataFilesAsync(string location);

    Task CreateApiFileAsync(string location);
}

public sealed class FileCreator : IFileCreator
{
    private static readonly JsonSerializerOptions serializerSettings = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly ILogger<FileCreator> logger;
    private readonly ITransformSqlFiles transformer;
    private readonly ITop2000AssemblyData top2000Data;

    public FileCreator(ILogger<FileCreator> logger, ITransformSqlFiles transformer, ITop2000AssemblyData top2000Data)
    {
        this.logger = logger;
        this.transformer = transformer;
        this.top2000Data = top2000Data;
    }

    public async Task CreateDataFilesAsync(string location)
    {
        var toUpload = top2000Data
            .GetAllSqlFiles()
            .ToList();

        foreach (var file in toUpload)
        {
            logger.LogInformation("Saving {File} to disk", file);

            var contents = await top2000Data.GetScriptContentAsync(file);
            var path = Path.Combine(location, "sql");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Path.Combine(path, file);

            await File.WriteAllTextAsync(fileName, contents);
        }
    }

    public async Task CreateApiFileAsync(string location)
    {
        var versions = transformer.Transform();

        foreach (var version in versions)
        {
            logger.LogInformation("Saving version {Version} to disk", version.Version);

            var path = Path.Combine(location, "versions", version.Version);
            var json = JsonSerializer.Serialize(version.Upgrades.Select(x => x.FileName), serializerSettings);

            Directory.CreateDirectory(path);

            var fileName = Path.Combine(path, "upgrades");

            await File.WriteAllTextAsync(fileName, json).ConfigureAwait(false);
        }
    }
}