using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Detecta cuando la canción termina y fuerza la transición al PostGameplay
/// Soluciona el problema de que el video sigue pero no pasa a la siguiente escena
/// </summary>
public class EnhancedSongEndDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public bool enableDetection = true;
    public float checkInterval = 0.5f;
    public float endThreshold = 0.5f; // Segundos antes del final para detectar
    
    [Header("Auto Detection")]
    public AudioSource mainAudioSource;
    public bool autoFindAudioSource = true;
    
    [Header("Transition Settings")]
    public string postGameplaySceneName = "PostGameplay";
    public float transitionDelay = 1f;
    
    [Header("Debug Info")]
    public bool showDebugLogs = true;
    public float songDuration = 0f;
    public float currentTime = 0f;
    public bool songEnded = false;
    
    private float lastCheckTime = 0f;
    private bool transitionTriggered = false;
    
    void Start()
    {
        if (autoFindAudioSource)
        {
            FindMainAudioSource();
        }
    }
    
    void Update()
    {
        if (enableDetection && !transitionTriggered && Time.time - lastCheckTime > checkInterval)
        {
            CheckSongEnd();
            lastCheckTime = Time.time;
        }
    }
    
    /// <summary>
    /// Busca el AudioSource principal automáticamente
    /// </summary>
    void FindMainAudioSource()
    {
        if (mainAudioSource == null)
        {
            AudioSource[] audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            
            foreach (AudioSource audio in audioSources)
            {
                if (audio != null && audio.clip != null && audio.clip.length > 30f)
                {
                    // Probablemente la canción principal
                    if (audio.isPlaying || audio.enabled)
                    {
                        mainAudioSource = audio;
                        songDuration = audio.clip.length;
                        
                        if (showDebugLogs)
                        {
                            Debug.Log($"[SongEndDetector] ✅ AudioSource principal encontrado: {audio.clip.name} ({songDuration:F1}s)");
                        }
                        break;
                    }
                }
            }
            
            if (mainAudioSource == null && showDebugLogs)
            {
                Debug.LogWarning("[SongEndDetector] ⚠️ No se encontró AudioSource principal");
            }
        }
    }
    
    /// <summary>
    /// Verifica si la canción ha terminado
    /// </summary>
    void CheckSongEnd()
    {
        if (mainAudioSource == null || mainAudioSource.clip == null)
        {
            return;
        }
        
        currentTime = mainAudioSource.time;
        songDuration = mainAudioSource.clip.length;
        
        // Verificar si la canción terminó
        bool audioFinished = !mainAudioSource.isPlaying && currentTime > 0f;
        bool nearEnd = (songDuration - currentTime) <= endThreshold;
        bool timeExceeded = currentTime >= songDuration - 0.1f;
        
        songEnded = audioFinished || nearEnd || timeExceeded;
        
        if (songEnded && !transitionTriggered)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[SongEndDetector] 🎵 Canción terminada detectada!");
                Debug.Log($"Audio Playing: {mainAudioSource.isPlaying}");
                Debug.Log($"Current Time: {currentTime:F1}s / {songDuration:F1}s");
                Debug.Log($"Time Remaining: {(songDuration - currentTime):F1}s");
            }
            
            TriggerPostGameplayTransition();
        }
    }
    
    /// <summary>
    /// Activa la transición al PostGameplay
    /// </summary>
    void TriggerPostGameplayTransition()
    {
        transitionTriggered = true;
        
        if (showDebugLogs)
        {
            Debug.Log($"[SongEndDetector] 🚀 Iniciando transición a {postGameplaySceneName} en {transitionDelay}s...");
        }
        
        // Guardar datos antes de la transición
        SaveGameplayData();
        
        // Ejecutar transición con delay
        Invoke(nameof(LoadPostGameplayScene), transitionDelay);
    }
    
    /// <summary>
    /// Guarda los datos del gameplay antes de la transición
    /// </summary>
    void SaveGameplayData()
    {
        // Forzar guardado de todos los DataManagers
        if (RealDataCapture.Instance != null)
        {
            RealDataCapture.Instance.ForceCaptureAllData();
        }
        
        // Guardar datos adicionales
        PlayerPrefs.SetFloat("SongCompletionTime", currentTime);
        PlayerPrefs.SetFloat("SongTotalDuration", songDuration);
        PlayerPrefs.SetFloat("SongCompletionPercentage", (currentTime / songDuration) * 100f);
        PlayerPrefs.SetString("TransitionReason", "SongEnded");
        PlayerPrefs.Save();
        
        if (showDebugLogs)
        {
            Debug.Log($"[SongEndDetector] 💾 Datos guardados - Completion: {((currentTime / songDuration) * 100f):F1}%");
        }
    }
    
    /// <summary>
    /// Carga la escena PostGameplay
    /// </summary>
    void LoadPostGameplayScene()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[SongEndDetector] 🎬 Cargando escena: {postGameplaySceneName}");
        }
        
        try
        {
            SceneManager.LoadScene(postGameplaySceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SongEndDetector] ❌ Error cargando escena {postGameplaySceneName}: {e.Message}");
            
            // Intentar con nombres alternativos
            string[] alternativeNames = { "Post Gameplay", "PostGame", "Results", "GameResults" };
            
            foreach (string altName in alternativeNames)
            {
                try
                {
                    SceneManager.LoadScene(altName);
                    Debug.Log($"[SongEndDetector] ✅ Escena cargada con nombre alternativo: {altName}");
                    return;
                }
                catch
                {
                    // Continuar con el siguiente nombre
                }
            }
            
            Debug.LogError("[SongEndDetector] ❌ No se pudo cargar ninguna escena PostGameplay");
        }
    }
    
    /// <summary>
    /// Fuerza la transición manualmente (para testing)
    /// </summary>
    [ContextMenu("Force Transition to PostGameplay")]
    public void ForceTransition()
    {
        Debug.Log("[SongEndDetector] 🔧 Forzando transición manual...");
        
        if (!transitionTriggered)
        {
            TriggerPostGameplayTransition();
        }
    }
    
    /// <summary>
    /// Muestra información de debug
    /// </summary>
    [ContextMenu("Show Song Status")]
    public void ShowSongStatus()
    {
        if (mainAudioSource != null && mainAudioSource.clip != null)
        {
            Debug.Log("🎵 ESTADO DE LA CANCIÓN:");
            Debug.Log("═══════════════════════");
            Debug.Log($"Canción: {mainAudioSource.clip.name}");
            Debug.Log($"Duración: {mainAudioSource.clip.length:F1}s");
            Debug.Log($"Tiempo Actual: {mainAudioSource.time:F1}s");
            Debug.Log($"Reproduciendo: {mainAudioSource.isPlaying}");
            Debug.Log($"Progreso: {((mainAudioSource.time / mainAudioSource.clip.length) * 100f):F1}%");
            Debug.Log($"Tiempo Restante: {(mainAudioSource.clip.length - mainAudioSource.time):F1}s");
            Debug.Log($"Transición Activada: {transitionTriggered}");
        }
        else
        {
            Debug.LogWarning("[SongEndDetector] No hay AudioSource o clip disponible");
        }
    }
    
    /// <summary>
    /// Resetea el detector (útil si se reinicia la canción)
    /// </summary>
    public void ResetDetector()
    {
        transitionTriggered = false;
        songEnded = false;
        
        if (showDebugLogs)
        {
            Debug.Log("[SongEndDetector] 🔄 Detector reseteado");
        }
    }
}
