using UnityEngine;

/// <summary>
/// Script para configurar automáticamente todos los DataManagers
/// Ejecutar una vez y luego eliminar este script
/// </summary>
public class AutoSetupDataManagers : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    public bool removeThisScriptAfterSetup = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupAllDataManagers();
        }
    }
    
    /// <summary>
    /// Configura automáticamente todos los DataManagers
    /// </summary>
    [ContextMenu("Setup All Data Managers")]
    public void SetupAllDataManagers()
    {
        Debug.Log("[AutoSetup] Configurando todos los DataManagers...");
        
        GameObject targetObject = this.gameObject;
        
        // Agregar RealDataCapture si no existe
        if (targetObject.GetComponent<RealDataCapture>() == null)
        {
            targetObject.AddComponent<RealDataCapture>();
            Debug.Log("✅ RealDataCapture agregado");
        }
        
        // Agregar ScoreDataManager si no existe
        if (targetObject.GetComponent<ScoreDataManager>() == null)
        {
            targetObject.AddComponent<ScoreDataManager>();
            Debug.Log("✅ ScoreDataManager agregado");
        }
        
        // Agregar ComboDataManager si no existe
        if (targetObject.GetComponent<ComboDataManager>() == null)
        {
            targetObject.AddComponent<ComboDataManager>();
            Debug.Log("✅ ComboDataManager agregado");
        }
        
        // Agregar SongDataManager si no existe
        if (targetObject.GetComponent<SongDataManager>() == null)
        {
            targetObject.AddComponent<SongDataManager>();
            Debug.Log("✅ SongDataManager agregado");
        }
        
        // Agregar ArtistDataManager si no existe
        if (targetObject.GetComponent<ArtistDataManager>() == null)
        {
            targetObject.AddComponent<ArtistDataManager>();
            Debug.Log("✅ ArtistDataManager agregado");
        }
        
        // Agregar GameplayUIManager si no existe
        if (targetObject.GetComponent<GameplayUIManager>() == null)
        {
            targetObject.AddComponent<GameplayUIManager>();
            Debug.Log("✅ GameplayUIManager agregado");
        }
        
        // Agregar EnhancedSongEndDetector si no existe
        if (targetObject.GetComponent<EnhancedSongEndDetector>() == null)
        {
            targetObject.AddComponent<EnhancedSongEndDetector>();
            Debug.Log("✅ EnhancedSongEndDetector agregado");
        }
        
        // Agregar GameplayDifficultyManager si no existe
        if (targetObject.GetComponent<GameplayDifficultyManager>() == null)
        {
            targetObject.AddComponent<GameplayDifficultyManager>();
            Debug.Log("✅ GameplayDifficultyManager agregado");
        }
        
        Debug.Log("[AutoSetup] ¡Todos los DataManagers configurados correctamente!");
        
        // Mostrar resumen
        ShowComponentsSummary();
        
        // Eliminar este script después de la configuración
        if (removeThisScriptAfterSetup)
        {
            Debug.Log("[AutoSetup] Eliminando AutoSetupDataManagers...");
            
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }
    }
    
    /// <summary>
    /// Muestra resumen de componentes agregados
    /// </summary>
    void ShowComponentsSummary()
    {
        Debug.Log("📋 RESUMEN DE COMPONENTES AGREGADOS:");
        Debug.Log("═══════════════════════════════════");
        
        GameObject obj = this.gameObject;
        
        Debug.Log($"RealDataCapture: {(obj.GetComponent<RealDataCapture>() != null ? "✅ PRESENTE" : "❌ FALTANTE")}");
        Debug.Log($"ScoreDataManager: {(obj.GetComponent<ScoreDataManager>() != null ? "✅ PRESENTE" : "❌ FALTANTE")}");
        Debug.Log($"ComboDataManager: {(obj.GetComponent<ComboDataManager>() != null ? "✅ PRESENTE" : "❌ FALTANTE")}");
        Debug.Log($"SongDataManager: {(obj.GetComponent<SongDataManager>() != null ? "✅ PRESENTE" : "❌ FALTANTE")}");
        Debug.Log($"ArtistDataManager: {(obj.GetComponent<ArtistDataManager>() != null ? "✅ PRESENTE" : "❌ FALTANTE")}");
        Debug.Log($"GameplayUIManager: {(obj.GetComponent<GameplayUIManager>() != null ? "✅ PRESENTE" : "❌ FALTANTE")}");
        Debug.Log($"EnhancedSongEndDetector: {(obj.GetComponent<EnhancedSongEndDetector>() != null ? "✅ PRESENTE" : "❌ FALTANTE")}");
        Debug.Log($"GameplayDifficultyManager: {(obj.GetComponent<GameplayDifficultyManager>() != null ? "✅ PRESENTE" : "❌ FALTANTE")}");
        
        Debug.Log("\n🚀 SIGUIENTE PASO:");
        Debug.Log("Ejecuta el juego y revisa la consola para verificar que todo funcione correctamente.");
    }
    
    /// <summary>
    /// Elimina todos los DataManagers (para limpiar si es necesario)
    /// </summary>
    [ContextMenu("Remove All Data Managers")]
    public void RemoveAllDataManagers()
    {
        Debug.Log("[AutoSetup] Eliminando todos los DataManagers...");
        
        GameObject obj = this.gameObject;
        
        // Eliminar componentes
        if (obj.GetComponent<RealDataCapture>() != null)
        {
            DestroyImmediate(obj.GetComponent<RealDataCapture>());
            Debug.Log("❌ RealDataCapture eliminado");
        }
        
        if (obj.GetComponent<ScoreDataManager>() != null)
        {
            DestroyImmediate(obj.GetComponent<ScoreDataManager>());
            Debug.Log("❌ ScoreDataManager eliminado");
        }
        
        if (obj.GetComponent<ComboDataManager>() != null)
        {
            DestroyImmediate(obj.GetComponent<ComboDataManager>());
            Debug.Log("❌ ComboDataManager eliminado");
        }
        
        if (obj.GetComponent<SongDataManager>() != null)
        {
            DestroyImmediate(obj.GetComponent<SongDataManager>());
            Debug.Log("❌ SongDataManager eliminado");
        }
        
        if (obj.GetComponent<ArtistDataManager>() != null)
        {
            DestroyImmediate(obj.GetComponent<ArtistDataManager>());
            Debug.Log("❌ ArtistDataManager eliminado");
        }
        
        if (obj.GetComponent<GameplayUIManager>() != null)
        {
            DestroyImmediate(obj.GetComponent<GameplayUIManager>());
            Debug.Log("❌ GameplayUIManager eliminado");
        }
        
        Debug.Log("[AutoSetup] Todos los DataManagers eliminados");
    }
}
