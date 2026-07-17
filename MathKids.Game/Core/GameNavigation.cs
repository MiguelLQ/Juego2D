namespace MathKids.Game.Core;

public sealed class GameNavigation
{
    private GameScreen? _pendingScreen;

    public void NavigateTo(GameScreen screen) => _pendingScreen = screen;

    public bool TryConsume(out GameScreen screen)
    {
        if (_pendingScreen is not { } pending)
        {
            screen = default;
            return false;
        }

        screen = pending;
        _pendingScreen = null;
        return true;
    }
}
