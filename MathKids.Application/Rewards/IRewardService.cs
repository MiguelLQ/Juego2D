using MathKids.Domain.Rewards;

namespace MathKids.Application.Rewards;

public interface IRewardService { RewardGrant Calculate(bool isCorrect, int consecutiveCorrectAnswers); }
