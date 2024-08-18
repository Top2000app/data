var connectionString = @"Server=(localdb)\mssqllocaldb;Database=Top2000;";

EnsureDatabase.For
    .SqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptEmbeddedInDataLibrary()
    .WithTransactionPerScript()
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

var originForegroundColor = Console.BackgroundColor;

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error.ToString());
    Console.ForegroundColor = originForegroundColor;
    return -1;
}

Console.BackgroundColor = ConsoleColor.Green;
Console.WriteLine("Success!!");
Console.ForegroundColor = originForegroundColor;

return 0;