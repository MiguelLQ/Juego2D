using MathKids.Game.Core;
using SkiaSharp;

namespace MathKids.Game.Components.Characters;

public enum ChancaMood { Curious, Celebrating, Encouraging }

public sealed class ChancaGuide : IDisposable
{
    private readonly SKPaint _fill = new() { IsAntialias = true };
    private readonly SKPaint _stroke = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round, StrokeJoin = SKStrokeJoin.Round };
    private readonly SKPath _path = new();
    private float _elapsed;

    public void Update(GameTime gameTime) => _elapsed += gameTime.DeltaSeconds;

    public void Draw(SKCanvas canvas, float x, float y, float scale, ChancaMood mood)
    {
        var bob = MathF.Sin(_elapsed * 2.4f) * 7f;
        canvas.Save();
        canvas.Translate(x, y + bob);
        canvas.Scale(scale);

        _fill.Color = new SKColor(55, 35, 44);
        canvas.DrawOval(new SKRect(-120f, -40f, 120f, 205f), _fill);
        _fill.Color = new SKColor(181, 116, 72);
        canvas.DrawOval(new SKRect(-104f, -8f, 104f, 205f), _fill);
        canvas.DrawCircle(-105f, 92f, 23f, _fill); canvas.DrawCircle(105f, 92f, 23f, _fill);

        _fill.Color = new SKColor(73, 42, 91);
        canvas.DrawRoundRect(new SKRect(-122f, 0f, 122f, 52f), 14f, 14f, _fill);
        _fill.Color = new SKColor(241, 174, 55);
        for (var index = -2; index <= 2; index++)
        {
            _path.Reset(); _path.MoveTo(index * 42f - 18f, 8f); _path.LineTo(index * 42f, 42f); _path.LineTo(index * 42f + 18f, 8f); _path.Close();
            canvas.DrawPath(_path, _fill);
        }

        _fill.Color = SKColors.White;
        canvas.DrawOval(new SKRect(-76f, 68f, -15f, 132f), _fill); canvas.DrawOval(new SKRect(15f, 68f, 76f, 132f), _fill);
        _fill.Color = new SKColor(40, 29, 27);
        var blink = MathF.Sin(_elapsed * 0.8f) > 0.987f;
        if (blink)
        {
            _stroke.Color = _fill.Color; _stroke.StrokeWidth = 7f;
            canvas.DrawLine(-66f, 102f, -27f, 102f, _stroke); canvas.DrawLine(27f, 102f, 66f, 102f, _stroke);
        }
        else
        {
            canvas.DrawCircle(-45f, 101f, 13f, _fill); canvas.DrawCircle(45f, 101f, 13f, _fill);
        }
        _stroke.Color = new SKColor(93, 52, 36); _stroke.StrokeWidth = 8f;
        if (mood == ChancaMood.Celebrating)
            canvas.DrawArc(new SKRect(-52f, 112f, 52f, 190f), 12f, 156f, false, _stroke);
        else if (mood == ChancaMood.Encouraging)
            canvas.DrawArc(new SKRect(-42f, 153f, 42f, 195f), 205f, 130f, false, _stroke);
        else
            canvas.DrawLine(-30f, 166f, 30f, 166f, _stroke);

        _fill.Color = new SKColor(226, 126, 62);
        _path.Reset(); _path.MoveTo(-130f, 205f); _path.LineTo(130f, 205f); _path.LineTo(92f, 395f); _path.LineTo(-92f, 395f); _path.Close();
        canvas.DrawPath(_path, _fill);
        _fill.Color = new SKColor(65, 45, 90);
        canvas.DrawRect(-105f, 255f, 105f, 305f, _fill);
        _fill.Color = new SKColor(247, 185, 59);
        for (var index = -2; index <= 2; index++) canvas.DrawCircle(index * 42f, 280f, 10f, _fill);

        _stroke.Color = new SKColor(181, 116, 72); _stroke.StrokeWidth = 28f;
        var armLift = mood == ChancaMood.Celebrating ? -80f : MathF.Sin(_elapsed * 2f) * 14f;
        canvas.DrawLine(-112f, 235f, -180f, 320f + armLift, _stroke);
        canvas.DrawLine(112f, 235f, 180f, 300f - armLift * 0.35f, _stroke);
        _fill.Color = new SKColor(83, 208, 194);
        canvas.DrawCircle(185f, 300f - armLift * 0.35f, 24f, _fill);
        canvas.Restore();
    }

    public void Dispose()
    {
        _fill.Dispose(); _stroke.Dispose(); _path.Dispose();
    }
}
