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

public sealed class AdditionBingoScene : KidsSceneBase
{
    private readonly IExerciseGenerator _exerciseGenerator;
    private readonly IGameSessionService _sessionService;
    private readonly IRandomProvider _randomProvider;
    private readonly IAudioService _audioService;
    private readonly GameNavigation _navigation;
    private readonly PlayerGameState _state;
    private readonly GraphicButton[] _cells = new GraphicButton[9];
    private readonly AnswerFeedbackSequence _feedbackSequence = new(1.45f, 1.9f);
    private readonly BingoFestivalBackdrop _backdrop = new();
    private readonly CondorGuide _condor = new();
    private readonly SpeechBubble _speechBubble = new(new SKRect(390f, 365f, 985f, 600f), SpeechTailSide.Left, new SKColor(255, 255, 255, 246), new SKColor(52, 67, 102), 31f);
    private MathExercise? _exercise;
    private GraphicButton? _selectedCell;
    private int _bingoPoints;
    private CondorMood _mood = CondorMood.Flying;
    private float _elapsed;

    public AdditionBingoScene(IExerciseGenerator exerciseGenerator, IGameSessionService sessionService, IRandomProvider randomProvider, IAudioService audioService, GameNavigation navigation, PlayerGameState state)
    {
        _exerciseGenerator = exerciseGenerator;
        _sessionService = sessionService;
        _randomProvider = randomProvider;
        _audioService = audioService;
        _navigation = navigation;
        _state = state;
        _speechBubble.Text = "Encuentra el resultado en el tablero";
        for (var index = 0; index < _cells.Length; index++)
        {
            var column = index % 3;
            var row = index / 3;
            var button = new GraphicButton(new(120f + column * 290f, 900f + row * 215f, 260f, 185f), string.Empty)
            {
                NormalColor = ((index + row) % 3) switch
                {
                    0 => new SKColor(104, 184, 245),
                    1 => new SKColor(157, 216, 73),
                    _ => new SKColor(255, 190, 54)
                },
                TextColor = new SKColor(18, 55, 112),
                TextSize = 72f,
                CornerRadius = 38f
            };
            button.Clicked += OnCellClicked;
            _cells[index] = button;
        }
    }

    public override GameScreen Screen => GameScreen.AdditionBingo;
    public override void Enter() { _bingoPoints = 0; LoadRound(); }
    public override void Exit() { }

