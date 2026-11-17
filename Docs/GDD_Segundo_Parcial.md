# Ecos del Umbral — Documento de diseño (Recuperatorio del Segundo Parcial)

Autor: Nicolas Gabriel Morey (NekoMorey)
Motor: Unity 6000.3.5f1, URP 17.3
Plataforma: Windows, publicado en itch.io
Entrega: viernes 20/11/2026

## 1. Idea general

Un guerrero baja a las ruinas de un umbral antiguo para frenar a los guardianes que lo cuidan. Recorre las ruinas, encuentra una llave, vence a un caballero corrupto y cruza un portal hacia una isla en el abismo, donde lo espera el Kraken Pistolero, un pulpo con cuatro pistolas.

Lo que quise lograr:
1. Que explorar valga la pena: los cofres de mejora, las tablillas y la cripta secreta están fuera del camino directo.
2. Que cada jefe se juegue distinto: cuerpo a cuerpo y esquive contra el caballero, cobertura y movimiento contra el Kraken.
3. Un clima de ruina misteriosa con luz horneada, antorchas, partículas y sonido.

## 2. Jugabilidad

Tercera persona con cámara orbital (Cinemachine). El jugador camina, salta, rueda para esquivar, dispara con la pistola de la mano izquierda y pega con la daga de la mano derecha. Puede fijar la mira en cualquier enemigo cercano.

Ciclo básico: explorar, conseguir mejoras, enfrentar enemigos, vencer al jefe, abrir el cofre y seguir al próximo tramo.

Recursos:
- Vida: baja con los ataques enemigos.
- Estamina: se gasta al rodar y al golpear con la daga (8 por golpe); se recupera 18 por segundo después de 0,8 s sin usarla.
- Fuerza: sube el daño de las balas y de la daga.
- Llave del Umbral: abre la puerta de la arena.

## 3. Mecánicas

| Mecánica | Descripción | Script |
|---|---|---|
| Movimiento y rodado | Relativo a la cámara, salto, rodado con estamina | `ControlJugador` |
| Pistola (click izquierdo) | Bala luminosa que daña lo que sea dañable | `Proyectil`, `IDanable` |
| Daga (click derecho) | Golpe corto frontal, gasta estamina | `ArmasJugador` |
| Fijar objetivo (R) | Toma al enemigo dañable más cercano | `FijarObjetivo` |
| Llave y puerta | La llave está en la cripta y abre la arena | `RecogerLlave`, `PuertaCerrada` |
| Cofres de mejora | Suben vida, estamina o fuerza | `CofreMejora` |
| Interruptor disparable | Un sello que se rompe de un tiro y libera un cofre | `InterruptorDisparable` |
| Pared frágil | Se rompe y deja entrar a la cripta | `Destructible` (tag) |
| Tablillas | Textos sueltos que cuentan la historia | `TablillaHistoria` |
| Checkpoint y caída | Al caer se vuelve al último checkpoint | `ZonaJuego` |
| Portal | Lleva al nivel 2, cura y guarda checkpoint | `Portal` |

## 4. Mapa

### Nivel 1: Ruinas del Umbral
- Vestíbulo de inicio con luces de fuego.
- Cripta secreta: se entra rompiendo una pared agrietada del vestíbulo. Tiene la llave, un cofre de estamina y dos centinelas.
- Jaula de parkour: un sello disparable libera un cofre de fuerza.
- Arena del jefe: puerta cerrada con la llave. Adentro, el caballero corrupto.
- Cofre final: al abrirlo se activa el portal.
- Exploración: terreno de 420 por 420 con colinas y rocas grandes, un sendero de losas con faroles que sale de la puerta sur y seis puestos de avanzada (Barranca, Colina, Mirador, Norte, Olvidado y Ruina). Cada puesto tiene un cofre y los más alejados suman tiradores y tablillas.
- Las ruinas están apoyadas sobre el terreno, no flotando.

### Nivel 2: Isla del Abismo
- Plataforma de llegada, puente, pilares de cobertura, coral luminoso y cuatro pozas de donde sale el jefe.
- Mar abajo con un trigger que devuelve al checkpoint.
- Dos centinelas en el camino.

## 5. Referencias
- Ruinas y fuego: Dark Souls y Hollow Knight.
- Patrones de balas del jefe: Cuphead y Enter the Gungeon.
- Colores: piedra fría, luz cálida de fuego, azul verdoso del coral en el abismo.

## 6. Sistemas

### Iluminación
- Luz direccional en Mixed y el resto en Baked, con sombras suaves.
- Objetos estáticos marcados para el bake, grupos de Light Probes y cinco Reflection Probes.
- La mazmorra tiene 70 antorchas y luces de relleno horneadas para que no quede a oscuras.
- Los ajustes del bake están en `UmbralIluminacion.lighting`. `GestorLuces` no pisa la luz horneada.

### Post-procesado
- Un perfil para las ruinas y otro para el abismo (`PerfilRuinas`, `PerfilAbismo`).

### Animación
- Héroe y caballero armados con huesos y controlados por Animator. Héroe: reposo, correr, saltar, rodar, disparo y tajo. Caballero: reposo, caminar, ataque, embestida y aplastar.
- La pistola levanta el brazo al disparar.

### Partículas
- Llamas, polvo en vestíbulo, arena y cripta, aura del jefe, coral y chispas.

### Cámara
- Cinemachine 3 con `CinemachineOrbitalFollow` y cámaras de secuencia para las intros de los jefes (`TomaCinematica`).

### Audio
- Música ambiental distinta para las ruinas y el abismo, con una capa de tensión durante el combate, y efectos sintetizados por código (`DirectorMusica`, `EfectosJuego`). Hay tres líneas de voz (`Narracion`).

## 7. Inteligencia artificial

### Centinela
Máquina de estados: patrulla, persecución, ataque y retorno, con NavMeshAgent. 30 de vida.

### Tirador
Enemigo a distancia de los puestos (28 de vida): cuando ve al jugador se mueve de costado y le dispara cada tanto.

### Caballero corrupto (`IAJefe`)
- Fase 1: persigue, ataca de cerca y embiste.
- Fase 2 (menos del 50 % de vida): más rápido, tinte rojo y golpe al suelo con marcador de área.

### Kraken Pistolero (`JefePulpo`)
- 650 de vida; se activa cuando el jugador se acerca (rango 26).
- Patrones: ráfaga, abanico y espiral; en furia suma morteros.
- Se sumerge y reaparece en otra poza.
- Al morir aparece la pantalla de victoria.

## 8. Interfaz
- Barras de vida y estamina, barra del jefe activo, avisos en pantalla, pantalla de victoria y de derrota.
- Menú principal (jugar, controles, créditos, salir) y menú de pausa con volumen.

## 9. Desafío final
Vencer al Kraken Pistolero esquivando sus patrones, usando los pilares como cobertura y aprovechando los momentos en que sale de las pozas.

## 10. Estado

Terminado: nivel 1 completo con cripta, llave, puerta, cofres, interruptor, enemigos, puestos, sendero, caballero de dos fases, terreno, iluminación horneada, antorchas, partículas, animaciones, post-procesado, menú, pausa, música, nivel 2 con el Kraken y portal entre niveles.

Pendiente: pulido de balance y una prueba completa de punta a punta antes de grabar el video.
