namespace MathKids.Game.Core;

public sealed class GameLoop(GameConfiguration configuration)
{
    private TimeSpan? _lastTimestamp;
    private TimeSpan _total;

    public GameTime Advance(TimeSpan timestamp)
    {
        var elapsed = _lastTimestamp is null ? TimeSpan.Zero : timestamp - _lastTimestamp.Value;
        _lastTimestamp = timestamp;
        if (elapsed < TimeSpan.Zero) elapsed = TimeSpan.Zero;
        var maximum = TimeSpan.FromSeconds(configuration.MaximumDeltaSeconds);
        if (elapsed > maximum) elapsed = maximum;
        _total += elapsed;
        return new GameTime(_total, elapsed);
    }

    public void ResetTimestamp() => _lastTimestamp = null;
}
