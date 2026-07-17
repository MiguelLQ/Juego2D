using MathKids.Game.Common;

namespace MathKids.Game.Input.Touch;

public readonly record struct GameInput(long PointerId, GameInputType Type, GamePoint Position);
public enum GameInputType { Pressed, Moved, Released, Cancelled }
