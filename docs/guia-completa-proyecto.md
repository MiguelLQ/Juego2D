# Guía completa del proyecto MateAventura

## 1. Propósito

MateAventura es una aplicación educativa para niños de primer y segundo grado con apariencia de videojuego 2D. Está construida con .NET 10, .NET MAUI y SkiaSharp, sin utilizar Unity.

La aplicación actualmente contiene:

- Una pantalla de Inicio animada.
- Un catálogo de Juegos con cuatro espacios.
- Un módulo funcional de sumas.
- Un módulo funcional de bingo de sumas.
- Una pantalla de Progreso.
- Navegación inferior entre Inicio, Juegos y Progreso.
- Botón para volver desde cada juego.
- Sistema provisional de estrellas, monedas, aciertos e intentos.
- Escalado automático para diferentes tamaños de pantalla.
- Animaciones de botones, respuestas y celebraciones.
- Pruebas unitarias para la lógica matemática y partes del motor.

Por ahora, los fondos, la mascota, los iconos y las decoraciones están dibujados directamente con SkiaSharp. Esto permite validar la distribución y el funcionamiento antes de incorporar imágenes finales.

---

## 2. Cómo abrir y ejecutar el proyecto

La solución principal es:

```text
D:\Juego2D\Juego2dApp\MathKids.sln
```

El proyecto que debe ejecutarse es:

```text
MathKids.Mobile
```

`MathKids.Mobile` es el proyecto Android. Los demás proyectos son bibliotecas y no pueden iniciarse directamente como una aplicación móvil.

### Desde Visual Studio

1. Abre `MathKids.sln`.
2. Selecciona `MathKids.Mobile` como proyecto de inicio.
3. Selecciona un emulador Android o un dispositivo físico.
4. Presiona Ejecutar.

### Desde terminal

```powershell
cd D:\Juego2D\Juego2dApp
dotnet build .\MathKids.Mobile\MathKids.Mobile.csproj -f net10.0-android
```

Para ejecutar las pruebas:

```powershell
dotnet test .\MathKids.Tests\MathKids.Tests.csproj
```

---

## 3. Arquitectura general

La solución está dividida en seis proyectos:

```text
MathKids.Domain
MathKids.Application
MathKids.Game
MathKids.Infrastructure
MathKids.Mobile
MathKids.Tests
```

La dirección principal de las dependencias es:

```text
MathKids.Domain
       ↑
MathKids.Application
       ↑
MathKids.Game ← MathKids.Mobile
       ↑              ↑
MathKids.Tests   MathKids.Infrastructure
```

Una explicación simplificada sería:

- `Domain` define qué es un ejercicio, una respuesta o una recompensa.
- `Application` genera ejercicios y coordina reglas de aplicación.
- `Game` dibuja, anima y controla las escenas jugables.
- `Infrastructure` implementa servicios externos o provisionales.
- `Mobile` inicia Android y conecta MAUI con el motor.
- `Tests` comprueba que la lógica funcione correctamente.

No deben existir dependencias circulares.

---

## 4. MathKids.Domain

Ubicación:

```text
MathKids.Domain/
```

Es la capa más central. No depende de MAUI, Android, SkiaSharp ni persistencia.

### Carpetas principales

#### Exercises

Contiene los modelos matemáticos:

- `MathExercise`: ejercicio completo.
- `MathOperation`: suma, resta, multiplicación o división.
- `DifficultyLevel`: dificultad del ejercicio.
- `AnswerOption`: una opción de respuesta.
- `ExerciseResult`: resultado después de responder.

`MathExercise` valida que:

- Exista una respuesta correcta.
- Las opciones no estén duplicadas.
- Solo una opción esté marcada como correcta.

#### Levels

Contiene modelos relacionados con niveles y desbloqueos.

#### Progress

Contiene el modelo de progreso persistente del jugador.

#### Rewards

Contiene modelos de estrellas y monedas entregadas.

#### Profiles

Contiene la información básica del perfil infantil.

### Regla importante

Nunca se debe agregar código SkiaSharp o MAUI dentro de `MathKids.Domain`.

---

## 5. MathKids.Application

Ubicación:

```text
MathKids.Application/
```

Contiene los servicios y casos de uso que trabajan con el dominio.

### Abstractions

Define contratos que otras capas implementan:

- `IClock`: obtiene la hora actual.
- `IRandomProvider`: genera valores aleatorios.
- `IAudioService`: reproduce efectos de sonido.
- `ILocalSettings`: guarda configuraciones locales simples.

### Exercises

Contiene:

- `IExerciseGenerator`.
- `AdditionExerciseGenerator`.

`AdditionExerciseGenerator` crea sumas según la dificultad y genera tres respuestas únicas.

Esta clase no sabe nada sobre botones, pantallas o SkiaSharp. Solamente produce datos matemáticos.

### Sessions

`GameSessionService` evalúa una respuesta y crea un `ExerciseResult`.

### Progress

Contiene los contratos y servicios para registrar progreso.

### Rewards

Calcula estrellas y monedas según los resultados.

