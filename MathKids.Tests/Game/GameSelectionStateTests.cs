using MathKids.Domain.Exercises;
using MathKids.Game.Core;

namespace MathKids.Tests.Game;

public sealed class GameSelectionStateTests
{
    [Theory]
    [InlineData(GameModule.MathAdventure, GameScreen.Addition)]
    [InlineData(GameModule.CondorBingo, GameScreen.AdditionBingo)]
    [InlineData(GameModule.PumaMath, GameScreen.PumaAddition)]
    [InlineData(GameModule.ChankaLaboratory, GameScreen.ChancaLaboratory)]
    public void SelectModule_MapsToItsGame(GameModule module, GameScreen screen)
    {
        var selection = new GameSelectionState();
        selection.SelectModule(module);
        Assert.Equal(screen, selection.TargetScreen);
    }

    [Theory]
    [InlineData(MathOperation.Addition)]
    [InlineData(MathOperation.Subtraction)]
    [InlineData(MathOperation.Multiplication)]
    [InlineData(MathOperation.Division)]
    public void SelectOperation_KeepsChosenOperation(MathOperation operation)
    {
        var selection = new GameSelectionState();
        selection.SelectOperation(operation);
        Assert.Equal(operation, selection.Operation);
    }
}
