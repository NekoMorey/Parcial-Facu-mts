# Documento de High Concept

**Proyecto:** *El Guardián del Umbral*  
**Instancia Evaluativa:** Parcial 1 — Documento Conceptual y Prototipo Jugable  
**Asignatura:** Desarrollo de Videojuegos / Motores Gráficos  
**Ciclo Lectivo:** 2026  

---

## 1. Datos de la Entrega

* **Título del Videojuego:** *El Guardián del Umbral* (Título de trabajo: *Threshold Guardian*)
* **Nombre del Equipo:** Morey Game Studio
* **Integrantes:** Facundo Morey *(añadir integrantes adicionales si corresponde)*
* **Motor y Plataforma:** Unity 6 (`6000.3.x LTS`), PC (Teclado y Ratón)
* **Perspectiva:** Tercera Persona (Cámara orbital dinámica y sistema de Lock-On con Cinemachine)
* **Género Principal:** Acción / Aventura
* **Subgénero:** *Souls-lite* / Plataformas 3D

---

## 2. Premisa Central y Sinopsis

### Premisa
Un guerrero iniciado despierta en un complejo ancestral suspendido en el vacío, donde la única vía de escape y consagración consiste en atravesar una serie de cámaras rituales: demostrar dominio espacial y agilidad en un tramo de plataformas inestables, y finalmente vencer en combate singular al colosal **Guardián del Umbral** para absorber su poder y reclamar la recompensa de ascensión.

### Tono y Atmósfera
La experiencia se ambienta en una ruina ritual de corte monolítico y místico. El tono combina la tensión metódica del combate de precisión (*timing*, lectura de animaciones y administración de estamina) con el dinamismo del plataformeo en tercera persona, transmitiendo una sensación de desafío superable mediante la destreza mecánica del jugador.

---

## 3. Género, Subgénero y Enfoque de Experiencia

### Clasificación
* **Género Principal:** **Acción** — El foco radica en la destreza motriz, el tiempo de respuesta inmediato frente a los patrones de ataque enemigos y la puntería balística.
* **Subgéneros:**
  * ***Souls-lite*:** Incorpora gestión de recursos inmediatos (estamina), ventanas de invulnerabilidad al esquivar (*i-frames*), cámara enfocada en un único oponente de gran jerarquía y combates de alta letalidad.
  * **Plataformas 3D:** Exige precisión de salto y lectura tridimensional del espacio para sortear desniveles y plataformas flotantes antes de acceder a la arena de combate.

### Tipo de Experiencia Buscada
Se busca una experiencia de **tensión, dominio y gratificación táctica**:
1. **Tensión y Precisión:** El jugador no puede atacar indiscriminadamente; debe vigilar su estamina, calcular cuándo rodar para aprovechar la invulnerabilidad y cuándo castigar con proyectiles.
2. **Dominio del Espacio:** El entorno no es un decorado plano; exige sortear alturas mediante saltos calibrados para llegar al objetivo.
3. **Gratificación por Superación:** Derrotar al guardián recompensa al jugador no solo en lo conceptual, sino en lo sistémico (aumento tangible de atributos, crecimiento físico del personaje y efectos audiovisuales de poder).

---

## 4. Rol y Objetivo del Jugador

* **Rol del Jugador:** Un retador que debe probar su valía frente a las antiguas pruebas del Umbral.
* **Objetivo Principal:** Superar el circuito de pruebas físicas (Lobby y Parkour) y derrotar al Guardián del Umbral en la arena de combate para desbloquear el Cofre de Ascensión.
* **Objetivos Secundarios:**
  * Identificar y destruir estructuras frágiles del entorno con proyectiles.
  * Optimizar el uso de la estamina para no quedar expuesto ante la embestida del jefe.
  * Abrir el cofre sagrado para consolidar la subida de nivel y escalar estadísticas.

---

## 5. Core Gameplay Loop

```
       ┌─────────────────────────────────────────────────────┐
       │             LOBBY / FASE DE PREPARACIÓN             │
       │   Orientación de cámara, reconocimiento de mandos   │
       └──────────────────────────┬──────────────────────────┘
                                  │
                                  ▼
       ┌─────────────────────────────────────────────────────┐
       │             PARKOUR / DESAFÍO ESPACIAL              │
       │      Saltos de precisión, lectura de desniveles      │
       └──────────────────────────┬──────────────────────────┘
                                  │
                                  ▼
       ┌─────────────────────────────────────────────────────┐
       │            ARENA DE COMBATE (BOSS FIGHT)            │
       │  Lectura de patrones (Melee / Carga) ➔ Lock-On      │
       │  Esquive con I-Frames ➔ Disparo de Proyectiles Físicos│
       └──────────────────────────┬──────────────────────────┘
                                  │
                                  ▼
       ┌─────────────────────────────────────────────────────┐
       │               VICTORIA Y ASCENSIÓN                  │
       │      Derrota del Boss ➔ Apertura de Cofre           │
       │      Bonificación de Stats y Crecimiento Físico     │
       └─────────────────────────────────────────────────────┘
```

