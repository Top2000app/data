﻿using Microsoft.Extensions.DependencyInjection;

namespace Top2000.Data.ClientDatabase;

public static class ConfigureServices
{
    
    public static IServiceCollection AddClientDatabase(this IServiceCollection services, DirectoryInfo appDataDirectory, string clientDatabaseName = "Top2000v2.db")
    {
        services.AddHttpClient("top2000", c =>
        {
            c.BaseAddress = new Uri("https://data.top2000.app/)");
        });

        return services
            .AddTransient<OnlineDataSource>()
            .AddTransient<Top2000AssemblyDataSource>()
            .AddTransient<IUpdateClientDatabase, UpdateDatabase>()
            .AddTransient<ITop2000AssemblyData, Top2000Data>()
            .AddTransient<SQLiteAsyncConnection>(f =>
            {
                var databasePath = Path.Combine(appDataDirectory.FullName, clientDatabaseName);

                return new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache, storeDateTimeAsTicks: false);
            });
    }

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
