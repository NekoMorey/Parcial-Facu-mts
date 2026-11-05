# Ecos del Umbral: Vertical Slice de Combate 3D

Experiencia vertical slice en **Unity 6**: el jugador cruza un recinto antiguo, supera una ruta de plataformas y enfrenta al Guardián del Umbral en una arena. El combate en tercera persona combina fijado de objetivo, proyectiles físicos, esquive con invulnerabilidad, gestión de salud/estamina y una recompensa final.

---

## 📋 Índice
1. [Descripción General](#-descripción-general)
2. [High Concept actualizado](#-high-concept-actualizado)
3. [Requisitos y Tecnologías](#-requisitos-y-tecnologías)
4. [Guía de Controles](#-guía-de-controles)
5. [Mecánicas Implementadas](#-mecánicas-implementadas)
6. [Arquitectura de Scripts](#-arquitectura-de-scripts)
7. [Estructura del Proyecto](#-estructura-del-proyecto)
8. [Instrucciones de Instalación y Ejecución](#-instrucciones-de-instalación-y-ejecución)
9. [Créditos](#créditos)

---

## 🎮 Descripción General

La experiencia integra sistemas de movimiento, cámara, combate, progresión y presentación en un bucle de juego completo:
* **Entrada y Movimiento:** Control en tercera persona relativo a la cámara con capacidad de desplazarse en círculos (*strafing*) al fijar objetivo.
* **Cámara Inteligente:** Sistema de seguimiento y enfoque con Cinemachine.
* **Combate Dinámico:** Lanzamiento de proyectiles físicos con fuerza de impulso y daño que escala según los atributos del jugador.
* **Inteligencia Artificial:** Comportamiento del jefe gobernado por `NavMeshAgent` con patrones de ataque cuerpo a cuerpo y embestidas telegrafiadas.
* **Interfaz de Usuario (HUD):** Monitoreo en tiempo real de salud y estamina del jugador, barra de vida del boss, y menús de Pausa y Fin de Partida.
* **Interacción y Progresión:** Cofre final con mejoras permanentes, feedback audiovisual y pantalla de victoria.
* **Recorrido:** Ruta de lobby, plataformas y arena con checkpoint al final del parkour y recuperación al caer.
* **Integración audiovisual:** Música ambiental y efectos sonoros sintetizados en ejecución, partículas de impacto y recompensa, animaciones de reposo/ataque/carga del boss y luces de acento en el recorrido.

---

## 💡 High Concept actualizado

**Título provisional:** Ecos del Umbral. **Género:** acción y aventura 3D en tercera persona.

El jugador atraviesa un recinto antiguo, supera una ruta vertical de plataformas y derrota al Guardián del Umbral en su arena. El combate combina movimiento libre, fijado de objetivo, proyectiles físicos y esquives que consumen estamina. Al vencer, el jugador abre el cofre final, recibe una mejora permanente y alcanza la pantalla de victoria.

**Bucle principal:** explorar la ruta → administrar salud y estamina → leer y evitar los ataques del guardián → contraatacar → reclamar la recompensa. La cámara orbital y el HUD contextual apoyan la lectura del combate.

## 🛠 Requisitos y Tecnologías

* **Motor:** Unity 6 (versión recomendada: `6000.3.27f2` o compatible).
* **Render Pipeline:** Universal Render Pipeline (URP 17.3.0).
* **Paquetes Principales:**
  * **Unity Cinemachine (v3.1.7):** Manejo de cámara en tercera persona y seguimiento orbital.
  * **AI Navigation / NavMesh (v2.0.16):** Cálculo de rutas y locomoción de la IA del Boss.
  * **TextMeshPro (v2.0.0):** Renderizado de fuentes e interfaces de usuario nítidas.
  * **Unity ProBuilder (v6.1.4):** Prototipado y modelado de geometría de niveles.

---

## 🕹 Guía de Controles

| Acción | Tecla / Input | Descripción |
| :--- | :--- | :--- |
| **Moverse** | `W` `A` `S` `D` | Movimiento relativo a la vista de la cámara. |
| **Rotar Cámara** | `Mouse` | Control orbital libre de la perspectiva de juego. |
| **Fijar / Lock-On** | `R` | Fija la cámara en el Boss más cercano dentro del rango. |
| **Saltar** | `Espacio` | Salto vertical cuando el personaje está en el suelo. |
| **Esquivar (Roll)** | `Shift Izq` | Realiza una maniobra evasiva con invulnerabilidad temporal (*i-frames*). |
| **Disparar Roca** | `Clic Izquierdo` | Lanza un proyectil balístico hacia la dirección de mira o el objetivo fijado. |
| **Interactuar** | `E` | Abre cofres y activa objetos interactuables al estar cerca. |
| **Pausar / Reanudar** | `Escape (ESC)` | Detiene el tiempo del juego y despliega el menú de opciones. |

---

## ⚙ Mecánicas Implementadas

### 1. Sistema de Movimiento y Cámara Lock-On
* **Cámara Orbital:** Se apoya en Cinemachine para un movimiento fluido y reactivo alrededor del personaje.
* **Fijación de Objetivo (`TargetLock.cs`):** Mediante detección esférica (`Physics.OverlapSphere`), localiza al jefe etiquetado como `Boss`. Al fijarlo, ajusta el ángulo horizontal de la cámara automáticamente y permite que el jugador camine encarando al objetivo en todo momento.

### 2. Esquive y Ventanas de Invulnerabilidad (I-Frames)
* El jugador puede realizar esquives direccionales ejecutados a través de corrutinas (`DodgeRoutine`).
* Consume estamina y otorga la bandera `isInvulnerable`, evitando que los ataques del jefe resten salud durante la animación de rodar.

### 3. Combate por Proyectiles Físicos (`Rock.cs`)
* Los proyectiles son instanciados con componentes `Rigidbody` y disparados mediante un impulso de física vectorial.
* El daño del proyectil se escala dinámicamente consultando la estadística `baseStrength` de `PlayerStats`.
* Detección de colisiones contra el boss (`BossStats`) y objetos con la etiqueta `Destructible`.

### 4. Inteligencia Artificial del Jefe (`BossAI.cs`)
* Controlado por un `NavMeshAgent` que busca activamente la posición del jugador.
* **Rango de Ataque Melee:** Al estar a corta distancia, frena su desplazamiento, encara al jugador y ejecuta un ataque básico con enfriamiento (*cooldown*).
* **Ataque de Carga / Embestida:** Al encontrarse a media o larga distancia, inicia una corrutina de embestida acelerada con daño mayor si logra impactar al jugador.

### 5. Progresión y Recompensas (`ChestBase.cs`)
* El cofre final valida si el jefe ha sido derrotado.
* Al ser abierto por el jugador (tecla `E`):
  * Incrementa el nivel del personaje.
  * Otorga salud máxima y fuerza base adicional.
  * Aumenta la escala del modelo del personaje (`scaleMultiplier = 1.25x`) e instancia efectos visuales de subida de nivel (`levelUpVFX`).

### 6. HUD y Gestión de Estado
* Barras de estado en tiempo real (Salud y Estamina) para el jugador.
* Barra de jefe contextual que se activa únicamente mientras el boss permanezca activo en la escena.
* Control de flujo de juego con pausa (`Time.timeScale = 0f`) y pantalla de Game Over con opción de reinicio de escena.

---

## 📂 Arquitectura de Scripts

Todos los scripts se ubican en `Assets/Scripts/`:

```
Assets/Scripts/
├── PlayerController.cs   # Movimiento, salto, esquive, disparo y pausa.
├── GameFeedback.cs       # Música/ambiente y feedback de sonidos y partículas en ejecución.
├── GameplayTrigger.cs    # Checkpoint, recuperación al caer y eventos de ruta.
├── PlayerStats.cs        # Gestión de vida, estamina, niveles, daño recibido y Game Over.
├── TargetLock.cs         # Lógica de detección y fijación de objetivo para Cinemachine.
├── BossAI.cs             # Comportamiento del Boss con NavMesh (ataque melee y carga rápida).
├── BossStats.cs          # Puntos de salud, recepción de daño y lógica de derrota del Boss.
├── Rock.cs               # Comportamiento físico del proyectil y resolución de colisiones.
├── HUDManager.cs         # HUD, aviso de interacción y cierre de victoria.
├── ChestBase.cs          # Interacción y recompensa compartidas por los cofres.
├── Chest.cs              # Componente de cofre usado por la escena.
├── RewardChest.cs        # Variante compatible con la misma lógica.
└── IInteractable.cs      # Contrato/Interfaz estándar para elementos interactuables (Interact()).
```

---

## 🗂 Estructura del Proyecto

```
Parcial-Facu-mts/
├── Assets/
│   ├── Animations/       # Controlador y clips de animación del boss.
│   ├── Materials/        # Materiales del recorrido y la arena.
│   ├── Textures/         # Textura procedural de piedra basáltica.
│   ├── Prefabs/          # Prefabs del juego (proyectil Rock, etc.)
│   ├── Scenes/           # Escena principal de juego (scene.unity)
│   ├── Scripts/          # Código fuente en C#
│   └── TextMesh Pro/     # Recursos y fuentes para la UI
├── Packages/             # Definición de dependencias de Unity (manifest.json)
├── ProjectSettings/      # Configuración del proyecto, capas, tags y motor
└── README.md             # Documentación del proyecto
```

---

## 🚀 Instrucciones de Instalación y Ejecución

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/NekoMorey/Parcial-Facu-mts.git
   ```
2. **Abrir con Unity Hub:**
   * Haz clic en **Add project from disk**.
   * Selecciona la carpeta del proyecto clonado.
   * Asegúrate de abrirlo con **Unity 6** (`6000.3.x`).
3. **Cargar la escena:**
   * En la ventana *Project*, dirígete a `Assets/Scenes/`.
   * Abre la escena **`scene.unity`**.
4. **Ejecutar:**
   * Presiona el botón de **Play** en la parte superior del editor de Unity para probar el juego.

---

## Créditos

- Desarrollador: Facundo Morey ([@NekoMorey](https://github.com/NekoMorey)).