---

## 6. Mecánicas de Juego y Sistemas

### Mecánica Principal: Combate Balístico Táctico con Esquive de Invulnerabilidad
El combate se basa en el ciclo de **evasión y respuesta calculada**:
* **Lanzamiento de Proyectiles Físicos:** El jugador dispara rocas con parábola balística (`Clic Izquierdo`) impulsadas por masa y fuerza. El daño infligido escala directamente con la estadística de Fuerza (`baseStrength`) del personaje.
* **Esquive con Ventana de Invulnerabilidad (*I-Frames*):** Al pulsar `Shift Izquierdo`, el jugador ejecuta una rodada direccional rápida. Durante los primeros instantes de la maniobra, el personaje se vuelve completamente invulnerable al daño (`isInvulnerable = true`). Esta maniobra tiene un costo fijo de estamina, impidiendo el uso continuo descontrolado.

### Mecánicas Secundarias y de Apoyo
* **Salto Tridimensional (`Espacio`):** Control vertical mediante detector de suelo y gravedad acumulativa, permitiendo sortear vacíos y escalar plataformas de alturas variables.
* **Sistema de Fijado de Objetivo (*Lock-On* con `R`):** Al detectar al jefe en rango, la cámara orbital ajusta automáticamente sus ejes horizontal y de inclinación vertical (*pitch*) hacia el enemigo. Además, permite al jugador alternar entre:
  * **Disparo Libre (Sin fijar):** El proyectil viaja recto en la dirección que apunta el personaje (ideal para impactar muros destructibles).
  * **Disparo Teledirigido (Con fijar):** El personaje encara al jefe y el proyectil se lanza con corrección predictiva hacia su centro de masa.
* **Gestión de Recursos (Salud y Estamina):**
  * La **Salud** disminuye ante los ataques cuerpo a cuerpo o cargas del jefe; al llegar a cero desencadena el estado de *Game Over*.
  * La **Estamina** se consume al esquivar y se regenera automáticamente de forma continua a una tasa calibrada por segundo.
* **Interacción de Entorno (`E`):** Detección de proximidad y activación de elementos del mundo (cofres de recompensa) sujetos a condiciones de estado (jefe derrotado).

---

## 7. Perspectiva, Cámara y Control Espacial

* **Perspectiva:** Tercera persona con el avatar centrado y visible en todo momento.
* **Comportamiento de Cámara:**
  * Implementada con **Unity Cinemachine 3** (`CinemachineCamera`, `CinemachineOrbitalFollow`, `CinemachineInputAxisController` y `CinemachineDeoccluder`).
  * **Cámara Libre:** En fases de exploración y parkour, la cámara orbita libremente alrededor del avatar con amortiguación suave de inercia (`AccelTime: 0.08`), sin recentrado automático forzado para garantizar total comodidad visual.
  * **Cámara de Duelo (*Lock-On*):** Durante el combate, enfoca y enmarca al jefe y al jugador simultáneamente mediante interpolación esférica suave (`Mathf.LerpAngle`), adaptando tanto el ángulo horizontal como la inclinación vertical.
  * **Antiobstrucción (*Deoccluder*):** Detecta colisiones contra muros de la arena y acerca la cámara con suavizado continuo (`SmoothingTime: 0.2`) para evitar recortes o saltos visuales bruscos.

---

## 8. Diseño de Niveles y Configuración del Mundo

El escenario responde a una progresión lineal dividida en tres microambientes funcionales:

```
[ LOBBY (z: -20) ] ────▶ [ PARKOUR VERTICAL (z: 10 a 45) ] ────▶ [ ARENA DEL GUARDIÁN (z: 75) ]
 (Área de inicio y         (5 plataformas escalonadas              (Recinto circular monumental
  orientación segura)       con suelo de seguridad inferior)        con combate y cofre de victoria)
```

1. **Sala 1 — El Lobby (Spawn):**
   * Espacio cerrado, amplio y seguro.
   * Su propósito es permitir al jugador aclimatarse al control de cámara, rotación y movimiento libre antes de afrontar riesgos.
