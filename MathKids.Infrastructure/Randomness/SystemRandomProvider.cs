using MathKids.Application.Abstractions;

namespace MathKids.Infrastructure.Randomness;

public sealed class SystemRandomProvider : IRandomProvider { public int Next(int minInclusive, int maxExclusive) => Random.Shared.Next(minInclusive, maxExclusive); }