### DependencyInjection

`AddMathKidsApplication()` registra todos los servicios de esta capa.

---

## 6. MathKids.Game

Ubicación:

```text
MathKids.Game/
```

Es el motor gráfico y jugable. Aquí se encuentra casi toda la interfaz visible.

### 6.1 Core

#### GameController

Es el coordinador principal del motor.

Responsabilidades:

- Iniciar la primera escena.
- Actualizar la escena actual.
- Dibujar la escena.
- Aplicar el escalado del viewport.
- Convertir la entrada física en entrada lógica.
- Procesar solicitudes de navegación.

#### GameLoop

Calcula el tiempo transcurrido entre cuadros.

El delta está limitado para evitar saltos grandes cuando la aplicación vuelve del segundo plano.

#### GameTime

Representa:

- Tiempo total del juego.
- Tiempo transcurrido desde el cuadro anterior.
- Delta en segundos.

#### GameViewport

La interfaz está diseñada en una resolución lógica fija:

```text
1080 × 1920
```

`GameViewport` adapta esa resolución al dispositivo real.

Calcula:

- Escala uniforme.
- Offset horizontal.
- Offset vertical.
- Letterboxing.
- Conversión física a lógica.
- Conversión lógica a física.

Gracias a esto, las escenas utilizan siempre coordenadas de 1080 × 1920, sin importar la resolución del teléfono.

#### SceneManager

Mantiene las escenas registradas y controla cuál está activa.

Cuando cambia de escena:

1. Ejecuta `Exit()` en la escena anterior.
2. Selecciona la nueva escena.
3. Ejecuta `Enter()` en la nueva escena.

#### GameNavigation

Las escenas solicitan navegación mediante:

```csharp
_navigation.NavigateTo(GameScreen.Games);
```

La navegación se procesa en el siguiente cuadro. Esto evita que una escena conozca directamente al `SceneManager` y elimina dependencias circulares.

#### GameScreen

Enumera las pantallas actuales:

```text
Home
Games
Addition
AdditionBingo
Progress
```

#### PlayerGameState

Mantiene temporalmente:

- Estrellas.
- Monedas.
- Respuestas correctas.
- Intentos realizados.

`PlayerGameState` carga y actualiza estos datos mediante `IProgressService`. La implementación activa utiliza SQLite, por lo que el progreso se conserva al cerrar y volver a abrir la aplicación.

### 6.2 Scenes

Cada pantalla del juego es una escena.

Todas implementan `IGameScene`:

```csharp
public interface IGameScene
{
    GameScreen Screen { get; }
    void Enter();
    void Exit();
    void Update(GameTime gameTime);
    void Draw(SKCanvas canvas, GameViewport viewport);
    void HandleInput(GameInput input);
}
```

#### Enter

Se ejecuta al entrar a la escena. Aquí se reinician contadores o se prepara un ejercicio.

#### Exit

Se ejecuta antes de abandonar la escena.

#### Update

Actualiza animaciones, temporizadores y transiciones.

No debe dibujar ni acceder a una base de datos.

#### Draw

Dibuja el estado actual con SkiaSharp.

No debe cargar imágenes ni generar ejercicios.

#### HandleInput

Recibe entrada táctil ya transformada a coordenadas lógicas.

### 6.3 KidsSceneBase

Archivo:

```text
MathKids.Game/Scenes/Abstractions/KidsSceneBase.cs
```

Contiene los elementos visuales compartidos:

- Fondo azul.
- Nubes.
- Colinas.
- Camino.
- Logo MateAventura.
- Indicador de monedas.
- Mascota provisional dibujada.
- Botón para volver.
- Estrellas.
- Pinturas reutilizables.

Cuando se incorporen imágenes, esta clase será uno de los principales puntos de sustitución.

### 6.4 Escenas actuales

#### HomeScene

Pantalla inicial.

Incluye:

- Logo.
- Mensaje animado de bienvenida.
- Mascota provisional.
- Acceso directo a sumas.
- Acceso directo a bingo.
- Barra inferior.

#### GamesMenuScene

Catálogo de juegos.

Contiene cuatro módulos:

- Aventura de sumas.
- Bingo de sumas.
- Próximo juego 1.
- Próximo juego 2.

Los dos módulos futuros son visuales y todavía no responden al toque.

#### AdditionDemoScene

Juego principal de sumas.

Flujo:

1. Solicita un ejercicio a `IExerciseGenerator`.
2. Muestra la operación.
3. Muestra tres botones.
4. Evalúa la respuesta mediante `IGameSessionService`.
5. Si falla, muestra rojo y permite reintentar.
6. Si acierta, muestra verde, partículas y una nueva suma.
7. Actualiza estrellas y monedas.

#### AdditionBingoScene

Juego de bingo infantil.

Flujo:

1. Genera una suma.
2. Construye un tablero de nueve respuestas únicas.
3. El niño busca el resultado correcto.
4. Una respuesta incorrecta se marca temporalmente en rojo.
5. Una respuesta correcta suma un punto de bingo.
6. El objetivo es llegar a cinco puntos.

