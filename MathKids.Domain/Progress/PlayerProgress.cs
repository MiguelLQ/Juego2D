namespace MathKids.Domain.Progress;

public sealed record PlayerProgress(
    Guid ProfileId,
    int CompletedChallenges,
    int CorrectAnswers,
    int Stars,
    int Coins,
    DateTimeOffset UpdatedAtUtc,
    string? LastGame)
{
    public static PlayerProgress Empty(Guid profileId) => new(profileId, 0, 0, 0, 25, DateTimeOffset.UnixEpoch, null);
}
