# Architecture

## Direction of dependencies

```text
Domain <- Application <- Infrastructure
   ^           ^
   +-------- Game <- Mobile
               ^
              Tests
```

`Domain` remains portable and testable. `Application` coordinates use cases and defines ports. `Infrastructure` supplies device-independent provisional adapters. `Game` owns rendering and input but asks Application for exercises. `Mobile` is the composition root and lifecycle host.

## Runtime flow

`GameHostPage` owns one dispatcher timer. Each tick invalidates the Skia surface. `GameController` advances the clamped `GameLoop`, updates the active scene, applies the `GameViewport` transform and draws. Touch coordinates are transformed from physical pixels to the 1080 × 1920 logical space before hit testing.

## Decisions

- Scene-oriented lightweight engine instead of a general-purpose ECS.
- Constructor injection; scenes never use a service locator.
- Cached `SKPaint` and bitmap ownership through disposable components and `AssetManager`.
- Android-only target in phase one to keep builds fast; other platform folders are retained for future work.
- SQLite progress repository stored in the app-private data directory, with the game depending only on Application contracts.
- Audio contract with a null adapter until curated child-friendly sound assets are introduced.
