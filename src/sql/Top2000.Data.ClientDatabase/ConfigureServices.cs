using Microsoft.Extensions.DependencyInjection;

namespace Top2000.Data.ClientDatabase;

public class Top2000Builder
{
    public string? Directory { get; private set; }
    public string? Name {get; private set; }
    public bool OnlineUpdatesEnabled { get; private set; }
    public Uri? UpdateUri { get; private set; }

    /// <summary>
    /// Specify the directory in which to create the SQLite database
    /// </summary>
    public Top2000Builder DatabaseDirectory(string directory)
    {
        Directory = directory;
        return this;
    }

    /// <summary>
    /// Specify the name for the SQLite database
    /// </summary>
    /// <param name="name">Name of the database</param>
    public Top2000Builder DatabaseName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    /// Enables the online update feature
    /// </summary>
    /// <returns></returns>
    public Top2000Builder EnableOnlineUpdates()
    {
        OnlineUpdatesEnabled = true;
        return this;
    }

    /// <summary>
    /// Enabled the online update feature with a custom Uri
    /// </summary>
    /// <param name="updateUri">Custom Uri</param>
    public Top2000Builder EnableOnlineUpdates(Uri updateUri)
    {
        UpdateUri = updateUri;
        OnlineUpdatesEnabled = true;
        return this;
    }
}

public static class ConfigureServices
{
    public static IServiceCollection AddTop2000(this IServiceCollection services, Action<Top2000Builder>? configure = null)
    {
        var builder = new Top2000Builder();
       
        configure?.Invoke(builder);

        services
            .AddTransient<Top2000AssemblyDataSource>()
            .AddTransient<IUpdateClientDatabase, UpdateDatabase>()
            .AddTransient<ITop2000AssemblyData, Top2000Data>()
            .AddSingleton(builder);
        
        services.AddTransient<SQLiteAsyncConnection>(provider =>
        {
            var top2000Builder = provider.GetRequiredService<Top2000Builder>();
            var databasePath = Path.Combine(top2000Builder.Directory ?? "", top2000Builder.Name ?? "top2000.db");
            
            return new SQLiteAsyncConnection(databasePath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache,
                storeDateTimeAsTicks: false);
        });

        if (builder.OnlineUpdatesEnabled)
        {
            services
                .AddTransient<OnlineDataSource>()
                .AddHttpClient("top2000", client =>
                {
                    client.BaseAddress = builder.UpdateUri ?? new Uri("https://data.top2000.app/");
                });
        }

        return services;
    }
    
    [Obsolete("Method is deprecated, please use AddTop2000ClientDatabase instead.")]
    public static IServiceCollection AddClientDatabase(this IServiceCollection services, DirectoryInfo appDataDirectory, string clientDatabaseName = "Top2000v2.db")
    {
        services.AddHttpClient("top2000", c =>
        {
            c.BaseAddress = new Uri("https://data.top2000.app/)");
        });

        return services
            .AddTransient<Top2000AssemblyDataSource>()
            .AddTransient<IUpdateClientDatabase, UpdateDatabase>()
            .AddTransient<ITop2000AssemblyData, Top2000Data>()
            .AddTransient<SQLiteAsyncConnection>(f =>
            {
                var databasePath = Path.Combine(appDataDirectory.FullName, clientDatabaseName);

                return new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, storeDateTimeAsTicks: false);
            });
    }

    [Obsolete("Method is deprecated, please use AddTop2000ClientDatabase instead.")]
    public static IServiceCollection AddOfflineClientDatabase(this IServiceCollection services, DirectoryInfo appDataDirectory, string clientDatabaseName = "Top2000v2.db")
    {
        return services
            .AddTransient<Top2000AssemblyDataSource>()
            .AddTransient<IUpdateClientDatabase, UpdateDatabase>()
            .AddTransient<ITop2000AssemblyData, Top2000Data>()
            .AddTransient<SQLiteAsyncConnection>(f =>
            {
                var databasePath = Path.Combine(appDataDirectory.FullName, clientDatabaseName);

                return new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, storeDateTimeAsTicks: false);
            });
    }
}
