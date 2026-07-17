using SkiaSharp;

namespace MathKids.Game.Components;

public enum SpeechTailSide { Left, Right }

public sealed class SpeechBubble : IDisposable
{
    private readonly SKPaint _fill = new() { IsAntialias = true };
    private readonly SKPaint _shadow = new() { IsAntialias = true, Color = new SKColor(30, 48, 70, 42) };
    private readonly SKPaint _text = new() { IsAntialias = true, TextAlign = SKTextAlign.Center, Typeface = SKTypeface.FromFamilyName("sans-serif", SKFontStyle.Bold) };
    private readonly SKPath _tail = new();
    private readonly SKRect _bounds;
    private readonly SpeechTailSide _tailSide;
    private readonly float _preferredTextSize;
    private string[] _lines = [];

    public SpeechBubble(SKRect bounds, SpeechTailSide tailSide, SKColor background, SKColor textColor, float textSize = 32f)
    {
        _bounds = bounds;
        _tailSide = tailSide;
        _fill.Color = background;
        _text.Color = textColor;
        _text.TextSize = textSize;
        _preferredTextSize = textSize;
    }

    public string Text
    {
        set => _lines = Wrap(value, _bounds.Width - 80f, 3);
    }

    public void Draw(SKCanvas canvas, float offsetY = 0f)
    {
        var rect = new SKRect(_bounds.Left, _bounds.Top + offsetY, _bounds.Right, _bounds.Bottom + offsetY);
        var shadowRect = new SKRect(rect.Left + 10f, rect.Top + 13f, rect.Right + 10f, rect.Bottom + 13f);
        canvas.DrawRoundRect(shadowRect, 50f, 50f, _shadow);
        canvas.DrawRoundRect(rect, 50f, 50f, _fill);
        _tail.Reset();
        if (_tailSide == SpeechTailSide.Left)
        {
            _tail.MoveTo(rect.Left + 65f, rect.Bottom - 48f); _tail.LineTo(rect.Left - 62f, rect.Bottom + 35f); _tail.LineTo(rect.Left + 105f, rect.Bottom - 5f);
        }
        else
        {
            _tail.MoveTo(rect.Right - 65f, rect.Bottom - 48f); _tail.LineTo(rect.Right + 62f, rect.Bottom + 35f); _tail.LineTo(rect.Right - 105f, rect.Bottom - 5f);
        }
        _tail.Close();
        canvas.DrawPath(_tail, _fill);

        var lineHeight = _text.TextSize * 1.25f;
        var totalHeight = Math.Max(1, _lines.Length) * lineHeight;
        var baseline = rect.MidY - totalHeight / 2f - _text.FontMetrics.Ascent;
        for (var index = 0; index < _lines.Length; index++) canvas.DrawText(_lines[index], rect.MidX, baseline + index * lineHeight, _text);
    }

    private string[] Wrap(string text, float maxWidth, int maxLines)
    {
        _text.TextSize = _preferredTextSize;
        if (string.IsNullOrWhiteSpace(text)) return [];
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>(maxLines);
        var current = words[0];
        for (var index = 1; index < words.Length; index++)
        {
            var candidate = current + " " + words[index];
            if (_text.MeasureText(candidate) <= maxWidth || lines.Count == maxLines - 1) current = candidate;
            else { lines.Add(current); current = words[index]; }
        }
        lines.Add(current);
        while (lines.Count > maxLines)
        {
            lines[^2] += " " + lines[^1];
            lines.RemoveAt(lines.Count - 1);
        }
        if (lines.Any(line => _text.MeasureText(line) > maxWidth))
        {
            while (_text.TextSize > 23f && lines.Any(line => _text.MeasureText(line) > maxWidth)) _text.TextSize -= 1f;
        }
        return [.. lines];
    }

    public void Dispose() { _fill.Dispose(); _shadow.Dispose(); _text.Dispose(); _tail.Dispose(); }
}
