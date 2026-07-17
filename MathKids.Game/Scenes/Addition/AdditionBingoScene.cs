using MathKids.Application.Abstractions;
using MathKids.Application.Exercises;
using MathKids.Application.Sessions;
using MathKids.Domain.Exercises;
using MathKids.Game.Common;
using MathKids.Game.Components.Buttons;
using MathKids.Game.Core;
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
    private readonly AnswerFeedbackSequence _feedbackSequence = new();
    private MathExercise? _exercise;
    private GraphicButton? _selectedCell;
    private int _bingoPoints;
    private string _message = "Encuentra el resultado en el tablero";
    private SKColor _messageColor = new(35, 73, 123);

    public AdditionBingoScene(IExerciseGenerator exerciseGenerator, IGameSessionService sessionService, IRandomProvider randomProvider, IAudioService audioService, GameNavigation navigation, PlayerGameState state)
    {
        _exerciseGenerator = exerciseGenerator;
        _sessionService = sessionService;
        _randomProvider = randomProvider;
        _audioService = audioService;
        _navigation = navigation;
        _state = state;
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
        for (var index = 0; index < _cells.Length; index++) _cells[index].Update(gameTime);
        var completion = _feedbackSequence.Update(gameTime.DeltaSeconds);
        if (completion == AnswerFeedbackCompletion.Retry)
        {
            if (_selectedCell is not null) _selectedCell.VisualState = ButtonVisualState.Normal;
            _selectedCell = null;
            _message = "Busca otra casilla";
            _messageColor = new(35, 73, 123);
            SetCellsEnabled(true);
        }
        else if (completion == AnswerFeedbackCompletion.NextExercise)
        {
            LoadRound();
        }
    }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        DrawWorldBackground(canvas, viewport);
        DrawBrandHeader(canvas, 165f, 0.88f);
        DrawCoinBadge(canvas, _state.Coins);
        DrawBackButton(canvas);
        FillPaint.Color = new SKColor(255, 250, 235);
        canvas.DrawRoundRect(new SKRect(55f, 350f, 1025f, 1645f), 78f, 78f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(45f, 335f, 1015f, 1630f), 78f, 78f, FillPaint);
        FillPaint.Color = new SKColor(116, 75, 213);
        canvas.DrawRoundRect(new SKRect(250f, 285f, 830f, 410f), 42f, 42f, FillPaint);
        TextPaint.TextSize = 48f; TextPaint.Color = SKColors.White;
        canvas.DrawText("Bingo de sumas", 540f, 365f, TextPaint);

        TextPaint.TextSize = 42f; TextPaint.Color = new SKColor(63, 91, 127);
        canvas.DrawText("Encuentra en el tablero:", 540f, 515f, TextPaint);
        TextPaint.TextSize = 105f; TextPaint.Color = new SKColor(18, 55, 112);
        if (_exercise is not null) canvas.DrawText($"{_exercise.LeftOperand} + {_exercise.RightOperand} = ?", 540f, 675f, TextPaint);
        TextPaint.TextSize = 42f; TextPaint.Color = _messageColor;
        canvas.DrawText(_message, 540f, 790f, TextPaint);
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
        _message = "Encuentra el resultado en el tablero";
        _messageColor = new(35, 73, 123);
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
            _message = _bingoPoints == 5 ? "\u00A1BINGO! \u2605" : "\u00A1Casilla correcta!";
            _messageColor = new(38, 170, 66);
            _audioService.PlayEffect(AudioCue.Correct);
        }
        else
        {
            _state.RecordAttempt("addition_bingo");
            cell.VisualState = ButtonVisualState.Incorrect;
            _message = "Esa no es, prueba otra";
            _messageColor = new(225, 78, 91);
            _audioService.PlayEffect(AudioCue.TryAgain);
        }
    }

    private void SetCellsEnabled(bool value) { for (var index = 0; index < _cells.Length; index++) _cells[index].IsEnabled = value; }

    public override void Dispose()
    {
        for (var index = 0; index < _cells.Length; index++) _cells[index].Dispose();
        base.Dispose();
    }
}
