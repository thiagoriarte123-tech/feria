using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Gestor de UI para la escena PostGameplay
/// Muestra los datos recopilados de score, combo, canción y artista
/// </summary>
public class PostGameplayUIManager : MonoBehaviour
{
    [Header("TextMeshPro References - Game Data")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI artistText;
    
    [Header("TextMeshPro References - Additional Info")]
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI completionText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI rankText;
    
    [Header("Auto Setup")]
    public bool autoFindComponents = true;
    public bool loadDataOnStart = true;
    
    [Header("Data Managers")]
    private ScoreDataManager scoreManager;
    private ComboDataManager comboManager;
    private SongDataManager songManager;
    private ArtistDataManager artistManager;
    
    [Header("Final Data")]
    public int finalScore = 0;
    public int finalMaxCombo = 0;
    public string finalSongName = "";
    public string finalArtist = "";
    public string finalUserName = "";
    public string finalDifficulty = "Difícil";
    public float finalCompletion = 0f;
    public float finalAccuracy = 0f;
    public string finalRank = "C";
    
    [Header("Real Gameplay Data")]
    public int realTotalNotes = 0;
    public int realHitNotes = 0;
    public int realMissedNotes = 0;
    public int realPerfectHits = 0;
    public int realGreatHits = 0;
    public int realGoodHits = 0;
    
    void Start()
    {
        if (autoFindComponents)
        {
            FindUIComponents();
        }
        
        if (loadDataOnStart)
        {
            LoadAllData();
            UpdateUI();
        }
    }
    
    /// <summary>
    /// Busca automáticamente los componentes UI
    /// </summary>
    void FindUIComponents()
    {
        Debug.Log("[PostGameplayUIManager] Buscando componentes UI...");
        
        // Buscar todos los TextMeshPro en la escena
        TextMeshProUGUI[] allTMPTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI tmpComponent in allTMPTexts)
        {
            string name = tmpComponent.name.ToLower();
            string text = tmpComponent.text.ToLower();
            
            Debug.Log($"[PostGameplayUIManager] Analizando TMP: {tmpComponent.name} - Texto: '{tmpComponent.text}'");
            
            // Identificar por nombre o contenido
            if (name.Contains("score") || text.Contains("score") || text.Contains("15000") || text.Contains("15,000"))
            {
                scoreText = tmpComponent;
                Debug.Log($"✅ Score Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("combo") || text.Contains("combo") || text.Contains("150"))
            {
                maxComboText = tmpComponent;
                Debug.Log($"✅ Max Combo Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("song") || name.Contains("cancion") || text.Contains("test song") || text.Contains("song"))
            {
                songNameText = tmpComponent;
                Debug.Log($"✅ Song Name Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("artist") || name.Contains("artista") || text.Contains("test artist") || text.Contains("artist"))
            {
                artistText = tmpComponent;
                Debug.Log($"✅ Artist Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("user") || name.Contains("player") || name.Contains("jugador"))
            {
                userNameText = tmpComponent;
                Debug.Log($"✅ User Name Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("completion") || text.Contains("completion") || text.Contains("82.3%") || text.Contains("%"))
            {
                completionText = tmpComponent;
                Debug.Log($"✅ Completion Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("accuracy") || name.Contains("precision") || text.Contains("accuracy"))
            {
                accuracyText = tmpComponent;
                Debug.Log($"✅ Accuracy Text encontrado: {tmpComponent.name}");
            }
            else if (name.Contains("rank") || name.Contains("grade") || text.Contains("rank") || text.Contains("grade"))
            {
                rankText = tmpComponent;
                Debug.Log($"✅ Rank Text encontrado: {tmpComponent.name}");
            }
        }
    }
    
    /// <summary>
    /// Carga todos los datos de los gestores
    /// </summary>
    void LoadAllData()
    {
        Debug.Log("[PostGameplayUIManager] Cargando datos...");
        
        // Obtener instancias de los gestores
        scoreManager = ScoreDataManager.Instance;
        comboManager = ComboDataManager.Instance;
        songManager = SongDataManager.Instance;
        artistManager = ArtistDataManager.Instance;
        
        // Cargar datos desde los gestores
        if (scoreManager != null)
        {
            finalScore = scoreManager.GetCurrentScore();
            Debug.Log($"[PostGameplayUIManager] Score cargado: {finalScore}");
        }
        
        if (comboManager != null)
        {
            finalMaxCombo = comboManager.GetMaxCombo();
            Debug.Log($"[PostGameplayUIManager] Max Combo cargado: {finalMaxCombo}");
        }
        
        if (songManager != null)
        {
            finalSongName = songManager.GetCurrentSongName();
            finalCompletion = songManager.GetSongProgress();
            Debug.Log($"[PostGameplayUIManager] Canción cargada: {finalSongName} ({finalCompletion:F1}%)");
        }
        
        if (artistManager != null)
        {
            finalArtist = artistManager.GetCurrentArtist();
            Debug.Log($"[PostGameplayUIManager] Artista cargado: {finalArtist}");
        }
        
        // Cargar datos reales capturados
        LoadRealCapturedData();
        
        // Cargar datos adicionales desde PlayerPrefs como respaldo
        LoadBackupData();
        
        // Cargar nombre de usuario
        finalUserName = PlayerPrefs.GetString("UserName", "Jugador");
        
        // Cargar dificultad (siempre "Difícil")
        LoadDifficultyData();
        
        // Cargar datos reales del gameplay
        LoadRealGameplayData();
    }
    
    /// <summary>
    /// Carga datos reales capturados por RealDataCapture
    /// </summary>
    void LoadRealCapturedData()
    {
        Debug.Log("[PostGameplayUIManager] Cargando datos reales capturados...");
        
        // Verificar si hay datos reales capturados
        if (PlayerPrefs.HasKey("RealSongName"))
        {
            string realSong = PlayerPrefs.GetString("RealSongName", "");
            if (!string.IsNullOrEmpty(realSong) && realSong != "Canción Actual")
            {
                finalSongName = realSong;
                Debug.Log($"[PostGameplayUIManager] ✅ Canción real cargada: {finalSongName}");
            }
        }
        
        if (PlayerPrefs.HasKey("RealArtist"))
        {
            string realArtist = PlayerPrefs.GetString("RealArtist", "");
            if (!string.IsNullOrEmpty(realArtist) && realArtist != "Artista Desconocido")
            {
                finalArtist = realArtist;
                Debug.Log($"[PostGameplayUIManager] ✅ Artista real cargado: {finalArtist}");
            }
        }
        
        if (PlayerPrefs.HasKey("RealScore"))
        {
            int realScore = PlayerPrefs.GetInt("RealScore", 0);
            if (realScore > finalScore)
            {
                finalScore = realScore;
                Debug.Log($"[PostGameplayUIManager] ✅ Score real cargado: {finalScore}");
            }
        }
        
        if (PlayerPrefs.HasKey("RealMaxCombo"))
        {
            int realMaxCombo = PlayerPrefs.GetInt("RealMaxCombo", 0);
            if (realMaxCombo > finalMaxCombo)
            {
                finalMaxCombo = realMaxCombo;
                Debug.Log($"[PostGameplayUIManager] ✅ Max Combo real cargado: {finalMaxCombo}");
            }
        }
        
        if (PlayerPrefs.HasKey("RealSongProgress"))
        {
            float realProgress = PlayerPrefs.GetFloat("RealSongProgress", 0f);
            if (realProgress > finalCompletion)
            {
                finalCompletion = realProgress;
                Debug.Log($"[PostGameplayUIManager] ✅ Progreso real cargado: {finalCompletion:F1}%");
            }
        }
    }
    
    /// <summary>
    /// Carga datos de respaldo desde PlayerPrefs
    /// </summary>
    void LoadBackupData()
    {
        // Si no hay datos de los gestores, usar PlayerPrefs (pero no valores por defecto altos)
        if (finalScore == 0)
        {
            finalScore = PlayerPrefs.GetInt("CurrentScore", PlayerPrefs.GetInt("LastScore", 0));
        }
        
        if (finalMaxCombo == 0)
        {
            finalMaxCombo = PlayerPrefs.GetInt("MaxCombo", PlayerPrefs.GetInt("LastMaxCombo", 0));
        }
        
        if (string.IsNullOrEmpty(finalSongName))
        {
            finalSongName = PlayerPrefs.GetString("CurrentSongName", PlayerPrefs.GetString("LastSongName", ""));
            if (string.IsNullOrEmpty(finalSongName))
            {
                finalSongName = "Sin Canción";
            }
        }
        
        if (string.IsNullOrEmpty(finalArtist))
        {
            finalArtist = PlayerPrefs.GetString("CurrentArtist", PlayerPrefs.GetString("LastArtist", ""));
            if (string.IsNullOrEmpty(finalArtist))
            {
                finalArtist = "Sin Artista";
            }
        }
        
        if (finalCompletion == 0f)
        {
            finalCompletion = PlayerPrefs.GetFloat("LastCompletion", 0f);
        }
        
        Debug.Log("[PostGameplayUIManager] Datos de respaldo cargados");
    }
    
    /// <summary>
    /// Carga datos de dificultad (siempre "Difícil")
    /// </summary>
    void LoadDifficultyData()
    {
        // Obtener dificultad del GameplayDifficultyManager si existe
        if (GameplayDifficultyManager.Instance != null)
        {
            finalDifficulty = GameplayDifficultyManager.Instance.GetDisplayDifficulty();
        }
        else
        {
            // Conversión manual si no hay GameplayDifficultyManager
            string originalDifficulty = PlayerPrefs.GetString("SelectedDifficulty", 
                                       PlayerPrefs.GetString("Difficulty", "Experto"));
            
            string lower = originalDifficulty.ToLower();
            
            // SOLO convertir "Experto" a "Difícil", mantener el resto
            if (lower.Contains("experto") || lower.Contains("expert"))
            {
                finalDifficulty = "Difícil";
            }
            else if (lower.Contains("dificil") || lower.Contains("difícil") || lower.Contains("hard"))
            {
                finalDifficulty = "Difícil";
            }
            else if (lower.Contains("normal") || lower.Contains("medium"))
            {
                finalDifficulty = "Normal";
            }
            else if (lower.Contains("facil") || lower.Contains("fácil") || lower.Contains("easy"))
            {
                finalDifficulty = "Fácil";
            }
            else
            {
                // Mantener original si no coincide con nada conocido
                finalDifficulty = string.IsNullOrEmpty(originalDifficulty) ? "Difícil" : originalDifficulty;
            }
        }
        
        // Guardar la dificultad convertida
        PlayerPrefs.SetString("DisplayDifficulty", finalDifficulty);
        PlayerPrefs.Save();
        
        Debug.Log($"[PostGameplayUIManager] ✅ Dificultad cargada: {finalDifficulty}");
    }
    
    /// <summary>
    /// Carga datos reales del gameplay desde ScoreManager
    /// </summary>
    void LoadRealGameplayData()
    {
        Debug.Log("[PostGameplayUIManager] Cargando datos reales del gameplay...");
        
        // Buscar ScoreManager en la escena actual o en DontDestroyOnLoad
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        
        if (scoreManager != null)
        {
            // Capturar datos reales
            realTotalNotes = scoreManager.totalNotes;
            realHitNotes = scoreManager.hitNotes;
            realMissedNotes = scoreManager.missedNotes;
            realPerfectHits = scoreManager.perfectHits;
            realGreatHits = scoreManager.greatHits;
            realGoodHits = scoreManager.goodHits;
            
            // Calcular completion real
            if (realTotalNotes > 0)
            {
                finalCompletion = ((float)realHitNotes / realTotalNotes) * 100f;
            }
            
            Debug.Log($"[PostGameplayUIManager] ✅ Datos reales cargados:");
            Debug.Log($"  Total Notas: {realTotalNotes}");
            Debug.Log($"  Notas Acertadas: {realHitNotes}");
            Debug.Log($"  Notas Perdidas: {realMissedNotes}");
            Debug.Log($"  Perfect: {realPerfectHits}, Great: {realGreatHits}, Good: {realGoodHits}");
            Debug.Log($"  Completion Real: {finalCompletion:F1}%");
        }
        else
        {
            // Intentar cargar desde PlayerPrefs como respaldo
            realTotalNotes = PlayerPrefs.GetInt("RealTotalNotes", 0);
            realHitNotes = PlayerPrefs.GetInt("RealHitNotes", 0);
            realMissedNotes = PlayerPrefs.GetInt("RealMissedNotes", 0);
            
            if (realTotalNotes == 0 && realHitNotes == 0)
            {
                Debug.Log("[PostGameplayUIManager] ⚠️ No se encontraron datos reales del gameplay");
            }
            else
            {
                Debug.Log($"[PostGameplayUIManager] 📱 Datos cargados desde PlayerPrefs: {realHitNotes}/{realTotalNotes}");
            }
        }
    }
    
    /// <summary>
    /// Actualiza todos los elementos UI
    /// </summary>
    void UpdateUI()
    {
        Debug.Log("[PostGameplayUIManager] Actualizando UI...");
        
        // Actualizar score
        if (scoreText != null)
        {
            scoreText.text = $"Score: {finalScore:N0}";
            Debug.Log($"✅ Score UI actualizado: {finalScore:N0}");
        }
        
        // Actualizar combo
        if (maxComboText != null)
        {
            maxComboText.text = $"Max Combo: {finalMaxCombo}";
            Debug.Log($"✅ Max Combo UI actualizado: {finalMaxCombo}");
        }
        
        // Actualizar canción
        if (songNameText != null)
        {
            songNameText.text = finalSongName;
            Debug.Log($"✅ Song Name UI actualizado: {finalSongName}");
        }
        
        // Actualizar artista
        if (artistText != null)
        {
            artistText.text = $"by {finalArtist}";
            Debug.Log($"✅ Artist UI actualizado: by {finalArtist}");
        }
        
        // Actualizar nombre de usuario
        if (userNameText != null)
        {
            userNameText.text = $"Jugador: {finalUserName}";
            Debug.Log($"✅ User Name UI actualizado: {finalUserName}");
        }
        
        // Actualizar dificultad (buscar TextMeshPro que contenga "dificultad" o "difficulty")
        UpdateDifficultyUI();
        
        // Actualizar completion (usar datos reales)
        if (completionText != null)
        {
            if (realTotalNotes > 0)
            {
                float realCompletion = ((float)realHitNotes / realTotalNotes) * 100f;
                completionText.text = $"Completion: {realCompletion:F1}%";
                Debug.Log($"✅ Completion UI actualizado (real): {realCompletion:F1}% ({realHitNotes}/{realTotalNotes})");
            }
            else
            {
                completionText.text = "Completion: 0.0%";
                Debug.Log("✅ Completion UI: Sin datos reales, mostrando 0%");
            }
        }
        
        // Calcular y actualizar accuracy
        if (accuracyText != null)
        {
            float accuracy = CalculateAccuracy();
            accuracyText.text = $"Accuracy: {accuracy:F1}%";
            Debug.Log($"✅ Accuracy UI actualizado: {accuracy:F1}%");
        }
        
        // Calcular y actualizar rank
        if (rankText != null)
        {
            string rank = CalculateRank();
            rankText.text = $"Rank: {rank}";
            Debug.Log($"✅ Rank UI actualizado: {rank}");
        }
    }
    
    /// <summary>
    /// Calcula la accuracy basada en los datos reales disponibles
    /// </summary>
    float CalculateAccuracy()
    {
        // Usar datos reales cargados
        if (realTotalNotes > 0)
        {
            float realAccuracy = ((float)realHitNotes / realTotalNotes) * 100f;
            Debug.Log($"[PostGameplayUIManager] Accuracy real calculada: {realAccuracy:F1}% ({realHitNotes}/{realTotalNotes})");
            return realAccuracy;
        }
        
        // Intentar obtener datos directamente del ScoreManager como respaldo
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null && scoreManager.totalNotes > 0)
        {
            float realAccuracy = ((float)scoreManager.hitNotes / scoreManager.totalNotes) * 100f;
            Debug.Log($"[PostGameplayUIManager] Accuracy desde ScoreManager: {realAccuracy:F1}% ({scoreManager.hitNotes}/{scoreManager.totalNotes})");
            return realAccuracy;
        }
        
        // Si no hay datos reales, mostrar 0%
        Debug.Log("[PostGameplayUIManager] Sin datos reales para calcular accuracy, mostrando 0%");
        return 0f;
    }
    
    /// <summary>
    /// Calcula el rank basado en el score
    /// </summary>
    string CalculateRank()
    {
        if (finalScore >= 50000)
            return "S";
        else if (finalScore >= 40000)
            return "A";
        else if (finalScore >= 30000)
            return "B";
        else if (finalScore >= 20000)
            return "C";
        else if (finalScore >= 10000)
            return "D";
        else
            return "F";
    }
    
    /// <summary>
    /// Establece datos manualmente (para testing)
    /// </summary>
    public void SetGameplayData(int score, int maxCombo, string songName, string artist, string userName = "")
    {
        finalScore = score;
        finalMaxCombo = maxCombo;
        finalSongName = songName;
        finalArtist = artist;
        
        if (!string.IsNullOrEmpty(userName))
        {
            finalUserName = userName;
        }
        
        UpdateUI();
        
        Debug.Log($"[PostGameplayUIManager] Datos establecidos manualmente: {songName} by {artist} - Score: {score}");
    }
    
    /// <summary>
    /// Configura el botón de retorno al menú
    /// </summary>
    void SetupReturnButton()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (Button button in allButtons)
        {
            string name = button.name.ToLower();
            
            // Buscar botón de retorno
            if (name.Contains("return") || name.Contains("menu") || name.Contains("back"))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    Debug.Log("[PostGameplayUIManager] Volviendo al MainMenu...");
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                });
                
                Debug.Log($"✅ Botón de retorno configurado: {button.name}");
                break;
            }
        }
    }
    
    /// <summary>
    /// Muestra un resumen completo de los datos
    /// </summary>
    [ContextMenu("Show Complete Summary")]
    public void ShowCompleteSummary()
    {
        string summary = "=== RESUMEN COMPLETO POST-GAMEPLAY ===\n";
        summary += $"Jugador: {finalUserName}\n";
        summary += $"Canción: {finalSongName}\n";
        summary += $"Artista: {finalArtist}\n";
        summary += $"Score Final: {finalScore:N0}\n";
        summary += $"Max Combo: {finalMaxCombo}\n";
        summary += $"Completion: {finalCompletion:F1}%\n";
        summary += $"Accuracy: {CalculateAccuracy():F1}%\n";
        summary += $"Rank: {CalculateRank()}\n";
        
        Debug.Log(summary);
    }
    
    /// <summary>
    /// Actualiza la UI de dificultad
    /// </summary>
    void UpdateDifficultyUI()
    {
        // Buscar TextMeshPro que pueda contener información de dificultad
        TMPro.TextMeshProUGUI[] allTexts = FindObjectsByType<TMPro.TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (var text in allTexts)
        {
            if (text != null && !string.IsNullOrEmpty(text.text))
            {
                string textContent = text.text.ToLower();
                
                // SOLO buscar y reemplazar "Experto" por "Difícil"
                if (textContent.Contains("experto") || textContent.Contains("expert"))
                {
                    text.text = text.text.Replace("Experto", "Difícil")
                                        .Replace("Expert", "Difícil")
                                        .Replace("EXPERTO", "DIFÍCIL")
                                        .Replace("EXPERT", "DIFÍCIL")
                                        .Replace("experto", "difícil")
                                        .Replace("expert", "difícil");
                    
                    Debug.Log($"✅ 'Experto' cambiado a 'Difícil' en UI: {text.text}");
                }
                // Para otros textos de dificultad, solo mostrar si no está ya presente
                else if ((textContent.Contains("dificultad") || textContent.Contains("difficulty")) &&
                         !textContent.Contains(finalDifficulty.ToLower()))
                {
                    text.text = $"Dificultad: {finalDifficulty}";
                    Debug.Log($"✅ Dificultad UI establecida: {finalDifficulty}");
                }
            }
        }
    }
    
    void OnEnable()
    {
        // Configurar botón de retorno
        Invoke(nameof(SetupReturnButton), 0.5f);
    }
}
