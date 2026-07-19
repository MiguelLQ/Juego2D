using MathKids.Application.Abstractions;
using MathKids.Game.Components;
using MathKids.Game.Core;
using MathKids.Game.Input.Touch;
using MathKids.Game.Scenes.Abstractions;
using SkiaSharp;

namespace MathKids.Game.Scenes.Results;

public sealed class ProgressScene : KidsSceneBase
{
    private readonly PlayerGameState _state;
    private readonly IAudioService _audioService;
    private readonly BottomNavigationBar _navigationBar;
    private string _lastGameTitle = "A\u00FAn no hay partidas guardadas";
    private string _lastPlayedText = string.Empty;
    private SKColor _lastGameColor = new(116, 75, 213);

    public ProgressScene(GameNavigation navigation, PlayerGameState state, IAudioService audioService)
    {
        _state = state;
        _audioService = audioService;
        _navigationBar = new BottomNavigationBar(navigation, GameScreen.Progress, audioService);
    }

    public override GameScreen Screen => GameScreen.Progress;
    public override void Enter()
    {
        _state.Reload();
        _audioService.PlayMusic(MusicCue.Home);
        (_lastGameTitle, _lastGameColor) = _state.LastGame switch
        {
            "addition" => ("Aventura Matem\u00E1tica", new SKColor(74, 167, 227)),
            "addition_bingo" => ("Bingo del C\u00F3ndor", new SKColor(91, 178, 87)),
            "puma_addition" => ("Matem\u00E1ticas con el Puma", new SKColor(239, 155, 71)),
            "chanca_laboratory" => ("Laboratorio Chanka", new SKColor(118, 82, 151)),
            _ => ("A\u00FAn no hay partidas guardadas", new SKColor(116, 75, 213))
        };
        _lastPlayedText = _state.UpdatedAtUtc == DateTimeOffset.UnixEpoch
            ? string.Empty
            : $"{_state.UpdatedAtUtc.ToLocalTime():dd/MM/yyyy}  \u2022  {_state.UpdatedAtUtc.ToLocalTime():HH:mm}";
    }
    public override void Exit() { }
    public override void Update(GameTime gameTime) { }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        DrawWorldBackground(canvas, viewport);
        DrawBrandHeader(canvas, 175f, 0.92f);
        DrawCoinBadge(canvas, _state.Coins);
        DrawAudioButton(canvas, _audioService.IsMuted);
        TextPaint.TextSize = 58f; TextPaint.Color = new SKColor(29, 69, 122);
        canvas.DrawText("Mi progreso", 540f, 335f, TextPaint);

        FillPaint.Color = new SKColor(255, 250, 235);
        canvas.DrawRoundRect(new SKRect(75f, 410f, 1005f, 1505f), 78f, 78f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(60f, 395f, 990f, 1490f), 78f, 78f, FillPaint);
        DrawStatCard(canvas, 115f, 490f, new SKColor(255, 194, 55), ProgressStatIcon.Star, _state.Stars.ToString(), "Estrellas");
        DrawStatCard(canvas, 570f, 490f, new SKColor(104, 184, 245), ProgressStatIcon.Correct, _state.CorrectAnswers.ToString(), "Aciertos");
        DrawStatCard(canvas, 115f, 890f, new SKColor(157, 216, 73), ProgressStatIcon.Attempts, _state.CompletedChallenges.ToString(), "Intentos");
        DrawStatCard(canvas, 570f, 890f, new SKColor(183, 132, 226), ProgressStatIcon.Coin, _state.Coins.ToString(), "Monedas");
        DrawLastActivity(canvas);
        _navigationBar.Draw(canvas);
    }

    public override void HandleInput(GameInput input)
    {
        if (TryHandleAudioButton(input, _audioService)) return;
        _navigationBar.HandleInput(input);
    }

    private void DrawStatCard(SKCanvas canvas, float x, float y, SKColor color, ProgressStatIcon icon, string value, string label)
    {
        FillPaint.Color = color;
        canvas.DrawRoundRect(new SKRect(x, y, x + 395f, y + 330f), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 215);
        canvas.DrawCircle(x + 95f, y + 95f, 62f, FillPaint);
        DrawStatIcon(canvas, x + 95f, y + 95f, icon, new SKColor(30, 66, 116));
        TextPaint.TextSize = 92f; TextPaint.Color = SKColors.White;
        canvas.DrawText(value, x + 270f, y + 128f, TextPaint);
        TextPaint.TextSize = 42f;
        canvas.DrawText(label, x + 197f, y + 252f, TextPaint);
    }

    private void DrawStatIcon(SKCanvas canvas, float x, float y, ProgressStatIcon icon, SKColor color)
    {
        if (icon == ProgressStatIcon.Star)
        {
            DrawStar(canvas, x, y, 42f, color);
            return;
        }
        StrokePaint.Color = color; StrokePaint.StrokeWidth = 12f;
        FillPaint.Color = color;
        if (icon == ProgressStatIcon.Correct)
        {
            canvas.DrawCircle(x, y, 40f, StrokePaint);
            canvas.DrawLine(x - 22f, y, x - 5f, y + 19f, StrokePaint);
            canvas.DrawLine(x - 5f, y + 19f, x + 27f, y - 22f, StrokePaint);
        }
        else if (icon == ProgressStatIcon.Attempts)
        {
            canvas.DrawCircle(x, y, 39f, StrokePaint);
            canvas.DrawLine(x, y, x, y - 24f, StrokePaint);
            canvas.DrawLine(x, y, x + 21f, y + 12f, StrokePaint);
            canvas.DrawCircle(x, y, 7f, FillPaint);
        }
        else
        {
            canvas.DrawCircle(x, y, 41f, FillPaint);
            FillPaint.Color = new SKColor(255, 255, 255, 220);
            canvas.DrawCircle(x, y, 29f, FillPaint);
            DrawStar(canvas, x, y, 19f, color);
        }
    }

    private void DrawLastActivity(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(42, 61, 91, 35);
        canvas.DrawRoundRect(new SKRect(112f, 1275f, 938f, 1465f), 48f, 48f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 245);
        canvas.DrawRoundRect(new SKRect(102f, 1262f, 928f, 1452f), 48f, 48f, FillPaint);
        FillPaint.Color = _lastGameColor;
        canvas.DrawCircle(202f, 1357f, 62f, FillPaint);
        StrokePaint.Color = SKColors.White; StrokePaint.StrokeWidth = 9f;
        canvas.DrawLine(184f, 1390f, 184f, 1315f, StrokePaint);
        canvas.DrawRoundRect(new SKRect(184f, 1315f, 232f, 1355f), 8f, 8f, StrokePaint);
        TextPaint.TextSize = 23f; TextPaint.Color = _lastGameColor;
        canvas.DrawText("\u00DALTIMA ACTIVIDAD", 565f, 1308f, TextPaint);
        TextPaint.TextSize = _lastGameTitle.Length > 25 ? 31f : 36f; TextPaint.Color = new SKColor(35, 65, 105);
        canvas.DrawText(_lastGameTitle, 565f, 1362f, TextPaint);
        if (_lastPlayedText.Length > 0)
        {
            TextPaint.TextSize = 25f; TextPaint.Color = new SKColor(96, 112, 133);
            canvas.DrawText(_lastPlayedText, 565f, 1413f, TextPaint);
        }
    }

    public override void Dispose() { _navigationBar.Dispose(); base.Dispose(); }
}

internal enum ProgressStatIcon { Star, Correct, Attempts, Coin }