#### ProgressScene

Muestra:

- Estrellas ganadas.
- Aciertos.
- Intentos.
- Monedas.

### 6.5 Components

#### GraphicButton

Botón reutilizable dibujado con SkiaSharp.

Permite configurar:

- Color normal.
- Color de texto.
- Tamaño de texto.
- Radio de esquinas.
- Rectángulo táctil.
- Estado normal, correcto o incorrecto.
- Animación de presión.
- Animación de retroalimentación.

#### BottomNavigationBar

Dibuja y controla:

- Inicio.
- Juegos.
- Progreso.

No se agregó acceso para padres.

### 6.6 Graphics

#### FloatTween

Interpola valores con el tiempo. Actualmente se utiliza para las animaciones de escala de los botones.

#### AssetManager

Mantiene imágenes `SKBitmap` cargadas en memoria.

Funciones principales:

```csharp
assetManager.Add("fox", stream);
var fox = assetManager.Get("fox");
```

El mismo bitmap se reutiliza en todos los cuadros.

### 6.7 Input

#### GameInput

Representa una entrada táctil:

- Presionado.
- Movido.
- Soltado.
- Cancelado.

#### HitTest

Comprueba si una coordenada está dentro de un rectángulo.

Las imágenes futuras no deben controlar directamente la interacción. Se deben conservar los `GameRectangle` como áreas táctiles.

---

## 7. MathKids.Infrastructure

Ubicación:

```text
MathKids.Infrastructure/
```

Implementa servicios definidos en Application.

### Implementaciones actuales

- `SystemClock`: hora actual.
- `SystemRandomProvider`: números aleatorios.
- `MemoryLocalSettings`: configuración temporal en memoria.
- `SqliteProgressRepository`: progreso persistente en SQLite.
- `InMemoryProgressRepository`: alternativa utilizada por pruebas unitarias rápidas.
- `NullAudioService`: implementación silenciosa de audio.

`NullAudioService` permite que las escenas soliciten sonidos sin fallar, aunque todavía no existan archivos de audio.

En el futuro, esta capa podrá ampliar:

- Migraciones para nuevas versiones del esquema SQLite.
- Preferencias persistentes.
- Audio real.
- Repositorios locales.

---

## 8. MathKids.Mobile

Ubicación:

```text
MathKids.Mobile/
```

Es el proyecto ejecutable de Android.

### MauiProgram

Es el punto de composición.

Registra:

```csharp
services.AddMathKidsApplication();
services.AddMathKidsInfrastructure();
services.AddMathKidsGame();
services.AddMathKidsMobile();
```

También activa SkiaSharp.

### App

Crea la ventana y conecta los eventos de pausa y reanudación.

### GameHostPage

Contiene el `SKCanvasView`.

Responsabilidades:

- Mantener la superficie SkiaSharp.
- Solicitar aproximadamente 60 cuadros por segundo.
- Delegar el dibujo a `GameController`.
- Convertir eventos táctiles de SkiaSharp en `GameInput`.
- Pausar el motor al ir a segundo plano.
- Reanudarlo al volver.

No debe contener reglas matemáticas ni lógica específica de los juegos.

---

## 9. MathKids.Tests

Ubicación:

```text
MathKids.Tests/
```

Pruebas actuales:

- Generación correcta de sumas.
- Rangos según dificultad.
- Opciones sin duplicados.
- Evaluación de respuestas.
- Escalado de viewport.
- Conversión de coordenadas.
- Secuencia de acierto y reintento.
- Solicitudes de navegación.
- Acumulación de progreso.

Cuando se agregue un nuevo tipo de ejercicio, se deben agregar pruebas antes de conectarlo con una escena.

---

## 10. Flujo completo de ejecución

El flujo de un cuadro es:

```text
DispatcherTimer de MAUI
        ↓
SKCanvasView.InvalidateSurface
        ↓
GameHostPage.OnPaintSurface
        ↓
GameController.Render
        ↓
GameLoop.Advance
        ↓
Escena.Update
        ↓
GameViewport aplica escala y offsets
        ↓
Escena.Draw
```

El flujo táctil es:

```text
Toque Android
        ↓
SKCanvasView.Touch
        ↓
GameController.HandleInput
        ↓
GameViewport.PhysicalToLogical
        ↓
Escena.HandleInput
        ↓
HitTest / GraphicButton
```

---

## 11. Cómo agregar una nueva escena

Ejemplo: un juego de restas.

### Paso 1: agregar el identificador

En `GameScreen.cs`:

```csharp
public enum GameScreen
{
    Home,
    Games,
    Addition,
    AdditionBingo,
    Subtraction,
    Progress
}
```

### Paso 2: crear la escena

```csharp
public sealed class SubtractionScene : KidsSceneBase
{
    public override GameScreen Screen => GameScreen.Subtraction;

    public override void Enter() { }
    public override void Exit() { }
    public override void Update(GameTime gameTime) { }
    public override void Draw(SKCanvas canvas, GameViewport viewport) { }
    public override void HandleInput(GameInput input) { }
}
```

