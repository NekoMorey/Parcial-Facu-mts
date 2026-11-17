# Informe de testing — Ecos del Umbral (Segundo Parcial)

## 1. Pruebas hechas

| # | Prueba | Resultado |
|---|---|---|
| 1 | Bake de iluminación con luces Mixed y Baked, probes y asset de lighting | Bien, la escena se guarda con los lightmaps |
| 2 | Armar la escena de cero desde los menús `Tools > Umbral` | Bien, sin errores de compilación |
| 3 | Lock-on sobre los centinelas de la cripta | Fallaba, ya corregido |
| 4 | Caballero flotando sobre el suelo | Se corrigió en dos pasos |
| 5 | Pared agrietada de la cripta | Se rompe con disparos o con la daga y deja pasar |
| 6 | Llave y puerta de la arena | La llave abre la puerta |
| 7 | Regeneración de estamina | Se recupera después de una pausa corta |
| 8 | Ruinas flotando sobre el terreno | Corregido, ahora apoyan en el suelo |
| 9 | Cofres y tiradores de los puestos fuera de lugar | Corregido, se acomodan sobre el terreno |
| 10 | Se perdió la animación de movimiento | Los clips se guardaban vacíos, se regeneraron |
| 11 | La pistola no levantaba el brazo al disparar | Corregido |
| 12 | El jefe no embestía | El NavMesh estaba viejo, se reconstruyó (falta confirmar jugando) |
| 13 | Mazmorra demasiado oscura | Se agregaron antorchas y luces de relleno y se volvió a hornear |
| 14 | Renombrado de scripts y assets a español | Compila y el juego arranca sin errores en consola |
| 15 | Partida completa: nivel 1, portal, nivel 2 y victoria | Falta probarla de punta a punta |

## 2. Problemas encontrados

1. No se podía fijar la mira en los enemigos de la cripta. El lock-on solo aceptaba el tag `Boss`. Ahora acepta cualquier objeto `IDanable` y apunta al centro de su collider.
2. El caballero flotaba. Primero se probó con el `baseOffset` del NavMeshAgent y no alcanzó. Después se agregó un raycast hacia abajo que lo apoya en el suelo.
3. La iluminación estaba en tiempo real y sin guardar. Fue la devolución principal del profesor. Se pasó todo a Baked y Mixed, se agregaron probes y se guardó la escena.
4. La estamina no se recuperaba bien. Se agregó una regeneración con retardo.
5. Las zonas de caída funcionaban una sola vez. Ahora se pueden reutilizar.
6. Al hacer el terreno, las ruinas quedaban flotando. Se levanta el terreno hasta el nivel del piso de las ruinas y se vuelven a apoyar puestos y cofres.

## 3. Bugs conocidos
- El caballero puede flotar un poco en terreno irregular.
- Algunas balas del Kraken atraviesan geometría fina.
- Los lightmaps pesan mucho en el repositorio (conviene Git LFS).
- Falta probar la partida completa de punta a punta.

## 4. Opiniones de jugadores
Completar con lo que digan al menos tres personas que prueben el build.

| Jugador | Qué le gustó | Qué le costó | Sugerencia |
|---|---|---|---|
| 1 | | | |
| 2 | | | |
| 3 | | | |

## 5. Mejoras a futuro
- Más enemigos a distancia en el nivel 1.
- Coleccionables con contador.
- Frasco de curación.
- Música distinta en la fase 2 de los jefes.
