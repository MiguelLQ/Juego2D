namespace MathKids.Game.Core;

public sealed record GameConfiguration(float LogicalWidth, float LogicalHeight, double MaximumDeltaSeconds)
{
    public static GameConfiguration Default { get; } = new(1080f, 1920f, 0.05d);
}
