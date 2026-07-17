using MathKids.Application.Abstractions;
using MathKids.Application.Progress;
using MathKids.Infrastructure.Audio;
using MathKids.Infrastructure.Persistence;
using MathKids.Infrastructure.Randomness;
using MathKids.Infrastructure.Settings;
using MathKids.Infrastructure.Time;
using Microsoft.Extensions.DependencyInjection;

namespace MathKids.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMathKidsInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IRandomProvider, SystemRandomProvider>();
        services.AddSingleton<ILocalSettings, MemoryLocalSettings>();
        services.AddSingleton<IAudioService, NullAudioService>();
        services.AddSingleton<IProgressRepository, SqliteProgressRepository>();
        return services;
    }
}
