using MathKids.Domain.Progress;

namespace MathKids.Application.Progress;

public interface IProgressService
{
    PlayerProgress GetProgress(Guid profileId);
    PlayerProgress RecordCorrectAnswer(Guid profileId, int stars, int coins, string game);
    PlayerProgress RecordAttempt(Guid profileId, string game);
}
