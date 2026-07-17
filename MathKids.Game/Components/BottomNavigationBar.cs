using MathKids.Application.Abstractions;
using MathKids.Game.Common;
using MathKids.Game.Core;
using MathKids.Game.Input.HitTesting;
using MathKids.Game.Input.Touch;
using SkiaSharp;

namespace MathKids.Game.Components;

public sealed class BottomNavigationBar(GameNavigation navigation, GameScreen activeScreen, IAudioService audioService) : IDisposable
{
    private readonly SKPaint _fill = new() { IsAntialias = true };
    private readonly SKPaint _stroke = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 10f, StrokeCap = SKStrokeCap.Round, StrokeJoin = SKStrokeJoin.Round };
    private readonly SKPaint _text = new() { IsAntialias = true, TextAlign = SKTextAlign.Center, Typeface = SKTypeface.FromFamilyName("sans-serif", SKFontStyle.Bold), TextSize = 36f };
    private readonly SKPath _iconPath = new();
    private static readonly GameRectangle HomeBounds = new(65f, 1690f, 275f, 210f);
    private static readonly GameRectangle GamesBounds = new(402f, 1690f, 275f, 210f);
    private static readonly GameRectangle ProgressBounds = new(740f, 1690f, 275f, 210f);

    public void Draw(SKCanvas canvas)
    {
        _fill.Color = new SKColor(255, 251, 239);
        canvas.DrawRoundRect(new SKRect(20f, 1660f, 1060f, 1950f), 75f, 75f, _fill);
        DrawItem(canvas, HomeBounds, GameScreen.Home, "Inicio");
        DrawItem(canvas, GamesBounds, GameScreen.Games, "Juegos");
        DrawItem(canvas, ProgressBounds, GameScreen.Progress, "Progreso");
    }

    public void HandleInput(GameInput input)
    {
        if (input.Type != GameInputType.Released) return;
        if (HitTest.Contains(HomeBounds, input.Position)) Navigate(GameScreen.Home);
        else if (HitTest.Contains(GamesBounds, input.Position)) Navigate(GameScreen.Games);
        else if (HitTest.Contains(ProgressBounds, input.Position)) Navigate(GameScreen.Progress);
    }

    private void Navigate(GameScreen screen) { audioService.PlayEffect(AudioCue.Tap); navigation.NavigateTo(screen); }

    private void DrawItem(SKCanvas canvas, GameRectangle bounds, GameScreen screen, string label)
    {
        var isActive = activeScreen == screen;
        if (isActive)
        {
            _fill.Color = new SKColor(235, 225, 255);
            canvas.DrawRoundRect(new SKRect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom), 45f, 45f, _fill);
        }
        var color = isActive ? new SKColor(107, 70, 204) : new SKColor(30, 71, 126);
        _fill.Color = color; _stroke.Color = color; _text.Color = color;
        var centerX = bounds.CenterX;
        if (screen == GameScreen.Home) DrawHome(canvas, centerX, bounds.Top + 70f);
        else if (screen == GameScreen.Games) DrawGamepad(canvas, centerX, bounds.Top + 70f);
        else DrawBars(canvas, centerX, bounds.Top + 72f);
        canvas.DrawText(label, centerX, bounds.Bottom - 22f, _text);
    }

    private void DrawHome(SKCanvas canvas, float x, float y)
    {
        _iconPath.Reset();
        _iconPath.MoveTo(x - 62f, y); _iconPath.LineTo(x, y - 55f); _iconPath.LineTo(x + 62f, y); _iconPath.LineTo(x + 48f, y); _iconPath.LineTo(x + 48f, y + 58f); _iconPath.LineTo(x - 48f, y + 58f); _iconPath.LineTo(x - 48f, y); _iconPath.Close();
        canvas.DrawPath(_iconPath, _fill);
    }

    private void DrawGamepad(SKCanvas canvas, float x, float y)
    {
        canvas.DrawRoundRect(new SKRect(x - 76f, y - 40f, x + 76f, y + 60f), 40f, 40f, _fill);
        _stroke.Color = SKColors.White; _stroke.StrokeWidth = 9f;
        canvas.DrawLine(x - 43f, y + 8f, x - 5f, y + 8f, _stroke); canvas.DrawLine(x - 24f, y - 11f, x - 24f, y + 27f, _stroke);
        canvas.DrawCircle(x + 36f, y - 1f, 8f, _stroke); canvas.DrawCircle(x + 58f, y + 20f, 8f, _stroke);
    }

    private void DrawBars(SKCanvas canvas, float x, float y)
    {
        canvas.DrawRoundRect(new SKRect(x - 68f, y + 5f, x - 30f, y + 58f), 9f, 9f, _fill);
        canvas.DrawRoundRect(new SKRect(x - 18f, y - 28f, x + 20f, y + 58f), 9f, 9f, _fill);
        canvas.DrawRoundRect(new SKRect(x + 32f, y - 62f, x + 70f, y + 58f), 9f, 9f, _fill);
    }

    public void Dispose() { _fill.Dispose(); _stroke.Dispose(); _text.Dispose(); _iconPath.Dispose(); }
}
