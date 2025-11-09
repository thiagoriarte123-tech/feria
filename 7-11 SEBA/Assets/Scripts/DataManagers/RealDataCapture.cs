using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Capturador de datos reales del gameplay
/// Se conecta con todos los sistemas existentes para obtener información real
/// </summary>
public class RealDataCapture : MonoBehaviour
{
    [Header("Auto Detection")]
    public bool captureOnStart = true;
    public bool continuousCapture = true;
    public float captureInterval = 0.5f;
    
    [Header("Detected Components")]
    public ScoreManager scoreManager;
    public AudioSource mainAudioSource;
    public GameObject gameplayManager;
    
    [Header("Captured Data")]
    public string detectedSongName = "";
    public string detectedArtist = "";
    public int detectedScore = 0;
    public int detectedCombo = 0;
    public int detectedMaxCombo = 0;
    public float songProgress = 0f;
    
    private float lastCaptureTime = 0f;
    private static RealDataCapture instance;
    
    public static RealDataCapture Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<RealDataCapture>();
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
        if (captureOnStart)
        {
            StartCapture();
        }
    }
    
    void Update()
    {
        if (continuousCapture && Time.time - lastCaptureTime > captureInterval)
        {
            CaptureAllData();
            lastCaptureTime = Time.time;
        }
    }
    
    /// <summary>
    /// Inicia la captura de datos
    /// </summary>
    public void StartCapture()
    {
        Debug.Log("[RealDataCapture] Iniciando captura de datos reales...");
        
        FindAllComponents();
        DetectSongInformation();
        CaptureAllData();
        SaveCapturedData();
    }
    
    /// <summary>
    /// Busca todos los componentes necesarios
    /// </summary>
    void FindAllComponents()
    {
        // Buscar ScoreManager
        if (scoreManager == null)
        {
            scoreManager = FindFirstObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                Debug.Log("[RealDataCapture] ✅ ScoreManager encontrado");
            }
        }
        
        // Buscar AudioSource principal
        if (mainAudioSource == null)
        {
            AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource audio in audioSources)
            {
                if (audio != null && audio.clip != null)
                {
                    if (audio.isPlaying || audio.clip.length > 30f) // Canción principal
                    {
                        mainAudioSource = audio;
                        Debug.Log($"[RealDataCapture] ✅ AudioSource principal encontrado: {audio.clip.name}");
                        break;
                    }
                }
            }
        }
        
        // Buscar GameplayManager
        if (gameplayManager == null)
        {
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.ToLower().Contains("gameplay") && obj.name.ToLower().Contains("manager"))
                {
                    gameplayManager = obj;
                    Debug.Log($"[RealDataCapture] ✅ GameplayManager encontrado: {obj.name}");
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Detecta información de la canción
    /// </summary>
    void DetectSongInformation()
    {
        Debug.Log("[RealDataCapture] Detectando información de la canción...");
        
        // Método 1: PlayerPrefs (más confiable)
        if (PlayerPrefs.HasKey("SelectedSongName"))
        {
            detectedSongName = PlayerPrefs.GetString("SelectedSongName", "");
            detectedArtist = PlayerPrefs.GetString("SelectedArtist", "");
            
            if (!string.IsNullOrEmpty(detectedSongName))
            {
                Debug.Log($"[RealDataCapture] 📱 Canción desde PlayerPrefs: {detectedSongName} by {detectedArtist}");
                return;
            }
        }
        
        // Método 2: AudioSource
        if (mainAudioSource != null && mainAudioSource.clip != null)
        {
            detectedSongName = CleanSongName(mainAudioSource.clip.name);
            detectedArtist = DetectArtistFromSongName(detectedSongName);
            
            Debug.Log($"[RealDataCapture] 🎵 Canción desde AudioSource: {detectedSongName} by {detectedArtist}");
            return;
        }
        
        // Método 3: Buscar en nombres de archivos de audio
        AudioClip[] allClips = Resources.FindObjectsOfTypeAll<AudioClip>();
        foreach (AudioClip clip in allClips)
        {
            if (clip != null && clip.length > 30f) // Probablemente una canción
            {
                detectedSongName = CleanSongName(clip.name);
                detectedArtist = DetectArtistFromSongName(detectedSongName);
                
                Debug.Log($"[RealDataCapture] 🎼 Canción desde Resources: {detectedSongName} by {detectedArtist}");
                break;
            }
        }
        
        // Valores por defecto si no se encuentra nada
        if (string.IsNullOrEmpty(detectedSongName))
        {
            detectedSongName = "Canción Actual";
            detectedArtist = "Artista Desconocido";
            Debug.Log("[RealDataCapture] ⚠️ Usando valores por defecto");
        }
    }
    
    /// <summary>
    /// Captura todos los datos actuales
    /// </summary>
    void CaptureAllData()
    {
        // Capturar datos del ScoreManager
        if (scoreManager != null)
        {
            detectedScore = scoreManager.score;
            detectedCombo = scoreManager.currentCombo;
            detectedMaxCombo = scoreManager.maxCombo;
            
            // Guardar datos de notas para PostGameplay
            PlayerPrefs.SetInt("RealTotalNotes", scoreManager.totalNotes);
            PlayerPrefs.SetInt("RealHitNotes", scoreManager.hitNotes);
            PlayerPrefs.SetInt("RealMissedNotes", scoreManager.missedNotes);
            PlayerPrefs.SetInt("RealPerfectHits", scoreManager.perfectHits);
            PlayerPrefs.SetInt("RealGreatHits", scoreManager.greatHits);
            PlayerPrefs.SetInt("RealGoodHits", scoreManager.goodHits);
        }
        
        // Calcular progreso de la canción
        if (mainAudioSource != null && mainAudioSource.clip != null)
        {
            songProgress = (mainAudioSource.time / mainAudioSource.clip.length) * 100f;
        }
        
        // Actualizar gestores de datos
        UpdateDataManagers();
    }
    
    /// <summary>
    /// Actualiza los gestores de datos con la información capturada
    /// </summary>
    void UpdateDataManagers()
    {
        // Actualizar ScoreDataManager
        if (ScoreDataManager.Instance != null)
        {
            ScoreDataManager.Instance.UpdateScore(detectedScore);
        }
        
        // Actualizar ComboDataManager
        if (ComboDataManager.Instance != null)
        {
            ComboDataManager.Instance.SetCombo(detectedCombo);
        }
        
        // Actualizar SongDataManager
        if (SongDataManager.Instance != null && !string.IsNullOrEmpty(detectedSongName))
        {
            float duration = mainAudioSource != null && mainAudioSource.clip != null ? mainAudioSource.clip.length : 0f;
            SongDataManager.Instance.SetCurrentSong(detectedSongName, duration);
        }
        
        // Actualizar ArtistDataManager
        if (ArtistDataManager.Instance != null && !string.IsNullOrEmpty(detectedArtist))
        {
            ArtistDataManager.Instance.SetCurrentArtist(detectedArtist);
        }
    }
    
    /// <summary>
    /// Guarda los datos capturados
    /// </summary>
    void SaveCapturedData()
    {
        PlayerPrefs.SetString("RealSongName", detectedSongName);
        PlayerPrefs.SetString("RealArtist", detectedArtist);
        PlayerPrefs.SetInt("RealScore", detectedScore);
        PlayerPrefs.SetInt("RealCombo", detectedCombo);
        PlayerPrefs.SetInt("RealMaxCombo", detectedMaxCombo);
        PlayerPrefs.SetFloat("RealSongProgress", songProgress);
        PlayerPrefs.Save();
        
        Debug.Log($"[RealDataCapture] 💾 Datos guardados: {detectedSongName} by {detectedArtist} - Score: {detectedScore}");
    }
    
    /// <summary>
    /// Limpia el nombre de la canción
    /// </summary>
    string CleanSongName(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return "Canción Desconocida";
        
        string cleaned = rawName.Replace("_", " ").Replace("-", " ");
        System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        cleaned = textInfo.ToTitleCase(cleaned.ToLower());
        
        return cleaned;
    }
    
    /// <summary>
    /// Detecta artista desde el nombre de la canción
    /// </summary>
    string DetectArtistFromSongName(string songName)
    {
        if (string.IsNullOrEmpty(songName)) return "Artista Desconocido";
        
        string lowerName = songName.ToLower();
        
        // Detectar artistas específicos por nombre de canción
        if (lowerName.Contains("phineas") || lowerName.Contains("ferb") || lowerName.Contains("ardillas"))
            return "Phineas y Ferb";
        else if (lowerName.Contains("baile") || lowerName.Contains("inolvidable"))
            return "Artista Latino";
        else if (lowerName.Contains("rock") || lowerName.Contains("metal"))
            return "Rock Band";
        else if (lowerName.Contains("pop"))
            return "Pop Artist";
        else if (lowerName.Contains("electronic") || lowerName.Contains("edm") || lowerName.Contains("techno"))
            return "Electronic Artist";
        else if (lowerName.Contains("jazz"))
            return "Jazz Artist";
        else if (lowerName.Contains("classical") || lowerName.Contains("clasico"))
            return "Classical Artist";
        else
        {
            // Intentar extraer artista del path de la canción
            string artistFromPath = ExtractArtistFromPath();
            if (!string.IsNullOrEmpty(artistFromPath))
                return artistFromPath;
            
            return "Artista Desconocido";
        }
    }
    
    /// <summary>
    /// Extrae el artista del path de la canción
    /// </summary>
    string ExtractArtistFromPath()
    {
        // Buscar en PlayerPrefs primero
        if (PlayerPrefs.HasKey("SelectedArtist"))
        {
            string selectedArtist = PlayerPrefs.GetString("SelectedArtist", "");
            if (!string.IsNullOrEmpty(selectedArtist) && selectedArtist != "Artista Desconocido")
            {
                return selectedArtist;
            }
        }
        
        // Intentar extraer del path de StreamingAssets
        if (mainAudioSource != null && mainAudioSource.clip != null)
        {
            string clipName = mainAudioSource.clip.name;
            
            // Buscar patrones como "Artista - Canción"
            if (clipName.Contains(" - "))
            {
                string[] parts = clipName.Split(new string[] { " - " }, System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    return CleanArtistName(parts[0]);
                }
            }
            
            // Buscar en el path si está disponible
            string[] pathParts = clipName.Split('/', '\\');
            foreach (string part in pathParts)
            {
                if (!string.IsNullOrEmpty(part) && 
                    !part.ToLower().Contains("songs") && 
                    !part.ToLower().Contains("streaming") &&
                    !part.ToLower().Contains("assets") &&
                    !part.ToLower().Contains(".mp3") &&
                    !part.ToLower().Contains(".wav") &&
                    !part.ToLower().Contains(".ogg"))
                {
                    return CleanArtistName(part);
                }
            }
        }
        
        return "";
    }
    
    /// <summary>
    /// Limpia el nombre del artista
    /// </summary>
    string CleanArtistName(string rawArtist)
    {
        if (string.IsNullOrEmpty(rawArtist)) return "";
        
        string cleaned = rawArtist.Replace("_", " ").Replace("-", " ").Trim();
        System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("es-ES", false).TextInfo;
        cleaned = textInfo.ToTitleCase(cleaned.ToLower());
        
        return cleaned;
    }
    
    /// <summary>
    /// Fuerza la captura inmediata de todos los datos
    /// </summary>
    [ContextMenu("Force Capture All Data")]
    public void ForceCaptureAllData()
    {
        Debug.Log("[RealDataCapture] 🔄 Forzando captura de datos...");
        StartCapture();
        ShowCapturedDataSummary();
    }
    
    /// <summary>
    /// Muestra resumen de datos capturados
    /// </summary>
    [ContextMenu("Show Captured Data Summary")]
    public void ShowCapturedDataSummary()
    {
        Debug.Log("📊 RESUMEN DE DATOS CAPTURADOS:");
        Debug.Log("═══════════════════════════════════");
        Debug.Log($"🎵 Canción: {detectedSongName}");
        Debug.Log($"🎤 Artista: {detectedArtist}");
        Debug.Log($"🏆 Score: {detectedScore:N0}");
        Debug.Log($"🔥 Combo: {detectedCombo}");
        Debug.Log($"⭐ Max Combo: {detectedMaxCombo}");
        Debug.Log($"📈 Progreso: {songProgress:F1}%");
        
        if (scoreManager != null)
        {
            Debug.Log("🎯 DATOS DE NOTAS:");
            Debug.Log($"  Total: {scoreManager.totalNotes}");
            Debug.Log($"  Acertadas: {scoreManager.hitNotes}");
            Debug.Log($"  Perdidas: {scoreManager.missedNotes}");
            Debug.Log($"  Perfect: {scoreManager.perfectHits}");
            Debug.Log($"  Great: {scoreManager.greatHits}");
            Debug.Log($"  Good: {scoreManager.goodHits}");
        }
        
        Debug.Log($"🎮 ScoreManager: {(scoreManager != null ? "✅ CONECTADO" : "❌ NO ENCONTRADO")}");
        Debug.Log($"🔊 AudioSource: {(mainAudioSource != null ? "✅ CONECTADO" : "❌ NO ENCONTRADO")}");
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            SaveCapturedData();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Al reanudar
        {
            SaveCapturedData();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) // Al perder foco
        {
            SaveCapturedData();
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reiniciar captura en nueva escena
        if (scene.name.Contains("Gameplay"))
        {
            Invoke(nameof(StartCapture), 1f); // Esperar un poco para que todo se inicialice
        }
    }
}