2. **Sala 2 — El Tramo de Parkour:**
   * Sala de transición vertical compuesta por 5 plataformas elevadas de salto escalonado (`Y: 1` hasta `Y: 6.2`).
   * Pone a prueba el cálculo de distancias y el salto con la barra espaciadora.
   * Cuenta con un foso inferior seguro y rampa de recuperación en caso de caída para no frustrar la progresión con muertes arbitrarias de vacío.
3. **Sala 3 — La Arena del Guardián:**
   * Recinto cuadrangular monumental con muros perimetrales altos para confinar el combate.
   * Alberga al **Guardián del Umbral** en su centro y el **Cofre de Recompensa** al fondo.

---

## 9. Inteligencia Artificial: El Guardián del Umbral

Gobernado por `NavMeshAgent` y una máquina de comportamiento por rangos de distancia:
* **Estado de Reposo / Espera:** Mientras el jugador se mantenga fuera de su amplio radio de detección (`detectionRange = 38m`), el jefe permanece estático y su barra de vida en el HUD permanece oculta.
* **Activación de Combate:** Al entrar en rango, el jefe ruge/se activa, persigue al jugador y activa en pantalla la barra de salud con su nombre (*Guardián del Umbral*).
* **Patrón Melee (Corta distancia ≤ 4m):** Detiene su marcha, encara al jugador y ejecuta golpes contundentes con *cooldown*.
* **Patrón de Embestida / Carga (Media/Larga distancia ≥ 8m):** Si el jugador se aleja para disparar a distancia, el jefe telegrafía una embestida acelerada de alta velocidad (`chargeSpeed = 16`), forzando al jugador a utilizar el esquive con *i-frames* en el momento preciso.
* **Muerte y Consecuencia:** Al agotarse su vida, el jefe se desactiva, oculta la barra del HUD y desbloquea el estado `isBossDefeated` en el cofre final.

---

## 10. Progresión y Recompensa

* **Condición de Desbloqueo:** El cofre final es inerte hasta que la IA del jefe confirma su caída.
* **Recompensa de Victoria:**
  * **Aumento de Atributos:** Incremento permanente del nivel del personaje, aumento de la Salud Máxima (`+50 HP`) y Fuerza de Ataque (`+15 Fuerza`).
  * **Retroalimentación Audiovisual:** Instanciación de efectos visuales de partículas de subida de nivel (`levelUpVFX`) y aumento de la escala física del modelo 3D del personaje a **1.25x**, reflejando visualmente el poder adquirido.

---

## 11. Organización y Roles del Equipo

| Rol | Responsabilidades en el Proyecto |
| :--- | :--- |
| **Producción y Gestión** | Definición del alcance del prototipo, planificación de hitos (Parcial 1 y 2) y control de requerimientos académicos. |
| **Programación de Jugabilidad** | Control del personaje (`PlayerController`), físicas balísticas (`Rock`), máquina de estados de estamina/salud (`PlayerStats`) e interfaces (`IInteractable`). |
| **Programación de IA y Sistemas** | Lógica de comportamiento del Boss con NavMesh (`BossAI`), gestión de vida (`BossStats`) y HUD contextual (`HUDManager`). |
| **Diseño de Juego (*Game Design*)** | Calibración de velocidades, costes de estamina, ventanas de invulnerabilidad, curvas de daño y core loop. |
| **Diseño de Niveles (*Level Design*)** | Estructuración espacial de las tres salas (Lobby, Parkour y Arena) y ubicación de obstáculos y puntos de interés. |
| **Cámara y Arte Técnico** | Configuración de Cinemachine, suavizado de colisiones (*Deoccluder*), iluminación y shaders URP. |
| **Control de Calidad (QA)** | Pruebas de colisiones, verificación de balance de combate, detección de bugs de estado y fluidez de control. |

---

## 12. Viabilidad Técnica y Estado del Prototipo

* **Estado de Desarrollo:** El prototipo base ya se encuentra completamente funcional y operativo en **Unity 6**:
  * Locomoción, salto y esquive con invulnerabilidad implementados y calibrados.
  * Disparo físico libre y fijado por Lock-On coordinado con Cinemachine.
  * IA del jefe con NavMesh, detección reactiva, patrones melee y de carga rápida.
  * HUD reactivo que muestra vida, estamina y panel contextual del jefe.
  * Cofre de recompensa con progresión de atributos y escalado visual del personaje.
* **Proyección hacia Parcial 2:**
  * Incorporación de modelos y texturas finales con estética estilizada / fantasía oscura.
  * Animaciones de combate (caminar, correr, rodar, saltar y golpes del jefe con *rig* humanoide).
  * Implementación de efectos de sonido (SFX de pasos, impacto de roca, golpe del jefe y fanfarria de victoria) y música dinámica adaptativa.