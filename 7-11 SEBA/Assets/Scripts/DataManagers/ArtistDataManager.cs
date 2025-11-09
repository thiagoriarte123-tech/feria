using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestor independiente para datos de artista
/// No depende del GameManager, es completamente independiente
/// </summary>
public class ArtistDataManager : MonoBehaviour
{
    [Header("Artist Configuration")]
    public bool autoSave = true;
    public float saveInterval = 3f;
    
    [Header("Current Artist Data")]
    public string currentArtist = "";
    public string currentArtistGenre = "";
    
    [Header("Artist History")]
    public string lastPlayedArtist = "";
    public int differentArtistsPlayed = 0;
    
    private float lastSaveTime = 0f;
    private List<string> playedArtists = new List<string>();
    private static ArtistDataManager instance;
    
    public static ArtistDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ArtistDataManager>();
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
        LoadArtistData();
    }
    
    void Update()
    {
        if (autoSave && Time.time - lastSaveTime > saveInterval)
        {
            SaveArtistData();
            lastSaveTime = Time.time;
        }
    }
    
    /// <summary>
    /// Establece el artista actual
    /// </summary>
    public void SetCurrentArtist(string artistName)
    {
        currentArtist = CleanArtistName(artistName);
        currentArtistGenre = DetectGenreFromArtist(currentArtist);
        
        // Actualizar historial
        if (!string.IsNullOrEmpty(currentArtist) && !playedArtists.Contains(currentArtist))
        {
            playedArtists.Add(currentArtist);
            differentArtistsPlayed = playedArtists.Count;
        }
        
        lastPlayedArtist = currentArtist;
        
        Debug.Log($"[ArtistDataManager] Artista establecido: {currentArtist} ({currentArtistGenre})");
    }
    
    /// <summary>
    /// Establece el artista actual con género específico
    /// </summary>
    public void SetCurrentArtist(string artistName, string genre)
    {
        SetCurrentArtist(artistName);
        currentArtistGenre = genre;
        
        Debug.Log($"[ArtistDataManager] Artista con género establecido: {currentArtist} - {genre}");
    }
    
    /// <summary>
    /// Obtiene el artista actual
    /// </summary>
    public string GetCurrentArtist()
    {
        return currentArtist;
    }
    
    /// <summary>
    /// Obtiene el género del artista actual
    /// </summary>
    public string GetCurrentArtistGenre()
    {
        return currentArtistGenre;
    }
    
    /// <summary>
    /// Obtiene el último artista reproducido
    /// </summary>
    public string GetLastPlayedArtist()
    {
        return lastPlayedArtist;
    }
    
    /// <summary>
    /// Obtiene el número de artistas diferentes reproducidos
    /// </summary>
    public int GetDifferentArtistsPlayed()
    {
        return differentArtistsPlayed;
    }
    
    /// <summary>
    /// Obtiene la lista de artistas reproducidos
    /// </summary>
    public List<string> GetPlayedArtists()
    {
        return new List<string>(playedArtists);
    }
    
    /// <summary>
    /// Verifica si un artista ya fue reproducido
    /// </summary>
    public bool HasPlayedArtist(string artistName)
    {
        return playedArtists.Contains(CleanArtistName(artistName));
    }
    
    /// <summary>
    /// Limpia el nombre del artista
    /// </summary>
    string CleanArtistName(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return "Artista Desconocido";
        
        // Reemplazar caracteres comunes
        string cleaned = rawName.Replace("_", " ").Replace("-", " ");
        
        // Capitalizar
        System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        cleaned = textInfo.ToTitleCase(cleaned.ToLower());
        
        return cleaned;
    }
    
    /// <summary>
    /// Detecta el género basado en el nombre del artista
    /// </summary>
    string DetectGenreFromArtist(string artistName)
    {
        if (string.IsNullOrEmpty(artistName)) return "Desconocido";
        
        string lowerName = artistName.ToLower();
        
        if (lowerName.Contains("latino") || lowerName.Contains("baile") || lowerName.Contains("salsa"))
            return "Latino";
        else if (lowerName.Contains("rock") || lowerName.Contains("metal"))
            return "Rock";
        else if (lowerName.Contains("pop") || lowerName.Contains("mainstream"))
            return "Pop";
        else if (lowerName.Contains("electronic") || lowerName.Contains("edm") || lowerName.Contains("techno"))
            return "Electronic";
        else if (lowerName.Contains("jazz") || lowerName.Contains("blues"))
            return "Jazz";
        else if (lowerName.Contains("classical") || lowerName.Contains("orchestra"))
            return "Classical";
        else if (lowerName.Contains("phineas") || lowerName.Contains("ferb") || lowerName.Contains("cartoon"))
            return "Soundtrack";
        else
            return "Variado";
    }
    
    /// <summary>
    /// Detecta artista desde el nombre de la canción
    /// </summary>
    public string DetectArtistFromSongName(string songName)
    {
        if (string.IsNullOrEmpty(songName)) return "Artista Desconocido";
        
        string lowerName = songName.ToLower();
        
        if (lowerName.Contains("baile") || lowerName.Contains("inolvidable"))
            return "Artista Latino";
        else if (lowerName.Contains("phineas") || lowerName.Contains("ferb"))
            return "Phineas y Ferb";
        else if (lowerName.Contains("rock"))
            return "Rock Band";
        else if (lowerName.Contains("pop"))
            return "Pop Artist";
        else if (lowerName.Contains("electronic") || lowerName.Contains("edm"))
            return "Electronic Artist";
        else
            return "Artista Desconocido";
    }
    
    /// <summary>
    /// Guarda los datos de artista
    /// </summary>
    public void SaveArtistData()
    {
        PlayerPrefs.SetString("CurrentArtist", currentArtist);
        PlayerPrefs.SetString("CurrentArtistGenre", currentArtistGenre);
        PlayerPrefs.SetString("LastPlayedArtist", lastPlayedArtist);
        PlayerPrefs.SetInt("DifferentArtistsPlayed", differentArtistsPlayed);
        
        // Guardar lista de artistas reproducidos
        string artistsString = string.Join("|", playedArtists);
        PlayerPrefs.SetString("PlayedArtists", artistsString);
        
        PlayerPrefs.Save();
        
        Debug.Log($"[ArtistDataManager] Datos guardados - {currentArtist} ({differentArtistsPlayed} artistas diferentes)");
    }
    
    /// <summary>
    /// Carga los datos de artista
    /// </summary>
    public void LoadArtistData()
    {
        currentArtist = PlayerPrefs.GetString("CurrentArtist", "");
        currentArtistGenre = PlayerPrefs.GetString("CurrentArtistGenre", "");
        lastPlayedArtist = PlayerPrefs.GetString("LastPlayedArtist", "");
        differentArtistsPlayed = PlayerPrefs.GetInt("DifferentArtistsPlayed", 0);
        
        // Cargar lista de artistas reproducidos
        string artistsString = PlayerPrefs.GetString("PlayedArtists", "");
        if (!string.IsNullOrEmpty(artistsString))
        {
            playedArtists = new List<string>(artistsString.Split('|'));
        }
        else
        {
            playedArtists = new List<string>();
        }
        
        Debug.Log($"[ArtistDataManager] Datos cargados - {currentArtist} ({differentArtistsPlayed} artistas diferentes)");
    }
    
    /// <summary>
    /// Limpia todos los datos de artista
    /// </summary>
    public void ClearArtistData()
    {
        currentArtist = "";
        currentArtistGenre = "";
        lastPlayedArtist = "";
        differentArtistsPlayed = 0;
        playedArtists.Clear();
        
        PlayerPrefs.DeleteKey("CurrentArtist");
        PlayerPrefs.DeleteKey("CurrentArtistGenre");
        PlayerPrefs.DeleteKey("LastPlayedArtist");
        PlayerPrefs.DeleteKey("DifferentArtistsPlayed");
        PlayerPrefs.DeleteKey("PlayedArtists");
        PlayerPrefs.Save();
        
        Debug.Log("[ArtistDataManager] Datos de artista limpiados");
    }
    
    /// <summary>
    /// Obtiene estadísticas del artista
    /// </summary>
    public string GetArtistStats()
    {
        return $"Artista actual: {currentArtist}\n" +
               $"Género: {currentArtistGenre}\n" +
               $"Artistas diferentes: {differentArtistsPlayed}\n" +
               $"Último reproducido: {lastPlayedArtist}";
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            SaveArtistData();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Al reanudar
        {
            SaveArtistData();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) // Al perder foco
        {
            SaveArtistData();
        }
    }
}
