# Sistema de Records con Archivos de Texto

Este sistema maneja 3 archivos de texto para guardar records de canciones:

## 📁 Archivos Creados

### 1. `current_player.txt`
- **Propósito**: Guarda el nombre del jugador actual
- **Contenido**: Solo el nombre del jugador
- **Ejemplo**: `Player1`

### 2. `current_game.txt`
- **Propósito**: Guarda el score y combo de la canción actual (se sobreescribe)
- **Contenido**: Información de la partida actual
- **Ejemplo**:
```
Song: Baile Inolvidable
Score: 95000
Combo: 150
Updated: 2024-11-04 14:30:25
```

### 3. `all_records.txt`
- **Propósito**: Historial completo de todos los records por canción
- **Contenido**: Lista de todos los records guardados
- **Ejemplo**:
```
# Records History
# Format: [SONG_NAME] Player: PLAYER_NAME | Score: SCORE | Combo: COMBO | Date: DATE

[Baile Inolvidable] Player: Player1 | Score: 95000 | Combo: 150 | Date: 2024-11-04 14:30:25
[Baile Inolvidable] Player: Player2 | Score: 87000 | Combo: 120 | Date: 2024-11-04 14:25:10
[momito] Player: Player1 | Score: 78000 | Combo: 100 | Date: 2024-11-04 14:20:05
```

## 🚀 Cómo Usar

### 1. Configuración Inicial

1. **Crear GameObject para GameRecordManager**:
   - En tu escena principal, crea un GameObject vacío
   - Nómbralo "RecordManager"
   - Agrega el script `GameRecordManager.cs`

### 2. Configurar ScoreView (ScrollView)

**Para ScrollView con Prefabs (Recomendado):**
1. En tu ScrollView, agrega el script `ScoreViewRecordDisplay.cs` al Content panel
2. Crea un prefab para los records:
   - Crea un GameObject con Image (background)
   - Agrega Text components para: RankText, PlayerText, ScoreText, ComboText
   - Guárdalo como prefab
   - Opcionalmente agrega el script `RecordEntryPrefabSetup.cs` al prefab
3. En el Inspector de `ScoreViewRecordDisplay`:
   - Marca `Use Prefab System` = true
   - Asigna el `Record Prefab`
   - Asigna el `Content Parent` (Content del ScrollView)
   - Asigna `Song Name Text` y `No Records Text`

**Para UI Fija (Alternativa):**
1. Agrega el script `ScoreViewRecordDisplay.cs` a tu ScoreView
2. Marca `Use Prefab System` = false
3. Asigna manualmente los 3 Text components en `Record Texts`

### 3. Integración con tu Juego

#### En el Script de tu Juego Principal:

```csharp
// Al iniciar una canción
GameRecordManager.Instance.OnSongStart("Nombre de la Canción");

// Durante el juego (actualizar score y combo)
GameRecordManager.Instance.OnScoreUpdate(currentScore, currentCombo);

// Al terminar la canción
GameRecordManager.Instance.OnSongEnd(finalScore, maxCombo);

// Cambiar nombre del jugador
GameRecordManager.Instance.SetPlayerName("NuevoNombre");
```

#### En el MainMenu (para mostrar records):

```csharp
// Cuando se selecciona una canción
ScoreViewRecordDisplay scoreView = FindObjectOfType<ScoreViewRecordDisplay>();
scoreView.SetSong("Nombre de la Canción");

// O usando el evento de selección de canción
scoreView.OnSongSelectedByName("Nombre de la Canción");
```

### 4. Configuración Detallada de ScrollView

#### Estructura del Prefab de Record:
```
RecordPrefab (GameObject)
├── Background (Image) - Fondo del record
├── RankText (Text) - "1.", "2.", "3."
├── PlayerText (Text) - Nombre del jugador
├── ScoreText (Text) - Puntaje
└── ComboText (Text) - Combo máximo
```

#### Configuración del ScrollView:

En el Inspector del `ScoreViewRecordDisplay`:

1. **Song Name Text**: Text component que muestra el nombre de la canción
2. **Record Texts**: Array de 3 Text components para mostrar los top 3 records
3. **Record Panels**: Array de 3 GameObjects que contienen cada record (opcional)
4. **No Records Text**: Text component que se muestra cuando no hay records

