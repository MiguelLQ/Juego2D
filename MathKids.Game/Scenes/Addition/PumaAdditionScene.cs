using MathKids.Application.Abstractions;
using MathKids.Application.Exercises;
using MathKids.Application.Sessions;
using MathKids.Domain.Exercises;
using MathKids.Game.Common;
using MathKids.Game.Components;
using MathKids.Game.Components.Buttons;
using MathKids.Game.Components.Characters;
using MathKids.Game.Core;
using MathKids.Game.Graphics.Drawing;
using MathKids.Game.Input.Touch;
using MathKids.Game.Scenes.Abstractions;
using SkiaSharp;

namespace MathKids.Game.Scenes.Addition;

public sealed class PumaAdditionScene : KidsSceneBase
{
    private readonly IPumaExerciseGenerator _exerciseGenerator;
    private readonly IGameSessionService _sessionService;
    private readonly IAudioService _audioService;
    private readonly GameNavigation _navigation;
    private readonly PlayerGameState _state;
    private readonly AndeanPumaBackdrop _backdrop = new();
    private readonly PumaGuide _puma = new();
    private readonly AnswerFeedbackSequence _feedback = new(1.8f, 2.1f);
    private readonly GraphicButton[] _answerButtons;
    private readonly SKPath _itemPath = new();
    private readonly SpeechBubble _speechBubble = new(new SKRect(405f, 280f, 990f, 525f), SpeechTailSide.Left, new SKColor(255, 255, 255, 246), new SKColor(52, 76, 91), 34f);
    private MathExercise? _exercise;
    private GraphicButton? _selectedButton;
    private PumaMood _mood = PumaMood.Thinking;
    private CountingItemKind _itemKind;
    private int _itemSequence = -1;
    private float _elapsed;

    public PumaAdditionScene(IPumaExerciseGenerator exerciseGenerator, IGameSessionService sessionService, IAudioService audioService, GameNavigation navigation, PlayerGameState state)
    {
        _exerciseGenerator = exerciseGenerator;
        _sessionService = sessionService;
        _audioService = audioService;
        _navigation = navigation;
        _state = state;
        _speechBubble.Text = "Cuenta los objetos de la mesa";
        _answerButtons =
        [
            CreateButton(new(105f, 1325f, 265f, 225f), new SKColor(94, 181, 239)),
            CreateButton(new(407f, 1325f, 265f, 225f), new SKColor(151, 210, 70)),
            CreateButton(new(709f, 1325f, 265f, 225f), new SKColor(255, 184, 56))
        ];
    }

    public override GameScreen Screen => GameScreen.PumaAddition;
    public override void Enter() => LoadNextExercise();
    public override void Exit() { }

