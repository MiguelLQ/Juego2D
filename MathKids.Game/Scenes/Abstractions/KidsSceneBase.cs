using MathKids.Game.Common;
using MathKids.Game.Core;
using MathKids.Game.Input.HitTesting;
using MathKids.Game.Input.Touch;
using SkiaSharp;

namespace MathKids.Game.Scenes.Abstractions;

public abstract class KidsSceneBase : IGameScene, IDisposable
{
    private readonly SKPath _leftEar = new();
    private readonly SKPath _rightEar = new();
    private readonly SKPath _shapePath = new();
    protected readonly SKPaint FillPaint = new() { IsAntialias = true };
    protected readonly SKPaint StrokePaint = new() { IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeCap = SKStrokeCap.Round, StrokeJoin = SKStrokeJoin.Round };
    protected readonly SKPaint TextPaint = new() { IsAntialias = true, TextAlign = SKTextAlign.Center, Typeface = SKTypeface.FromFamilyName("sans-serif", SKFontStyle.Bold) };
    protected readonly SKPaint ShadowPaint = new() { IsAntialias = true, Color = new SKColor(36, 71, 100, 35) };
    protected static readonly GameRectangle BackButtonBounds = new(42f, 54f, 112f, 112f);

    protected KidsSceneBase()
    {
        _leftEar.MoveTo(140f, 550f); _leftEar.LineTo(205f, 350f); _leftEar.LineTo(290f, 545f); _leftEar.Close();
        _rightEar.MoveTo(325f, 545f); _rightEar.LineTo(415f, 350f); _rightEar.LineTo(470f, 570f); _rightEar.Close();
    }