## 🎮 Funcionalidades

### GameRecordManager

- ✅ **Singleton**: Una sola instancia en toda la aplicación
- ✅ **Auto-save**: Guarda automáticamente al terminar canciones
- ✅ **Eventos**: Notifica cuando se guardan records
- ✅ **Persistencia**: Los archivos se guardan en `Application.persistentDataPath`

### ScoreViewRecordDisplay

- ✅ **Top 3**: Muestra los mejores 3 records por canción
- ✅ **Colores por ranking**: Oro, Plata, Bronce
- ✅ **Auto-refresh**: Se actualiza automáticamente
- ✅ **Formato personalizable**: Puedes cambiar cómo se muestran los records

### RecordFileManager

- ✅ **Manejo de archivos**: Crea y maneja los 3 archivos automáticamente
- ✅ **Parsing robusto**: Lee y escribe datos de forma segura
- ✅ **Ordenamiento**: Ordena records por score y combo
- ✅ **Búsqueda**: Encuentra records por canción específica

## 📍 Ubicación de Archivos

Los archivos se guardan en:
- **Windows**: `%USERPROFILE%\AppData\LocalLow\<CompanyName>\<ProductName>\Records\`
- **Mac**: `~/Library/Application Support/<CompanyName>/<ProductName>/Records/`
- **Linux**: `~/.config/unity3d/<CompanyName>/<ProductName>/Records/`

## 🛠️ Métodos Útiles

### Para Debugging:

```csharp
// Agregar records de prueba
GameRecordManager.Instance.AddTestRecords();

// Limpiar todos los records
GameRecordManager.Instance.ClearAllRecords();

// Imprimir records en consola
GameRecordManager.Instance.PrintAllRecords();

// Ver top 3 de una canción específica
var top3 = GameRecordManager.Instance.GetTop3Records("Nombre Canción");
```

### Para Obtener Datos:

```csharp
// Obtener nombre del jugador actual
string playerName = GameRecordManager.Instance.GetCurrentPlayerName();

// Obtener todos los records de una canción
var records = GameRecordManager.Instance.GetAllRecordsForSong("Nombre Canción");

// Obtener datos del juego actual
var currentGame = GameRecordManager.Instance.GetCurrentGameData();
```

## 🎯 Ejemplo de Integración Completa

```csharp
public class MiJuego : MonoBehaviour
{
    void Start()
    {
        // Configurar nombre del jugador
        GameRecordManager.Instance.SetPlayerName("MiNombre");
    }
    
    void IniciarCancion(string nombreCancion)
    {
        GameRecordManager.Instance.OnSongStart(nombreCancion);
    }
    
    void Update()
    {
        // Actualizar score durante el juego
        GameRecordManager.Instance.OnScoreUpdate(scoreActual, comboActual);
    }
    
    void TerminarCancion()
    {
        GameRecordManager.Instance.OnSongEnd(scoreFinal, comboMaximo);
        
        // Mostrar records en UI
        ScoreViewRecordDisplay scoreView = FindObjectOfType<ScoreViewRecordDisplay>();
        scoreView.RefreshRecords();
    }
}
```

## ⚠️ Notas Importantes

1. **GameRecordManager debe estar en la escena** antes de usar cualquier funcionalidad
2. **Los archivos se crean automáticamente** la primera vez que se usa el sistema
3. **El sistema es thread-safe** y maneja errores de archivo automáticamente
4. **Los records se ordenan por score** (mayor a menor), luego por combo
5. **El formato de fecha** es `yyyy-MM-dd HH:mm:ss`

## 🐛 Troubleshooting

### Si no se guardan los records:
- Verifica que `GameRecordManager.Instance` no sea null
- Revisa la consola por errores de permisos de archivo
- Asegúrate de que `autoSaveRecords` esté en true

### Si no se muestran en ScoreView:
- Verifica las referencias de UI en el Inspector
- Asegúrate de llamar `SetSong()` con el nombre correcto
- Revisa que el nombre de la canción coincida exactamente

### Si los archivos no se encuentran:
- Los archivos están en `Application.persistentDataPath/Records/`
- Usa `GameRecordManager.Instance.GetRecordsFolderPath()` para ver la ruta exacta
