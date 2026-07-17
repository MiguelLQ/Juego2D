using MathKids.Application.Abstractions;
using Microsoft.Maui.Storage;

namespace MathKids.Mobile.Settings;

public sealed class MauiLocalSettings : ILocalSettings
{
    public string? Get(string key) => Preferences.Default.ContainsKey(key) ? Preferences.Default.Get(key, string.Empty) : null;
    public void Set(string key, string value) => Preferences.Default.Set(key, value);
}