### Paso 3: registrar la escena

En `MathKids.Game/DependencyInjection/ServiceCollectionExtensions.cs`:

```csharp
services.AddSingleton<SubtractionScene>();
services.AddSingleton<IGameScene>(provider => provider.GetRequiredService<SubtractionScene>());
```

### Paso 4: navegar

```csharp
_navigation.NavigateTo(GameScreen.Subtraction);
```

### Paso 5: agregar el acceso visual

Agrega una tarjeta en `GamesMenuScene` con su propio `GameRectangle`.

---

## 12. Cómo agregar un nuevo módulo matemático

No se debe comenzar por la pantalla. Primero se crea la lógica.

Orden recomendado:

1. Crear modelos o reglas en Domain si son necesarios.
2. Crear el generador en Application.
3. Agregar pruebas unitarias.
4. Registrar el generador con inyección de dependencias.
5. Crear la escena en Game.
6. Agregar navegación.
7. Compilar Android.

Ejemplos de módulos futuros:

- Restas con objetos.
- Comparación mayor/menor.
- Secuencias numéricas.
- Conteo visual.
- Memoria de parejas matemáticas.
- Rompecabezas de formas.

---

## 13. Dónde colocar imágenes

La ubicación recomendada para imágenes utilizadas directamente por SkiaSharp es:

```text
MathKids.Mobile/Resources/Raw/Game/
```

Estructura sugerida:

```text
Resources/Raw/Game/
├── backgrounds/
│   ├── home_background.webp
│   ├── games_background.webp
│   └── addition_background.webp
├── characters/
│   ├── fox_idle.webp
│   ├── fox_happy.webp
│   └── fox_thinking.webp
├── ui/
│   ├── logo_mateaventura.webp
│   ├── button_home.webp
│   ├── button_games.webp
│   ├── button_progress.webp
│   ├── card_game.webp
│   └── coin.webp
├── decorations/
│   ├── star.webp
│   ├── cloud.webp
│   ├── confetti.webp
│   └── flower.webp
└── games/
    ├── addition_icon.webp
    └── bingo_icon.webp
```

Los archivos dentro de `Resources/Raw` ya son incluidos por el proyecto MAUI mediante el patrón configurado en `MathKids.Mobile.csproj`.

### Reglas para nombres

Usar:

```text
fox_happy.webp
addition_background.webp
button_progress.webp
```

Evitar:

```text
Zorro Feliz Final 2.webp
FONDO NUEVO.png
botón inicio.png
```

Reglas:

- Solo minúsculas.
- Sin espacios.
- Sin tildes.
- Usar guion bajo.
- Nombres descriptivos.

---

## 14. PNG o WebP

### WebP

Recomendado para:

- Fondos.
- Personajes.
- Decoraciones.
- Tarjetas complejas.
- Assets finales de producción.

Ventajas:

- Menor tamaño.
- Soporta transparencia.
- Reduce el peso del APK.

### PNG

Recomendado para:

- Assets temporales de diseño.
- Elementos donde se requiere fidelidad sin pérdida.
- Archivos que el diseñador todavía está modificando.

Para producción, normalmente conviene convertir los PNG grandes a WebP.

---

## 15. Tamaños sugeridos de imágenes

La resolución lógica es 1080 × 1920.

Tamaños aproximados:

| Elemento | Tamaño sugerido |
|---|---:|
| Fondo completo | 1080 × 1920 |
| Logo | 800 × 250 |
| Mascota principal | 450 × 650 |
| Icono de juego | 256 × 256 |
| Icono navegación | 160 × 160 |
| Moneda | 96 × 96 |
| Estrella | 128 × 128 |
| Decoración pequeña | 64–192 px |

No es necesario exportar imágenes al doble o triple de la resolución lógica. Esto aumenta memoria y tiempo de carga sin mejorar notablemente la apariencia.

---

## 16. Cómo cargar imágenes con AssetManager

`MathKids.Game` no puede usar directamente `FileSystem` de MAUI porque eso rompería la separación entre Game y Mobile.

La carga física debe comenzar en `MathKids.Mobile` y el bitmap resultante debe almacenarse en `AssetManager`.

Ejemplo conceptual de un precargador dentro de Mobile:

```csharp
using MathKids.Game.Graphics.Assets;

public sealed class GameAssetPreloader(AssetManager assetManager)
{
    public async Task LoadAsync()
    {
        await LoadBitmapAsync("fox_idle", "Game/characters/fox_idle.webp");
        await LoadBitmapAsync("logo", "Game/ui/logo_mateaventura.webp");
        await LoadBitmapAsync("home_background", "Game/backgrounds/home_background.webp");
    }

    private async Task LoadBitmapAsync(string key, string path)
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync(path);
        assetManager.Add(key, stream);
    }
}
```

Este precargador deberá registrarse en `AddMathKidsMobile()` y ejecutarse antes de iniciar el game loop.

### Regla crítica

