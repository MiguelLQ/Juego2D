using MathKids.Game.Common;
using MathKids.Game.Components;
using MathKids.Game.Core;
using MathKids.Game.Graphics.Drawing;
using MathKids.Game.Input.Touch;
using MathKids.Game.Scenes.Abstractions;
using SkiaSharp;

namespace MathKids.Game.Scenes.Home;

public sealed class HomeScene : KidsSceneBase
{
    private static readonly GameRectangle AdditionBounds = new(90f, 1010f, 420f, 250f);
    private static readonly GameRectangle BingoBounds = new(570f, 1010f, 420f, 250f);
    private static readonly GameRectangle PumaBounds = new(90f, 1300f, 420f, 230f);
    private static readonly GameRectangle ChancaBounds = new(570f, 1300f, 420f, 230f);
    private readonly GameNavigation _navigation;
    private readonly PlayerGameState _state;
    private readonly BottomNavigationBar _navigationBar;
    private readonly AndeanDashboardBackdrop _backdrop = new();
    private float _elapsed;

    public HomeScene(GameNavigation navigation, PlayerGameState state)
    {
        _navigation = navigation;
        _state = state;
        _navigationBar = new BottomNavigationBar(navigation, GameScreen.Home);
    }

    public override GameScreen Screen => GameScreen.Home;
    public override void Enter() { }
    public override void Exit() { }
    public override void Update(GameTime gameTime) { _elapsed += gameTime.DeltaSeconds; _backdrop.Update(gameTime); }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        _backdrop.Draw(canvas, viewport);
        DrawBrandHeader(canvas, 180f, 0.96f);
        DrawCoinBadge(canvas, _state.Coins);
        DrawAudioButton(canvas);

        var bob = MathF.Sin(_elapsed * 2.4f) * 12f;
        DrawFoxMascot(canvas, -75f, bob);
        FillPaint.Color = new SKColor(255, 255, 255, 242);
        canvas.DrawRoundRect(new SKRect(500f, 315f + bob, 1000f, 560f + bob), 65f, 65f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(485f, 300f + bob, 985f, 545f + bob), 65f, 65f, FillPaint);
        TextPaint.TextSize = 52f; TextPaint.Color = new SKColor(30, 71, 126);
        canvas.DrawText("\u00A1Hola, peque\u00F1o genio!", 735f, 395f + bob, TextPaint);
        TextPaint.TextSize = 37f; TextPaint.Color = new SKColor(74, 101, 135);
        canvas.DrawText("Elige una aventura y gana estrellas", 735f, 465f + bob, TextPaint);

        FillPaint.Color = new SKColor(255, 250, 235, 248);
        canvas.DrawRoundRect(new SKRect(55f, 860f, 1025f, 1615f), 76f, 76f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(45f, 845f, 1015f, 1600f), 76f, 76f, FillPaint);
        FillPaint.Color = new SKColor(116, 75, 213);
        canvas.DrawRoundRect(new SKRect(230f, 790f, 850f, 910f), 40f, 40f, FillPaint);
        TextPaint.TextSize = 44f; TextPaint.Color = SKColors.White;
        canvas.DrawText("\u2605  Elige un juego  \u2605", 540f, 866f, TextPaint);

