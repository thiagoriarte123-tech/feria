using UnityEngine;

/// <summary>
/// Gestor independiente para datos de score
/// Se conecta con el ScoreManager existente para capturar datos reales
/// </summary>
public class ScoreDataManager : MonoBehaviour
{
    [Header("Score Configuration")]
    public bool autoSave = true;
    public float saveInterval = 2f;
    public bool captureFromExistingScoreManager = true;
    
    [Header("Current Score Data")]
    public int currentScore = 0;
    public int maxScore = 0;
    
    [Header("Real-time Capture")]
    public ScoreManager existingScoreManager;
    
    private float lastSaveTime = 0f;
    private static ScoreDataManager instance;
    
    public static ScoreDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ScoreDataManager>();
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
        LoadScoreData();
        FindExistingScoreManager();
    }
    
    void Update()
    {
        if (captureFromExistingScoreManager)
        {
            CaptureRealScoreData();
        }
        
        if (autoSave && Time.time - lastSaveTime > saveInterval)
        {
            SaveScoreData();
            lastSaveTime = Time.time;
        }
    }
    
    /// <summary>
    /// Busca el ScoreManager existente en la escena
    /// </summary>
    void FindExistingScoreManager()
    {
        if (existingScoreManager == null)
        {
            existingScoreManager = FindFirstObjectByType<ScoreManager>();
            
            if (existingScoreManager != null)
            {
                Debug.Log("[ScoreDataManager] ScoreManager existente encontrado y conectado");
            }
            else
            {
                Debug.Log("[ScoreDataManager] No se encontró ScoreManager existente");
            }
        }
    }
    
    /// <summary>
    /// Captura datos reales del ScoreManager existente
    /// </summary>
    void CaptureRealScoreData()
    {
        if (existingScoreManager != null)
        {
            // Capturar score actual
            int realScore = existingScoreManager.score;
            if (realScore != currentScore)
            {
                currentScore = realScore;
                
                if (currentScore > maxScore)
                {
                    maxScore = currentScore;
                }
                
                Debug.Log($"[ScoreDataManager] Score capturado del ScoreManager: {currentScore}");
            }
        }
    }
    
    /// <summary>
    /// Actualiza el score actual
    /// </summary>
    public void UpdateScore(int newScore)
    {
        currentScore = newScore;
        
        if (currentScore > maxScore)
        {
            maxScore = currentScore;
        }
        
        Debug.Log($"[ScoreDataManager] Score actualizado: {currentScore}");
    }
    
    /// <summary>
    /// Añade puntos al score actual
    /// </summary>
    public void AddScore(int points)
    {
        UpdateScore(currentScore + points);
    }
    
    /// <summary>
    /// Obtiene el score actual
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    /// <summary>
    /// Obtiene el score máximo
    /// </summary>
    public int GetMaxScore()
    {
        return maxScore;
    }
    
    /// <summary>
    /// Resetea el score actual
    /// </summary>
    public void ResetCurrentScore()
    {
        currentScore = 0;
        Debug.Log("[ScoreDataManager] Score reseteado");
    }
    
    /// <summary>
    /// Guarda los datos de score
    /// </summary>
    public void SaveScoreData()
    {
        PlayerPrefs.SetInt("CurrentScore", currentScore);
        PlayerPrefs.SetInt("MaxScore", maxScore);
        PlayerPrefs.Save();
        
        Debug.Log($"[ScoreDataManager] Datos guardados - Current: {currentScore}, Max: {maxScore}");
    }
    
    /// <summary>
    /// Carga los datos de score
    /// </summary>
    public void LoadScoreData()
    {
        currentScore = PlayerPrefs.GetInt("CurrentScore", 0);
        maxScore = PlayerPrefs.GetInt("MaxScore", 0);
        
        Debug.Log($"[ScoreDataManager] Datos cargados - Current: {currentScore}, Max: {maxScore}");
    }
    
    /// <summary>
    /// Limpia todos los datos de score
    /// </summary>
    public void ClearScoreData()
    {
        currentScore = 0;
        maxScore = 0;
        
        PlayerPrefs.DeleteKey("CurrentScore");
        PlayerPrefs.DeleteKey("MaxScore");
        PlayerPrefs.Save();
        
        Debug.Log("[ScoreDataManager] Datos de score limpiados");
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            SaveScoreData();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Al reanudar
        {
            SaveScoreData();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) // Al perder foco
        {
            SaveScoreData();
        }
    }
}
