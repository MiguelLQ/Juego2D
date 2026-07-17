# Sustituir dibujos por imágenes

1. Exporta cada elemento por separado como WebP o PNG transparente: logo, mascota, iconos, fondos y decoraciones.
2. Coloca los archivos en `MathKids.Mobile/Resources/Raw/Game` usando nombres en minúsculas.
3. Carga cada archivo una sola vez al entrar o preparar la escena y regístralo en `AssetManager`; nunca lo decodifiques dentro de `Draw`.
4. Conserva los mismos rectángulos lógicos usados por los dibujos actuales y reemplaza cada grupo por `canvas.DrawBitmap`.
5. Mantén en SkiaSharp los textos, barras de progreso, números y áreas táctiles para que sigan siendo dinámicos.
6. Libera los bitmaps cuando cierre el motor y prueba al menos una pantalla 16:9 y otra más alta.

WebP suele ser preferible para fondos y personajes con transparencia por su menor tamaño. PNG es útil durante diseño cuando se necesita máxima fidelidad sin pérdida.
