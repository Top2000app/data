var connectionString = @"Server=localhost;Database=Top2000;User Id=sa;Password=aT$0&0zYF5!Q*55N;Trust Server Certificate=True;";

EnsureDatabase.For
    .SqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptEmbeddedInDataLibrary()
    .WithTransactionPerScript()
    .LogToConsole()
    .Build() ?? throw new InvalidOperationException($"upgrader is null");

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error.ToString());
    Console.ResetColor();
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!!");
Console.ResetColor();

return 0;