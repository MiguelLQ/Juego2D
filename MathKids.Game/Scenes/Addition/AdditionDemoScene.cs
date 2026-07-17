using MathKids.Application.Abstractions;
using MathKids.Application.Exercises;
using MathKids.Application.Sessions;
using MathKids.Domain.Exercises;
using MathKids.Game.Common;
using MathKids.Game.Components.Buttons;
using MathKids.Game.Core;
using MathKids.Game.Graphics.Drawing;
using MathKids.Game.Input.Touch;
using MathKids.Game.Scenes.Abstractions;
using SkiaSharp;

namespace MathKids.Game.Scenes.Addition;

public sealed class AdditionDemoScene : KidsSceneBase
{
    private readonly IExerciseGenerator _exerciseGenerator;
    private readonly IGameSessionService _sessionService;
    private readonly IAudioService _audioService;
    private readonly GameNavigation _navigation;
    private readonly PlayerGameState _state;
    private readonly GraphicButton[] _answerButtons;
    private readonly AnswerFeedbackSequence _feedbackSequence = new();
    private readonly SKPaint _celebrationPaint = new() { IsAntialias = true };
    private readonly SKPathEffect _dashedBorder = SKPathEffect.CreateDash([18f, 14f], 0f);
    private readonly AdditionAdventureBackdrop _backdrop = new();
    private MathExercise? _exercise;
    private GraphicButton? _selectedButton;
    private int _roundCorrectAnswers;
    private string _message = "Elige la respuesta correcta";
    private SKColor _messageColor = new(37, 75, 126);
    private float _elapsed;

    public AdditionDemoScene(IExerciseGenerator exerciseGenerator, IGameSessionService sessionService, IAudioService audioService, GameNavigation navigation, PlayerGameState state)
    {
        _exerciseGenerator = exerciseGenerator;
        _sessionService = sessionService;
        _audioService = audioService;
        _navigation = navigation;
        _state = state;
        _answerButtons =
        [
            CreateButton(new(90f, 1300f, 280f, 270f), new SKColor(104, 184, 245)),
            CreateButton(new(400f, 1300f, 280f, 270f), new SKColor(151, 211, 70)),
            CreateButton(new(710f, 1300f, 280f, 270f), new SKColor(255, 190, 54))
        ];
    }

    public override GameScreen Screen => GameScreen.Addition;

    public override void Enter()
    {
        _audioService.PlayMusic(MusicCue.Addition);
        _roundCorrectAnswers = 0;
        LoadNextExercise();
    }

    public override void Exit() { }

    public override void Update(GameTime gameTime)
    {
        _elapsed += gameTime.DeltaSeconds;
        _backdrop.Update(gameTime);
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Update(gameTime);
        var completion = _feedbackSequence.Update(gameTime.DeltaSeconds);
        if (completion == AnswerFeedbackCompletion.Retry)
        {
            if (_selectedButton is not null) _selectedButton.VisualState = ButtonVisualState.Normal;
            _selectedButton = null;
            _message = "Vuelve a intentarlo";
            _messageColor = new(37, 75, 126);
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
        DrawBrandHeader(canvas, 165f, 0.88f);
        DrawCoinBadge(canvas, _state.Coins);
        DrawAudioButton(canvas, _audioService.IsMuted);
        DrawBackButton(canvas);
        DrawFoxMascot(canvas, -115f, MathF.Sin(_elapsed * 2.5f) * 9f);
        DrawRoundProgress(canvas);

        FillPaint.Color = new SKColor(255, 249, 234);
        canvas.DrawRoundRect(new SKRect(45f, 780f, 1035f, 1665f), 82f, 82f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(35f, 765f, 1025f, 1650f), 82f, 82f, FillPaint);
        StrokePaint.Color = new SKColor(235, 211, 174); StrokePaint.StrokeWidth = 5f; StrokePaint.PathEffect = _dashedBorder;
        canvas.DrawRoundRect(new SKRect(70f, 800f, 990f, 1615f), 62f, 62f, StrokePaint);
        StrokePaint.PathEffect = null;

        FillPaint.Color = new SKColor(115, 73, 211);
        canvas.DrawRoundRect(new SKRect(220f, 715f, 860f, 835f), 40f, 40f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(210f, 700f, 850f, 820f), 40f, 40f, FillPaint);
        TextPaint.TextSize = 42f; TextPaint.Color = SKColors.White;
        canvas.DrawText("\u2605  Resuelve para ganar estrellas  \u2605", 530f, 777f, TextPaint);

        TextPaint.TextSize = 132f; TextPaint.Color = new SKColor(18, 55, 112);
        if (_exercise is not null) canvas.DrawText($"{_exercise.LeftOperand} + {_exercise.RightOperand} = ?", 530f, 1075f, TextPaint);
        TextPaint.TextSize = 50f; TextPaint.Color = _messageColor;
        canvas.DrawText(_message, 530f, 1205f, TextPaint);
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Draw(canvas);
        if (_feedbackSequence.State == AnswerFeedbackState.Correct) DrawCelebration(canvas, _feedbackSequence.Progress);
    }

    public override void HandleInput(GameInput input)
    {
        if (TryHandleAudioButton(input, _audioService)) return;
        if (IsReleasedInside(input, BackButtonBounds))
        {
            _audioService.PlayEffect(AudioCue.Tap);
            _navigation.NavigateTo(GameScreen.Games);
            return;
        }
        if (_feedbackSequence.IsActive) return;
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].HandleInput(input);
    }

