using MathKids.Game.Common;
using MathKids.Game.Core;

namespace MathKids.Tests.Game;

public sealed class GameViewportTests
{
    [Fact]
    public void Resize_WideSurface_AddsHorizontalLetterboxing()
    {
        var viewport = new GameViewport(1080f, 1920f);
        viewport.Resize(1200f, 1920f);
        Assert.Equal(1f, viewport.Scale, 3);
        Assert.Equal(60f, viewport.OffsetX, 3);
        Assert.Equal(0f, viewport.OffsetY, 3);
    }

    [Fact]
    public void CoordinateConversion_RoundTrips()
    {
        var viewport = new GameViewport(1080f, 1920f);
        viewport.Resize(720f, 1280f);
        var logical = new GamePoint(540f, 960f);
        var physical = viewport.LogicalToPhysical(logical);
        var result = viewport.PhysicalToLogical(physical);
        Assert.Equal(logical.X, result.X, 3);
        Assert.Equal(logical.Y, result.Y, 3);
    }
}
