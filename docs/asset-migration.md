# Migrar dibujos a imagenes y sonidos

## Donde colocar los recursos

Los recursos que SkiaSharp abrira como `Stream` deben guardarse en:

```text
MathKids.Mobile/Resources/Raw/
|-- Game/
|   |-- Backgrounds/
|   |-- Characters/
|   |-- Objects/
|   `-- Ui/
`-- Audio/
    |-- Effects/
    `-- Music/
```

Ejemplos:

```text
Game/Backgrounds/home_andes.webp
Game/Characters/puma_idle.webp
Game/Characters/condor_happy.webp
Game/Objects/cookie.webp
Audio/Effects/correct.wav
Audio/Music/menu_theme.mp3
```

El proyecto ya incluye `Resources/Raw/**` como `MauiAsset`. Al abrir un recurso no se escribe `Resources/Raw`; se usa la ruta logica `Game/...` o `Audio/...`.

Usar nombres en minusculas, sin espacios ni tildes. WebP es recomendable para produccion; PNG resulta util durante el diseno y para recursos sin perdida.

## Precargar PNG o WebP

La lectura fisica pertenece a `MathKids.Mobile` y el bitmap se almacena en el `AssetManager` de `MathKids.Game`:

```csharp
await using var stream = await FileSystem.OpenAppPackageFileAsync(
    "Game/Characters/puma_idle.webp");

assetManager.Add("puma_idle", stream);
```

La precarga debe ejecutarse una vez antes de iniciar `GameController`. Nunca se debe abrir o decodificar un archivo dentro de `Draw` o `Update`.

## Dibujar una imagen en el canvas

```csharp
private readonly AssetManager _assets;
private SKBitmap? _puma;

public override void Enter()
{
    _puma = _assets.Get("puma_idle");
}

private void DrawPuma(SKCanvas canvas, float bob)
{
    if (_puma is null) return;
    var destination = new SKRect(55f, 280f + bob, 410f, 615f + bob);
    canvas.DrawBitmap(_puma, destination);
}
```

El `SKRect` utiliza coordenadas logicas de 1080x1920. La imagen no reemplaza la zona tactil: los botones deben conservar sus `GameRectangle`.

Para expresiones diferentes se cargan varias claves, por ejemplo `puma_idle`, `puma_happy` y `puma_encouraging`, y se selecciona el bitmap segun el estado de la mascota.

## Fondo adaptable a tablet

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

Esto cubre movil y tablet sin deformar la ilustracion.

## Que dibujos se pueden reemplazar

| Dibujo actual | Recurso sugerido |
|---|---|
| `AndeanDashboardBackdrop` | `Game/Backgrounds/home_andes.webp` |
| `AdditionAdventureBackdrop` | `Game/Backgrounds/addition_world.webp` |
| `BingoFestivalBackdrop` | `Game/Backgrounds/bingo_condor.webp` |
| `AndeanPumaBackdrop` | `Game/Backgrounds/puma_andes.webp` |
| `ChancaLaboratoryBackdrop` | `Game/Backgrounds/chanka_lab.webp` |
| `DrawFoxMascot` | `Game/Characters/fox_*.webp` |
| `PumaGuide` | `Game/Characters/puma_*.webp` |
| `CondorGuide` | `Game/Characters/condor_*.webp` |
| `ChancaGuide` | `Game/Characters/chanka_yachaq_*.webp` |

Conviene mantener con SkiaSharp los numeros, ecuaciones, dialogos, barras, particulas y deteccion tactil.

## Donde colocar sonidos

```text
MathKids.Mobile/Resources/Raw/Audio/
|-- Effects/
|   |-- tap.wav
|   |-- correct.wav
|   `-- try_again.wav
`-- Music/
    `-- menu_theme.mp3
```

Las rutas logicas son `Audio/Effects/correct.wav` y `Audio/Music/menu_theme.mp3`.

El proyecto usa actualmente `NullAudioService`, por eso no se escucha nada. Para activar audio se crea `MauiAudioService`, se precargan los clips y se reemplaza el registro:

```csharp
services.AddSingleton<IAudioService, NullAudioService>();
```

por:

```csharp
services.AddSingleton<IAudioService, MauiAudioService>();
```

Mapeo actual:

```text
AudioCue.Tap       -> Audio/Effects/tap.wav
AudioCue.Correct   -> Audio/Effects/correct.wav
AudioCue.TryAgain  -> Audio/Effects/try_again.wav
```

Usar WAV para efectos cortos y MP3, OGG o M4A para musica. La musica, volumen y mute requieren ampliar `IAudioService`.
