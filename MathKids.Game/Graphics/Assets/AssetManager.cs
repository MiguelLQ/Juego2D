using SkiaSharp;

namespace MathKids.Game.Graphics.Assets;

public sealed class AssetManager : IDisposable
{
    private readonly Dictionary<string, SKBitmap> _bitmaps = new(StringComparer.OrdinalIgnoreCase);
    public void Add(string key, Stream stream)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(stream);
        var bitmap = SKBitmap.Decode(stream) ?? throw new InvalidDataException($"Unable to decode asset '{key}'.");
        if (_bitmaps.Remove(key, out var previous)) previous.Dispose();
        _bitmaps.Add(key, bitmap);
    }
    public SKBitmap Get(string key) => _bitmaps.TryGetValue(key, out var bitmap) ? bitmap : throw new KeyNotFoundException($"Asset '{key}' is not loaded.");
    public void Dispose() { foreach (var bitmap in _bitmaps.Values) bitmap.Dispose(); _bitmaps.Clear(); }
}
