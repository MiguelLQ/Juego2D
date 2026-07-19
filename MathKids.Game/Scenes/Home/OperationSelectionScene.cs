using MathKids.Application.Abstractions;
using MathKids.Domain.Exercises;
using MathKids.Game.Common;
using MathKids.Game.Core;
using MathKids.Game.Graphics.Drawing;
using MathKids.Game.Input.Touch;
using MathKids.Game.Scenes.Abstractions;
using SkiaSharp;

namespace MathKids.Game.Scenes.Home;

public sealed class OperationSelectionScene : KidsSceneBase
{
    private static readonly GameRectangle AdditionBounds = new(90f, 590f, 420f, 365f);
    private static readonly GameRectangle SubtractionBounds = new(570f, 590f, 420f, 365f);
    private static readonly GameRectangle MultiplicationBounds = new(90f, 1010f, 420f, 365f);
    private static readonly GameRectangle DivisionBounds = new(570f, 1010f, 420f, 365f);
    private readonly GameNavigation _navigation;
    private readonly GameSelectionState _selection;
    private readonly PlayerGameState _state;
    private readonly IAudioService _audioService;
    private readonly AndeanDashboardBackdrop _backdrop = new();
    private float _elapsed;

    public OperationSelectionScene(GameNavigation navigation, GameSelectionState selection, PlayerGameState state, IAudioService audioService)
    {
        _navigation = navigation;
        _selection = selection;
        _state = state;
        _audioService = audioService;
    }

    public override GameScreen Screen => GameScreen.OperationSelection;
    public override void Enter() => _audioService.PlayMusic(MusicCue.Home);
    public override void Exit() { }
    public override void Update(GameTime gameTime) { _elapsed += gameTime.DeltaSeconds; _backdrop.Update(gameTime); }

    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        _backdrop.Draw(canvas, viewport);
        DrawBackButton(canvas);
        DrawCoinBadge(canvas, _state.Coins);
        DrawAudioButton(canvas, _audioService.IsMuted);
        FillPaint.Color = new SKColor(255, 250, 236, 246);
        canvas.DrawRoundRect(new SKRect(55f, 305f, 1025f, 1505f), 76f, 76f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(43f, 290f, 1013f, 1490f), 76f, 76f, FillPaint);
        TextPaint.TextSize = 49f; TextPaint.Color = new SKColor(39, 75, 119);
        canvas.DrawText(_selection.ModuleTitle, 540f, 405f, TextPaint);
        TextPaint.TextSize = 34f; TextPaint.Color = new SKColor(83, 101, 122);
        canvas.DrawText("\u00BFQu\u00E9 quieres practicar?", 540f, 475f, TextPaint);
        DrawOperationCard(canvas, AdditionBounds, MathOperation.Addition, new SKColor(72, 174, 225));
        DrawOperationCard(canvas, SubtractionBounds, MathOperation.Subtraction, new SKColor(242, 132, 91));
        DrawOperationCard(canvas, MultiplicationBounds, MathOperation.Multiplication, new SKColor(123, 91, 201));
        DrawOperationCard(canvas, DivisionBounds, MathOperation.Division, new SKColor(78, 184, 129));
        TextPaint.TextSize = 28f; TextPaint.Color = new SKColor(72, 91, 112);
        canvas.DrawText("Puedes volver y elegir otra operaci\u00F3n cuando quieras", 540f, 1445f, TextPaint);
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
        if (IsReleasedInside(input, AdditionBounds)) Select(MathOperation.Addition);
        else if (IsReleasedInside(input, SubtractionBounds)) Select(MathOperation.Subtraction);
        else if (IsReleasedInside(input, MultiplicationBounds)) Select(MathOperation.Multiplication);
        else if (IsReleasedInside(input, DivisionBounds)) Select(MathOperation.Division);
    }

    private void Select(MathOperation operation)
    {
        _selection.SelectOperation(operation);
        _audioService.PlayEffect(AudioCue.Tap);
        _navigation.NavigateTo(_selection.TargetScreen);
    }

    private void DrawOperationCard(SKCanvas canvas, GameRectangle bounds, MathOperation operation, SKColor color)
    {
        var lift = MathF.Sin(_elapsed * 2f + (int)operation) * 4f;
        canvas.DrawRoundRect(new SKRect(bounds.Left + 8f, bounds.Top + 15f + lift, bounds.Right + 8f, bounds.Bottom + 15f + lift), 55f, 55f, ShadowPaint);
        FillPaint.Color = color;
        canvas.DrawRoundRect(new SKRect(bounds.Left, bounds.Top + lift, bounds.Right, bounds.Bottom + lift), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(255, 255, 255, 225);
        canvas.DrawCircle(bounds.CenterX, bounds.Top + 125f + lift, 78f, FillPaint);
        TextPaint.TextSize = 92f; TextPaint.Color = color;
        canvas.DrawText(operation.Symbol(), bounds.CenterX, bounds.Top + 158f + lift, TextPaint);
        TextPaint.TextSize = operation == MathOperation.Multiplication ? 36f : 42f; TextPaint.Color = SKColors.White;
        canvas.DrawText(operation.DisplayName(), bounds.CenterX, bounds.Top + 285f + lift, TextPaint);
    }

    public override void Dispose() { _backdrop.Dispose(); base.Dispose(); }
}