    public override void Update(GameTime gameTime)
    {
        _backdrop.Update(gameTime);
        _condor.Update(gameTime);
        _elapsed += gameTime.DeltaSeconds;
        for (var index = 0; index < _cells.Length; index++) _cells[index].Update(gameTime);
        var completion = _feedbackSequence.Update(gameTime.DeltaSeconds);
        if (completion == AnswerFeedbackCompletion.Retry)
        {
            if (_selectedCell is not null) _selectedCell.VisualState = ButtonVisualState.Normal;
            _selectedCell = null;
            _mood = CondorMood.Flying;
            _speechBubble.Text = "Mira con calma y busca otra casilla";
            SetCellsEnabled(true);
        }
        else if (completion == AnswerFeedbackCompletion.NextExercise)
        {
            LoadRound();
        }
    }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        _backdrop.Draw(canvas, viewport);
        DrawBrandHeader(canvas, 165f, 0.88f);
        DrawCoinBadge(canvas, _state.Coins);
        DrawAudioButton(canvas);
        DrawBackButton(canvas);
        _condor.Draw(canvas, 190f, 455f, 0.55f, _mood);
        _speechBubble.Draw(canvas, MathF.Sin(_elapsed * 2f) * 3f);
        FillPaint.Color = new SKColor(255, 250, 235);
        canvas.DrawRoundRect(new SKRect(55f, 675f, 1025f, 1645f), 78f, 78f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(45f, 660f, 1015f, 1630f), 78f, 78f, FillPaint);
        FillPaint.Color = new SKColor(116, 75, 213);
        canvas.DrawRoundRect(new SKRect(250f, 605f, 830f, 730f), 42f, 42f, FillPaint);
        TextPaint.TextSize = 47f; TextPaint.Color = SKColors.White;
        canvas.DrawText("Bingo del C\u00F3ndor", 540f, 686f, TextPaint);
        TextPaint.TextSize = 77f; TextPaint.Color = new SKColor(18, 55, 112);
        if (_exercise is not null) canvas.DrawText($"{_exercise.LeftOperand} + {_exercise.RightOperand} = ?", 540f, 830f, TextPaint);
        for (var index = 0; index < _cells.Length; index++) _cells[index].Draw(canvas);
        TextPaint.TextSize = 36f; TextPaint.Color = new SKColor(34, 72, 122);
        canvas.DrawText($"BINGO: {_bingoPoints}/5", 540f, 1585f, TextPaint);
    }

    public override void HandleInput(GameInput input)
    {
        if (IsReleasedInside(input, BackButtonBounds))
        {
            _navigation.NavigateTo(GameScreen.Games);
            return;
        }
        if (_feedbackSequence.IsActive) return;
        for (var index = 0; index < _cells.Length; index++) _cells[index].HandleInput(input);
    }

    private void LoadRound()
    {
        _exercise = _exerciseGenerator.Generate(DifficultyLevel.Beginner);
        _selectedCell = null;
        _feedbackSequence.Reset();
        _mood = CondorMood.Flying;
        _speechBubble.Text = "Vuela con la mirada y encuentra el resultado";
        Span<int> values = stackalloc int[9];
        values[0] = _exercise.CorrectAnswer;
        for (var index = 1; index < values.Length; index++)
        {
            int candidate;
            do candidate = _randomProvider.Next(0, 21); while (values[..index].Contains(candidate));
            values[index] = candidate;
        }
        for (var index = values.Length - 1; index > 0; index--)
        {
            var swapIndex = _randomProvider.Next(0, index + 1);
            (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
        }
        for (var index = 0; index < _cells.Length; index++)
        {
            _cells[index].Text = values[index].ToString();
            _cells[index].VisualState = ButtonVisualState.Normal;
            _cells[index].IsEnabled = true;
        }
    }

    private void OnCellClicked(GraphicButton cell)
    {
        if (_exercise is null || !int.TryParse(cell.Text, out var answer)) return;
        var result = _sessionService.Evaluate(_exercise, answer, TimeSpan.Zero);
        _selectedCell = cell;
        SetCellsEnabled(false);
        cell.PlayFeedbackAnimation();
        _feedbackSequence.Start(result.IsCorrect);
        if (result.IsCorrect)
        {
            _bingoPoints = Math.Min(5, _bingoPoints + 1);
            _state.RecordCorrectAnswer("addition_bingo");
            cell.VisualState = ButtonVisualState.Correct;
            _mood = CondorMood.Happy;
            _speechBubble.Text = _bingoPoints == 5 ? "\u00A1BINGO! Llegaste a la cima" : "\u00A1Gran vuelo! Casilla correcta";
            _audioService.PlayEffect(AudioCue.Correct);
        }
        else
        {
            _state.RecordAttempt("addition_bingo");
            cell.VisualState = ButtonVisualState.Incorrect;
            _mood = CondorMood.Encouraging;
            _speechBubble.Text = "Buen intento. Abre tus alas y prueba otra";
            _audioService.PlayEffect(AudioCue.TryAgain);
        }
    }

    private void SetCellsEnabled(bool value) { for (var index = 0; index < _cells.Length; index++) _cells[index].IsEnabled = value; }

    public override void Dispose()
    {
        for (var index = 0; index < _cells.Length; index++) _cells[index].Dispose();
        _backdrop.Dispose();
        _condor.Dispose();
        _speechBubble.Dispose();
        base.Dispose();
    }
}
