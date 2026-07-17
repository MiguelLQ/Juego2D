using MathKids.Domain.Progress;
using MathKids.Application.Abstractions;

namespace MathKids.Application.Progress;

public sealed class ProgressService(IProgressRepository repository, IClock clock) : IProgressService
{
    public PlayerProgress GetProgress(Guid profileId) => repository.Get(profileId);

    public PlayerProgress RecordCorrectAnswer(Guid profileId, int stars, int coins, string game)
    {
        var current = repository.Get(profileId);
        var updated = current with
        {
            CompletedChallenges = current.CompletedChallenges + 1,
            CorrectAnswers = current.CorrectAnswers + 1,
            Stars = current.Stars + Math.Max(0, stars),
            Coins = current.Coins + Math.Max(0, coins),
            UpdatedAtUtc = clock.UtcNow,
            LastGame = game
        };
        repository.Save(updated);
        return updated;
    }

    public PlayerProgress RecordAttempt(Guid profileId, string game)
    {
        var current = repository.Get(profileId);
        var updated = current with
        {
            CompletedChallenges = current.CompletedChallenges + 1,
            UpdatedAtUtc = clock.UtcNow,
            LastGame = game
        };
        repository.Save(updated);
        return updated;
    }
}