    public abstract GameScreen Screen { get; }
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SKCanvas canvas, GameViewport viewport);
    public abstract void HandleInput(GameInput input);

    protected void DrawWorldBackground(SKCanvas canvas, GameViewport viewport)
    {
        FillPaint.Color = new SKColor(111, 204, 242);
        canvas.DrawRect(viewport.VisibleLogicalLeft, viewport.VisibleLogicalTop, viewport.VisibleLogicalRight, viewport.VisibleLogicalBottom, FillPaint);
        FillPaint.Color = new SKColor(226, 248, 255, 220);
        DrawCloud(canvas, 70f, 250f, 1.1f);
        DrawCloud(canvas, 840f, 390f, 0.85f);
        DrawCloud(canvas, 165f, 1020f, 0.7f);
        FillPaint.Color = new SKColor(149, 221, 83);
        canvas.DrawOval(new SKRect(-160f, 1450f, 690f, 2000f), FillPaint);
        FillPaint.Color = new SKColor(83, 190, 102);
        canvas.DrawOval(new SKRect(420f, 1470f, 1260f, 2040f), FillPaint);
        FillPaint.Color = new SKColor(255, 222, 134);
        _shapePath.Reset();
        _shapePath.MoveTo(490f, 1920f); _shapePath.CubicTo(500f, 1760f, 680f, 1690f, 590f, 1510f); _shapePath.LineTo(720f, 1510f); _shapePath.CubicTo(820f, 1720f, 650f, 1800f, 700f, 1920f); _shapePath.Close();
        canvas.DrawPath(_shapePath, FillPaint);
    }

    protected void DrawBrandHeader(SKCanvas canvas, float y, float scale = 1f)
    {
        TextPaint.TextSize = 86f * scale;
        StrokePaint.TextSize = TextPaint.TextSize;
        StrokePaint.TextAlign = SKTextAlign.Center;
        StrokePaint.Typeface = TextPaint.Typeface;
        StrokePaint.StrokeWidth = 18f * scale;
        StrokePaint.Color = SKColors.White;
        canvas.DrawText("Mate", 360f, y, StrokePaint);
        canvas.DrawText("Aventura", 685f, y, StrokePaint);
        TextPaint.Color = new SKColor(244, 104, 47);
        canvas.DrawText("Mate", 360f, y, TextPaint);
        TextPaint.Color = new SKColor(42, 136, 224);
        canvas.DrawText("Aventura", 685f, y, TextPaint);
    }

    protected void DrawCoinBadge(SKCanvas canvas, int coins)
    {
        FillPaint.Color = new SKColor(255, 255, 255, 235);
        canvas.DrawRoundRect(new SKRect(815f, 50f, 1040f, 155f), 52f, 52f, ShadowPaint);
        canvas.DrawRoundRect(new SKRect(805f, 38f, 1030f, 143f), 52f, 52f, FillPaint);
        FillPaint.Color = new SKColor(255, 184, 35);
        canvas.DrawCircle(860f, 90f, 39f, FillPaint);
        FillPaint.Color = new SKColor(255, 220, 77);
        canvas.DrawCircle(860f, 90f, 28f, FillPaint);
        DrawStar(canvas, 860f, 90f, 17f, new SKColor(239, 146, 24));
        TextPaint.TextSize = 42f;
        TextPaint.Color = new SKColor(19, 53, 105);
        canvas.DrawText(coins.ToString(), 945f, 105f, TextPaint);
    }

    protected void DrawFoxMascot(SKCanvas canvas, float offsetY = 0f, float bob = 0f)
    {
        canvas.Save();
        canvas.Translate(0f, offsetY + bob);
        var tailSwing = bob * 1.8f;
        FillPaint.Color = new SKColor(230, 96, 24);
        _shapePath.Reset(); _shapePath.MoveTo(175f, 770f); _shapePath.CubicTo(5f + tailSwing, 720f, 30f + tailSwing, 945f, 230f, 860f); _shapePath.CubicTo(170f, 840f, 155f, 805f, 175f, 770f); _shapePath.Close();
        canvas.DrawPath(_shapePath, FillPaint);
        FillPaint.Color = new SKColor(255, 239, 207);
        _shapePath.Reset(); _shapePath.MoveTo(62f + tailSwing, 824f); _shapePath.CubicTo(25f + tailSwing, 770f, 38f + tailSwing, 738f, 90f + tailSwing, 742f); _shapePath.CubicTo(118f + tailSwing, 757f, 132f + tailSwing, 781f, 142f + tailSwing, 806f); _shapePath.Close(); canvas.DrawPath(_shapePath, FillPaint);
        FillPaint.Color = new SKColor(240, 112, 31);
        canvas.DrawPath(_leftEar, FillPaint); canvas.DrawPath(_rightEar, FillPaint);
        canvas.DrawOval(new SKRect(135f, 475f, 475f, 780f), FillPaint);
        FillPaint.Color = new SKColor(255, 232, 201);
        canvas.DrawOval(new SKRect(165f, 590f, 445f, 760f), FillPaint);
        FillPaint.Color = SKColors.White;
        canvas.DrawOval(new SKRect(205f, 550f, 285f, 655f), FillPaint); canvas.DrawOval(new SKRect(325f, 550f, 405f, 655f), FillPaint);
        FillPaint.Color = new SKColor(33, 37, 51);
        canvas.DrawCircle(252f, 610f, 20f, FillPaint); canvas.DrawCircle(362f, 610f, 20f, FillPaint); canvas.DrawCircle(307f, 676f, 24f, FillPaint);
        FillPaint.Color = new SKColor(255, 157, 124, 145); canvas.DrawOval(new SKRect(182f, 658f, 240f, 692f), FillPaint); canvas.DrawOval(new SKRect(374f, 658f, 432f, 692f), FillPaint);
        StrokePaint.Color = new SKColor(109, 55, 31); StrokePaint.StrokeWidth = 7f;
        canvas.DrawLine(218f, 563f, 271f, 548f, StrokePaint); canvas.DrawLine(343f, 548f, 396f, 563f, StrokePaint);
        StrokePaint.Color = new SKColor(90, 48, 28); StrokePaint.StrokeWidth = 9f;
        canvas.DrawArc(new SKRect(260f, 660f, 355f, 735f), 15f, 150f, false, StrokePaint);
        FillPaint.Color = new SKColor(70, 177, 90);
        canvas.DrawRoundRect(new SKRect(205f, 755f, 420f, 900f), 55f, 55f, FillPaint);
        FillPaint.Color = new SKColor(52, 142, 76);
        _shapePath.Reset(); _shapePath.MoveTo(245f, 752f); _shapePath.LineTo(310f, 822f); _shapePath.LineTo(375f, 752f); _shapePath.LineTo(350f, 850f); _shapePath.LineTo(272f, 850f); _shapePath.Close(); canvas.DrawPath(_shapePath, FillPaint);
        FillPaint.Color = new SKColor(255, 232, 201); canvas.DrawCircle(215f, 842f, 30f, FillPaint); canvas.DrawCircle(414f, 842f, 30f, FillPaint);
        canvas.Restore();
    }

    protected void DrawBackButton(SKCanvas canvas)
    {
        FillPaint.Color = new SKColor(255, 255, 255, 235);
        canvas.DrawCircle(98f, 110f, 57f, ShadowPaint);
        canvas.DrawCircle(92f, 102f, 55f, FillPaint);
        StrokePaint.Color = new SKColor(39, 84, 139); StrokePaint.StrokeWidth = 13f;
        canvas.DrawLine(112f, 70f, 76f, 102f, StrokePaint); canvas.DrawLine(76f, 102f, 112f, 134f, StrokePaint);
    }

    protected void DrawAudioButton(SKCanvas canvas, bool muted = false)
    {
        FillPaint.Color = new SKColor(255, 255, 255, 235);
        canvas.DrawCircle(980f, 217f, 48f, ShadowPaint);
        canvas.DrawCircle(974f, 210f, 46f, FillPaint);
        FillPaint.Color = new SKColor(43, 101, 153);
        canvas.DrawRoundRect(new SKRect(944f, 197f, 961f, 223f), 5f, 5f, FillPaint);
        _shapePath.Reset(); _shapePath.MoveTo(960f, 197f); _shapePath.LineTo(982f, 179f); _shapePath.LineTo(982f, 241f); _shapePath.LineTo(960f, 223f); _shapePath.Close();
        canvas.DrawPath(_shapePath, FillPaint);
        StrokePaint.Color = muted ? new SKColor(225, 84, 91) : new SKColor(43, 101, 153); StrokePaint.StrokeWidth = 7f;
        if (muted) canvas.DrawLine(996f, 189f, 1022f, 231f, StrokePaint);
        else { canvas.DrawArc(new SKRect(978f, 191f, 1012f, 229f), -55f, 110f, false, StrokePaint); canvas.DrawArc(new SKRect(978f, 179f, 1029f, 241f), -52f, 104f, false, StrokePaint); }
    }

    protected static bool IsReleasedInside(GameInput input, GameRectangle bounds) => input.Type == GameInputType.Released && HitTest.Contains(bounds, input.Position);

    protected void DrawStar(SKCanvas canvas, float centerX, float centerY, float radius, SKColor color)
    {
        _shapePath.Reset();
        for (var index = 0; index < 10; index++)
        {
            var angle = -MathF.PI / 2f + index * MathF.PI / 5f;
            var pointRadius = index % 2 == 0 ? radius : radius * 0.46f;
            var x = centerX + MathF.Cos(angle) * pointRadius;
            var y = centerY + MathF.Sin(angle) * pointRadius;
            if (index == 0) _shapePath.MoveTo(x, y); else _shapePath.LineTo(x, y);
        }
        _shapePath.Close();
        FillPaint.Color = color;
        canvas.DrawPath(_shapePath, FillPaint);
    }

    private void DrawCloud(SKCanvas canvas, float x, float y, float scale)
    {
        canvas.DrawCircle(x, y, 72f * scale, FillPaint);
        canvas.DrawCircle(x + 78f * scale, y - 35f * scale, 92f * scale, FillPaint);
        canvas.DrawCircle(x + 170f * scale, y, 74f * scale, FillPaint);
        canvas.DrawRoundRect(new SKRect(x, y, x + 175f * scale, y + 65f * scale), 28f, 28f, FillPaint);
    }

    public virtual void Dispose()
    {
        _leftEar.Dispose(); _rightEar.Dispose(); _shapePath.Dispose(); FillPaint.Dispose(); StrokePaint.Dispose(); TextPaint.Dispose(); ShadowPaint.Dispose();
    }
}
