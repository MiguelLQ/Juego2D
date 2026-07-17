using MathKids.Mobile.Pages;

namespace MathKids.Mobile;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly GameHostPage _gameHostPage;

    public App(GameHostPage gameHostPage)
    {
        InitializeComponent();
        _gameHostPage = gameHostPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(_gameHostPage);
        window.Stopped += (_, _) => _gameHostPage.PauseGame();
        window.Resumed += (_, _) => _gameHostPage.StartGame();
        return window;
    }
}
