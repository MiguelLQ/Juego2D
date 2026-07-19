using MathKids.Domain.Exercises;

namespace MathKids.Game.Core;

public sealed class GameSelectionState
{
    public GameModule Module { get; private set; } = GameModule.MathAdventure;
    public MathOperation Operation { get; private set; } = MathOperation.Addition;

    public void SelectModule(GameModule module) => Module = module;
    public void SelectOperation(MathOperation operation) => Operation = operation;

    public GameScreen TargetScreen => Module switch
    {
        GameModule.MathAdventure => GameScreen.Addition,
        GameModule.CondorBingo => GameScreen.AdditionBingo,
        GameModule.PumaMath => GameScreen.PumaAddition,
        GameModule.ChankaLaboratory => GameScreen.ChancaLaboratory,
        _ => throw new ArgumentOutOfRangeException()
    };

    public string ModuleTitle => Module switch
    {
        GameModule.MathAdventure => "Aventura Matem\u00E1tica",
        GameModule.CondorBingo => "Bingo del C\u00F3ndor",
        GameModule.PumaMath => "Matem\u00E1ticas con el Puma",
        GameModule.ChankaLaboratory => "Laboratorio Chanka",
        _ => string.Empty
    };
}

public enum GameModule { MathAdventure, CondorBingo, PumaMath, ChankaLaboratory }

public static class MathOperationPresentation
{
    public static string Symbol(this MathOperation operation) => operation switch
    {
        MathOperation.Addition => "+",
        MathOperation.Subtraction => "\u2212",
        MathOperation.Multiplication => "\u00D7",
        MathOperation.Division => "\u00F7",
        _ => "?"
    };

    public static string DisplayName(this MathOperation operation) => operation switch
    {
        MathOperation.Addition => "Suma",
        MathOperation.Subtraction => "Resta",
        MathOperation.Multiplication => "Multiplicaci\u00F3n",
        MathOperation.Division => "Divisi\u00F3n",
        _ => string.Empty
    };
}