Nunca hacer esto dentro de `Draw`:

```csharp
var bitmap = SKBitmap.Decode("fox.webp");
```

`Draw` se ejecuta aproximadamente 60 veces por segundo. Decodificar una imagen ahí causaría bloqueos, consumo de memoria y cierres de la aplicación.

---

## 17. Cómo dibujar una imagen cargada

Después de cargarla:

```csharp
var fox = assetManager.Get("fox_idle");
var destination = new SKRect(120f, 300f, 520f, 900f);
canvas.DrawBitmap(fox, destination);
```

El rectángulo está expresado en coordenadas lógicas de 1080 × 1920.

### Sustituir la mascota provisional

Actualmente la mascota se dibuja en:

```text
KidsSceneBase.DrawFoxMascot
```

Proceso recomendado:

1. Inyectar `AssetManager` en las escenas que necesiten mascota.
2. Obtener `fox_idle` durante `Enter()` o una preparación previa.
3. Guardar la referencia en un campo.
4. Sustituir las formas por `DrawBitmap`.
5. Mantener el parámetro de movimiento vertical para conservar la animación.

Ejemplo:

```csharp
private SKBitmap? _fox;

public override void Enter()
{
    _fox = _assetManager.Get("fox_idle");
}

private void DrawFox(SKCanvas canvas, float bob)
{
    var rect = new SKRect(90f, 300f + bob, 510f, 900f + bob);
    canvas.DrawBitmap(_fox, rect);
}
```

---

## 18. Qué elementos conviene mantener dibujados

No todo debe convertirse en imagen.

Conviene mantener dinámicos en SkiaSharp:

- Operaciones matemáticas.
- Números de respuesta.
- Mensajes de acierto y error.
- Barras de progreso.
- Contadores.
- Monedas y estrellas acumuladas.
- Fondos simples de botones.
- Áreas táctiles.
- Partículas sencillas.

Conviene convertir en imágenes:

- Mascota.
- Logo final.
- Fondos ilustrados.
- Iconos complejos.
- Casas, árboles y paisajes.
- Decoraciones artísticas.
- Marcos detallados.

Esta combinación mantiene la interfaz bonita, dinámica y eficiente.

---

## 19. Orden de dibujo recomendado

Dentro de una escena:

```text
1. Fondo completo
2. Paisaje y decoraciones lejanas
3. Personajes
4. Tarjetas y paneles
5. Textos y operaciones
6. Botones
7. Efectos y partículas
8. Navegación superior o inferior
```

SkiaSharp dibuja en orden. Lo último aparece encima.

---

## 20. Mantener la interacción al cambiar dibujos por imágenes

Una imagen no debe reemplazar la lógica del botón.

El patrón correcto es:

```csharp
private static readonly GameRectangle PlayButtonBounds = new(100f, 900f, 400f, 220f);
```

Para dibujar:

```csharp
canvas.DrawBitmap(buttonBitmap, new SKRect(
    PlayButtonBounds.Left,
    PlayButtonBounds.Top,
    PlayButtonBounds.Right,
    PlayButtonBounds.Bottom));
```

Para tocar:

```csharp
if (IsReleasedInside(input, PlayButtonBounds))
{
    _navigation.NavigateTo(GameScreen.Addition);
}
```

De esta forma, cambiar el arte no rompe el comportamiento.

---

## 21. Animaciones con imágenes

### Movimiento simple

Se puede conservar el movimiento actual cambiando solamente la posición:

```csharp
var bob = MathF.Sin(elapsed * 2.4f) * 12f;
var destination = new SKRect(100f, 300f + bob, 500f, 900f + bob);
canvas.DrawBitmap(fox, destination);
```

### Sprite sheets

Para animaciones de caminar o celebrar:

1. Exportar un sprite sheet.
2. Definir el rectángulo de origen del cuadro actual.
3. Cambiar el cuadro desde `Update`.
4. Dibujarlo con origen y destino.

```csharp
canvas.DrawBitmap(spriteSheet, sourceRect, destinationRect);
```

No se deben cambiar cuadros utilizando varios timers. El `GameLoop` debe controlar todas las animaciones.

---

## 22. Sonidos

Los sonidos pueden colocarse en:

```text
MathKids.Mobile/Resources/Raw/Audio/
```

Estructura sugerida:

```text
Audio/
├── tap.wav
├── correct.wav
├── try_again.wav
├── star.wav
└── menu_music.mp3
```

`IAudioService` ya existe. Se deberá reemplazar `NullAudioService` por una implementación real.

Los efectos deben ser cortos y la música debe respetar pausa, reanudación y configuración de volumen.

---

## 23. Reglas de rendimiento

- No crear `SKPaint` en cada cuadro.
- No decodificar imágenes dentro de `Draw`.
- No consultar SQLite dentro de `Update` o `Draw`.
- No usar múltiples timers para animaciones.
- Reutilizar `SKBitmap`, `SKPath` y `SKPaint`.
- Liberar recursos `IDisposable`.
- Evitar LINQ dentro del game loop.
- Mantener un solo game loop.
- Limitar el delta al reanudar.
- Cargar únicamente los assets necesarios.
- Reducir imágenes grandes antes de incluirlas.

