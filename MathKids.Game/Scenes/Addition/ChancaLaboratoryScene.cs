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

public sealed class ChancaLaboratoryScene : KidsSceneBase
{
    private readonly IExerciseGenerator _exerciseGenerator;
    private readonly IGameSessionService _sessionService;
    private readonly IAudioService _audioService;
    private readonly GameNavigation _navigation;
    private readonly PlayerGameState _state;
    private readonly ChancaLaboratoryBackdrop _backdrop = new();
    private readonly ChancaGuide _guide = new();
    private readonly AnswerFeedbackSequence _feedback = new(1.9f, 2.3f);
    private readonly GraphicButton[] _answerButtons;
    private readonly SKPath _tubePath = new();
    private readonly SpeechBubble _speechBubble = new(new SKRect(84f, 273f, 639f, 510f), SpeechTailSide.Right, new SKColor(255, 248, 220, 248), new SKColor(67, 48, 70), 30f);
    private MathExercise? _exercise;
    private GraphicButton? _selectedButton;
    private ChancaMood _mood = ChancaMood.Curious;
    private int _selectedValue;
    private float _elapsed;

    public ChancaLaboratoryScene(IExerciseGenerator exerciseGenerator, IGameSessionService sessionService, IAudioService audioService, GameNavigation navigation, PlayerGameState state)
    {
        _exerciseGenerator = exerciseGenerator;
        _sessionService = sessionService;
        _audioService = audioService;
        _navigation = navigation;
        _state = state;
        _speechBubble.Text = "Activa el laboratorio con una respuesta";
        _answerButtons =
        [
            CreateButton(new(92f, 850f, 190f, 170f), new SKColor(239, 86, 143)),
            CreateButton(new(92f, 1060f, 190f, 170f), new SKColor(73, 190, 225)),
            CreateButton(new(92f, 1270f, 190f, 170f), new SKColor(245, 177, 49))
        ];
    }

    public override GameScreen Screen => GameScreen.ChancaLaboratory;
    public override void Enter() => LoadNextExperiment();
    public override void Exit() { }

    public override void Update(GameTime gameTime)
    {
        _elapsed += gameTime.DeltaSeconds;
        _backdrop.Update(gameTime);
        _guide.Update(gameTime);
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Update(gameTime);
        var completion = _feedback.Update(gameTime.DeltaSeconds);
        if (completion == AnswerFeedbackCompletion.Retry)
        {
            if (_selectedButton is not null) _selectedButton.VisualState = ButtonVisualState.Normal;
            _selectedButton = null;
            _mood = ChancaMood.Curious;
            SetDialog("Observa la suma y prueba otra esfera");
            SetButtonsEnabled(true);
        }
        else if (completion == AnswerFeedbackCompletion.NextExercise)
        {
            LoadNextExperiment();
        }
    }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        _backdrop.Draw(canvas, viewport);
        DrawBackButton(canvas);
        DrawCoinBadge(canvas, _state.Coins);
        DrawAudioButton(canvas);
        DrawHeader(canvas);
        _guide.Draw(canvas, 790f, 265f, 0.62f, _mood);
        DrawSpeechBubble(canvas);
        DrawLaboratory(canvas);
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Draw(canvas);
        if (_feedback.IsActive) DrawTravelingSphere(canvas);
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

    private GraphicButton CreateButton(GameRectangle bounds, SKColor color)
    {
        var button = new GraphicButton(bounds, string.Empty)
        {
            NormalColor = color,
            TextColor = SKColors.White,
            TextSize = 72f,
            CornerRadius = 85f
        };
        button.Clicked += OnAnswerClicked;
        return button;
    }

    private void LoadNextExperiment()
    {
        _exercise = _exerciseGenerator.Generate(DifficultyLevel.Beginner);
        _selectedButton = null;
        _selectedValue = 0;
        _feedback.Reset();
        _mood = ChancaMood.Curious;
        SetDialog("Elige la esfera que completa la suma");
        for (var index = 0; index < _answerButtons.Length; index++)
        {
            _answerButtons[index].Text = _exercise.Options[index].Value.ToString();
            _answerButtons[index].VisualState = ButtonVisualState.Normal;
            _answerButtons[index].IsEnabled = true;
        }
    }

