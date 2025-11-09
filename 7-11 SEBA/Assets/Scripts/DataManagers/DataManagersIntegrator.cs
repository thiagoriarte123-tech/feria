using UnityEngine;

/// <summary>
/// Integrador de todos los gestores de datos
/// Facilita la comunicación entre los diferentes sistemas
/// </summary>
public class DataManagersIntegrator : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool initializeOnStart = true;
    public bool createMissingManagers = true;
    
    [Header("Manager References")]
    public ScoreDataManager scoreManager;
    public ComboDataManager comboManager;
    public SongDataManager songManager;
    public ArtistDataManager artistManager;
    
    [Header("Integration Settings")]
    public bool syncDataBetweenManagers = true;
    public float syncInterval = 1f;
    
    private float lastSyncTime = 0f;
    private static DataManagersIntegrator instance;
    
    public static DataManagersIntegrator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<DataManagersIntegrator>();
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
        if (initializeOnStart)
        {
            InitializeAllManagers();
        }
    }
    
    void Update()
    {
        if (syncDataBetweenManagers && Time.time - lastSyncTime > syncInterval)
        {
            SyncAllData();
            lastSyncTime = Time.time;
        }
    }
    
    /// <summary>
    /// Inicializa todos los gestores de datos
    /// </summary>
    public void InitializeAllManagers()
    {
        Debug.Log("[DataManagersIntegrator] Inicializando todos los gestores...");
        
        // Obtener o crear ScoreDataManager
        scoreManager = ScoreDataManager.Instance;
        if (scoreManager == null && createMissingManagers)
        {
            GameObject scoreObj = new GameObject("ScoreDataManager");
            scoreManager = scoreObj.AddComponent<ScoreDataManager>();
            Debug.Log("✅ ScoreDataManager creado");
        }
        
        // Obtener o crear ComboDataManager
        comboManager = ComboDataManager.Instance;
        if (comboManager == null && createMissingManagers)
        {
            GameObject comboObj = new GameObject("ComboDataManager");
            comboManager = comboObj.AddComponent<ComboDataManager>();
            Debug.Log("✅ ComboDataManager creado");
        }
        
        // Obtener o crear SongDataManager
        songManager = SongDataManager.Instance;
        if (songManager == null && createMissingManagers)
        {
            GameObject songObj = new GameObject("SongDataManager");
            songManager = songObj.AddComponent<SongDataManager>();
            Debug.Log("✅ SongDataManager creado");
        }
        
        // Obtener o crear ArtistDataManager
        artistManager = ArtistDataManager.Instance;
        if (artistManager == null && createMissingManagers)
        {
            GameObject artistObj = new GameObject("ArtistDataManager");
            artistManager = artistObj.AddComponent<ArtistDataManager>();
            Debug.Log("✅ ArtistDataManager creado");
        }
        
        Debug.Log("[DataManagersIntegrator] Todos los gestores inicializados");
    }
    
    /// <summary>
    /// Sincroniza datos entre todos los gestores
    /// </summary>
    void SyncAllData()
    {
        // Esta función puede expandirse para sincronizar datos específicos
        // Por ahora, solo asegura que todos los gestores guarden sus datos
        
        if (scoreManager != null)
        {
            scoreManager.SaveScoreData();
        }
        
        if (comboManager != null)
        {
            comboManager.SaveComboData();
        }
        
        if (songManager != null)
        {
            songManager.SaveSongData();
        }
        
        if (artistManager != null)
        {
            artistManager.SaveArtistData();
        }
    }
    
    /// <summary>
    /// Registra un hit en el gameplay (actualiza score y combo)
    /// </summary>
    public void RegisterHit(int points = 100)
    {
        if (scoreManager != null)
        {
            scoreManager.AddScore(points);
        }
        
        if (comboManager != null)
        {
            comboManager.IncrementCombo();
        }
        
        Debug.Log($"[DataManagersIntegrator] Hit registrado: +{points} puntos");
    }
    
    /// <summary>
    /// Registra un fallo en el gameplay (resetea combo)
    /// </summary>
    public void RegisterMiss()
    {
        if (comboManager != null)
        {
            comboManager.ResetCombo();
        }
        
        Debug.Log("[DataManagersIntegrator] Miss registrado");
    }
    
    /// <summary>
    /// Establece la información de la canción actual
    /// </summary>
    public void SetCurrentSong(string songName, string artist, float duration = 0f)
    {
        if (songManager != null)
        {
            songManager.SetCurrentSong(songName, duration);
        }
        
        if (artistManager != null)
        {
            artistManager.SetCurrentArtist(artist);
        }
        
        Debug.Log($"[DataManagersIntegrator] Canción establecida: {songName} by {artist}");
    }
    
    /// <summary>
    /// Detecta automáticamente la información de la canción
    /// </summary>
    public void AutoDetectSongInfo()
    {
        // Buscar AudioSource activo
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource audio in audioSources)
        {
            if (audio != null && audio.clip != null && audio.isPlaying)
            {
                string songName = audio.clip.name;
                string artist = "Artista Desconocido";
                
                if (artistManager != null)
                {
                    artist = artistManager.DetectArtistFromSongName(songName);
                }
                
                SetCurrentSong(songName, artist, audio.clip.length);
                return;
            }
        }
        
        // Verificar PlayerPrefs como respaldo
        if (PlayerPrefs.HasKey("SelectedSongName"))
        {
            string selectedSong = PlayerPrefs.GetString("SelectedSongName");
            string selectedArtist = PlayerPrefs.GetString("SelectedArtist", "Artista Desconocido");
            
            SetCurrentSong(selectedSong, selectedArtist);
        }
    }
    
    /// <summary>
    /// Resetea todos los datos de la sesión actual
    /// </summary>
    public void ResetAllSessionData()
    {
        if (scoreManager != null)
        {
            scoreManager.ResetCurrentScore();
        }
        
        if (comboManager != null)
        {
            comboManager.ResetSessionCombo();
        }
        
        if (songManager != null)
        {
            songManager.ResetSessionData();
        }
        
        Debug.Log("[DataManagersIntegrator] Todos los datos de sesión reseteados");
    }
    
    /// <summary>
    /// Obtiene un resumen completo de todos los datos
    /// </summary>
    public GameplayDataSummary GetCompleteDataSummary()
    {
        GameplayDataSummary summary = new GameplayDataSummary();
        
        if (scoreManager != null)
        {
            summary.currentScore = scoreManager.GetCurrentScore();
            summary.maxScore = scoreManager.GetMaxScore();
        }
        
        if (comboManager != null)
        {
            summary.currentCombo = comboManager.GetCurrentCombo();
            summary.maxCombo = comboManager.GetMaxCombo();
            summary.totalCombos = comboManager.GetTotalCombos();
        }
        
        if (songManager != null)
        {
            summary.songName = songManager.GetCurrentSongName();
            summary.songDuration = songManager.GetSongDuration();
            summary.currentTime = songManager.GetCurrentTime();
            summary.songProgress = songManager.GetSongProgress();
        }
        
        if (artistManager != null)
        {
            summary.artist = artistManager.GetCurrentArtist();
            summary.artistGenre = artistManager.GetCurrentArtistGenre();
        }
        
        summary.userName = PlayerPrefs.GetString("UserName", "Jugador");
        
        return summary;
    }
    
    /// <summary>
    /// Guarda todos los datos inmediatamente
    /// </summary>
    public void SaveAllData()
    {
        Debug.Log("[DataManagersIntegrator] Guardando todos los datos...");
        
        if (scoreManager != null)
        {
            scoreManager.SaveScoreData();
        }
        
        if (comboManager != null)
        {
            comboManager.SaveComboData();
        }
        
        if (songManager != null)
        {
            songManager.SaveSongData();
        }
        
        if (artistManager != null)
        {
            artistManager.SaveArtistData();
        }
        
        Debug.Log("[DataManagersIntegrator] Todos los datos guardados");
    }
    
    /// <summary>
    /// Limpia todos los datos
    /// </summary>
    public void ClearAllData()
    {
        Debug.Log("[DataManagersIntegrator] Limpiando todos los datos...");
        
        if (scoreManager != null)
        {
            scoreManager.ClearScoreData();
        }
        
        if (comboManager != null)
        {
            comboManager.ClearComboData();
        }
        
        if (songManager != null)
        {
            songManager.ClearSongData();
        }
        
        if (artistManager != null)
        {
            artistManager.ClearArtistData();
        }
        
        Debug.Log("[DataManagersIntegrator] Todos los datos limpiados");
    }
    
    /// <summary>
    /// Muestra el estado de todos los gestores
    /// </summary>
    [ContextMenu("Show All Managers Status")]
    public void ShowAllManagersStatus()
    {
        Debug.Log("=== ESTADO DE TODOS LOS GESTORES ===");
        Debug.Log($"ScoreDataManager: {(scoreManager != null ? "✅ ACTIVO" : "❌ INACTIVO")}");
        Debug.Log($"ComboDataManager: {(comboManager != null ? "✅ ACTIVO" : "❌ INACTIVO")}");
        Debug.Log($"SongDataManager: {(songManager != null ? "✅ ACTIVO" : "❌ INACTIVO")}");
        Debug.Log($"ArtistDataManager: {(artistManager != null ? "✅ ACTIVO" : "❌ INACTIVO")}");
        
        GameplayDataSummary summary = GetCompleteDataSummary();
        Debug.Log("\n=== RESUMEN DE DATOS ===");
        Debug.Log($"Score: {summary.currentScore:N0} (Max: {summary.maxScore:N0})");
        Debug.Log($"Combo: {summary.currentCombo} (Max: {summary.maxCombo})");
        Debug.Log($"Canción: {summary.songName} by {summary.artist}");
        Debug.Log($"Progreso: {summary.songProgress:F1}%");
        Debug.Log($"Usuario: {summary.userName}");
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            SaveAllData();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Al reanudar
        {
            SaveAllData();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) // Al perder foco
        {
            SaveAllData();
        }
    }
}

/// <summary>
/// Estructura para resumir todos los datos del gameplay
/// </summary>
[System.Serializable]
public class GameplayDataSummary
{
    public int currentScore = 0;
    public int maxScore = 0;
    public int currentCombo = 0;
    public int maxCombo = 0;
    public int totalCombos = 0;
    public string songName = "";
    public string artist = "";
    public string artistGenre = "";
    public float songDuration = 0f;
    public float currentTime = 0f;
    public float songProgress = 0f;
    public string userName = "";
}
