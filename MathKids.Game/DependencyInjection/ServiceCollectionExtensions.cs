using MathKids.Game.Core;
using MathKids.Game.Graphics.Assets;
using MathKids.Game.Scenes.Abstractions;
using MathKids.Game.Scenes.Addition;
using MathKids.Game.Scenes.Home;
using MathKids.Game.Scenes.Results;
using Microsoft.Extensions.DependencyInjection;

namespace MathKids.Game.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMathKidsGame(this IServiceCollection services)
    {
        services.AddSingleton(GameConfiguration.Default);
        services.AddSingleton(provider => { var configuration = provider.GetRequiredService<GameConfiguration>(); return new GameViewport(configuration.LogicalWidth, configuration.LogicalHeight); });
        services.AddSingleton<GameLoop>();
        services.AddSingleton<GameNavigation>();
        services.AddSingleton<PlayerGameState>();
        services.AddSingleton<AssetManager>();
        services.AddSingleton<HomeScene>();
        services.AddSingleton<GamesMenuScene>();
        services.AddSingleton<AdditionDemoScene>();
        services.AddSingleton<AdditionBingoScene>();
        services.AddSingleton<ProgressScene>();
        services.AddSingleton<IGameScene>(provider => provider.GetRequiredService<HomeScene>());
        services.AddSingleton<IGameScene>(provider => provider.GetRequiredService<GamesMenuScene>());
        services.AddSingleton<IGameScene>(provider => provider.GetRequiredService<AdditionDemoScene>());
        services.AddSingleton<IGameScene>(provider => provider.GetRequiredService<AdditionBingoScene>());
        services.AddSingleton<IGameScene>(provider => provider.GetRequiredService<ProgressScene>());
        services.AddSingleton<SceneManager>();
        services.AddSingleton<GameController>();
        return services;
    }
}
