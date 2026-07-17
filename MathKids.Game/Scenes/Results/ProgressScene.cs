using MathKids.Game.Components;
using MathKids.Game.Core;
using MathKids.Game.Input.Touch;
using MathKids.Game.Scenes.Abstractions;
using SkiaSharp;

namespace MathKids.Game.Scenes.Results;

public sealed class ProgressScene : KidsSceneBase
{
    private readonly PlayerGameState _state;
    private readonly BottomNavigationBar _navigationBar;
    private string _lastGameText = "A\u00FAn no hay partidas guardadas";
    private string _lastPlayedText = string.Empty;

    public ProgressScene(GameNavigation navigation, PlayerGameState state)
    {
        _state = state;
        _navigationBar = new BottomNavigationBar(navigation, GameScreen.Progress);
    }

    public override GameScreen Screen => GameScreen.Progress;
    public override void Enter()
    {
        _state.Reload();
        _lastGameText = _state.LastGame switch
        {
            "addition" => "\u00DAltimo juego: Aventura de sumas",
            "addition_bingo" => "\u00DAltimo juego: Bingo de sumas",
            _ => "A\u00FAn no hay partidas guardadas"
        };
        _lastPlayedText = _state.UpdatedAtUtc == DateTimeOffset.UnixEpoch
            ? string.Empty
            : $"Guardado: {_state.UpdatedAtUtc.ToLocalTime():dd/MM/yyyy HH:mm}";
    }
    public override void Exit() { }
    public override void Update(GameTime gameTime) { }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        DrawWorldBackground(canvas, viewport);
        DrawBrandHeader(canvas, 175f, 0.92f);
        DrawCoinBadge(canvas, _state.Coins);
        TextPaint.TextSize = 58f; TextPaint.Color = new SKColor(29, 69, 122);
        canvas.DrawText("Mi progreso", 540f, 335f, TextPaint);

        FillPaint.Color = new SKColor(255, 250, 235);
        canvas.DrawRoundRect(new SKRect(75f, 410f, 1005f, 1505f), 78f, 78f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(60f, 395f, 990f, 1490f), 78f, 78f, FillPaint);
        DrawStatCard(canvas, 115f, 490f, new SKColor(255, 194, 55), "\u2605", _state.Stars.ToString(), "Estrellas");
        DrawStatCard(canvas, 570f, 490f, new SKColor(104, 184, 245), "\u2713", _state.CorrectAnswers.ToString(), "Aciertos");
        DrawStatCard(canvas, 115f, 890f, new SKColor(157, 216, 73), "+", _state.CompletedChallenges.ToString(), "Intentos");
        DrawStatCard(canvas, 570f, 890f, new SKColor(183, 132, 226), "\u25CF", _state.Coins.ToString(), "Monedas");
        TextPaint.TextSize = 34f; TextPaint.Color = new SKColor(65, 91, 127);
        canvas.DrawText(_lastGameText, 525f, 1365f, TextPaint);
        TextPaint.TextSize = 29f;
        if (_lastPlayedText.Length > 0) canvas.DrawText(_lastPlayedText, 525f, 1420f, TextPaint);
        _navigationBar.Draw(canvas);
    }

    public override void HandleInput(GameInput input) => _navigationBar.HandleInput(input);

    private void DrawStatCard(SKCanvas canvas, float x, float y, SKColor color, string icon, string value, string label)
    {
        FillPaint.Color = color;
        canvas.DrawRoundRect(new SKRect(x, y, x + 395f, y + 330f), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 215);
        canvas.DrawCircle(x + 95f, y + 95f, 62f, FillPaint);
        TextPaint.TextSize = 62f; TextPaint.Color = new SKColor(30, 66, 116);
        canvas.DrawText(icon, x + 95f, y + 116f, TextPaint);
        TextPaint.TextSize = 92f; TextPaint.Color = SKColors.White;
        canvas.DrawText(value, x + 270f, y + 128f, TextPaint);
        TextPaint.TextSize = 42f;
        canvas.DrawText(label, x + 197f, y + 252f, TextPaint);
    }

    public override void Dispose() { _navigationBar.Dispose(); base.Dispose(); }
}
