using MathKids.Game.Core;
using SkiaSharp;

namespace MathKids.Game.Components.Characters;

public enum PumaMood { Thinking, Happy, Encouraging }

public sealed class PumaGuide : IDisposable
{
    private readonly SKPaint _fill = new() { IsAntialias = true };
    private readonly SKPaint _stroke = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round, StrokeJoin = SKStrokeJoin.Round };
    private readonly SKPath _path = new();
    private float _elapsed;

    public void Update(GameTime gameTime) => _elapsed += gameTime.DeltaSeconds;

    public void Draw(SKCanvas canvas, float x, float y, float scale, PumaMood mood)
    {
        var bob = MathF.Sin(_elapsed * 2.2f) * 8f;
        canvas.Save();
        canvas.Translate(x, y + bob);
        canvas.Scale(scale);
        _stroke.Color = new SKColor(112, 66, 34); _stroke.StrokeWidth = 14f;
        canvas.DrawArc(new SKRect(-155f, 95f, 130f, 330f), 135f, 205f, false, _stroke);
        _fill.Color = new SKColor(213, 153, 73);
        canvas.DrawOval(new SKRect(-118f, 30f, 128f, 280f), _fill);
        canvas.DrawCircle(-83f, 32f, 58f, _fill); canvas.DrawCircle(83f, 32f, 58f, _fill);
        _fill.Color = new SKColor(244, 191, 114);
        canvas.DrawCircle(-83f, 32f, 28f, _fill); canvas.DrawCircle(83f, 32f, 28f, _fill);
        _fill.Color = new SKColor(226, 168, 91);
        canvas.DrawOval(new SKRect(-145f, 20f, 145f, 230f), _fill);
        _fill.Color = new SKColor(250, 220, 174);
        canvas.DrawOval(new SKRect(-100f, 112f, 100f, 232f), _fill);
        _fill.Color = SKColors.White;
        canvas.DrawOval(new SKRect(-82f, 72f, -17f, 142f), _fill); canvas.DrawOval(new SKRect(17f, 72f, 82f, 142f), _fill);
        var blink = MathF.Sin(_elapsed * 0.72f) > 0.985f;
        _fill.Color = new SKColor(38, 31, 27);
        if (blink)
        {
            _stroke.Color = _fill.Color; _stroke.StrokeWidth = 8f;
            canvas.DrawLine(-68f, 111f, -30f, 111f, _stroke); canvas.DrawLine(30f, 111f, 68f, 111f, _stroke);
        }
        else
        {
            canvas.DrawCircle(-50f, 110f, 15f, _fill); canvas.DrawCircle(50f, 110f, 15f, _fill);
        }
        canvas.DrawCircle(0f, 158f, 19f, _fill);
        _stroke.Color = new SKColor(102, 55, 36); _stroke.StrokeWidth = 8f;
        if (mood == PumaMood.Happy)
            canvas.DrawArc(new SKRect(-52f, 150f, 52f, 220f), 12f, 156f, false, _stroke);
        else if (mood == PumaMood.Encouraging)
            canvas.DrawArc(new SKRect(-42f, 180f, 42f, 225f), 205f, 130f, false, _stroke);
        else
            canvas.DrawLine(-35f, 198f, 35f, 198f, _stroke);
        _fill.Color = new SKColor(66, 153, 91);
        _path.Reset(); _path.MoveTo(-112f, 235f); _path.LineTo(112f, 235f); _path.LineTo(72f, 300f); _path.LineTo(-72f, 300f); _path.Close();
        canvas.DrawPath(_path, _fill);
        _fill.Color = new SKColor(226, 168, 91);
        canvas.DrawCircle(-88f, 280f, 32f, _fill); canvas.DrawCircle(88f, 280f, 32f, _fill);
        _fill.Color = new SKColor(250, 220, 174);
        canvas.DrawOval(new SKRect(-110f, 274f, -66f, 304f), _fill); canvas.DrawOval(new SKRect(66f, 274f, 110f, 304f), _fill);
        _fill.Color = new SKColor(116, 74, 43, 115);
        canvas.DrawCircle(-112f, 85f, 9f, _fill); canvas.DrawCircle(114f, 112f, 8f, _fill); canvas.DrawCircle(-94f, 165f, 7f, _fill);
        _stroke.Color = new SKColor(106, 68, 47); _stroke.StrokeWidth = 4f;
        canvas.DrawLine(-45f, 166f, -125f, 145f, _stroke); canvas.DrawLine(-45f, 180f, -128f, 183f, _stroke); canvas.DrawLine(45f, 166f, 125f, 145f, _stroke); canvas.DrawLine(45f, 180f, 128f, 183f, _stroke);
        canvas.Restore();
    }

    public void Dispose()
    {
        _fill.Dispose(); _stroke.Dispose(); _path.Dispose();
    }
}
