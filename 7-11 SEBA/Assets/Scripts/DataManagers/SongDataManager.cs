using UnityEngine;

/// <summary>
/// Gestor independiente para datos de canción
/// No depende del GameManager, es completamente independiente
/// </summary>
public class SongDataManager : MonoBehaviour
{
    [Header("Song Configuration")]
    public bool autoSave = true;
    public float saveInterval = 3f;
    
    [Header("Current Song Data")]
    public string currentSongName = "";
    public string currentSongPath = "";
    public float songDuration = 0f;
    public float currentTime = 0f;
    
    [Header("Song History")]
    public string lastPlayedSong = "";
    public int songsPlayed = 0;
    
    private float lastSaveTime = 0f;
    private AudioSource audioSource;
    private static SongDataManager instance;
    
    public static SongDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SongDataManager>();
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
        LoadSongData();
        DetectAudioSource();
    }
    
    void Update()
    {
        UpdateCurrentTime();
        
        if (autoSave && Time.time - lastSaveTime > saveInterval)
        {
            SaveSongData();
            lastSaveTime = Time.time;
        }
    }
    
    /// <summary>
    /// Detecta el AudioSource automáticamente
    /// </summary>
    void DetectAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
            
            if (audioSource != null && audioSource.clip != null)
            {
                SetCurrentSong(audioSource.clip.name, audioSource.clip.length);
                Debug.Log($"[SongDataManager] AudioSource detectado: {currentSongName}");
            }
        }
    }
    
    /// <summary>
    /// Actualiza el tiempo actual de la canción
    /// </summary>
    void UpdateCurrentTime()
    {
        if (audioSource != null && audioSource.clip != null && audioSource.isPlaying)
        {
            currentTime = audioSource.time;
        }
    }
    
    /// <summary>
    /// Establece la canción actual
    /// </summary>
    public void SetCurrentSong(string songName, float duration = 0f)
    {
        currentSongName = CleanSongName(songName);
        songDuration = duration;
        currentTime = 0f;
        
        // Actualizar historial
        if (!string.IsNullOrEmpty(lastPlayedSong) && lastPlayedSong != currentSongName)
        {
            songsPlayed++;
        }
        
        lastPlayedSong = currentSongName;
        
        Debug.Log($"[SongDataManager] Canción establecida: {currentSongName} ({duration}s)");
    }
    
    /// <summary>
    /// Establece la canción actual con ruta
    /// </summary>
    public void SetCurrentSong(string songName, string songPath, float duration = 0f)
    {
        SetCurrentSong(songName, duration);
        currentSongPath = songPath;
        
        Debug.Log($"[SongDataManager] Canción con ruta establecida: {currentSongName} - {songPath}");
    }
    
    /// <summary>
    /// Obtiene el nombre de la canción actual
    /// </summary>
    public string GetCurrentSongName()
    {
        return currentSongName;
    }
    
    /// <summary>
    /// Obtiene la duración de la canción actual
    /// </summary>
    public float GetSongDuration()
    {
        return songDuration;
    }
    
    /// <summary>
    /// Obtiene el tiempo actual de reproducción
    /// </summary>
    public float GetCurrentTime()
    {
        return currentTime;
    }
    
    /// <summary>
    /// Obtiene el progreso de la canción (0-100%)
    /// </summary>
    public float GetSongProgress()
    {
        if (songDuration <= 0f) return 0f;
        return (currentTime / songDuration) * 100f;
    }
    
    /// <summary>
    /// Obtiene la última canción reproducida
    /// </summary>
    public string GetLastPlayedSong()
    {
        return lastPlayedSong;
    }
    
    /// <summary>
    /// Obtiene el número total de canciones reproducidas
    /// </summary>
    public int GetSongsPlayed()
    {
        return songsPlayed;
    }
    
    /// <summary>
    /// Limpia el nombre de la canción
    /// </summary>
    string CleanSongName(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return "Canción Desconocida";
        
        // Reemplazar caracteres comunes
        string cleaned = rawName.Replace("_", " ").Replace("-", " ");
        
        // Capitalizar
        System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        cleaned = textInfo.ToTitleCase(cleaned.ToLower());
        
        return cleaned;
    }
    
    /// <summary>
    /// Guarda los datos de canción
    /// </summary>
    public void SaveSongData()
    {
        PlayerPrefs.SetString("CurrentSongName", currentSongName);
        PlayerPrefs.SetString("CurrentSongPath", currentSongPath);
        PlayerPrefs.SetFloat("SongDuration", songDuration);
        PlayerPrefs.SetFloat("CurrentTime", currentTime);
        PlayerPrefs.SetString("LastPlayedSong", lastPlayedSong);
        PlayerPrefs.SetInt("SongsPlayed", songsPlayed);
        PlayerPrefs.Save();
        
        Debug.Log($"[SongDataManager] Datos guardados - {currentSongName} ({GetSongProgress():F1}%)");
    }
    
    /// <summary>
    /// Carga los datos de canción
    /// </summary>
    public void LoadSongData()
    {
        currentSongName = PlayerPrefs.GetString("CurrentSongName", "");
        currentSongPath = PlayerPrefs.GetString("CurrentSongPath", "");
        songDuration = PlayerPrefs.GetFloat("SongDuration", 0f);
        currentTime = PlayerPrefs.GetFloat("CurrentTime", 0f);
        lastPlayedSong = PlayerPrefs.GetString("LastPlayedSong", "");
        songsPlayed = PlayerPrefs.GetInt("SongsPlayed", 0);
        
        Debug.Log($"[SongDataManager] Datos cargados - {currentSongName} (Canciones jugadas: {songsPlayed})");
    }
    
    /// <summary>
    /// Limpia todos los datos de canción
    /// </summary>
    public void ClearSongData()
    {
        currentSongName = "";
        currentSongPath = "";
        songDuration = 0f;
        currentTime = 0f;
        lastPlayedSong = "";
        songsPlayed = 0;
        
        PlayerPrefs.DeleteKey("CurrentSongName");
        PlayerPrefs.DeleteKey("CurrentSongPath");
        PlayerPrefs.DeleteKey("SongDuration");
        PlayerPrefs.DeleteKey("CurrentTime");
        PlayerPrefs.DeleteKey("LastPlayedSong");
        PlayerPrefs.DeleteKey("SongsPlayed");
        PlayerPrefs.Save();
        
        Debug.Log("[SongDataManager] Datos de canción limpiados");
    }
    
    /// <summary>
    /// Resetea solo los datos de la sesión actual
    /// </summary>
    public void ResetSessionData()
    {
        currentTime = 0f;
        Debug.Log("[SongDataManager] Datos de sesión reseteados");
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            SaveSongData();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Al reanudar
        {
            SaveSongData();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) // Al perder foco
        {
            SaveSongData();
        }
    }
}
