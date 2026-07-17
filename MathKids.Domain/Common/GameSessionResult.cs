using MathKids.Domain.Exercises;
using MathKids.Domain.Rewards;

namespace MathKids.Domain.Common;

public sealed record GameSessionResult(int TotalExercises, int CorrectAnswers, TimeSpan Duration, RewardGrant Reward, IReadOnlyList<ExerciseResult> Results);
