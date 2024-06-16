namespace Top2000.Data.StaticApiGenerator;

public sealed class PublishOnlyApplication : IRunApplication
{
    private readonly IFileCreator fileCreator;

    public PublishOnlyApplication(IFileCreator fileCreator)
    {
        this.fileCreator = fileCreator;
    }

    public async Task RunAsync()
    {
        var location = $"output";

        if (Directory.Exists(location))
        {
            Directory.Delete(location, recursive: true);
        }

        Directory.CreateDirectory(location);

        await Task.WhenAll
        (
            fileCreator.CreateApiFileAsync(location),
            fileCreator.CreateDataFilesAsync(location)
        );
    }
}