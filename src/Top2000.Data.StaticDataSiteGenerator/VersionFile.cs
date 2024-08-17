namespace Top2000.Data.StaticApiGenerator;

[DebuggerDisplay("{FileName}")]
public sealed class VersionFile
{
    private readonly List<VersionFile> upgrades;

    public VersionFile(string fileName)
    {
        this.Version = fileName.Split('-')[0];
        this.FileName = fileName;
        this.upgrades = [];
    }

    public string Version { get; set; }

    public string FileName { get; set; }

    public IReadOnlyCollection<VersionFile> Upgrades => upgrades;

    public void AddRange(IEnumerable<VersionFile> versionFiles)
    {
        upgrades.AddRange(versionFiles);
    }
}