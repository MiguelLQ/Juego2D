using MathKids.Game.Common;
using MathKids.Game.Components;
using MathKids.Game.Core;
using MathKids.Game.Input.Touch;
using MathKids.Game.Scenes.Abstractions;
using SkiaSharp;

namespace MathKids.Game.Scenes.Home;

public sealed class HomeScene : KidsSceneBase
{
    private static readonly GameRectangle AdditionBounds = new(105f, 1080f, 410f, 420f);
    private static readonly GameRectangle BingoBounds = new(565f, 1080f, 410f, 420f);
    private readonly GameNavigation _navigation;
    private readonly PlayerGameState _state;
    private readonly BottomNavigationBar _navigationBar;
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
    public override void Update(GameTime gameTime) => _elapsed += gameTime.DeltaSeconds;

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        DrawWorldBackground(canvas, viewport);
        DrawBrandHeader(canvas, 190f, 1.03f);
        DrawCoinBadge(canvas, _state.Coins);

        var bob = MathF.Sin(_elapsed * 2.4f) * 12f;
        DrawFoxMascot(canvas, -70f, bob);
        FillPaint.Color = new SKColor(255, 255, 255, 242);
        canvas.DrawRoundRect(new SKRect(500f, 315f + bob, 1000f, 560f + bob), 65f, 65f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(485f, 300f + bob, 985f, 545f + bob), 65f, 65f, FillPaint);
        TextPaint.TextSize = 52f; TextPaint.Color = new SKColor(30, 71, 126);
        canvas.DrawText("\u00A1Hola, peque\u00F1o genio!", 735f, 395f + bob, TextPaint);
        TextPaint.TextSize = 37f; TextPaint.Color = new SKColor(74, 101, 135);
        canvas.DrawText("Elige una aventura y gana estrellas", 735f, 465f + bob, TextPaint);

        FillPaint.Color = new SKColor(255, 250, 235);
        canvas.DrawRoundRect(new SKRect(55f, 860f, 1025f, 1615f), 76f, 76f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(45f, 845f, 1015f, 1600f), 76f, 76f, FillPaint);
        FillPaint.Color = new SKColor(116, 75, 213);
        canvas.DrawRoundRect(new SKRect(230f, 790f, 850f, 910f), 40f, 40f, FillPaint);
        TextPaint.TextSize = 44f; TextPaint.Color = SKColors.White;
        canvas.DrawText("\u2605  Elige un juego  \u2605", 540f, 866f, TextPaint);

        DrawGameCard(canvas, AdditionBounds, new SKColor(104, 184, 245), "Aventura", "de sumas", false);
        DrawGameCard(canvas, BingoBounds, new SKColor(157, 216, 73), "Bingo", "de sumas", true);
        _navigationBar.Draw(canvas);
    }

    public override void HandleInput(GameInput input)
    {
        if (IsReleasedInside(input, AdditionBounds)) _navigation.NavigateTo(GameScreen.Addition);
        else if (IsReleasedInside(input, BingoBounds)) _navigation.NavigateTo(GameScreen.AdditionBingo);
        else _navigationBar.HandleInput(input);
    }

    private void DrawGameCard(SKCanvas canvas, GameRectangle bounds, SKColor color, string title, string subtitle, bool bingo)
    {
        canvas.DrawRoundRect(new SKRect(bounds.Left + 8f, bounds.Top + 15f, bounds.Right + 8f, bounds.Bottom + 15f), 55f, 55f, ShadowPaint);
        FillPaint.Color = color;
        canvas.DrawRoundRect(new SKRect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 210);
        canvas.DrawCircle(bounds.CenterX, bounds.Top + 135f, 92f, FillPaint);
        TextPaint.Color = new SKColor(28, 65, 116); TextPaint.TextSize = bingo ? 39f : 64f;
        canvas.DrawText(bingo ? "BINGO" : "7 + 5", bounds.CenterX, bounds.Top + 155f, TextPaint);
        TextPaint.TextSize = 48f; TextPaint.Color = SKColors.White;
        canvas.DrawText(title, bounds.CenterX, bounds.Top + 285f, TextPaint);
        TextPaint.TextSize = 38f;
        canvas.DrawText(subtitle, bounds.CenterX, bounds.Top + 342f, TextPaint);
    }

    public override void Dispose() { _navigationBar.Dispose(); base.Dispose(); }
}
