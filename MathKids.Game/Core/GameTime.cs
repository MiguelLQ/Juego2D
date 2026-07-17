namespace MathKids.Game.Core;

public readonly record struct GameTime(TimeSpan Total, TimeSpan Elapsed)
{
    public float DeltaSeconds => (float)Elapsed.TotalSeconds;
}
