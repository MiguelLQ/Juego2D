using MathKids.Game.Core;
using SkiaSharp;

namespace MathKids.Game.Graphics.Drawing;

public interface IGameBackdrop : IDisposable
{
    void Update(GameTime gameTime);
    void Draw(SKCanvas canvas, GameViewport viewport);
}

public abstract class GameBackdropBase : IGameBackdrop
{
    protected readonly SKPaint FillPaint = new() { IsAntialias = true };
    protected readonly SKPaint StrokePaint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round };
    protected readonly SKPath Path = new();
    protected float Elapsed { get; private set; }

    public void Update(GameTime gameTime) => Elapsed += gameTime.DeltaSeconds;
    public abstract void Draw(SKCanvas canvas, GameViewport viewport);

    protected void DrawCloud(SKCanvas canvas, float x, float y, float scale, SKColor color)
    {
        FillPaint.Color = color;
        canvas.DrawCircle(x, y, 58f * scale, FillPaint);
        canvas.DrawCircle(x + 66f * scale, y - 28f * scale, 76f * scale, FillPaint);
        canvas.DrawCircle(x + 142f * scale, y, 60f * scale, FillPaint);
        canvas.DrawRoundRect(new SKRect(x, y, x + 148f * scale, y + 48f * scale), 24f, 24f, FillPaint);
    }

    public virtual void Dispose()
    {
        FillPaint.Dispose();
        StrokePaint.Dispose();
        Path.Dispose();
    }
}

public sealed class AdditionAdventureBackdrop : GameBackdropBase
{
    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        FillPaint.Color = new SKColor(105, 204, 244);
        canvas.DrawRect(viewport.VisibleLogicalLeft, viewport.VisibleLogicalTop, viewport.VisibleLogicalRight, viewport.VisibleLogicalBottom, FillPaint);
        var cloudShift = MathF.Sin(Elapsed * 0.35f) * 28f;
        DrawCloud(canvas, 90f + cloudShift, 290f, 1.05f, new SKColor(242, 252, 255, 220));
        DrawCloud(canvas, 790f - cloudShift, 430f, 0.82f, new SKColor(242, 252, 255, 210));
        FillPaint.Color = new SKColor(145, 218, 78);
        canvas.DrawOval(new SKRect(viewport.VisibleLogicalLeft - 80f, 1420f, 700f, 2050f), FillPaint);
        FillPaint.Color = new SKColor(72, 183, 101);
        canvas.DrawOval(new SKRect(420f, 1470f, viewport.VisibleLogicalRight + 160f, 2070f), FillPaint);
        FillPaint.Color = new SKColor(255, 220, 127);
        Path.Reset();
        Path.MoveTo(450f, 1980f); Path.CubicTo(480f, 1760f, 690f, 1690f, 590f, 1470f);
        Path.LineTo(730f, 1470f); Path.CubicTo(830f, 1710f, 630f, 1810f, 720f, 1980f); Path.Close();
        canvas.DrawPath(Path, FillPaint);
    }
}

