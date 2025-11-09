# Sistema de Detección de Dificultades

## Descripción
Este sistema analiza automáticamente los archivos `.chart` de las canciones para detectar qué dificultades están disponibles y desactiva los botones de dificultad correspondientes cuando una canción no tiene esa versión.

## Archivos Implementados

### 1. ChartDifficultyAnalyzer.cs
**Ubicación**: `Assets/Scripts/MainMenu/ChartDifficultyAnalyzer.cs`

**Funcionalidad**:
- Analiza archivos `.chart` para detectar secciones de dificultad
- Mapea las dificultades del juego a las secciones del archivo:
  - **Fácil**: `[EasySingle]` o `[Easy]`
  - **Medio**: `[MediumSingle]` o `[Medium]`
  - **Difícil**: `[HardSingle]`, `[Hard]`, `[ExpertSingle]`, o `[Expert]`

**Métodos principales**:
- `AnalyzeChart(string chartPath)`: Analiza un archivo .chart específico
- `AnalyzeSongFolder(string songFolderPath)`: Analiza una carpeta de canción
- `AnalyzeSongByName(string songName)`: Analiza una canción por nombre

### 2. DifficultyButtonManager.cs (Modificado)
**Ubicación**: `Assets/Scripts/MainMenu/DifficultyButtonManager.cs`

**Nuevas funcionalidades**:
- Detecta automáticamente las dificultades disponibles cuando se selecciona una canción
- Desactiva visualmente los botones de dificultad no disponibles
- Previene la selección de dificultades no disponibles
- Resetea la selección actual si la dificultad no está disponible en la nueva canción

## Cómo Funciona

### 1. Detección Automática
Cuando el usuario selecciona una canción:
1. El sistema analiza el archivo `notes.chart` de la canción
2. Busca secciones que indiquen dificultades disponibles
3. Actualiza el estado de los botones automáticamente

### 2. Estados de los Botones
- **Habilitado y No Seleccionado**: Color blanco grisáceo, clickeable
- **Habilitado y Seleccionado**: Color verde, indica selección actual
- **Deshabilitado**: Color gris, no clickeable, dificultad no disponible

### 3. Mapeo de Dificultades
El sistema mapea las dificultades del juego a las secciones del archivo .chart:

| Botón UI | Secciones .chart Reconocidas |
|----------|------------------------------|
| FACIL    | `[EasySingle]`, `[Easy]` |
| DIFICIL  | `[HardSingle]`, `[Hard]`, `[ExpertSingle]`, `[Expert]` |

**Nota**: `[MediumSingle]` y `[Medium]` se detectan pero no se usan actualmente en la UI.

## Configuración en Unity

### Requisitos
1. El `DifficultyButtonManager` debe estar configurado en la escena MainMenu
2. Las referencias a los botones `facilButton` y `dificilButton` deben estar asignadas
3. Los archivos de canciones deben seguir la estructura estándar:
   ```
   Songs/
   ├── NombreCancion/
   │   ├── notes.chart
   │   ├── song.ogg
   │   └── song.ini (opcional)
   ```

### Configuración Automática
El sistema funciona automáticamente una vez configurado. No requiere configuración adicional.

## Comportamiento del Sistema

### Al Iniciar el Juego
- Ambos botones aparecen habilitados (color blanco grisáceo)
- Ninguna dificultad está seleccionada

### Al Seleccionar una Canción
- El sistema analiza automáticamente el archivo `.chart`
- Los botones se actualizan según las dificultades disponibles:
  - **Disponible**: Botón habilitado (blanco grisáceo)
  - **No disponible**: Botón deshabilitado (gris)

### Al Intentar Seleccionar una Dificultad No Disponible
- El sistema previene la selección
- Se muestra un mensaje de advertencia en la consola
- El botón permanece en su estado actual

### Al Cambiar de Canción
- Si la dificultad actual no está disponible en la nueva canción, se resetea automáticamente
- Los botones se actualizan según las nuevas dificultades disponibles

## Ejemplos de Uso

### Verificar Dificultades Disponibles (Código)
```csharp
// Obtener el DifficultyButtonManager
DifficultyButtonManager diffManager = FindObjectOfType<DifficultyButtonManager>();

// Verificar si una dificultad específica está disponible
bool facilDisponible = diffManager.IsDifficultyAvailable("Facil");
bool dificilDisponible = diffManager.IsDifficultyAvailable("Dificil");

// Obtener información completa de dificultades
var difficulties = diffManager.GetCurrentSongDifficulties();
if (difficulties != null)
{
    Debug.Log($"Fácil: {difficulties.hasFacil}, Difícil: {difficulties.hasDificil}");
}
```

### Actualizar Dificultades Manualmente
```csharp
// Forzar actualización para una canción específica
diffManager.UpdateDifficultiesForSong("NombreCancion");
```

## Solución de Problemas

### Los botones no se desactivan
1. Verificar que el `DifficultyButtonManager` esté en la escena
2. Asegurar que las referencias a los botones estén asignadas
3. Verificar que el archivo `notes.chart` existe y es accesible
4. Revisar la consola para mensajes de error

### Dificultades no detectadas correctamente
1. Verificar el formato del archivo `.chart`
2. Asegurar que las secciones usen los nombres correctos:
   - `[EasySingle]` para fácil
   - `[HardSingle]` o `[ExpertSingle]` para difícil
3. Verificar que no hay caracteres especiales en los nombres de sección

### Rendimiento
- El análisis se realiza solo cuando se selecciona una canción
- Los archivos `.chart` se leen una sola vez por selección
- El sistema está optimizado para no afectar el rendimiento del juego

## Logs del Sistema
El sistema genera logs informativos en la consola:
- `📊 Chart analysis for [archivo]: [dificultades]`
- `🎵 Song [canción] difficulties - Facil: [bool], Dificil: [bool]`
- `⚠️ Cannot select [dificultad] - not available for current song`

Estos logs ayudan a diagnosticar problemas y verificar el funcionamiento correcto del sistema.
