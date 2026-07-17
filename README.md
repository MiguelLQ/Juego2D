# MateAventura

Aplicación educativa de matemáticas con apariencia de videojuego 2D para niños de primer y segundo grado. Incluye navegación entre Inicio, Juegos y Progreso, además de una aventura de sumas y un bingo de sumas funcionales.

## Tecnologías

- .NET 10, C# y .NET MAUI para Android.
- SkiaSharp para renderizado e interacción 2D.
- Inyección de dependencias nativa de .NET.
- xUnit para pruebas unitarias.
- SQLite local mediante `Microsoft.Data.Sqlite` para conservar progreso, puntos y última actividad.

## Proyectos

- `MathKids.Domain`: modelos y reglas centrales sin dependencias gráficas.
- `MathKids.Application`: generadores, sesiones, progreso y recompensas.
- `MathKids.Game`: game loop, escenas, viewport, entrada, componentes y recursos SkiaSharp.
- `MathKids.Infrastructure`: reloj, aleatoriedad, configuración, audio provisional y persistencia SQLite.
- `MathKids.Mobile`: host Android MAUI y superficie SkiaSharp.
- `MathKids.Tests`: pruebas de matemáticas y escalado.

Las dependencias avanzan hacia el dominio y no existen referencias circulares. Consulta `docs/architecture.md`.

La documentación detallada de capas, módulos, escenas, navegación y carga de imágenes está en `docs/guia-completa-proyecto.md`.

## Ejecutar Android

1. Instala el workload MAUI y un SDK/emulador Android.
2. Abre `MathKids.sln` en Visual Studio o Rider.
3. Selecciona `MathKids.Mobile` como proyecto de inicio y ejecuta Android.

Desde terminal: `dotnet build MathKids.Mobile/MathKids.Mobile.csproj -f net10.0-android`.

## Ejecutar pruebas

`dotnet test MathKids.Tests/MathKids.Tests.csproj`

## Agregar una escena

1. Implementa `IGameScene` dentro de `MathKids.Game/Scenes`.
2. Recibe servicios por constructor; no resuelvas dependencias desde la escena.
3. Registra la clase como ella misma y como `IGameScene` en `AddMathKidsGame`.
4. Cambia hacia ella con `SceneManager.ChangeScene<TScene>()`.

## Agregar un generador

Implementa `IExerciseGenerator` en Application o crea un contrato específico cuando convivan varias operaciones. El generador produce `MathExercise`; nunca debe depender de MAUI o SkiaSharp.

## Rendimiento

- Reutilizar pinturas y recursos gráficos.
- Cargar imágenes fuera de `Draw` y conservarlas en `AssetManager`.
- Evitar LINQ, persistencia y asignaciones innecesarias dentro del ciclo.
- Mantener un solo temporizador de cuadros y limitar el delta al reanudar.
- Liberar recursos SkiaSharp descartables.

## Alcance actual

La interfaz usa formas y texto SkiaSharp para que pueda validarse antes de incorporar PNG o WebP. El servicio de audio es provisional y silencioso. SQLite conserva el progreso del perfil local; aún no existen perfiles múltiples, backend, sincronización ni compras.
