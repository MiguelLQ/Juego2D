using MathKids.Game.Core;
using MathKids.Game.Input.Touch;
using SkiaSharp;

namespace MathKids.Game.Scenes.Abstractions;

public interface IGameScene
{
    GameScreen Screen { get; }
    void Enter();
    void Exit();
    void Update(GameTime gameTime);
    void Draw(SKCanvas canvas, GameViewport viewport);
    void HandleInput(GameInput input);
}
