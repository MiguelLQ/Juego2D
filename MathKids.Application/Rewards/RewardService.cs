using MathKids.Domain.Rewards;

namespace MathKids.Application.Rewards;

public sealed class RewardService : IRewardService
{
    public RewardGrant Calculate(bool isCorrect, int consecutiveCorrectAnswers) => isCorrect
        ? new RewardGrant(1, 5 + Math.Clamp(consecutiveCorrectAnswers / 3, 0, 3))
        : new RewardGrant(0, 0);
}
