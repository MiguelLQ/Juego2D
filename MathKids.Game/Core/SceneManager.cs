using MathKids.Game.Scenes.Abstractions;

namespace MathKids.Game.Core;

public sealed class SceneManager
{
    private readonly Dictionary<GameScreen, IGameScene> _scenes = [];
    public SceneManager(IEnumerable<IGameScene> scenes)
    {
        foreach (var scene in scenes) _scenes.Add(scene.Screen, scene);
    }
    public IGameScene? Current { get; private set; }
    public void ChangeScene(GameScreen screen)
    {
        if (!_scenes.TryGetValue(screen, out var next)) throw new InvalidOperationException($"Screen {screen} is not registered.");
        if (ReferenceEquals(Current, next)) return;
        Current?.Exit();
        Current = next;
        Current.Enter();
    }
}