    private void LoadNextExercise()
    {
        _exercise = _exerciseGenerator.Generate(DifficultyLevel.Beginner);
        _selectedButton = null;
        _feedbackSequence.Reset();
        _message = "Elige la respuesta correcta";
        _messageColor = new(37, 75, 126);
        for (var index = 0; index < _answerButtons.Length; index++)
        {
            _answerButtons[index].Text = _exercise.Options[index].Value.ToString();
            _answerButtons[index].VisualState = ButtonVisualState.Normal;
            _answerButtons[index].IsEnabled = true;
        }
    }

    private GraphicButton CreateButton(GameRectangle bounds, SKColor color)
    {
        var button = new GraphicButton(bounds, string.Empty) { NormalColor = color, TextColor = new SKColor(18, 55, 112), TextSize = 92f, CornerRadius = 48f };
        button.Clicked += OnAnswerClicked;
        return button;
    }

    private void OnAnswerClicked(GraphicButton selectedButton)
    {
        if (_exercise is null || !int.TryParse(selectedButton.Text, out var answer)) return;
        var result = _sessionService.Evaluate(_exercise, answer, TimeSpan.Zero);
        _selectedButton = selectedButton;
        SetButtonsEnabled(false);
        selectedButton.PlayFeedbackAnimation();
        _feedbackSequence.Start(result.IsCorrect);
        if (result.IsCorrect)
        {
            _roundCorrectAnswers++;
            _state.RecordCorrectAnswer("addition");
            selectedButton.VisualState = ButtonVisualState.Correct;
            _message = "\u00A1Muy bien! \u2605";
            _messageColor = new(38, 170, 66);
            _audioService.PlayEffect(AudioCue.Correct);
        }
        else
        {
            _state.RecordAttempt("addition");
            selectedButton.VisualState = ButtonVisualState.Incorrect;
            _message = "Casi... int\u00E9ntalo otra vez";
            _messageColor = new(225, 78, 91);
            _audioService.PlayEffect(AudioCue.TryAgain);
        }
    }

    private void DrawRoundProgress(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(255, 255, 255, 235);
        canvas.DrawRoundRect(new SKRect(495f, 330f, 1015f, 650f), 70f, 70f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(485f, 315f, 1005f, 635f), 70f, 70f, FillPaint);
        for (var index = 0; index < 3; index++) DrawStar(canvas, 600f + index * 145f, 430f, 62f, index < Math.Min(_roundCorrectAnswers, 3) ? new SKColor(255, 187, 36) : new SKColor(220, 232, 239));
        FillPaint.Color = new SKColor(213, 239, 248);
        canvas.DrawRoundRect(new SKRect(565f, 535f, 875f, 580f), 22f, 22f, FillPaint);
        FillPaint.Color = new SKColor(116, 205, 46);
        var progressWidth = 310f * Math.Min(_roundCorrectAnswers, 3) / 3f;
        if (progressWidth > 0f) canvas.DrawRoundRect(new SKRect(565f, 535f, 565f + progressWidth, 580f), 22f, 22f, FillPaint);
        TextPaint.TextSize = 38f; TextPaint.Color = new SKColor(24, 58, 109);
        canvas.DrawText($"{Math.Min(_roundCorrectAnswers, 3)}/3", 940f, 575f, TextPaint);
    }

    private void SetButtonsEnabled(bool value) { for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].IsEnabled = value; }

    private void DrawCelebration(SKCanvas canvas, float progress)
    {
        var expansion = 130f + progress * 290f;
        for (var index = 0; index < 14; index++)
        {
            var angle = index * MathF.PI * 2f / 14f;
            _celebrationPaint.Color = (index % 3) switch { 0 => new SKColor(255, 193, 43), 1 => new SKColor(61, 194, 168), _ => new SKColor(245, 101, 151) };
            canvas.DrawCircle(530f + MathF.Cos(angle) * expansion, 1120f + MathF.Sin(angle) * expansion, 14f + (1f - progress) * 14f, _celebrationPaint);
        }
    }

    public override void Dispose()
    {
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Dispose();
        _celebrationPaint.Dispose();
        _dashedBorder.Dispose();
        _backdrop.Dispose();
        base.Dispose();
    }
}
