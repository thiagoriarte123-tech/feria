# Sistema de Gestión de Datos Independiente

Este sistema proporciona gestores de datos completamente independientes del GameManager para manejar score, combo, información de canciones y artistas.

## 📁 Estructura de Archivos Creados

### Gestores de Datos (DataManagers/)
- `ScoreDataManager.cs` - Gestiona únicamente datos de puntuación
- `ComboDataManager.cs` - Gestiona únicamente datos de combo
- `SongDataManager.cs` - Gestiona únicamente datos de canciones
- `ArtistDataManager.cs` - Gestiona únicamente datos de artistas
- `DataManagersIntegrator.cs` - Integra todos los gestores

### Gestores de UI (UI/)
- `CrearUsuarioUIManager.cs` - Maneja TextMeshPro en escena CrearUsuario
- `GameplayUIManager.cs` - Maneja TextMeshPro de score/combo en Gameplay
- `PostGameplayUIManager.cs` - Muestra todos los datos en PostGameplay

## 🚀 Instrucciones de Implementación

### 1. Escena CrearUsuario
1. Crear un GameObject vacío llamado "CrearUsuarioUIManager"
2. Agregar el script `CrearUsuarioUIManager.cs`
3. El script detectará automáticamente los TextMeshPro de nombres
4. Si no los detecta automáticamente, asignar manualmente en el inspector

### 2. Escena Gameplay
1. Crear un GameObject vacío llamado "GameplayUIManager"
2. Agregar el script `GameplayUIManager.cs`
3. Crear un GameObject vacío llamado "DataManagersIntegrator"
4. Agregar el script `DataManagersIntegrator.cs`
5. Los scripts detectarán automáticamente:
   - TextMeshPro de Score (busca "ScoreText" o texto que contenga "Score")
   - TextMeshPro de Combo (busca "ComboText" o texto que contenga "Combo")

### 3. Escena PostGameplay
1. Crear un GameObject vacío llamado "PostGameplayUIManager"
2. Agregar el script `PostGameplayUIManager.cs`
3. El script detectará automáticamente todos los TextMeshPro y mostrará:
   - Score final
   - Combo máximo
   - Nombre de la canción
   - Artista
   - Nombre del usuario
   - Porcentaje de completion
   - Accuracy calculada
   - Rank basado en el score

## 🔧 Configuración Automática

### Detección Automática de TextMeshPro
Los scripts buscan automáticamente TextMeshPro basándose en:
- **Nombres de GameObject**: "ScoreText", "ComboText", "SongName", etc.
- **Contenido del texto**: "Score", "Combo", "15000", "82.3%", etc.
- **Palabras clave**: "score", "combo", "song", "artist", "user", etc.

### Detección de Información de Canciones
El sistema detecta automáticamente:
1. **AudioSource activo** - Obtiene nombre del clip
2. **PlayerPrefs** - "SelectedSongName", "SelectedArtist"
3. **Datos por defecto** - Si no encuentra información

## 📊 Uso de los Gestores

### En Scripts de Gameplay
```csharp
// Registrar un hit
DataManagersIntegrator.Instance.RegisterHit(100);

// Registrar un fallo
DataManagersIntegrator.Instance.RegisterMiss();

// Establecer canción manualmente
DataManagersIntegrator.Instance.SetCurrentSong("Mi Canción", "Mi Artista");

// Detectar canción automáticamente
DataManagersIntegrator.Instance.AutoDetectSongInfo();
```

### Acceso Individual a Gestores
```csharp
// Score
int currentScore = ScoreDataManager.Instance.GetCurrentScore();
ScoreDataManager.Instance.AddScore(100);

// Combo
int currentCombo = ComboDataManager.Instance.GetCurrentCombo();
ComboDataManager.Instance.IncrementCombo();

// Canción
string songName = SongDataManager.Instance.GetCurrentSongName();
float progress = SongDataManager.Instance.GetSongProgress();

// Artista
string artist = ArtistDataManager.Instance.GetCurrentArtist();
```

## 🎯 Identificación de TextMeshPro Existentes

### Escena CrearUsuario
Basándome en el análisis, busca TextMeshPro que contengan:
- InputField para nombre de usuario
- Texto de bienvenida
- Preview del nombre

### Escena Gameplay
Identificados en el análisis:
- **ScoreText** (fileID: 1060757511) - Muestra "Puntaje: X"
- **ComboText** (fileID: 1926369531) - Muestra "Combo: X"

### Escena PostGameplay
El script buscará automáticamente TextMeshPro con contenido como:
- "15000" o "15,000" (para score)
- "150" (para combo)
- "Test Song" (para nombre de canción)
- "Test Artist" (para artista)
- "82.3%" (para completion)

## 🔄 Persistencia de Datos

### PlayerPrefs Utilizados
- `CurrentScore` / `MaxScore`
- `CurrentCombo` / `MaxCombo` / `TotalCombos`
- `CurrentSongName` / `LastSongName`
- `CurrentArtist` / `LastArtist`
- `UserName` / `LastUserName`

### Guardado Automático
- Cada 2-3 segundos durante el gameplay
- Al cambiar de escena
- Al pausar/reanudar la aplicación
- Al perder/recuperar el foco

## 🛠️ Funciones de Testing

### Context Menu (Click derecho en el script)
- "Show Data Summary" - Muestra resumen de datos
- "Simulate Score Hit" - Simula un acierto
- "Simulate Miss" - Simula un fallo
- "Show All Managers Status" - Estado de todos los gestores

### Métodos Públicos para Testing
```csharp
// En GameplayUIManager
gameplayUIManager.SimulateScoreHit();
gameplayUIManager.SimulateMiss();

// En PostGameplayUIManager
postGameplayUIManager.SetGameplayData(15000, 150, "Mi Canción", "Mi Artista");
```

## ⚠️ Notas Importantes

1. **Independencia Total**: Los scripts NO dependen del GameManager existente
2. **Detección Automática**: Los scripts buscan automáticamente los TextMeshPro
3. **Respaldo de Datos**: Usa PlayerPrefs como respaldo si los gestores no están disponibles
4. **Singleton Pattern**: Cada gestor usa Singleton para persistir entre escenas
5. **DontDestroyOnLoad**: Los gestores persisten automáticamente entre escenas

## 🔍 Solución de Problemas

### Si no se detectan TextMeshPro automáticamente:
1. Verificar nombres de GameObjects
2. Asignar manualmente en el inspector
3. Revisar la consola para logs de detección

### Si no se muestran datos en PostGameplay:
1. Verificar que los gestores estén activos en Gameplay
2. Comprobar PlayerPrefs como respaldo
3. Usar métodos de testing para establecer datos manualmente

### Para debugging:
1. Usar "Show Data Summary" en Context Menu
2. Revisar logs en consola (prefijo con nombre del script)
3. Verificar que los gestores tengan DontDestroyOnLoad activo