    private void OnAnswerClicked(GraphicButton button)
    {
        if (_exercise is null || !int.TryParse(button.Text, out var answer)) return;
        var result = _sessionService.Evaluate(_exercise, answer, TimeSpan.Zero);
        _selectedButton = button;
        _selectedValue = answer;
        button.PlayFeedbackAnimation();
        SetButtonsEnabled(false);
        _feedback.Start(result.IsCorrect);
        if (result.IsCorrect)
        {
            _state.RecordCorrectAnswer("chanca_laboratory");
            button.VisualState = ButtonVisualState.Correct;
            _mood = ChancaMood.Celebrating;
            SetDialog("\u00A1Experimento logrado! Tu mente es brillante");
            _audioService.PlayEffect(AudioCue.Correct);
        }
        else
        {
            _state.RecordAttempt("chanca_laboratory");
            button.VisualState = ButtonVisualState.Incorrect;
            _mood = ChancaMood.Encouraging;
            SetDialog("Buen intento. Cada prueba nos ense\u00F1a algo");
            _audioService.PlayEffect(AudioCue.TryAgain);
        }
    }

    private void DrawHeader(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(41, 27, 58, 220);
        canvas.DrawRoundRect(new SKRect(205f, 45f, 800f, 184f), 42f, 42f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(195f, 35f, 790f, 174f), 42f, 42f, FillPaint);
        StrokePaint.Color = new SKColor(240, 178, 62); StrokePaint.StrokeWidth = 5f;
        canvas.DrawRoundRect(new SKRect(208f, 48f, 777f, 161f), 34f, 34f, StrokePaint);
        TextPaint.TextSize = 47f; TextPaint.Color = new SKColor(255, 207, 96);
        canvas.DrawText("El Laboratorio Chanka", 492f, 122f, TextPaint);
    }

    private void DrawSpeechBubble(SKCanvas canvas)
    {
        _speechBubble.Draw(canvas, MathF.Sin(_elapsed * 2.1f) * 3f);
    }

    private void DrawLaboratory(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(31, 23, 48, 115);
        canvas.DrawRoundRect(new SKRect(48f, 650f, 1032f, 1645f), 62f, 62f, FillPaint);
        FillPaint.Color = new SKColor(59, 45, 79, 245);
        canvas.DrawRoundRect(new SKRect(35f, 630f, 1019f, 1625f), 62f, 62f, FillPaint);
        StrokePaint.Color = new SKColor(71, 220, 207); StrokePaint.StrokeWidth = 24f;
        _tubePath.Reset();
        _tubePath.MoveTo(280f, 930f); _tubePath.CubicTo(390f, 930f, 335f, 720f, 470f, 720f);
        _tubePath.LineTo(760f, 720f); _tubePath.CubicTo(900f, 720f, 820f, 875f, 900f, 920f);
        canvas.DrawPath(_tubePath, StrokePaint);
        StrokePaint.Color = new SKColor(188, 255, 244, 145); StrokePaint.StrokeWidth = 7f;
        canvas.DrawPath(_tubePath, StrokePaint);

        FillPaint.Color = new SKColor(36, 28, 47);
        canvas.DrawRoundRect(new SKRect(345f, 780f, 735f, 1015f), 30f, 30f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(335f, 765f, 725f, 1000f), 30f, 30f, FillPaint);
        StrokePaint.Color = new SKColor(236, 177, 70); StrokePaint.StrokeWidth = 5f;
        canvas.DrawRoundRect(new SKRect(353f, 783f, 707f, 982f), 22f, 22f, StrokePaint);
        TextPaint.TextSize = 83f; TextPaint.Color = new SKColor(255, 244, 206);
        if (_exercise is not null) canvas.DrawText($"{_exercise.LeftOperand} + {_exercise.RightOperand} = ?", 530f, 915f, TextPaint);

        TextPaint.TextSize = 28f; TextPaint.Color = new SKColor(255, 218, 117);
        canvas.DrawText("ESFERAS", 187f, 795f, TextPaint);
        DrawReactionVessel(canvas);
        DrawControlLights(canvas);
    }

