using FluentAssertions;
using Top2000.Data;
namespace Top2000.Data.Tests;

[TestClass]
public class Top2000DataTests
{
    private readonly Top2000Data sut = new();

    [TestMethod]
    public void DataAssemblyIsTheAssemblyOfTheTop2000Data()
    {
        sut.DataAssembly.Should().BeSameAs(typeof(Top2000Data).Assembly);
    }

    [TestMethod]
    public async Task AllSqlFilesCanBeRead()
    {
        var filesAsTask = sut.GetAllSqlFiles()
            .Select(GetNameContent);

        var files = await Task.WhenAll(filesAsTask);

        foreach (var (name, content) in files)
        {
            content.Should().NotBeNullOrWhiteSpace($"the file '{name}' does not have content");
        }
    }

    [TestMethod]
    public void AllSqlFileCanBeStreamed()
    {
        var filesAsStream = sut.GetAllSqlFiles()
            .Select(sut.GetScriptStream)
            .ToList();

        foreach (var item in filesAsStream)
        {
            item.Should().NotBeNull();
            item.Dispose();
        }
    }

    [TestMethod]
    public void FileNamesDoNotContainSpaces()
    {
        var fileNames = sut.GetAllSqlFiles()
            .ToList();

        foreach (var fileName in fileNames)
        {
            fileName.Should().NotContain(" ", $"the file {fileName} cannot contain a space");
        }
    }

    private async Task<(string name, string content)> GetNameContent(string fileName)
    {
        var content = await sut.GetScriptContentAsync(fileName);
        return (fileName, content);
    }
}