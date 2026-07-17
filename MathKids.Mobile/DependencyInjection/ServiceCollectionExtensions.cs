using MathKids.Mobile.Pages;
using MathKids.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;

namespace MathKids.Mobile.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMathKidsMobile(this IServiceCollection services)
    {
        services.AddSingleton(new SqliteDatabaseOptions(Path.Combine(FileSystem.AppDataDirectory, "mathkids.db3")));
        services.AddSingleton<GameHostPage>();
        return services;
    }
}
