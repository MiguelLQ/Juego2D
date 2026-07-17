using MathKids.Application.Progress;
using MathKids.Domain.Progress;

namespace MathKids.Infrastructure.Persistence;

public sealed class InMemoryProgressRepository : IProgressRepository
{
    private readonly Dictionary<Guid, PlayerProgress> _progress = [];
    private readonly Lock _lock = new();
    public PlayerProgress Get(Guid profileId)
    {
        lock (_lock) return _progress.TryGetValue(profileId, out var progress) ? progress : PlayerProgress.Empty(profileId);
    }
    public void Save(PlayerProgress progress)
    {
        ArgumentNullException.ThrowIfNull(progress);
        lock (_lock) _progress[progress.ProfileId] = progress;
    }
}
