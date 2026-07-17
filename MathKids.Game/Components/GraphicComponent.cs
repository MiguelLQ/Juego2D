using MathKids.Game.Common;
using MathKids.Game.Core;
using SkiaSharp;

namespace MathKids.Game.Components;

public abstract class GraphicComponent : IDisposable
{
    protected GraphicComponent(GameRectangle bounds) => Bounds = bounds;
    public GameRectangle Bounds { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SKCanvas canvas);
    public abstract void Dispose();
}
