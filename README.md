Un fanmade de Chainsaw Man enfocado en la acción pura. En este hack and slash encarnás a Denji, enfrentándote al Demonio Zombie, el primer gran enemigo que aparece en el manga.
El juego pone el foco en el combate cuerpo a cuerpo, con golpes rápidos, una sensación agresiva y una habilidad especial que desata todo el poder de la motosierra. Cada enfrentamiento busca transmitir la brutalidad, el caos y la desesperación que definen el mundo de Chainsaw Man.

## Controles:
W, A, S, D – Movimiento
Click izquierdo – Golpear
Espacio – Activar habilidad especial
T – Alternar cámara libre / fijada al Demonio Zombie

---
---

## Sistemas

### BattleServiceLocator
Implementa un localizador de servicios para el contexto de batalla.  
Tutorial (inglés, nivel intermedio): [YouTube](https://www.youtube.com/watch?v=D4r5EyYQvwY)

### BattleBootstrapper
Inicializador temprano de la escena de batalla (`DefaultExecutionOrder -200`).  

**Qué hace**
- Registra servicios en el `BattleServiceLocator`:
  - Pool de partículas (`IParticleSystemPool`)
  - Pool de sonidos (`ISoundPoolHandler`)
  - Timer, jugador, resultados, puntaje
  - Checker de fin de batalla (`BattleEndCondictionMetChecker`)
- Configura parámetros globales del usuario:
  - Sensibilidad (`SharedFloat sensitivity`)
  - Volumen de audio (`GameAudioType` con valores del `AudioMixer`)

**Por qué es útil**
- Centraliza inicialización  
- Evita dependencias rígidas  
- Facilita depuración y mantenimiento  
- Aplica configuraciones del jugador automáticamente  

### BattleEndCondictionMetChecker
Comprueba si la batalla terminó.  
Escucha número de enemigos activos y salud del boss.  
Cuando ambos llegan a cero, dispara un evento de fin de batalla.  

### Sistema de pool de partículas
`ParticleSystemPool` optimiza rendimiento reutilizando instancias.  
- Pool independiente por catálogo (`ParticleEffectCatalog`)  
- Precarga prefabs al iniciar  
- Retorno automático al pool (`ParticleReturnToPool`)  
- Usa `ObjectPool<T>` de Unity  
- Método `Play`: instancia y reproduce en la posición indicada  

---

## Enemigos

### Boss
**AllyLauncher**
- Detecta enemigos cercanos  
- Los agarra y eleva  
- Los mantiene suspendidos  
- Los lanza con fuerza  

**MovementPattern**
- Flota con oscilación vertical  
- Se mueve entre rutas  
- Gira hacia el jugador  

**BossEnemiesGenerator**
- Organiza pools de enemigos  
- Controla aparición según vida del boss  
- Genera enemigos en bucle con posiciones aleatorias y atributos configurables  

### Miniboss
**MiniBossAttackSystem**  
Máquina de estados para ataques de miniboss. Ejecuta distintos tipos de ataques (golpes o agarres) dependiendo de las partes del cuerpo activas.  

### Zombies
Máquina de estados para comportamiento básico.  

---

## ScriptableObjects

### Shared Values
- Contenedor genérico (`T`) con valor compartido  
- Campo serializado configurable desde el editor  
- Propiedad pública `Value`  
- Evento `OnValueChanged` al cambiar  
- Ideal para valores globales: vida, puntuación, configuraciones  

### EventChannelSO
- Canal de evento en `ScriptableObject`  
- Emitir con `RaiseEvent()`  
- Escuchar con `OnEventRaised`  
- Desacopla emisores y receptores  
- Conexión fácil desde el inspector  
- Útil para notificaciones globales  