---

## 24. Próximos pasos recomendados

Orden sugerido:

1. Diseñar y exportar el logo final.
2. Crear la mascota en estados idle, feliz y pensando.
3. Reemplazar el fondo provisional.
4. Implementar `GameAssetPreloader`.
5. Incorporar efectos de sonido.
6. Agregar perfiles infantiles múltiples sobre SQLite.
7. Crear perfiles infantiles.
8. Agregar restas y conteo visual.
9. Crear una escena final de resultados por sesión.
10. Añadir selección de dificultad por grado.

---

## 25. Resumen rápido para el equipo

| Necesidad | Proyecto |
|---|---|
| Cambiar reglas matemáticas | MathKids.Domain / Application |
| Crear un generador | MathKids.Application |
| Crear una pantalla o juego | MathKids.Game |
| Dibujar con SkiaSharp | MathKids.Game |
| Agregar imágenes Android | MathKids.Mobile/Resources/Raw/Game |
| Cargar físicamente assets | MathKids.Mobile |
| Reutilizar bitmaps | MathKids.Game/AssetManager |
| Modificar SQLite o implementar audio | MathKids.Infrastructure |

---

## 26. Persistencia SQLite actual

La base se crea automáticamente con el nombre:

```text
mathkids.db3
```

En Android se guarda dentro de `FileSystem.AppDataDirectory`, un directorio privado de la aplicación. No se necesitan permisos de almacenamiento.

La tabla actual es `player_progress` y almacena:

- Identificador del perfil.
- Cantidad total de retos intentados.
- Respuestas correctas.
- Estrellas.
- Monedas.
- Fecha UTC de la última actualización.
- Identificador del último juego.

El repositorio está en:

```text
MathKids.Infrastructure/Persistence/SqliteProgressRepository.cs
```

La ruta física se configura en:

```text
MathKids.Mobile/DependencyInjection/ServiceCollectionExtensions.cs
```

Cada respuesta actualiza SQLite fuera de `Update` y `Draw`. La pantalla de Progreso vuelve a cargar los datos al entrar y muestra el último juego y la fecha guardada.

El esquema utiliza `PRAGMA user_version = 1`. Cuando se agreguen nuevas columnas se debe implementar una migración incremental en lugar de eliminar la base existente.
---

## 27. Juego Sumemos con el Puma

El tercer modulo presenta sumas mediante objetos concretos. Cada ejercicio dibuja dos grupos de galletas sobre una mesa, los separa con el signo `+` y ofrece tres respuestas grandes.

Archivos principales:

```text
MathKids.Application/Exercises/IPumaExerciseGenerator.cs
MathKids.Application/Exercises/PumaExerciseGenerator.cs
MathKids.Game/Scenes/Addition/PumaAdditionScene.cs
MathKids.Game/Components/Characters/PumaGuide.cs
```

El generador limita cada grupo a entre una y seis galletas y produce resultados de hasta doce. La escena registra la actividad con la clave `puma_addition`, por lo que sus puntos, aciertos, intentos y ultima partida quedan guardados en SQLite.

Si el nino falla, el boton se marca temporalmente y el puma muestra un mensaje de apoyo antes de habilitar nuevamente las respuestas. Si acierta, el puma felicita, se guardan las recompensas y se carga un ejercicio nuevo.

## 28. Fondos separados por juego

Los fondos estan en `MathKids.Game/Graphics/Drawing/GameBackdrops.cs` y tienen tres implementaciones independientes:

- `AdditionAdventureBackdrop`: cielo, nubes y colinas de la aventura clasica.
- `BingoFestivalBackdrop`: ambiente morado de fiesta, banderines y colinas.
- `AndeanPumaBackdrop`: sol, nubes, nevados, terrazas y camino andino.

Cada fondo implementa `IGameBackdrop`, recibe tiempo mediante `Update` y dibuja mediante `Draw`. Para cambiar solo el ambiente de un juego, se modifica su backdrop sin tocar las reglas matematicas ni los botones.

## 29. Adaptacion a tablets

`GameViewport` mantiene el contenido interactivo en una zona logica de 1080x1920. Sus propiedades `VisibleLogicalLeft`, `VisibleLogicalTop`, `VisibleLogicalRight` y `VisibleLogicalBottom` exponen el area adicional visible.

Los fondos dibujan hasta esos limites, evitando barras vacias en tablets con relaciones de aspecto diferentes. Los botones permanecen centrados en la zona segura, conservando posiciones y deteccion tactil consistentes.

Al sustituir fondos por PNG o WebP, la imagen debe cubrir los limites visibles y recortar el exceso, sin estirar los controles ni cambiar sus coordenadas logicas.

## 30. Juego El Laboratorio Chanka

El cuarto modulo adapta la idea de un laboratorio de sumas a un ambiente inspirado en la cultura Chanka. La interfaz utiliza patrones geometricos, muros de piedra, colores tierra, montanas, un yachaq animado y tecnologia fantastica de energia andina.

