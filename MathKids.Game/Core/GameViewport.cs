using MathKids.Game.Common;

namespace MathKids.Game.Core;

public sealed class GameViewport
{
    public GameViewport(float logicalWidth, float logicalHeight) { LogicalWidth = logicalWidth; LogicalHeight = logicalHeight; }
    public float LogicalWidth { get; }
    public float LogicalHeight { get; }
    public float PhysicalWidth { get; private set; }
    public float PhysicalHeight { get; private set; }
    public float Scale { get; private set; } = 1f;
    public float OffsetX { get; private set; }
    public float OffsetY { get; private set; }
    public float VisibleLogicalLeft => -OffsetX / Scale;
    public float VisibleLogicalTop => -OffsetY / Scale;
    public float VisibleLogicalRight => (PhysicalWidth - OffsetX) / Scale;
    public float VisibleLogicalBottom => (PhysicalHeight - OffsetY) / Scale;

    public void Resize(float physicalWidth, float physicalHeight)
    {
        PhysicalWidth = Math.Max(0f, physicalWidth);
        PhysicalHeight = Math.Max(0f, physicalHeight);
        if (PhysicalWidth == 0f || PhysicalHeight == 0f) return;
        Scale = Math.Min(PhysicalWidth / LogicalWidth, PhysicalHeight / LogicalHeight);
        OffsetX = (PhysicalWidth - LogicalWidth * Scale) / 2f;
        OffsetY = (PhysicalHeight - LogicalHeight * Scale) / 2f;
    }

    public GamePoint PhysicalToLogical(GamePoint point) => new((point.X - OffsetX) / Scale, (point.Y - OffsetY) / Scale);
    public GamePoint LogicalToPhysical(GamePoint point) => new(point.X * Scale + OffsetX, point.Y * Scale + OffsetY);
    public bool ContainsPhysical(GamePoint point) => point.X >= OffsetX && point.X <= OffsetX + LogicalWidth * Scale && point.Y >= OffsetY && point.Y <= OffsetY + LogicalHeight * Scale;
}
