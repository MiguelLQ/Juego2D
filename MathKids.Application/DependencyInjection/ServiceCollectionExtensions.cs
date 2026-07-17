using MathKids.Application.Exercises;
using MathKids.Application.Progress;
using MathKids.Application.Rewards;
using MathKids.Application.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace MathKids.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMathKidsApplication(this IServiceCollection services)
    {
        services.AddSingleton<IExerciseGenerator, AdditionExerciseGenerator>();
        services.AddSingleton<IGameSessionService, GameSessionService>();
        services.AddSingleton<IProgressService, ProgressService>();
        services.AddSingleton<IRewardService, RewardService>();
        return services;
    }
}
