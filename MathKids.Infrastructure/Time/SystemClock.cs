using MathKids.Application.Abstractions;

namespace MathKids.Infrastructure.Time;

public sealed class SystemClock : IClock { public DateTimeOffset UtcNow => DateTimeOffset.UtcNow; }
