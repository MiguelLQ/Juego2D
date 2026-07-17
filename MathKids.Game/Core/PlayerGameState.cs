using MathKids.Application.Progress;
using MathKids.Domain.Progress;

namespace MathKids.Game.Core;

public sealed class PlayerGameState(IProgressService progressService)
{
    public static readonly Guid DefaultProfileId = new("a834f61d-46dd-4bc8-9f31-d5706069b20a");
    private PlayerProgress _progress = progressService.GetProgress(DefaultProfileId);

    public int Stars => _progress.Stars;
    public int Coins => _progress.Coins;
    public int CorrectAnswers => _progress.CorrectAnswers;
    public int CompletedChallenges => _progress.CompletedChallenges;
    public DateTimeOffset UpdatedAtUtc => _progress.UpdatedAtUtc;
    public string? LastGame => _progress.LastGame;

    public void RecordCorrectAnswer(string game, int stars = 1, int coins = 5)
    {
        _progress = progressService.RecordCorrectAnswer(DefaultProfileId, stars, coins, game);
    }

    public void RecordAttempt(string game) => _progress = progressService.RecordAttempt(DefaultProfileId, game);

    public void Reload() => _progress = progressService.GetProgress(DefaultProfileId);
}