Archivos principales:

```text
MathKids.Game/Scenes/Addition/ChancaLaboratoryScene.cs
MathKids.Game/Components/Characters/ChancaGuide.cs
MathKids.Game/Graphics/Drawing/GameBackdrops.cs
```

La escena reutiliza `IExerciseGenerator` para crear la suma y tres opciones. Cada opcion se presenta como una esfera numerica. Al tocarla:

1. Se bloquean temporalmente las otras opciones.
2. La esfera viaja visualmente por el conducto.
3. El recipiente muestra burbujas y los valores usados en la suma.
4. El yachaq cambia de expresion y muestra un dialogo.
5. Si es correcta se guarda el progreso y aparece otro experimento.
6. Si es incorrecta se permite probar nuevamente el mismo ejercicio.

La actividad se guarda en SQLite con la clave `chanca_laboratory`. El fondo propio es `ChancaLaboratoryBackdrop` y puede reemplazarse por una ilustracion PNG o WebP sin modificar la logica del juego.

Para cambiar los textos de apoyo se actualiza `SpeechBubble.Text` desde `ChancaLaboratoryScene`. Para cambiar el personaje provisional se reemplaza el dibujo de `ChancaGuide` por sprites conservando los estados `Curious`, `Celebrating` y `Encouraging`.

## 31. Sistema visual profesional

`MathKids.Game/Components/SpeechBubble.cs` centraliza los dialogos de las mascotas. Divide el texto en hasta tres lineas, reduce el tamano cuando es necesario y evita que las frases salgan de la nube.

El dashboard utiliza `AndeanDashboardBackdrop`, iconos SkiaSharp propios y cuatro tarjetas compactas. Todas las pantallas muestran un boton visual de audio preparado para conectarse posteriormente con `IAudioService`.

El juego del Puma alterna galletas, caramelos y chupetes. El Bingo incorpora un Condor animado con estados de vuelo, celebracion y animo. Los personajes y fondos se actualizan desde el mismo `GameLoop`, sin temporizadores adicionales.

## 32. Agregar PNG y WebP al canvas

### Rutas recomendadas

```text
MathKids.Mobile/Resources/Raw/Game/
|-- Backgrounds/
|   |-- home_andes.webp
|   |-- addition_world.webp
|   |-- bingo_condor.webp
|   |-- puma_andes.webp
|   `-- chanka_lab.webp
|-- Characters/
|   |-- fox_idle.webp
|   |-- fox_happy.webp
|   |-- puma_idle.webp
|   |-- puma_happy.webp
|   |-- condor_idle.webp
|   `-- chanka_yachaq.webp
|-- Objects/
|   |-- cookie.webp
|   |-- candy.webp
|   `-- lollipop.webp
`-- Ui/
    |-- logo.webp
    |-- coin.webp
    `-- icons/
```

Usar nombres en minusculas, sin espacios ni tildes. WebP es recomendable para produccion porque reduce el APK y admite transparencia. PNG es adecuado para recursos temporales, interfaces sin perdida y archivos que siguen en diseno.

`MathKids.Mobile.csproj` ya incluye:

```xml
<MauiAsset Include="Resources\Raw\**"
           LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
```

Por eso la ruta logica omite `Resources/Raw`:

```csharp
await using var stream = await FileSystem.OpenAppPackageFileAsync(
    "Game/Characters/condor_idle.webp");
```

### Precargar antes del game loop

La lectura fisica se hace en `MathKids.Mobile`; `MathKids.Game` solo recibe los bitmaps mediante `AssetManager`:

```csharp
public sealed class GameAssetPreloader(AssetManager assets)
{
    private bool _loaded;

    public async Task EnsureLoadedAsync()
    {
        if (_loaded) return;

        await LoadAsync("home_background", "Game/Backgrounds/home_andes.webp");
        await LoadAsync("fox_idle", "Game/Characters/fox_idle.webp");
        await LoadAsync("puma_idle", "Game/Characters/puma_idle.webp");
        await LoadAsync("condor_idle", "Game/Characters/condor_idle.webp");
        await LoadAsync("chanka_yachaq", "Game/Characters/chanka_yachaq.webp");
        _loaded = true;
    }

    private async Task LoadAsync(string key, string logicalPath)
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync(logicalPath);
        assets.Add(key, stream);
    }
}
```

Registrar `GameAssetPreloader` en `AddMathKidsMobile()` y ejecutar `EnsureLoadedAsync()` antes de `GameController.Start()`.

No utilizar `SKBitmap.Decode`, `OpenAppPackageFileAsync` ni `AssetManager.Add` dentro de `Draw` o `Update`. Estas funciones se ejecutan aproximadamente 60 veces por segundo.

### Inyectar y dibujar

```csharp
private readonly AssetManager _assets;
private SKBitmap? _pumaBitmap;

public PumaAdditionScene(AssetManager assets /* otros servicios */)
{
    _assets = assets;
}

