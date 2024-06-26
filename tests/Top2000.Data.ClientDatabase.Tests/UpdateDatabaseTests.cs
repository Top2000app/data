﻿namespace Top2000.Data.ClientDatabase.Tests;

[TestClass]
public class UpdateDatabaseTests
{
    private const string DatabaseFileName = "unittest.db";
    private readonly SQLiteAsyncConnection connection;
    private readonly UpdateDatabase sut;
    private readonly Mock<ISource> sourceMock;

    public UpdateDatabaseTests()
    {
        if (File.Exists(DatabaseFileName))
        {
            File.Delete(DatabaseFileName);
        }

        connection = new SQLiteAsyncConnection("unittest.db");
        sut = new UpdateDatabase(connection);
        sourceMock = new Mock<ISource>();
    }

    [TestMethod]
    public async Task AllFileCanBeExcuteInTheDatabaseAsync()
    {
        var top2000Data = new Top2000Data();
        var dataSource = new Top2000AssemblyDataSource(top2000Data);
        await sut.RunAsync(dataSource);

        var totalFiles = top2000Data.GetAllSqlFiles();

        var journals = await connection.QueryAsync<Journal>("SELECT ScriptName FROM Journal");

        journals.Should().HaveCount(totalFiles.Count);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await connection.CloseAsync();

        if (File.Exists(DatabaseFileName))
        {
            File.Delete(DatabaseFileName);
        }
    }

    [TestMethod]
    public async Task ForEveryScriptAJournalIsInserted()
    {
        sourceMock
            .Setup(x => x.ExecutableScriptsAsync(It.IsAny<ImmutableSortedSet<string>>()))
            .ReturnsAsync(Create.ImmutableSortedSetFrom("000-First.sql"));

        sourceMock.Setup(x => x.ScriptContentsAsync("000-First.sql")).ReturnsAsync(new SqlScript("000-First.sql", string.Empty));

        await sut.RunAsync(sourceMock.Object);
        sourceMock.Verify(x => x.ScriptContentsAsync("000-First.sql"), Times.Once);

        var actuals = (await connection.Table<Journal>().ToListAsync())
            .Select(x => x.ScriptName);

        var expected = new[] { "000-First.sql" };

        actuals.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task AllSectionOfTheScriptAreExecuted()
    {
        sourceMock
            .Setup(x => x.ExecutableScriptsAsync(It.IsAny<ImmutableSortedSet<string>>()))
            .ReturnsAsync(Create.ImmutableSortedSetFrom("000-First.sql"));

        var sql = "CREATE TABLE Table1(Id INT NOT NULL, PRIMARY KEY(Id));INSERT INTO Table1(Id) VALUES (1),(2);";
        sourceMock.Setup(x => x.ScriptContentsAsync("000-First.sql")).ReturnsAsync(new SqlScript("000-First.sql", sql));

        await sut.RunAsync(sourceMock.Object);

        var table1 = await connection.QueryAsync<int>("SELECT * FROM Table1");
        table1.Count.Should().Be(2);
    }

    [TestMethod]
    public async Task ForFaultyScriptsJournalIsNotWritten()
    {
        sourceMock
            .Setup(x => x.ExecutableScriptsAsync(It.IsAny<ImmutableSortedSet<string>>()))
            .ReturnsAsync(Create.ImmutableSortedSetFrom("000-First.sql", "001-Second.sql"));

        var sql1 = "CREATE TABLE Table1(Id INT NOT NULL, PRIMARY KEY(Id));INSERT INTO Table1(Id) VALUES (1),(2);";
        var sql2 = "INSERT INTO Table1(Id) VALUES ('2')";
        sourceMock.Setup(x => x.ScriptContentsAsync("000-First.sql")).ReturnsAsync(new SqlScript("000-First.sql", sql1));
        sourceMock.Setup(x => x.ScriptContentsAsync("001-Second.sql")).ReturnsAsync(new SqlScript("001-Second.sql", sql2));

        await sut.Invoking(x => x.RunAsync(sourceMock.Object)).Should().ThrowAsync<Exception>();

        var expected = new[] { "000-First.sql" };
        var actuals = (await connection.Table<Journal>().ToListAsync())
            .Select(x => x.ScriptName);

        actuals.Should().BeEquivalentTo(expected);
    }
}