public sealed class AndeanDashboardBackdrop : GameBackdropBase
{
    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        FillPaint.Color = new SKColor(90, 193, 235);
        canvas.DrawRect(viewport.VisibleLogicalLeft, viewport.VisibleLogicalTop, viewport.VisibleLogicalRight, viewport.VisibleLogicalBottom, FillPaint);
        FillPaint.Color = new SKColor(255, 214, 82, 210);
        canvas.DrawCircle(900f, 280f, 90f + MathF.Sin(Elapsed) * 4f, FillPaint);
        var shift = MathF.Sin(Elapsed * 0.3f) * 35f;
        DrawCloud(canvas, 35f + shift, 280f, 0.72f, new SKColor(249, 254, 255, 210));
        DrawCloud(canvas, 760f - shift, 470f, 0.55f, new SKColor(249, 254, 255, 190));
        DrawPeak(canvas, viewport.VisibleLogicalLeft - 80f, 1170f, 550f, 520f);
        DrawPeak(canvas, 300f, 1200f, 620f, 690f);
        DrawPeak(canvas, 760f, 1180f, 540f, 510f);
        FillPaint.Color = new SKColor(88, 169, 79);
        canvas.DrawOval(new SKRect(viewport.VisibleLogicalLeft - 100f, 1120f, 720f, 1980f), FillPaint);
        FillPaint.Color = new SKColor(55, 139, 86);
        canvas.DrawOval(new SKRect(430f, 1190f, viewport.VisibleLogicalRight + 120f, 2010f), FillPaint);
        FillPaint.Color = new SKColor(224, 183, 97);
        Path.Reset(); Path.MoveTo(410f, 1920f); Path.CubicTo(440f, 1740f, 690f, 1650f, 570f, 1420f); Path.LineTo(730f, 1420f); Path.CubicTo(830f, 1680f, 630f, 1790f, 730f, 1920f); Path.Close(); canvas.DrawPath(Path, FillPaint);
        DrawTextileBand(canvas, viewport.VisibleLogicalLeft, 730f, viewport.VisibleLogicalRight);
    }

    private void DrawPeak(SKCanvas canvas, float x, float baseY, float width, float height)
    {
        FillPaint.Color = new SKColor(89, 119, 139);
        Path.Reset(); Path.MoveTo(x, baseY); Path.LineTo(x + width * 0.52f, baseY - height); Path.LineTo(x + width, baseY); Path.Close(); canvas.DrawPath(Path, FillPaint);
        FillPaint.Color = new SKColor(244, 250, 246);
        Path.Reset(); Path.MoveTo(x + width * 0.36f, baseY - height * 0.68f); Path.LineTo(x + width * 0.52f, baseY - height); Path.LineTo(x + width * 0.69f, baseY - height * 0.66f); Path.LineTo(x + width * 0.57f, baseY - height * 0.7f); Path.LineTo(x + width * 0.49f, baseY - height * 0.61f); Path.Close(); canvas.DrawPath(Path, FillPaint);
    }

    private void DrawTextileBand(SKCanvas canvas, float left, float top, float right)
    {
        FillPaint.Color = new SKColor(111, 67, 157, 210); canvas.DrawRect(left, top, right, top + 80f, FillPaint);
        StrokePaint.Color = new SKColor(255, 193, 65); StrokePaint.StrokeWidth = 10f;
        for (var x = left - 70f; x < right + 70f; x += 90f)
        {
            Path.Reset(); Path.MoveTo(x, top + 62f); Path.LineTo(x + 22f, top + 18f); Path.LineTo(x + 45f, top + 62f); Path.LineTo(x + 68f, top + 18f); canvas.DrawPath(Path, StrokePaint);
        }
    }
}

public sealed class BingoFestivalBackdrop : GameBackdropBase
{
    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        FillPaint.Color = new SKColor(147, 111, 224);
        canvas.DrawRect(viewport.VisibleLogicalLeft, viewport.VisibleLogicalTop, viewport.VisibleLogicalRight, viewport.VisibleLogicalBottom, FillPaint);
        FillPaint.Color = new SKColor(244, 148, 184, 120);
        canvas.DrawCircle(120f, 330f, 240f, FillPaint);
        FillPaint.Color = new SKColor(91, 197, 212, 125);
        canvas.DrawCircle(980f, 520f, 320f, FillPaint);
        FillPaint.Color = new SKColor(87, 79, 137, 165);
        Path.Reset(); Path.MoveTo(viewport.VisibleLogicalLeft, 1180f); Path.LineTo(210f, 640f); Path.LineTo(500f, 1180f); Path.LineTo(755f, 560f); Path.LineTo(viewport.VisibleLogicalRight, 1180f); Path.Close(); canvas.DrawPath(Path, FillPaint);
        FillPaint.Color = new SKColor(245, 245, 239, 180);
        Path.Reset(); Path.MoveTo(620f, 885f); Path.LineTo(755f, 560f); Path.LineTo(900f, 900f); Path.LineTo(805f, 845f); Path.LineTo(748f, 920f); Path.LineTo(695f, 842f); Path.Close(); canvas.DrawPath(Path, FillPaint);
        StrokePaint.Color = new SKColor(255, 250, 226);
        StrokePaint.StrokeWidth = 8f;
        canvas.DrawLine(viewport.VisibleLogicalLeft, 245f, viewport.VisibleLogicalRight, 360f, StrokePaint);
        for (var index = -2; index < 12; index++)
        {
            var x = index * 125f + (Elapsed * 16f % 125f);
            var y = 282f + x * 0.106f;
            FillPaint.Color = (index % 3) switch
            {
                0 => new SKColor(255, 196, 49),
                1 => new SKColor(92, 218, 176),
                _ => new SKColor(255, 112, 139)
            };
            Path.Reset(); Path.MoveTo(x, y); Path.LineTo(x + 72f, y + 8f); Path.LineTo(x + 35f, y + 95f); Path.Close();
            canvas.DrawPath(Path, FillPaint);
        }
        FillPaint.Color = new SKColor(84, 173, 103);
        canvas.DrawOval(new SKRect(viewport.VisibleLogicalLeft - 100f, 1510f, 700f, 2040f), FillPaint);
        FillPaint.Color = new SKColor(55, 139, 102);
        canvas.DrawOval(new SKRect(420f, 1540f, viewport.VisibleLogicalRight + 120f, 2050f), FillPaint);
    }
}

