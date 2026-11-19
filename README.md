# Ecos del Umbral

Juego de acción en tercera persona hecho en Unity 6 para la materia de motores gráficos. Se explora una zona de ruinas, se juntan mejoras, se derrota al Guardián del Umbral y se cruza un portal hacia una isla en el abismo donde espera el segundo jefe.

Esta es la versión del recuperatorio del Segundo Parcial.

## Qué hay en el juego

- Zona de ruinas sobre un terreno con colinas, un sendero con losas y faroles, y seis puestos de avanzada con cofres y enemigos.
- Cripta secreta detrás de una pared que se rompe a golpes o disparos. Adentro está la llave de la arena.
- Arena del Guardián (caballero corrupto de dos fases) y cofre final.
- Portal al Abismo: isla con puente, pilares de cobertura y el Kraken Pistolero.
- Pistola en la mano izquierda y daga en la mano derecha.
- Fijado de objetivo, esquive con invulnerabilidad, vida y estamina, mejoras permanentes en cofres.
- Iluminación horneada, antorchas dentro de la mazmorra, post-procesado distinto por zona.
- Música y efectos generados por código, líneas de voz, partículas, menú principal, pausa y pantalla de derrota.

## Controles

| Acción | Tecla |
|---|---|
| Moverse | W A S D |
| Cámara | Mouse |
| Saltar | Espacio |
| Rodar / esquivar | Shift izquierdo |
| Disparar pistola | Click izquierdo |
| Golpe de daga | Click derecho (gasta estamina) |
| Fijar objetivo | R |
| Interactuar | E |
| Pausa | Esc o P |

## Cómo abrirlo

1. Clonar el repositorio.
2. Abrir la carpeta desde Unity Hub con Unity 6000.3.5f1 (o una 6000.3 compatible).
3. Abrir `Assets/Escenas/Escena.unity` y apretar Play.

La build para Windows se genera desde el menú `Tools > Umbral > 6 Compilar para Windows` y queda en `Builds/Windows`.

## Carpetas

```
Assets/
  Animaciones/   controladores y clips (jugador, caballero, jefe)
  Audio/         líneas de voz y mezclador
  Configuracion/ perfiles de post-procesado, ajustes de iluminación y de render
  Editor/        herramientas que arman la escena (menú Tools > Umbral)
  Escenas/       escena principal y datos de iluminación
  Materiales/    materiales del recorrido, personajes y armas
  Prefabs/       jefes, cofres, balas, tablillas, llave, puerta de la arena
  Scripts/       código del juego
  Terreno/       datos y capas del terreno
  Texturas/      texturas de piedra, suelo y madera
Docs/            GDD, informe de testing y textos de entrega
```

## Scripts principales

- `ControlJugador`: movimiento, salto, esquive y disparo.
- `ArmasJugador`: golpe de daga y animación de la pistola.
- `AnimacionJugador`: parámetros del Animator del jugador.
- `EstadisticasJugador`: vida, estamina, nivel y derrota.
- `FijarObjetivo`: elige y mantiene el objetivo fijado.
- `Proyectil`: bala del jugador.
- `IAJefe` y `VidaJefe`: caballero con NavMesh, ataque, embestida y golpe al suelo.
- `JefePulpo`: Kraken con patrones de balas, cambio de poza y furia.
- `Centinela` y `Tirador`: enemigos comunes.
- `CofreBase`, `CofreMejora`, `RecogerLlave`, `PuertaCerrada`, `InterruptorDisparable`, `TablillaHistoria`, `Portal`: progresión.
- `GestorHUD`, `MenuPrincipal`, `MenuPausa`, `PantallaDerrota`: interfaz.
- `GestorLuces`, `ConfigPostProceso`: luces y post-procesado.
- `EfectosJuego`, `DirectorMusica`, `Narracion`, `EfectoDanio`: sonido y feedback.

## Créditos

Nicolas Gabriel Morey ([@NekoMorey](https://github.com/NekoMorey)).
