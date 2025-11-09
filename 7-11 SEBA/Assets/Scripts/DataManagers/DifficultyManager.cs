using UnityEngine;

/// <summary>
/// Gestor de dificultad que convierte "Experto" a "Difícil"
/// </summary>
public class GameplayDifficultyManager : MonoBehaviour
{
    [Header("Difficulty Settings")]
    public string currentDifficulty = "Difícil";
    public bool autoDetectDifficulty = true;
    
    [Header("Difficulty Mapping")]
    public bool convertExpertoToDificil = true;
    
    private static GameplayDifficultyManager instance;
    
    public static GameplayDifficultyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameplayDifficultyManager>();
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (autoDetectDifficulty)
        {
            DetectCurrentDifficulty();
        }
    }
    
    /// <summary>
    /// Detecta la dificultad actual del juego
    /// </summary>
    void DetectCurrentDifficulty()
    {
        // Método 1: Buscar en PlayerPrefs
        string detectedDifficulty = "";
        
        if (PlayerPrefs.HasKey("SelectedDifficulty"))
        {
            detectedDifficulty = PlayerPrefs.GetString("SelectedDifficulty", "");
        }
        else if (PlayerPrefs.HasKey("Difficulty"))
        {
            detectedDifficulty = PlayerPrefs.GetString("Difficulty", "");
        }
        else if (PlayerPrefs.HasKey("GameDifficulty"))
        {
            detectedDifficulty = PlayerPrefs.GetString("GameDifficulty", "");
        }
        
        // Método 2: Buscar en objetos de la escena
        if (string.IsNullOrEmpty(detectedDifficulty))
        {
            detectedDifficulty = DetectDifficultyFromScene();
        }
        
        // Aplicar conversión
        currentDifficulty = ConvertDifficulty(detectedDifficulty);
        
        // Guardar la dificultad convertida
        PlayerPrefs.SetString("DisplayDifficulty", currentDifficulty);
        PlayerPrefs.Save();
        
        Debug.Log($"[GameplayDifficultyManager] Dificultad detectada: '{detectedDifficulty}' → Convertida a: '{currentDifficulty}'");
    }
    
    /// <summary>
    /// Detecta dificultad desde objetos en la escena
    /// </summary>
    string DetectDifficultyFromScene()
    {
        // Buscar en GameObjects que puedan contener información de dificultad
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            string objName = obj.name.ToLower();
            
            if (objName.Contains("experto") || objName.Contains("expert"))
            {
                return "Experto";
            }
            else if (objName.Contains("dificil") || objName.Contains("hard"))
            {
                return "Difícil";
            }
            else if (objName.Contains("normal") || objName.Contains("medium"))
            {
                return "Normal";
            }
            else if (objName.Contains("facil") || objName.Contains("easy"))
            {
                return "Fácil";
            }
        }
        
        // Buscar en componentes de texto
        TMPro.TextMeshProUGUI[] textComponents = FindObjectsByType<TMPro.TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (var text in textComponents)
        {
            if (text != null && !string.IsNullOrEmpty(text.text))
            {
                string textContent = text.text.ToLower();
                
                if (textContent.Contains("experto") || textContent.Contains("expert"))
                {
                    return "Experto";
                }
                else if (textContent.Contains("difícil") || textContent.Contains("dificil") || textContent.Contains("hard"))
                {
                    return "Difícil";
                }
                else if (textContent.Contains("normal") || textContent.Contains("medium"))
                {
                    return "Normal";
                }
                else if (textContent.Contains("fácil") || textContent.Contains("facil") || textContent.Contains("easy"))
                {
                    return "Fácil";
                }
            }
        }
        
        return "Difícil"; // Por defecto
    }
    
    /// <summary>
    /// Convierte la dificultad según las reglas especificadas
    /// </summary>
    string ConvertDifficulty(string originalDifficulty)
    {
        if (string.IsNullOrEmpty(originalDifficulty))
        {
            return "Difícil";
        }
        
        string lower = originalDifficulty.ToLower();
        
        // SOLO convertir "Experto" a "Difícil"
        if (convertExpertoToDificil && (lower.Contains("experto") || lower.Contains("expert")))
        {
            return "Difícil";
        }
        
        // Mantener las otras dificultades como están
        if (lower.Contains("dificil") || lower.Contains("difícil") || lower.Contains("hard"))
        {
            return "Difícil";
        }
        
        if (lower.Contains("normal") || lower.Contains("medium"))
        {
            return "Normal";
        }
        
        if (lower.Contains("facil") || lower.Contains("fácil") || lower.Contains("easy"))
        {
            return "Fácil";
        }
        
        // Si no coincide con nada conocido, mantener original o "Difícil" por defecto
        return string.IsNullOrEmpty(originalDifficulty) ? "Difícil" : originalDifficulty;
    }
    
    /// <summary>
    /// Establece la dificultad manualmente
    /// </summary>
    public void SetDifficulty(string difficulty)
    {
        currentDifficulty = ConvertDifficulty(difficulty);
        PlayerPrefs.SetString("DisplayDifficulty", currentDifficulty);
        PlayerPrefs.Save();
        
        Debug.Log($"[GameplayDifficultyManager] Dificultad establecida: {currentDifficulty}");
    }
    
    /// <summary>
    /// Obtiene la dificultad actual (ya convertida)
    /// </summary>
    public string GetCurrentDifficulty()
    {
        return currentDifficulty;
    }
    
    /// <summary>
    /// Obtiene la dificultad para mostrar en UI
    /// </summary>
    public string GetDisplayDifficulty()
    {
        return PlayerPrefs.GetString("DisplayDifficulty", currentDifficulty);
    }
    
    /// <summary>
    /// Fuerza la detección de dificultad
    /// </summary>
    [ContextMenu("Force Detect Difficulty")]
    public void ForceDetectDifficulty()
    {
        Debug.Log("[GameplayDifficultyManager] 🔄 Forzando detección de dificultad...");
        DetectCurrentDifficulty();
        ShowDifficultyStatus();
    }
    
    /// <summary>
    /// Muestra el estado actual de la dificultad
    /// </summary>
    [ContextMenu("Show Difficulty Status")]
    public void ShowDifficultyStatus()
    {
        Debug.Log("🎯 ESTADO DE DIFICULTAD:");
        Debug.Log("═══════════════════════");
        Debug.Log($"Dificultad Actual: {currentDifficulty}");
        Debug.Log($"Dificultad Display: {GetDisplayDifficulty()}");
        Debug.Log($"PlayerPrefs 'SelectedDifficulty': {PlayerPrefs.GetString("SelectedDifficulty", "NO ENCONTRADO")}");
        Debug.Log($"PlayerPrefs 'Difficulty': {PlayerPrefs.GetString("Difficulty", "NO ENCONTRADO")}");
        Debug.Log($"Conversión Experto→Difícil: {(convertExpertoToDificil ? "✅ ACTIVA" : "❌ INACTIVA")}");
        Debug.Log("ℹ️ Otras dificultades (Fácil, Normal) se mantienen sin cambios");
    }
    
    /// <summary>
    /// Convierte todas las dificultades a "Difícil" (para testing)
    /// </summary>
    [ContextMenu("Convert All to Dificil")]
    public void ConvertAllToDificil()
    {
        currentDifficulty = "Difícil";
        PlayerPrefs.SetString("DisplayDifficulty", "Difícil");
        PlayerPrefs.SetString("SelectedDifficulty", "Difícil");
        PlayerPrefs.SetString("Difficulty", "Difícil");
        PlayerPrefs.Save();
        
        Debug.Log("[GameplayDifficultyManager] ✅ Todas las dificultades convertidas a 'Difícil'");
    }
}