public sealed class AndeanPumaBackdrop : GameBackdropBase
{
    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        FillPaint.Color = new SKColor(93, 188, 235);
        canvas.DrawRect(viewport.VisibleLogicalLeft, viewport.VisibleLogicalTop, viewport.VisibleLogicalRight, viewport.VisibleLogicalBottom, FillPaint);
        FillPaint.Color = new SKColor(255, 218, 83, 220);
        canvas.DrawCircle(865f, 245f, 92f + MathF.Sin(Elapsed * 1.2f) * 4f, FillPaint);
        var cloudShift = Elapsed * 13f % 1450f;
        DrawCloud(canvas, -280f + cloudShift, 315f, 0.72f, new SKColor(250, 254, 255, 215));
        DrawCloud(canvas, 1060f - cloudShift * 0.55f, 455f, 0.58f, new SKColor(250, 254, 255, 195));

        DrawMountain(canvas, viewport.VisibleLogicalLeft - 80f, 1170f, 470f, 500f, new SKColor(88, 126, 151), new SKColor(239, 246, 244));
        DrawMountain(canvas, 240f, 1190f, 520f, 660f, new SKColor(72, 113, 143), new SKColor(246, 250, 247));
        DrawMountain(canvas, 720f, 1200f, 520f, 560f, new SKColor(91, 129, 151), new SKColor(238, 245, 242));

        FillPaint.Color = new SKColor(100, 163, 71);
        canvas.DrawRect(viewport.VisibleLogicalLeft, 1190f, viewport.VisibleLogicalRight, viewport.VisibleLogicalBottom, FillPaint);
        var terraceColors = new[] { new SKColor(123, 181, 70), new SKColor(82, 151, 72), new SKColor(160, 190, 72) };
        for (var index = 0; index < 5; index++)
        {
            FillPaint.Color = terraceColors[index % terraceColors.Length];
            var top = 1280f + index * 145f;
            canvas.DrawRoundRect(new SKRect(viewport.VisibleLogicalLeft - 40f, top, viewport.VisibleLogicalRight + 40f, top + 105f), 50f, 50f, FillPaint);
        }
        FillPaint.Color = new SKColor(215, 179, 91);
        Path.Reset(); Path.MoveTo(420f, 1920f); Path.CubicTo(435f, 1735f, 630f, 1650f, 585f, 1460f);
        Path.LineTo(735f, 1460f); Path.CubicTo(805f, 1670f, 640f, 1790f, 725f, 1920f); Path.Close();
        canvas.DrawPath(Path, FillPaint);
    }

    private void DrawMountain(SKCanvas canvas, float x, float baseY, float width, float height, SKColor mountainColor, SKColor snowColor)
    {
        FillPaint.Color = mountainColor;
        Path.Reset(); Path.MoveTo(x, baseY); Path.LineTo(x + width * 0.52f, baseY - height); Path.LineTo(x + width, baseY); Path.Close();
        canvas.DrawPath(Path, FillPaint);
        FillPaint.Color = snowColor;
        Path.Reset();
        Path.MoveTo(x + width * 0.33f, baseY - height * 0.63f);
        Path.LineTo(x + width * 0.52f, baseY - height);
        Path.LineTo(x + width * 0.72f, baseY - height * 0.6f);
        Path.LineTo(x + width * 0.62f, baseY - height * 0.66f);
        Path.LineTo(x + width * 0.54f, baseY - height * 0.57f);
        Path.LineTo(x + width * 0.46f, baseY - height * 0.68f);
        Path.Close();
        canvas.DrawPath(Path, FillPaint);
    }
}