    public override void Update(GameTime gameTime)
    {
        _elapsed += gameTime.DeltaSeconds;
        _backdrop.Update(gameTime);
        _puma.Update(gameTime);
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Update(gameTime);
        var completion = _feedback.Update(gameTime.DeltaSeconds);
        if (completion == AnswerFeedbackCompletion.Retry)
        {
            if (_selectedButton is not null) _selectedButton.VisualState = ButtonVisualState.Normal;
            _selectedButton = null;
            _mood = PumaMood.Thinking;
            SetDialog("Cuenta despacio. \u00A1T\u00FA puedes!");
            SetButtonsEnabled(true);
        }
        else if (completion == AnswerFeedbackCompletion.NextExercise)
        {
            LoadNextExercise();
        }
    }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        _backdrop.Draw(canvas, viewport);
        DrawBackButton(canvas);
        DrawCoinBadge(canvas, _state.Coins);
        DrawAudioButton(canvas);
        DrawTitle(canvas);
        _puma.Draw(canvas, 205f, 300f, 0.95f, _mood);
        DrawSpeechBubble(canvas);
        DrawTable(canvas);
        if (_feedback.State == AnswerFeedbackState.Correct) DrawTableCelebration(canvas);
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Draw(canvas);
        TextPaint.TextSize = 31f; TextPaint.Color = new SKColor(74, 75, 63);
        canvas.DrawText("Toca la respuesta correcta", 540f, 1625f, TextPaint);
    }

    public override void HandleInput(GameInput input)
    {
        if (IsReleasedInside(input, BackButtonBounds))
        {
            _navigation.NavigateTo(GameScreen.Games);
            return;
        }
        if (_feedback.IsActive) return;
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].HandleInput(input);
    }

    private void LoadNextExercise()
    {
        _exercise = _exerciseGenerator.Generate();
        _selectedButton = null;
        _feedback.Reset();
        _mood = PumaMood.Thinking;
        _itemSequence = (_itemSequence + 1) % 3;
        _itemKind = (CountingItemKind)_itemSequence;
        SetDialog(GetQuestion());
        for (var index = 0; index < _answerButtons.Length; index++)
        {
            _answerButtons[index].Text = _exercise.Options[index].Value.ToString();
            _answerButtons[index].VisualState = ButtonVisualState.Normal;
            _answerButtons[index].IsEnabled = true;
        }
    }

    private GraphicButton CreateButton(GameRectangle bounds, SKColor color)
    {
        var button = new GraphicButton(bounds, string.Empty)
        {
            NormalColor = color,
            TextColor = new SKColor(25, 62, 105),
            TextSize = 82f,
            CornerRadius = 48f
        };
        button.Clicked += OnAnswerClicked;
        return button;
    }

    private void OnAnswerClicked(GraphicButton button)
    {
        if (_exercise is null || !int.TryParse(button.Text, out var answer)) return;
        var result = _sessionService.Evaluate(_exercise, answer, TimeSpan.Zero);
        _selectedButton = button;
        button.PlayFeedbackAnimation();
        SetButtonsEnabled(false);
        _feedback.Start(result.IsCorrect);
        if (result.IsCorrect)
        {
            _state.RecordCorrectAnswer("puma_addition");
            button.VisualState = ButtonVisualState.Correct;
            _mood = PumaMood.Happy;
            SetDialog("\u00A1Excelente! Sigue as\u00ED, campe\u00F3n.");
            _audioService.PlayEffect(AudioCue.Correct);
        }
        else
        {
            _state.RecordAttempt("puma_addition");
            button.VisualState = ButtonVisualState.Incorrect;
            _mood = PumaMood.Encouraging;
            SetDialog("\u00A1Casi! Cuenta otra vez. T\u00FA puedes.");
            _audioService.PlayEffect(AudioCue.TryAgain);
        }
    }

    private void DrawTitle(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(255, 250, 226, 242);
        canvas.DrawRoundRect(new SKRect(220f, 58f, 790f, 190f), 54f, 54f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(210f, 48f, 780f, 180f), 54f, 54f, FillPaint);
        TextPaint.TextSize = 52f; TextPaint.Color = new SKColor(117, 73, 35);
        canvas.DrawText("Sumemos con el Puma", 495f, 132f, TextPaint);
    }

    private void DrawSpeechBubble(SKCanvas canvas)
    {
        _speechBubble.Draw(canvas, MathF.Sin(_elapsed * 2f) * 3f);
    }

    private void DrawTable(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(118, 70, 38, 60);
        canvas.DrawRoundRect(new SKRect(50f, 610f, 1030f, 1265f), 72f, 72f, FillPaint);
        FillPaint.Color = new SKColor(217, 154, 82);
        canvas.DrawRoundRect(new SKRect(38f, 592f, 1018f, 1247f), 72f, 72f, FillPaint);
        FillPaint.Color = new SKColor(238, 184, 109);
        canvas.DrawRoundRect(new SKRect(65f, 620f, 991f, 1217f), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(129, 73, 173, 150);
        canvas.DrawRoundRect(new SKRect(88f, 650f, 968f, 720f), 26f, 26f, FillPaint);
        FillPaint.Color = new SKColor(255, 197, 66);
        for (var x = 125f; x < 950f; x += 95f) canvas.DrawCircle(x, 685f, 11f, FillPaint);
        StrokePaint.Color = new SKColor(172, 108, 56, 100); StrokePaint.StrokeWidth = 5f;
        for (var y = 720f; y < 1200f; y += 115f) canvas.DrawLine(90f, y, 965f, y + 16f, StrokePaint);
        TextPaint.TextSize = 72f; TextPaint.Color = new SKColor(84, 56, 38);
        canvas.DrawText("+", 405f, 950f, TextPaint);
        canvas.DrawText("=", 705f, 950f, TextPaint);
        TextPaint.TextSize = 100f; TextPaint.Color = new SKColor(120, 74, 191);
        canvas.DrawText("?", 875f, 963f, TextPaint);
        if (_exercise is null) return;
        DrawItemGroup(canvas, _exercise.LeftOperand, 215f, 895f);
        DrawItemGroup(canvas, _exercise.RightOperand, 555f, 895f);
        TextPaint.TextSize = 31f; TextPaint.Color = new SKColor(99, 62, 37);
        canvas.DrawText(_exercise.LeftOperand.ToString(), 215f, 1168f, TextPaint);
        canvas.DrawText(_exercise.RightOperand.ToString(), 555f, 1168f, TextPaint);
    }

    private void DrawItemGroup(SKCanvas canvas, int count, float centerX, float centerY)
    {
        for (var index = 0; index < count; index++)
        {
            var column = index % 3;
            var row = index / 3;
            var x = centerX + (column - 1) * 82f;
            var bounce = MathF.Sin(_elapsed * 2.7f + index) * (_feedback.State == AnswerFeedbackState.Correct ? 13f : 5f);
            var y = centerY + (row - 0.5f) * 105f + bounce;
            if (_itemKind == CountingItemKind.Cookie) DrawCookie(canvas, x, y, 39f);
            else if (_itemKind == CountingItemKind.Candy) DrawCandy(canvas, x, y, index);
            else DrawLollipop(canvas, x, y, index);
        }
    }

    private void DrawCandy(SKCanvas canvas, float x, float y, int index)
    {
        canvas.Save(); canvas.RotateDegrees(MathF.Sin(_elapsed * 2f + index) * 8f, x, y);
        FillPaint.Color = index % 2 == 0 ? new SKColor(239, 91, 143) : new SKColor(82, 190, 225);
        canvas.DrawRoundRect(new SKRect(x - 31f, y - 25f, x + 31f, y + 25f), 16f, 16f, FillPaint);
        _itemPath.Reset(); _itemPath.MoveTo(x - 30f, y - 19f); _itemPath.LineTo(x - 58f, y - 36f); _itemPath.LineTo(x - 52f, y + 35f); _itemPath.LineTo(x - 30f, y + 19f); _itemPath.Close(); canvas.DrawPath(_itemPath, FillPaint);
        _itemPath.Reset(); _itemPath.MoveTo(x + 30f, y - 19f); _itemPath.LineTo(x + 58f, y - 36f); _itemPath.LineTo(x + 52f, y + 35f); _itemPath.LineTo(x + 30f, y + 19f); _itemPath.Close(); canvas.DrawPath(_itemPath, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 90); canvas.DrawRoundRect(new SKRect(x - 18f, y - 14f, x + 12f, y - 5f), 5f, 5f, FillPaint);
        canvas.Restore();
    }

    private void DrawLollipop(SKCanvas canvas, float x, float y, int index)
    {
        StrokePaint.Color = new SKColor(245, 236, 207); StrokePaint.StrokeWidth = 9f; canvas.DrawLine(x, y + 18f, x, y + 61f, StrokePaint);
        FillPaint.Color = index % 2 == 0 ? new SKColor(129, 79, 202) : new SKColor(242, 103, 89); canvas.DrawCircle(x, y - 8f, 36f, FillPaint);
        StrokePaint.Color = new SKColor(255, 224, 94); StrokePaint.StrokeWidth = 7f;
        canvas.DrawArc(new SKRect(x - 24f, y - 32f, x + 24f, y + 16f), _elapsed * 80f + index * 30f, 245f, false, StrokePaint);
    }

    private void DrawTableCelebration(SKCanvas canvas)
    {
        for (var index = 0; index < 12; index++)
        {
            var angle = index * MathF.PI / 6f + _elapsed * 1.8f;
            FillPaint.Color = index % 2 == 0 ? new SKColor(255, 199, 55) : new SKColor(82, 206, 173);
            canvas.DrawCircle(530f + MathF.Cos(angle) * 420f, 950f + MathF.Sin(angle) * 250f, 9f + index % 3 * 3f, FillPaint);
        }
    }

    private string GetItemName() => _itemKind switch { CountingItemKind.Cookie => "galletas", CountingItemKind.Candy => "caramelos", _ => "chupetes" };

    private string GetQuestion() => _itemKind == CountingItemKind.Cookie
        ? "\u00BFCu\u00E1ntas galletas hay en total?"
        : $"\u00BFCu\u00E1ntos {GetItemName()} hay en total?";

    private void SetDialog(string text) => _speechBubble.Text = text;

    private void DrawCookie(SKCanvas canvas, float x, float y, float radius)
    {
        FillPaint.Color = new SKColor(148, 91, 42, 80);
        canvas.DrawCircle(x + 5f, y + 7f, radius, FillPaint);
        FillPaint.Color = new SKColor(235, 169, 76);
        canvas.DrawCircle(x, y, radius, FillPaint);
        FillPaint.Color = new SKColor(111, 65, 35);
        for (var index = 0; index < 5; index++)
        {
            var angle = index * 2f * MathF.PI / 5f + 0.4f;
            canvas.DrawCircle(x + MathF.Cos(angle) * radius * 0.56f, y + MathF.Sin(angle) * radius * 0.56f, 5.5f, FillPaint);
        }
        canvas.DrawCircle(x + 5f, y - 4f, 6f, FillPaint);
    }

    private void SetButtonsEnabled(bool enabled)
    {
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].IsEnabled = enabled;
    }

    public override void Dispose()
    {
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Dispose();
        _puma.Dispose();
        _backdrop.Dispose();
        _speechBubble.Dispose();
        _itemPath.Dispose();
        base.Dispose();
    }
}

internal enum CountingItemKind { Cookie, Candy, Lollipop }
