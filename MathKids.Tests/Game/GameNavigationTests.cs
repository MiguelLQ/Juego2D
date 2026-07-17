using MathKids.Game.Core;

namespace MathKids.Tests.Game;

public sealed class GameNavigationTests
{
    [Fact]
    public void NavigateTo_QueuesAndConsumesDestination()
    {
        var navigation = new GameNavigation();

        navigation.NavigateTo(GameScreen.AdditionBingo);

        Assert.True(navigation.TryConsume(out var destination));
        Assert.Equal(GameScreen.AdditionBingo, destination);
        Assert.False(navigation.TryConsume(out _));
    }
}
