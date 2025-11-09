using UnityEngine;
using TMPro;

/// <summary>
/// Gestor de UI para la escena Gameplay
/// Maneja los TextMeshPro de score y combo independientemente del GameManager
/// </summary>
public class GameplayUIManager : MonoBehaviour
{
    [Header("TextMeshPro References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI artistText;
    
    [Header("Auto Setup")]
    public bool autoFindComponents = true;
    public bool updateInRealTime = true;
    public float updateInterval = 0.1f;
    
    [Header("Data Managers")]
    private ScoreDataManager scoreManager;
    private ComboDataManager comboManager;
    private SongDataManager songManager;
    private ArtistDataManager artistManager;
    
    private float lastUpdateTime = 0f;
    
    void Start()
    {
        if (autoFindComponents)
        {
            FindUIComponents();
        }
        
        InitializeDataManagers();
        UpdateUI();
    }
    
    void Update()
    {
        if (updateInRealTime && Time.time - lastUpdateTime > updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
    }
    
    /// <summary>
    /// Busca automáticamente los componentes UI
    /// </summary>
    void FindUIComponents()
    {
        Debug.Log("[GameplayUIManager] Buscando componentes UI...");
        
        // Buscar todos los TextMeshPro en la escena
        TextMeshProUGUI[] allTMPTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI tmpComponent in allTMPTexts)
        {
            string name = tmpComponent.name.ToLower();
            string text = tmpComponent.text.ToLower();
            
            Debug.Log($"[GameplayUIManager] Analizando TMP: {tmpComponent.name} - Texto: '{tmpComponent.text}'");
            
            // Identificar por nombre o contenido
            if (name.Contains("score") || text.Contains("score") || text.Contains("puntaje"))
            {
                scoreText = tmpComponent;
                Debug.Log($"✅ Score Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("combo") || text.Contains("combo"))
            {
                comboText = tmpComponent;
                Debug.Log($"✅ Combo Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("song") || name.Contains("cancion") || text.Contains("song"))
            {
                songNameText = tmpComponent;
                Debug.Log($"✅ Song Name Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("artist") || name.Contains("artista") || text.Contains("artist"))
            {
                artistText = tmpComponent;
                Debug.Log($"✅ Artist Text encontrado: {tmpComponent.name}");
            }
        }
    }
    
    /// <summary>
    /// Inicializa los gestores de datos
    /// </summary>
    void InitializeDataManagers()
    {
        // Obtener o crear instancias de los gestores
        scoreManager = ScoreDataManager.Instance;
        if (scoreManager == null)
        {
            GameObject scoreManagerObj = new GameObject("ScoreDataManager");
            scoreManager = scoreManagerObj.AddComponent<ScoreDataManager>();
        }
        
        comboManager = ComboDataManager.Instance;
        if (comboManager == null)
        {
            GameObject comboManagerObj = new GameObject("ComboDataManager");
            comboManager = comboManagerObj.AddComponent<ComboDataManager>();
        }
        
        songManager = SongDataManager.Instance;
        if (songManager == null)
        {
            GameObject songManagerObj = new GameObject("SongDataManager");
            songManager = songManagerObj.AddComponent<SongDataManager>();
        }
        
        artistManager = ArtistDataManager.Instance;
        if (artistManager == null)
        {
            GameObject artistManagerObj = new GameObject("ArtistDataManager");
            artistManager = artistManagerObj.AddComponent<ArtistDataManager>();
        }
        
        Debug.Log("[GameplayUIManager] Gestores de datos inicializados");
    }
    
    /// <summary>
    /// Actualiza todos los elementos UI
    /// </summary>
    void UpdateUI()
    {
        UpdateScoreUI();
        UpdateComboUI();
        UpdateSongUI();
        UpdateArtistUI();
    }
    
    /// <summary>
    /// Actualiza la UI del score
    /// </summary>
    void UpdateScoreUI()
    {
        if (scoreText != null && scoreManager != null)
        {
            int currentScore = scoreManager.GetCurrentScore();
            scoreText.text = $"Score: {currentScore:N0}";
        }
    }
    
    /// <summary>
    /// Actualiza la UI del combo
    /// </summary>
    void UpdateComboUI()
    {
        if (comboText != null && comboManager != null)
        {
            int currentCombo = comboManager.GetCurrentCombo();
            comboText.text = $"Combo: {currentCombo}";
            
            // Cambiar color basado en el combo
            if (currentCombo >= 50)
            {
                comboText.color = Color.yellow;
            }
            else if (currentCombo >= 20)
            {
                comboText.color = Color.green;
            }
            else
            {
                comboText.color = Color.white;
            }
        }
    }
    
    /// <summary>
    /// Actualiza la UI de la canción
    /// </summary>
    void UpdateSongUI()
    {
        if (songNameText != null && songManager != null)
        {
            string songName = songManager.GetCurrentSongName();
            if (!string.IsNullOrEmpty(songName))
            {
                songNameText.text = songName;
            }
        }
    }
    
    /// <summary>
    /// Actualiza la UI del artista
    /// </summary>
    void UpdateArtistUI()
    {
        if (artistText != null && artistManager != null)
        {
            string artist = artistManager.GetCurrentArtist();
            if (!string.IsNullOrEmpty(artist))
            {
                artistText.text = $"by {artist}";
            }
        }
    }
    
    /// <summary>
    /// Simula incremento de score (para testing)
    /// </summary>
    [ContextMenu("Simulate Score Hit")]
    public void SimulateScoreHit()
    {
        if (scoreManager != null)
        {
            scoreManager.AddScore(100);
        }
        
        if (comboManager != null)
        {
            comboManager.IncrementCombo();
        }
        
        Debug.Log("[GameplayUIManager] Score hit simulado");
    }
    
    /// <summary>
    /// Simula fallo (resetea combo)
    /// </summary>
    [ContextMenu("Simulate Miss")]
    public void SimulateMiss()
    {
        if (comboManager != null)
        {
            comboManager.ResetCombo();
        }
        
        Debug.Log("[GameplayUIManager] Miss simulado");
    }
    
    /// <summary>
    /// Establece la información de la canción
    /// </summary>
    public void SetSongInfo(string songName, string artist)
    {
        if (songManager != null)
        {
            songManager.SetCurrentSong(songName);
        }
        
        if (artistManager != null)
        {
            artistManager.SetCurrentArtist(artist);
        }
        
        UpdateSongUI();
        UpdateArtistUI();
        
        Debug.Log($"[GameplayUIManager] Información de canción establecida: {songName} by {artist}");
    }
    
    /// <summary>
    /// Detecta automáticamente la información de la canción
    /// </summary>
    public void DetectSongInfo()
    {
        // Buscar AudioSource activo
        AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource audio in audioSources)
        {
            if (audio != null && audio.clip != null && audio.isPlaying)
            {
                string songName = audio.clip.name;
                
                if (songManager != null)
                {
                    songManager.SetCurrentSong(songName, audio.clip.length);
                }
                
                if (artistManager != null)
                {
                    string artist = artistManager.DetectArtistFromSongName(songName);
                    artistManager.SetCurrentArtist(artist);
                }
                
                Debug.Log($"[GameplayUIManager] Canción detectada automáticamente: {songName}");
                break;
            }
        }
        
        // También verificar PlayerPrefs
        if (PlayerPrefs.HasKey("SelectedSongName"))
        {
            string selectedSong = PlayerPrefs.GetString("SelectedSongName");
            string selectedArtist = PlayerPrefs.GetString("SelectedArtist", "Artista Desconocido");
            
            SetSongInfo(selectedSong, selectedArtist);
        }
    }
    
    /// <summary>
    /// Resetea todos los datos de la sesión
    /// </summary>
    public void ResetSessionData()
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
        
        UpdateUI();
        
        Debug.Log("[GameplayUIManager] Datos de sesión reseteados");
    }
    
    /// <summary>
    /// Obtiene un resumen de los datos actuales
    /// </summary>
    public string GetCurrentDataSummary()
    {
        string summary = "=== DATOS ACTUALES DEL GAMEPLAY ===\n";
        
        if (scoreManager != null)
        {
            summary += $"Score: {scoreManager.GetCurrentScore():N0}\n";
        }
        
        if (comboManager != null)
        {
            summary += $"Combo: {comboManager.GetCurrentCombo()}\n";
            summary += $"Max Combo: {comboManager.GetMaxCombo()}\n";
        }
        
        if (songManager != null)
        {
            summary += $"Canción: {songManager.GetCurrentSongName()}\n";
            summary += $"Progreso: {songManager.GetSongProgress():F1}%\n";
        }
        
        if (artistManager != null)
        {
            summary += $"Artista: {artistManager.GetCurrentArtist()}\n";
        }
        
        return summary;
    }
    
    /// <summary>
    /// Muestra el resumen en consola
    /// </summary>
    [ContextMenu("Show Data Summary")]
    public void ShowDataSummary()
    {
        Debug.Log(GetCurrentDataSummary());
    }
    
    void OnEnable()
    {
        // Detectar información de canción al activarse
        Invoke(nameof(DetectSongInfo), 0.5f);
    }
}