        DrawGameCard(canvas, AdditionBounds, new SKColor(74, 167, 227), "Aventura", "de sumas", 0);
        DrawGameCard(canvas, BingoBounds, new SKColor(91, 178, 87), "Bingo", "del C\u00F3ndor", 1);
        DrawPumaCard(canvas);
        DrawChancaCard(canvas);
        _navigationBar.Draw(canvas);
    }

    public override void HandleInput(GameInput input)
    {
        if (IsReleasedInside(input, AdditionBounds)) _navigation.NavigateTo(GameScreen.Addition);
        else if (IsReleasedInside(input, BingoBounds)) _navigation.NavigateTo(GameScreen.AdditionBingo);
        else if (IsReleasedInside(input, PumaBounds)) _navigation.NavigateTo(GameScreen.PumaAddition);
        else if (IsReleasedInside(input, ChancaBounds)) _navigation.NavigateTo(GameScreen.ChancaLaboratory);
        else _navigationBar.HandleInput(input);
    }

    private void DrawGameCard(SKCanvas canvas, GameRectangle bounds, SKColor color, string title, string subtitle, int icon)
    {
        canvas.DrawRoundRect(new SKRect(bounds.Left + 8f, bounds.Top + 15f, bounds.Right + 8f, bounds.Bottom + 15f), 55f, 55f, ShadowPaint);
        FillPaint.Color = color;
        canvas.DrawRoundRect(new SKRect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 210);
        canvas.DrawCircle(bounds.Left + 105f, bounds.CenterY, 72f, FillPaint);
        if (icon == 0) DrawAdditionIcon(canvas, bounds.Left + 105f, bounds.CenterY);
        else DrawCondorIcon(canvas, bounds.Left + 105f, bounds.CenterY);
        TextPaint.TextSize = 38f; TextPaint.Color = SKColors.White;
        canvas.DrawText(title, bounds.Left + 285f, bounds.CenterY - 13f, TextPaint);
        TextPaint.TextSize = 31f;
        canvas.DrawText(subtitle, bounds.Left + 285f, bounds.CenterY + 38f, TextPaint);
    }

    private void DrawPumaCard(SKCanvas canvas)
    {
        canvas.DrawRoundRect(new SKRect(PumaBounds.Left + 8f, PumaBounds.Top + 15f, PumaBounds.Right + 8f, PumaBounds.Bottom + 15f), 55f, 55f, ShadowPaint);
        FillPaint.Color = new SKColor(239, 155, 71);
        canvas.DrawRoundRect(new SKRect(PumaBounds.Left, PumaBounds.Top, PumaBounds.Right, PumaBounds.Bottom), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 215);
        canvas.DrawCircle(PumaBounds.Left + 92f, PumaBounds.CenterY, 62f, FillPaint);
        TextPaint.TextSize = 42f; TextPaint.Color = new SKColor(105, 66, 34);
        DrawPumaIcon(canvas, PumaBounds.Left + 92f, PumaBounds.CenterY);
        TextPaint.TextSize = 31f; TextPaint.Color = SKColors.White;
        canvas.DrawText("Sumemos", PumaBounds.Left + 290f, PumaBounds.CenterY - 12f, TextPaint);
        TextPaint.TextSize = 28f;
        canvas.DrawText("con el Puma", PumaBounds.Left + 290f, PumaBounds.CenterY + 38f, TextPaint);
    }

    private void DrawChancaCard(SKCanvas canvas)
    {
        canvas.DrawRoundRect(new SKRect(ChancaBounds.Left + 8f, ChancaBounds.Top + 15f, ChancaBounds.Right + 8f, ChancaBounds.Bottom + 15f), 55f, 55f, ShadowPaint);
        FillPaint.Color = new SKColor(118, 82, 151);
        canvas.DrawRoundRect(new SKRect(ChancaBounds.Left, ChancaBounds.Top, ChancaBounds.Right, ChancaBounds.Bottom), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 215);
        canvas.DrawCircle(ChancaBounds.Left + 92f, ChancaBounds.CenterY, 62f, FillPaint);
        TextPaint.TextSize = 34f; TextPaint.Color = new SKColor(78, 51, 104);
        DrawLaboratoryIcon(canvas, ChancaBounds.Left + 92f, ChancaBounds.CenterY);
        TextPaint.TextSize = 31f; TextPaint.Color = SKColors.White;
        canvas.DrawText("Laboratorio", ChancaBounds.Left + 290f, ChancaBounds.CenterY - 12f, TextPaint);
        TextPaint.TextSize = 28f;
        canvas.DrawText("Chanka", ChancaBounds.Left + 290f, ChancaBounds.CenterY + 38f, TextPaint);
    }

    private void DrawAdditionIcon(SKCanvas canvas, float x, float y)
    {
        TextPaint.TextSize = 47f; TextPaint.Color = new SKColor(28, 65, 116); canvas.DrawText("7+5", x, y + 16f, TextPaint);
    }

    private void DrawCondorIcon(SKCanvas canvas, float x, float y)
    {
        FillPaint.Color = new SKColor(40, 44, 54); canvas.DrawOval(new SKRect(x - 42f, y - 20f, x + 42f, y + 48f), FillPaint);
        canvas.DrawArc(new SKRect(x - 72f, y - 44f, x, y + 35f), 185f, 150f, true, FillPaint); canvas.DrawArc(new SKRect(x, y - 44f, x + 72f, y + 35f), 205f, 150f, true, FillPaint);
        FillPaint.Color = SKColors.White; canvas.DrawOval(new SKRect(x - 36f, y - 25f, x + 36f, y + 4f), FillPaint);
        FillPaint.Color = new SKColor(238, 175, 51); canvas.DrawCircle(x + 34f, y - 14f, 12f, FillPaint);
    }

    private void DrawPumaIcon(SKCanvas canvas, float x, float y)
    {
        FillPaint.Color = new SKColor(220, 155, 72); canvas.DrawCircle(x, y, 43f, FillPaint); canvas.DrawCircle(x - 34f, y - 32f, 20f, FillPaint); canvas.DrawCircle(x + 34f, y - 32f, 20f, FillPaint);
        FillPaint.Color = new SKColor(55, 39, 31); canvas.DrawCircle(x - 16f, y - 5f, 6f, FillPaint); canvas.DrawCircle(x + 16f, y - 5f, 6f, FillPaint); canvas.DrawCircle(x, y + 15f, 8f, FillPaint);
    }

    private void DrawLaboratoryIcon(SKCanvas canvas, float x, float y)
    {
        FillPaint.Color = new SKColor(69, 208, 192); canvas.DrawRoundRect(new SKRect(x - 34f, y - 5f, x + 34f, y + 48f), 15f, 15f, FillPaint);
        StrokePaint.Color = new SKColor(83, 53, 108); StrokePaint.StrokeWidth = 10f; canvas.DrawLine(x - 15f, y - 48f, x - 15f, y - 4f, StrokePaint); canvas.DrawLine(x + 15f, y - 48f, x + 15f, y - 4f, StrokePaint); canvas.DrawLine(x - 15f, y - 48f, x + 15f, y - 48f, StrokePaint);
        FillPaint.Color = new SKColor(244, 174, 50); canvas.DrawCircle(x, y + 24f, 11f, FillPaint);
    }

    public override void Dispose() { _navigationBar.Dispose(); _backdrop.Dispose(); base.Dispose(); }
}
