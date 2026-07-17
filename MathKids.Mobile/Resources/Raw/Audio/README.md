# Guia para agregar audio

El sistema de audio ya esta implementado. Solo debes copiar archivos con los nombres indicados y volver a compilar la aplicacion.

## Efectos obligatorios y opcionales

Colocar en `Effects/`:

```text
Effects/
|-- tap.wav
|-- correct.wav
|-- try_again.wav
|-- star.wav
`-- whoosh.wav
```

Uso:

| Archivo | Se reproduce cuando |
|---|---|
| `tap.wav` | Se pulsa una tarjeta, navegacion, volver o se reactiva audio |
| `correct.wav` | El nino responde correctamente |
| `try_again.wav` | La respuesta es incorrecta |
| `star.wav` | Reservado para premios y estrellas |
| `whoosh.wav` | Reservado para transiciones y objetos en movimiento |

Si falta un archivo, la aplicacion sigue funcionando y simplemente omite ese sonido.

## Musica por pantalla

Colocar en `Music/`:

```text
Music/
|-- menu_theme.mp3
|-- addition_theme.mp3
|-- condor_theme.mp3
|-- puma_theme.mp3
`-- chanka_lab_theme.mp3
```

Mapeo:

| Archivo | Pantalla |
|---|---|
| `menu_theme.mp3` | Inicio, menu de juegos y progreso |
| `addition_theme.mp3` | Aventura de sumas |
| `condor_theme.mp3` | Bingo del Condor |
| `puma_theme.mp3` | Sumemos con el Puma |
| `chanka_lab_theme.mp3` | Laboratorio Chanka |

La musica se reproduce en bucle, cambia automaticamente al entrar a otro juego y se pausa cuando la aplicacion pasa a segundo plano.

## Formatos recomendados

- Efectos: WAV, 44.1 kHz, 16 bits, mono o estereo.
- Musica: MP3, OGG o M4A entre 96 y 160 kbps.
- Efectos cortos: idealmente menos de dos segundos.
- Nombres: minusculas, sin espacios y sin tildes.

## Mute y volumen

El boton de altavoz visible en las pantallas ya activa y desactiva el audio. El estado se conserva mediante `Preferences`.

El volumen inicial es 80%. La musica utiliza el 55% del volumen general para no competir con las voces y efectos.

## Archivos de implementacion

```text
MathKids.Application/Abstractions/IAudioService.cs
MathKids.Mobile/Audio/MauiAudioService.cs
MathKids.Mobile/Settings/MauiLocalSettings.cs
MathKids.Mobile/Pages/GameHostPage.cs
MathKids.Mobile/MauiProgram.cs
```

La biblioteca usada es `Plugin.Maui.Audio` version 4.0.0.

## Agregar un efecto nuevo

1. Copiar el WAV dentro de `Effects/`.
2. Agregar un valor a `AudioCue`.
3. Agregar su ruta a `EffectPaths` en `MauiAudioService`.
4. Llamar `audioService.PlayEffect(AudioCue.NuevoEfecto)`.

## Agregar musica nueva

1. Copiar el archivo dentro de `Music/`.
2. Agregar un valor a `MusicCue`.
3. Agregar su ruta a `MusicPaths` en `MauiAudioService`.
4. Llamar `audioService.PlayMusic(MusicCue.NuevaMusica)` desde `Enter()` de la escena.

Nunca abras archivos de audio dentro de `Draw()` o `Update()`.
