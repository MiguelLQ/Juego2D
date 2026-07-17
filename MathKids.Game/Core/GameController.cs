using MathKids.Game.Common;
using MathKids.Game.Input.Touch;
using SkiaSharp;

namespace MathKids.Game.Core;

public sealed class GameController(SceneManager sceneManager, GameLoop gameLoop, GameViewport viewport, GameNavigation navigation)
{
    private bool _initialized;
    private bool _running;
    public void Start()
    {
        if (!_initialized) { sceneManager.ChangeScene(GameScreen.Home); _initialized = true; }
        gameLoop.ResetTimestamp(); _running = true;
    }
    public void Pause() { _running = false; gameLoop.ResetTimestamp(); }
    public void Render(SKCanvas canvas, int width, int height, TimeSpan timestamp)
    {
        viewport.Resize(width, height);
        canvas.Clear(new SKColor(30, 40, 66));
        var scene = sceneManager.Current;
        if (scene is null) return;
        if (_running) scene.Update(gameLoop.Advance(timestamp));
        if (navigation.TryConsume(out var nextScreen))
        {
            sceneManager.ChangeScene(nextScreen);
            scene = sceneManager.Current;
            if (scene is null) return;
        }
        canvas.Save(); canvas.Translate(viewport.OffsetX, viewport.OffsetY); canvas.Scale(viewport.Scale); scene.Draw(canvas, viewport); canvas.Restore();
    }
    public void HandleInput(long pointerId, GameInputType type, float physicalX, float physicalY)
    {
        var physical = new GamePoint(physicalX, physicalY);
        if (!viewport.ContainsPhysical(physical) && type == GameInputType.Pressed) return;
        sceneManager.Current?.HandleInput(new(pointerId, type, viewport.PhysicalToLogical(physical)));
    }
}