public override void Enter()
{
    _pumaBitmap = _assets.Get("puma_idle");
    LoadNextExercise();
}

private void DrawPumaImage(SKCanvas canvas, float bob)
{
    if (_pumaBitmap is null) return;
    var destination = new SKRect(55f, 280f + bob, 410f, 615f + bob);
    canvas.DrawBitmap(_pumaBitmap, destination);
}
```

Para cambiar expresiones se precargan varias claves (`puma_idle`, `puma_happy`, `puma_encouraging`) y se selecciona una segun `PumaMood`.

El `SKRect` utiliza coordenadas logicas de 1080x1920. La imagen solo cambia el dibujo; los `GameRectangle` de botones y zonas tactiles deben conservarse.

### Fondo cover para movil y tablet

```csharp
private static SKRect CalculateCover(SKBitmap bitmap, GameViewport viewport)
{
    var visibleWidth = viewport.VisibleLogicalRight - viewport.VisibleLogicalLeft;
    var visibleHeight = viewport.VisibleLogicalBottom - viewport.VisibleLogicalTop;
    var scale = Math.Max(visibleWidth / bitmap.Width, visibleHeight / bitmap.Height);
    var width = bitmap.Width * scale;
    var height = bitmap.Height * scale;
    var left = viewport.VisibleLogicalLeft + (visibleWidth - width) / 2f;
    var top = viewport.VisibleLogicalTop + (visibleHeight - height) / 2f;
    return new SKRect(left, top, left + width, top + height);
}
```

```csharp
canvas.DrawBitmap(background, CalculateCover(background, viewport));
```

Usar `cover` para fondos y rectangulos fijos tipo `contain` para personajes e iconos.

### Reglas de exportacion

- Personajes y objetos: fondo transparente.
- Fondos: imagen completa sin transparencia.
- Recortar espacio transparente innecesario.
- Mantener el mismo punto de apoyo entre estados de una mascota.
- No exportar fondos mucho mayores a 1080x1920 sin necesidad.
- Mantener textos, numeros, dialogos, progreso y particulas en SkiaSharp.

## 33. Agregar sonidos

### Carpetas exactas

```text
MathKids.Mobile/Resources/Raw/Audio/
|-- Effects/
|   |-- tap.wav
|   |-- correct.wav
|   |-- try_again.wav
|   |-- star.wav
|   `-- whoosh.wav
`-- Music/
    |-- menu_theme.mp3
    |-- puma_theme.mp3
    |-- condor_theme.mp3
    `-- chanka_lab_theme.mp3
```

Las rutas logicas son `Audio/Effects/correct.wav` y `Audio/Music/menu_theme.mp3`. Usar WAV para efectos breves y MP3, OGG o M4A para musica.

### Estado actual

El contrato esta en `MathKids.Application/Abstractions/IAudioService.cs`:

```csharp
public interface IAudioService
{
    void PlayEffect(AudioCue cue);
}

public enum AudioCue
{
    Tap,
    Correct,
    TryAgain
}
```

La implementacion registrada es `NullAudioService`, por eso actualmente no se escucha nada aunque las escenas llamen `PlayEffect`.

Mapeo recomendado:

```text
AudioCue.Tap       -> Audio/Effects/tap.wav
AudioCue.Correct   -> Audio/Effects/correct.wav
AudioCue.TryAgain  -> Audio/Effects/try_again.wav
```

Crear `MauiAudioService`, cargar esos clips una sola vez y reemplazar en `MathKids.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`:

```csharp
services.AddSingleton<IAudioService, NullAudioService>();
```

por:

```csharp
services.AddSingleton<IAudioService, MauiAudioService>();
```

La implementacion puede usar una biblioteca MAUI de audio o servicios nativos. Se debe verificar la API de la biblioteca elegida para la version instalada.

### Musica, volumen y mute

Para conectar el boton visual de audio, ampliar el contrato:

```csharp
public interface IAudioService
{
    bool IsMuted { get; }
    void PlayEffect(AudioCue cue);
    void PlayMusic(MusicCue cue, bool loop = true);
    void StopMusic();
    void SetMuted(bool muted);
    void SetVolume(float volume);
}
```

Guardar mute y volumen mediante `ILocalSettings`. La musica debe comenzar al entrar a una escena y detenerse o cambiar al salir, nunca desde `Draw` o `Update`.

Reglas:

- Precargar efectos pequenos al iniciar.
- Reutilizar reproductores y buffers.
- No abrir archivos en cada respuesta.
- Evitar musicas simultaneas.
- Pausar al enviar la aplicacion a segundo plano.
- Respetar mute antes de reproducir cualquier clip.

La version resumida se encuentra en `docs/asset-migration.md`.

| Configurar inicio Android | MathKids.Mobile |
| Agregar pruebas | MathKids.Tests |

La regla principal del proyecto es mantener separadas la lógica matemática, el renderizado y la plataforma móvil. Esto permite mejorar el arte, agregar nuevos juegos y cambiar la persistencia sin romper las demás partes de la aplicación.
