namespace MathKids.Application.Abstractions;

public interface ILocalSettings
{
    string? Get(string key);
    void Set(string key, string value);
}
