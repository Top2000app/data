using Microsoft.Extensions.DependencyInjection;

namespace Top2000.Data.Csv;

public static class ConfigureServices
{
    public static IServiceCollection AddTop2000(this IServiceCollection services,
        Action<Top2000ServiceBuilder>? configure = null)
    {
        return services;
    }
}
