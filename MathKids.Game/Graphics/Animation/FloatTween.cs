namespace MathKids.Game.Graphics.Animation;

public sealed class FloatTween
{
    private float _start;
    private float _end;
    private float _duration;
    private float _elapsed;
    public float Value { get; private set; } = 1f;
    public bool IsRunning { get; private set; }
    public void Start(float from, float to, float duration)
    {
        _start = from; _end = to; _duration = Math.Max(0.001f, duration); _elapsed = 0f; Value = from; IsRunning = true;
    }
    public void Update(float deltaSeconds)
    {
        if (!IsRunning) return;
        _elapsed = Math.Min(_duration, _elapsed + deltaSeconds);
        var progress = _elapsed / _duration;
        var eased = 1f - (1f - progress) * (1f - progress);
        Value = _start + (_end - _start) * eased;
        IsRunning = _elapsed < _duration;
    }
}