    private void DrawReactionVessel(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(24, 49, 66, 200);
        canvas.DrawRoundRect(new SKRect(735f, 1000f, 970f, 1500f), 45f, 45f, ShadowPaint);
        FillPaint.Color = new SKColor(70, 225, 191, 185);
        canvas.DrawRoundRect(new SKRect(720f, 980f, 955f, 1480f), 45f, 45f, FillPaint);
        FillPaint.Color = new SKColor(36, 120, 130, 150);
        canvas.DrawRect(738f, 1160f, 937f, 1450f, FillPaint);
        StrokePaint.Color = new SKColor(231, 255, 240); StrokePaint.StrokeWidth = 5f;
        for (var y = 1080f; y < 1410f; y += 72f) canvas.DrawLine(855f, y, 925f, y, StrokePaint);
        DrawIngredientOrb(canvas, 780f, 1405f, _exercise?.LeftOperand ?? 0, new SKColor(247, 169, 54));
        DrawIngredientOrb(canvas, 862f, 1405f, _exercise?.RightOperand ?? 0, new SKColor(224, 80, 135));
        for (var index = 0; index < 7; index++)
        {
            var rise = (_elapsed * 75f + index * 63f) % 240f;
            FillPaint.Color = new SKColor(235, 255, 242, 130);
            canvas.DrawCircle(755f + index % 3 * 70f, 1360f - rise, 7f + index % 2 * 4f, FillPaint);
        }
    }

    private void DrawIngredientOrb(SKCanvas canvas, float x, float y, int value, SKColor color)
    {
        FillPaint.Color = new SKColor(22, 35, 48, 90); canvas.DrawCircle(x + 4f, y + 7f, 34f, FillPaint);
        FillPaint.Color = color; canvas.DrawCircle(x, y, 34f, FillPaint);
        TextPaint.TextSize = 27f; TextPaint.Color = SKColors.White; canvas.DrawText(value.ToString(), x, y + 10f, TextPaint);
    }

    private void DrawControlLights(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(33, 27, 45);
        canvas.DrawRoundRect(new SKRect(330f, 1080f, 650f, 1510f), 35f, 35f, FillPaint);
        TextPaint.TextSize = 27f; TextPaint.Color = new SKColor(217, 203, 176);
        canvas.DrawText("ENERG\u00CDA CHANCA", 490f, 1140f, TextPaint);
        for (var index = 0; index < 4; index++)
        {
            FillPaint.Color = index % 2 == 0 ? new SKColor(244, 177, 53) : new SKColor(72, 213, 192);
            var glow = 17f + MathF.Sin(_elapsed * 3f + index) * 4f;
            canvas.DrawCircle(395f + index % 2 * 185f, 1245f + index / 2 * 145f, glow, FillPaint);
        }
    }

    private void DrawTravelingSphere(SKCanvas canvas)
    {
        var progress = _feedback.Progress;
        float x;
        float y;
        if (progress < 0.45f)
        {
            var local = progress / 0.45f;
            x = Lerp(_selectedButton?.Bounds.CenterX ?? 185f, 500f, local);
            y = Lerp(_selectedButton?.Bounds.CenterY ?? 1050f, 690f, local);
        }
        else
        {
            var local = (progress - 0.45f) / 0.55f;
            x = Lerp(500f, 835f, local);
            y = Lerp(690f, 1110f, local);
        }
        FillPaint.Color = new SKColor(255, 255, 255, 75);
        canvas.DrawCircle(x, y, 59f, FillPaint);
        FillPaint.Color = _feedback.State == AnswerFeedbackState.Correct ? new SKColor(80, 224, 151) : new SKColor(241, 103, 111);
        canvas.DrawCircle(x, y, 45f, FillPaint);
        TextPaint.TextSize = 39f; TextPaint.Color = SKColors.White;
        canvas.DrawText(_selectedValue.ToString(), x, y + 14f, TextPaint);
        for (var index = 0; index < 6; index++)
        {
            var angle = index * MathF.PI / 3f + _elapsed * 3f;
            FillPaint.Color = new SKColor(255, 216, 91, 170);
            canvas.DrawCircle(x + MathF.Cos(angle) * 72f, y + MathF.Sin(angle) * 72f, 7f, FillPaint);
        }
    }

    private static float Lerp(float start, float end, float amount) => start + (end - start) * Math.Clamp(amount, 0f, 1f);

    private void SetDialog(string text) => _speechBubble.Text = text;

    private void SetButtonsEnabled(bool enabled)
    {
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].IsEnabled = enabled;
    }

    public override void Dispose()
    {
        for (var index = 0; index < _answerButtons.Length; index++) _answerButtons[index].Dispose();
        _tubePath.Dispose(); _speechBubble.Dispose(); _guide.Dispose(); _backdrop.Dispose();
        base.Dispose();
    }
}
