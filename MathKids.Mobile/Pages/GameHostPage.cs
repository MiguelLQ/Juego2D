using System.Diagnostics;
using MathKids.Game.Core;
using MathKids.Game.Input.Touch;
using MathKids.Application.Abstractions;
using Microsoft.Maui.Dispatching;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MathKids.Mobile.Pages;

public sealed class GameHostPage : ContentPage
{
    private readonly GameController _gameController;
    private readonly IAudioService _audioService;
    private readonly SKCanvasView _canvasView;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private IDispatcherTimer? _frameTimer;
    public GameHostPage(GameController gameController, IAudioService audioService)
    {
        _gameController = gameController;
        _audioService = audioService;
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = Color.FromArgb("#1E2842");
        _canvasView = new SKCanvasView { EnableTouchEvents = true, IgnorePixelScaling = false, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        _canvasView.PaintSurface += OnPaintSurface;
        _canvasView.Touch += OnTouch;
        Content = _canvasView;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _audioService.InitializeAsync();
        StartGame();
    }
    protected override void OnDisappearing() { PauseGame(); base.OnDisappearing(); }
    public void StartGame()
    {
        _gameController.Start();
        _audioService.ResumeMusic();
        _frameTimer ??= CreateFrameTimer();
        if (!_frameTimer.IsRunning) _frameTimer.Start();
    }
    public void PauseGame()
    {
        if (_frameTimer?.IsRunning == true) _frameTimer.Stop();
        _audioService.PauseMusic();
        _gameController.Pause();
    }
    private IDispatcherTimer CreateFrameTimer()
    {
        var timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16.6667);
        timer.IsRepeating = true;
        timer.Tick += (_, _) => _canvasView.InvalidateSurface();
        return timer;
    }
    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs args) => _gameController.Render(args.Surface.Canvas, args.Info.Width, args.Info.Height, _clock.Elapsed);
    private void OnTouch(object? sender, SKTouchEventArgs args)
    {
        var type = args.ActionType switch
        {
            SKTouchAction.Pressed => GameInputType.Pressed,
            SKTouchAction.Moved => GameInputType.Moved,
            SKTouchAction.Released => GameInputType.Released,
            _ => GameInputType.Cancelled
        };
        _gameController.HandleInput(args.Id, type, args.Location.X, args.Location.Y);
        _canvasView.InvalidateSurface();
        args.Handled = true;
    }
}
