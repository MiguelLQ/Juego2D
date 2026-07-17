using MathKids.Game.Common;
using MathKids.Game.Components;
using MathKids.Game.Core;
using MathKids.Game.Input.Touch;
using MathKids.Game.Scenes.Abstractions;
using SkiaSharp;

namespace MathKids.Game.Scenes.Home;

public sealed class GamesMenuScene : KidsSceneBase
{
    private static readonly GameRectangle AdditionBounds = new(70f, 460f, 450f, 470f);
    private static readonly GameRectangle BingoBounds = new(560f, 460f, 450f, 470f);
    private static readonly GameRectangle ComingOneBounds = new(70f, 1010f, 450f, 470f);
    private static readonly GameRectangle ComingTwoBounds = new(560f, 1010f, 450f, 470f);
    private readonly GameNavigation _navigation;
    private readonly PlayerGameState _state;
    private readonly BottomNavigationBar _navigationBar;

    public GamesMenuScene(GameNavigation navigation, PlayerGameState state)
    {
        _navigation = navigation;
        _state = state;
        _navigationBar = new BottomNavigationBar(navigation, GameScreen.Games);
    }

    public override GameScreen Screen => GameScreen.Games;
    public override void Enter() { }
    public override void Exit() { }
    public override void Update(GameTime gameTime) { }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        DrawWorldBackground(canvas, viewport);
        DrawBrandHeader(canvas, 175f, 0.92f);
        DrawCoinBadge(canvas, _state.Coins);
        DrawAudioButton(canvas);
        TextPaint.TextSize = 57f; TextPaint.Color = new SKColor(29, 69, 122);
        canvas.DrawText("Elige tu aventura", 540f, 335f, TextPaint);
        DrawModule(canvas, AdditionBounds, new SKColor(96, 181, 246), "7 + 5", "Aventura de sumas", "Resuelve y gana estrellas", true);
        DrawModule(canvas, BingoBounds, new SKColor(151, 211, 70), "C\u00D3NDOR", "Bingo del C\u00F3ndor", "Completa el tablero andino", true);
        DrawModule(canvas, ComingOneBounds, new SKColor(247, 169, 75), "2 + 4", "Sumemos con el Puma", "Cuenta galletas en los Andes", true);
        DrawModule(canvas, ComingTwoBounds, new SKColor(118, 82, 151), "LAB", "Laboratorio Chanka", "Activa esferas de sabidur\u00EDa", true);
        _navigationBar.Draw(canvas);
    }

    public override void HandleInput(GameInput input)
    {
        if (IsReleasedInside(input, AdditionBounds)) _navigation.NavigateTo(GameScreen.Addition);
        else if (IsReleasedInside(input, BingoBounds)) _navigation.NavigateTo(GameScreen.AdditionBingo);
        else if (IsReleasedInside(input, ComingOneBounds)) _navigation.NavigateTo(GameScreen.PumaAddition);
        else if (IsReleasedInside(input, ComingTwoBounds)) _navigation.NavigateTo(GameScreen.ChancaLaboratory);
        else _navigationBar.HandleInput(input);
    }

    private void DrawModule(SKCanvas canvas, GameRectangle bounds, SKColor color, string icon, string title, string subtitle, bool enabled)
    {
        canvas.DrawRoundRect(new SKRect(bounds.Left + 9f, bounds.Top + 16f, bounds.Right + 9f, bounds.Bottom + 16f), 60f, 60f, ShadowPaint);
        FillPaint.Color = enabled ? color : color.WithAlpha(190);
        canvas.DrawRoundRect(new SKRect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom), 60f, 60f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 225);
        canvas.DrawCircle(bounds.CenterX, bounds.Top + 145f, 105f, FillPaint);
        TextPaint.TextSize = icon == "BINGO" ? 42f : 76f; TextPaint.Color = new SKColor(27, 64, 115);
        canvas.DrawText(icon, bounds.CenterX, bounds.Top + 170f, TextPaint);
        TextPaint.TextSize = 43f; TextPaint.Color = SKColors.White;
        canvas.DrawText(title, bounds.CenterX, bounds.Top + 305f, TextPaint);
        TextPaint.TextSize = 29f;
        canvas.DrawText(subtitle, bounds.CenterX, bounds.Top + 362f, TextPaint);
        if (!enabled)
        {
            FillPaint.Color = new SKColor(35, 55, 87, 145);
            canvas.DrawRoundRect(new SKRect(bounds.Left + 70f, bounds.Bottom - 70f, bounds.Right - 70f, bounds.Bottom - 18f), 25f, 25f, FillPaint);
            TextPaint.TextSize = 27f; TextPaint.Color = SKColors.White;
            canvas.DrawText("PR\u00D3XIMAMENTE", bounds.CenterX, bounds.Bottom - 34f, TextPaint);
        }
    }

    public override void Dispose() { _navigationBar.Dispose(); base.Dispose(); }
}
