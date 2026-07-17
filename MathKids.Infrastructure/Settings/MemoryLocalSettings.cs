using MathKids.Application.Abstractions;

namespace MathKids.Infrastructure.Settings;

public sealed class MemoryLocalSettings : ILocalSettings
{
    private readonly Dictionary<string, string> _values = new(StringComparer.Ordinal);
    public string? Get(string key) => _values.GetValueOrDefault(key);
    public void Set(string key, string value) => _values[key] = value;
}
