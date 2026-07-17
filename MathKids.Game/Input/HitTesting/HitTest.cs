using MathKids.Game.Common;

namespace MathKids.Game.Input.HitTesting;

public static class HitTest
{
    public static bool Contains(GameRectangle rectangle, GamePoint point) => point.X >= rectangle.Left && point.X <= rectangle.Right && point.Y >= rectangle.Top && point.Y <= rectangle.Bottom;
}
