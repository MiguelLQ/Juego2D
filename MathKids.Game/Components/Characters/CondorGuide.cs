using MathKids.Game.Core;
using SkiaSharp;

namespace MathKids.Game.Components.Characters;

public enum CondorMood { Flying, Happy, Encouraging }

public sealed class CondorGuide : IDisposable
{
    private readonly SKPaint _fill = new() { IsAntialias = true };
    private readonly SKPaint _stroke = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round, StrokeJoin = SKStrokeJoin.Round };
    private readonly SKPath _path = new();
    private float _elapsed;

    public void Update(GameTime gameTime) => _elapsed += gameTime.DeltaSeconds;

    public void Draw(SKCanvas canvas, float x, float y, float scale, CondorMood mood)
    {
        var flap = MathF.Sin(_elapsed * (mood == CondorMood.Happy ? 7f : 3.2f));
        var bob = MathF.Sin(_elapsed * 2f) * 9f;
        canvas.Save(); canvas.Translate(x, y + bob); canvas.Scale(scale);
        _fill.Color = new SKColor(38, 42, 52);
        DrawWing(canvas, -1f, flap); DrawWing(canvas, 1f, flap);
        canvas.DrawOval(new SKRect(-92f, -80f, 92f, 180f), _fill);
        _fill.Color = SKColors.White;
        canvas.DrawOval(new SKRect(-98f, -48f, 98f, 30f), _fill);
        _fill.Color = new SKColor(213, 119, 102);
        canvas.DrawOval(new SKRect(-68f, -140f, 68f, -15f), _fill);
        _fill.Color = new SKColor(246, 183, 58);
        _path.Reset(); _path.MoveTo(45f, -92f); _path.LineTo(135f, -65f); _path.LineTo(48f, -35f); _path.Close(); canvas.DrawPath(_path, _fill);
        _fill.Color = SKColors.White; canvas.DrawCircle(-22f, -87f, 22f, _fill); canvas.DrawCircle(24f, -87f, 22f, _fill);
        _fill.Color = new SKColor(28, 31, 38); canvas.DrawCircle(-18f, -85f, 8f, _fill); canvas.DrawCircle(28f, -85f, 8f, _fill);
        _stroke.Color = new SKColor(86, 46, 43); _stroke.StrokeWidth = 7f;
        if (mood == CondorMood.Happy) canvas.DrawArc(new SKRect(-28f, -58f, 44f, -8f), 10f, 155f, false, _stroke);
        else if (mood == CondorMood.Encouraging) canvas.DrawLine(-5f, -24f, 35f, -24f, _stroke);
        _fill.Color = new SKColor(246, 183, 58);
        canvas.DrawRoundRect(new SKRect(-65f, 165f, -15f, 205f), 15f, 15f, _fill); canvas.DrawRoundRect(new SKRect(15f, 165f, 65f, 205f), 15f, 15f, _fill);
        canvas.Restore();
    }

    private void DrawWing(SKCanvas canvas, float side, float flap)
    {
        var tipY = 30f + flap * 78f;
        _path.Reset(); _path.MoveTo(side * 65f, -25f); _path.CubicTo(side * 190f, -120f, side * 285f, tipY - 65f, side * 340f, tipY); _path.CubicTo(side * 245f, tipY + 5f, side * 165f, 100f, side * 62f, 105f); _path.Close();
        canvas.DrawPath(_path, _fill);
        _stroke.Color = new SKColor(245, 245, 235); _stroke.StrokeWidth = 11f;
        for (var index = 1; index <= 3; index++) canvas.DrawLine(side * (125f + index * 48f), tipY - 15f, side * (105f + index * 35f), tipY + 45f, _stroke);
    }

    public void Dispose() { _fill.Dispose(); _stroke.Dispose(); _path.Dispose(); }
}
