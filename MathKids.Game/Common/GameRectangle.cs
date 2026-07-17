namespace MathKids.Game.Common;

public readonly record struct GameRectangle(float Left, float Top, float Width, float Height)
{
    public float Right => Left + Width;
    public float Bottom => Top + Height;
    public float CenterX => Left + Width / 2f;
    public float CenterY => Top + Height / 2f;
}
