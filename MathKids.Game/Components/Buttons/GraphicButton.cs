using MathKids.Game.Common;
using MathKids.Game.Core;
using MathKids.Game.Graphics.Animation;
using MathKids.Game.Input.HitTesting;
using MathKids.Game.Input.Touch;
using SkiaSharp;

namespace MathKids.Game.Components.Buttons;

public sealed class GraphicButton : GraphicComponent
{
    private readonly SKPaint _fillPaint = new() { IsAntialias = true };
    private readonly SKPaint _shadowPaint = new() { IsAntialias = true, Color = new SKColor(34, 55, 92, 45) };
    private readonly SKPaint _textPaint = new() { IsAntialias = true, Color = SKColors.White, TextAlign = SKTextAlign.Center, Typeface = SKTypeface.FromFamilyName("sans-serif", SKFontStyle.Bold) };
    private readonly FloatTween _scaleTween = new();
    private bool _pressed;
    private ButtonAnimationPhase _animationPhase;
    public GraphicButton(GameRectangle bounds, string text) : base(bounds) { Text = text; _scaleTween.Start(1f, 1f, 0.001f); }
    public string Text { get; set; }
    public ButtonVisualState VisualState { get; set; }
    public SKColor NormalColor { get; set; } = new(90, 126, 242);
    public SKColor TextColor { get; set; } = SKColors.White;
    public float TextSize { get; set; } = 82f;
    public float CornerRadius { get; set; } = 42f;
    public event Action<GraphicButton>? Clicked;

    public override void Update(GameTime gameTime)
    {
        _scaleTween.Update(gameTime.DeltaSeconds);
        if (_scaleTween.IsRunning)
        {
            return;
        }

        if (_animationPhase == ButtonAnimationPhase.PulseOut)
        {
            _animationPhase = ButtonAnimationPhase.PulseBack;
            _scaleTween.Start(_scaleTween.Value, 1f, 0.18f);
        }
        else if (_animationPhase == ButtonAnimationPhase.PulseBack)
        {
            _animationPhase = ButtonAnimationPhase.None;
        }
    }

    public void PlayFeedbackAnimation()
    {
        _animationPhase = ButtonAnimationPhase.PulseOut;
        _scaleTween.Start(_scaleTween.Value, 1.12f, 0.12f);
    }
    public void HandleInput(GameInput input)
    {
        if (!IsEnabled) return;
        var contains = HitTest.Contains(Bounds, input.Position);
        if (input.Type == GameInputType.Pressed && contains)
        {
            _pressed = true;
            _scaleTween.Start(_scaleTween.Value, 0.93f, 0.08f);
        }
        else if (input.Type is GameInputType.Released or GameInputType.Cancelled)
        {
            var shouldClick = _pressed && contains && input.Type == GameInputType.Released;
            _pressed = false;
            _scaleTween.Start(_scaleTween.Value, 1f, 0.16f);
            if (shouldClick) Clicked?.Invoke(this);
        }
    }

    public override void Draw(SKCanvas canvas)
    {
        if (!IsVisible) return;
        _fillPaint.Color = VisualState switch
        {
            ButtonVisualState.Correct => new SKColor(64, 190, 128),
            ButtonVisualState.Incorrect => new SKColor(244, 114, 111),
            _ => NormalColor
        };
        _textPaint.Color = TextColor;
        var scale = _scaleTween.Value;
        canvas.Save();
        canvas.Scale(scale, scale, Bounds.CenterX, Bounds.CenterY);
        var shadow = new SKRect(Bounds.Left, Bounds.Top + 18f, Bounds.Right, Bounds.Bottom + 18f);
        var rectangle = new SKRect(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Bottom);
        canvas.DrawRoundRect(shadow, CornerRadius, CornerRadius, _shadowPaint);
        canvas.DrawRoundRect(rectangle, CornerRadius, CornerRadius, _fillPaint);
        _textPaint.TextSize = TextSize;
        var metrics = _textPaint.FontMetrics;
        var baseline = Bounds.CenterY - (metrics.Ascent + metrics.Descent) / 2f;
        canvas.DrawText(Text, Bounds.CenterX, baseline, _textPaint);
        canvas.Restore();
    }

    public override void Dispose() { _fillPaint.Dispose(); _shadowPaint.Dispose(); _textPaint.Dispose(); }
}

public enum ButtonVisualState { Normal, Correct, Incorrect }

internal enum ButtonAnimationPhase { None, PulseOut, PulseBack }