public sealed class ChancaLaboratoryBackdrop : GameBackdropBase
{
    public override void Draw(SKCanvas canvas, GameViewport viewport)
    {
        FillPaint.Color = new SKColor(63, 39, 87);
        canvas.DrawRect(viewport.VisibleLogicalLeft, viewport.VisibleLogicalTop, viewport.VisibleLogicalRight, viewport.VisibleLogicalBottom, FillPaint);

        FillPaint.Color = new SKColor(116, 78, 132);
        canvas.DrawCircle(875f, 245f, 108f, FillPaint);
        FillPaint.Color = new SKColor(255, 196, 82, 210);
        canvas.DrawCircle(875f, 245f, 72f + MathF.Sin(Elapsed * 1.4f) * 5f, FillPaint);

        FillPaint.Color = new SKColor(76, 60, 105);
        Path.Reset(); Path.MoveTo(viewport.VisibleLogicalLeft, 710f); Path.LineTo(180f, 370f); Path.LineTo(390f, 710f); Path.LineTo(625f, 315f); Path.LineTo(920f, 710f); Path.LineTo(viewport.VisibleLogicalRight, 480f); Path.LineTo(viewport.VisibleLogicalRight, 920f); Path.LineTo(viewport.VisibleLogicalLeft, 920f); Path.Close();
        canvas.DrawPath(Path, FillPaint);

        FillPaint.Color = new SKColor(162, 105, 73);
        canvas.DrawRect(viewport.VisibleLogicalLeft, 660f, viewport.VisibleLogicalRight, viewport.VisibleLogicalBottom, FillPaint);
        FillPaint.Color = new SKColor(190, 132, 87);
        for (var row = 0; row < 8; row++)
        {
            var top = 680f + row * 170f;
            var offset = row % 2 == 0 ? -65f : 25f;
            for (var column = -2; column < 10; column++)
            {
                var left = offset + column * 155f;
                canvas.DrawRoundRect(new SKRect(left, top, left + 138f, top + 145f), 18f, 18f, FillPaint);
            }
        }

        DrawChancaBand(canvas, viewport.VisibleLogicalLeft, 690f, viewport.VisibleLogicalRight);
        DrawChancaBand(canvas, viewport.VisibleLogicalLeft, 1780f, viewport.VisibleLogicalRight);

        for (var index = 0; index < 10; index++)
        {
            var phase = (Elapsed * 48f + index * 137f) % 900f;
            FillPaint.Color = index % 2 == 0 ? new SKColor(72, 224, 201, 95) : new SKColor(255, 202, 74, 95);
            canvas.DrawCircle(80f + index * 115f, 1680f - phase, 8f + index % 3 * 3f, FillPaint);
        }
    }

    private void DrawChancaBand(SKCanvas canvas, float left, float top, float right)
    {
        FillPaint.Color = new SKColor(48, 31, 66, 215);
        canvas.DrawRect(left, top, right, top + 72f, FillPaint);
        StrokePaint.StrokeWidth = 10f;
        StrokePaint.Color = new SKColor(239, 175, 67);
        for (var x = left - 80f; x < right + 80f; x += 95f)
        {
            Path.Reset(); Path.MoveTo(x, top + 55f); Path.LineTo(x + 24f, top + 18f); Path.LineTo(x + 48f, top + 55f); Path.LineTo(x + 72f, top + 18f);
            canvas.DrawPath(Path, StrokePaint);
        }
    }
}
