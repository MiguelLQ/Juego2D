namespace MathKids.Domain.Exercises;

public sealed record MathExercise
{
    public MathExercise(Guid id, MathOperation operation, DifficultyLevel difficulty, int leftOperand, int rightOperand, int correctAnswer, IReadOnlyList<AnswerOption> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.Count < 2 || options.Count(option => option.IsCorrect) != 1 || options.All(option => option.Value != correctAnswer))
            throw new ArgumentException("An exercise requires exactly one correct option.", nameof(options));
        if (options.Select(option => option.Value).Distinct().Count() != options.Count)
            throw new ArgumentException("Answer options must be unique.", nameof(options));

        Id = id;
        Operation = operation;
        Difficulty = difficulty;
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        CorrectAnswer = correctAnswer;
        Options = options;
    }

    public Guid Id { get; }
    public MathOperation Operation { get; }
    public DifficultyLevel Difficulty { get; }
    public int LeftOperand { get; }
    public int RightOperand { get; }
    public int CorrectAnswer { get; }
    public IReadOnlyList<AnswerOption> Options { get; }
    public bool IsCorrect(int answer) => answer == CorrectAnswer;
}
