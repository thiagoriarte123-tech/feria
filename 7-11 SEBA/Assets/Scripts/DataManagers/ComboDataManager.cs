using UnityEngine;

/// <summary>
/// Gestor independiente para datos de combo
/// No depende del GameManager, es completamente independiente
/// </summary>
public class ComboDataManager : MonoBehaviour
{
    [Header("Combo Configuration")]
    public bool autoSave = true;
    public float saveInterval = 2f;
    public bool captureFromExistingScoreManager = true;
    
    [Header("Current Combo Data")]
    public int currentCombo = 0;
    public int maxCombo = 0;
    public int totalCombos = 0;
    
    [Header("Real-time Capture")]
    public ScoreManager existingScoreManager;
    
    private float lastSaveTime = 0f;
    private static ComboDataManager instance;
    
    public static ComboDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ComboDataManager>();
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
        LoadComboData();
        FindExistingScoreManager();
    }
    
    void Update()
    {
        if (captureFromExistingScoreManager)
        {
            CaptureRealComboData();
        }
        
        if (autoSave && Time.time - lastSaveTime > saveInterval)
        {
            SaveComboData();
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
                Debug.Log("[ComboDataManager] ScoreManager existente encontrado y conectado");
            }
            else
            {
                Debug.Log("[ComboDataManager] No se encontró ScoreManager existente");
            }
        }
    }
    
    /// <summary>
    /// Captura datos reales del ScoreManager existente
    /// </summary>
    void CaptureRealComboData()
    {
        if (existingScoreManager != null)
        {
            // Capturar combo actual
            int realCombo = existingScoreManager.currentCombo;
            if (realCombo != currentCombo)
            {
                currentCombo = realCombo;
                Debug.Log($"[ComboDataManager] Combo capturado del ScoreManager: {currentCombo}");
            }
            
            // Capturar max combo
            int realMaxCombo = existingScoreManager.maxCombo;
            if (realMaxCombo > maxCombo)
            {
                maxCombo = realMaxCombo;
                Debug.Log($"[ComboDataManager] Max Combo actualizado: {maxCombo}");
            }
        }
    }
    
    /// <summary>
    /// Incrementa el combo actual
    /// </summary>
    public void IncrementCombo()
    {
        currentCombo++;
        totalCombos++;
        
        if (currentCombo > maxCombo)
        {
            maxCombo = currentCombo;
        }
        
        Debug.Log($"[ComboDataManager] Combo incrementado: {currentCombo}");
    }
    
    /// <summary>
    /// Resetea el combo actual (por fallo)
    /// </summary>
    public void ResetCombo()
    {
        currentCombo = 0;
        Debug.Log("[ComboDataManager] Combo reseteado por fallo");
    }
    
    /// <summary>
    /// Establece el combo actual directamente
    /// </summary>
    public void SetCombo(int newCombo)
    {
        currentCombo = newCombo;
        
        if (currentCombo > maxCombo)
        {
            maxCombo = currentCombo;
        }
        
        Debug.Log($"[ComboDataManager] Combo establecido: {currentCombo}");
    }
    
    /// <summary>
    /// Obtiene el combo actual
    /// </summary>
    public int GetCurrentCombo()
    {
        return currentCombo;
    }
    
    /// <summary>
    /// Obtiene el combo máximo
    /// </summary>
    public int GetMaxCombo()
    {
        return maxCombo;
    }
    
    /// <summary>
    /// Obtiene el total de combos realizados
    /// </summary>
    public int GetTotalCombos()
    {
        return totalCombos;
    }
    
    /// <summary>
    /// Guarda los datos de combo
    /// </summary>
    public void SaveComboData()
    {
        PlayerPrefs.SetInt("CurrentCombo", currentCombo);
        PlayerPrefs.SetInt("MaxCombo", maxCombo);
        PlayerPrefs.SetInt("TotalCombos", totalCombos);
        PlayerPrefs.Save();
        
        Debug.Log($"[ComboDataManager] Datos guardados - Current: {currentCombo}, Max: {maxCombo}, Total: {totalCombos}");
    }
    
    /// <summary>
    /// Carga los datos de combo
    /// </summary>
    public void LoadComboData()
    {
        currentCombo = PlayerPrefs.GetInt("CurrentCombo", 0);
        maxCombo = PlayerPrefs.GetInt("MaxCombo", 0);
        totalCombos = PlayerPrefs.GetInt("TotalCombos", 0);
        
        Debug.Log($"[ComboDataManager] Datos cargados - Current: {currentCombo}, Max: {maxCombo}, Total: {totalCombos}");
    }
    
    /// <summary>
    /// Limpia todos los datos de combo
    /// </summary>
    public void ClearComboData()
    {
        currentCombo = 0;
        maxCombo = 0;
        totalCombos = 0;
        
        PlayerPrefs.DeleteKey("CurrentCombo");
        PlayerPrefs.DeleteKey("MaxCombo");
        PlayerPrefs.DeleteKey("TotalCombos");
        PlayerPrefs.Save();
        
        Debug.Log("[ComboDataManager] Datos de combo limpiados");
    }
    
    /// <summary>
    /// Resetea solo el combo de la sesión actual
    /// </summary>
    public void ResetSessionCombo()
    {
        currentCombo = 0;
        Debug.Log("[ComboDataManager] Combo de sesión reseteado");
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            SaveComboData();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Al reanudar
        {
            SaveComboData();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) // Al perder foco
        {
            SaveComboData();
        }
    }
}
