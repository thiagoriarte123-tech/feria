using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Herramienta para limpiar y organizar el proyecto Clone Hero
/// </summary>
public class ProjectCleanupHelper : MonoBehaviour
{
    #if UNITY_EDITOR
    
    [MenuItem("Clone Hero/Limpieza/1. Listar Archivos a Limpiar")]
    public static void ListFilesToClean()
    {
        Debug.Log("=== 🧹 ANÁLISIS DE LIMPIEZA DEL PROYECTO ===\n");
        
        int totalFiles = 0;
        
        // 1. Archivos .bak
        Debug.Log("📁 ARCHIVOS .BAK (Copias de seguridad):");
        string[] bakFiles = Directory.GetFiles(Application.dataPath, "*.bak", SearchOption.AllDirectories);
        foreach (string file in bakFiles)
        {
            Debug.Log($"  - {GetRelativePath(file)}");
            totalFiles++;
        }
        Debug.Log($"Total archivos .bak: {bakFiles.Length}\n");
        
        // 2. Archivos .md en Scripts/Gameplay
        Debug.Log("📄 ARCHIVOS .MD (Documentación):");
        string gameplayPath = Path.Combine(Application.dataPath, "Scripts", "Gameplay");
        if (Directory.Exists(gameplayPath))
        {
            string[] mdFiles = Directory.GetFiles(gameplayPath, "*.md", SearchOption.TopDirectoryOnly);
            foreach (string file in mdFiles)
            {
                Debug.Log($"  - {GetRelativePath(file)}");
                totalFiles++;
            }
            Debug.Log($"Total archivos .md en Gameplay: {mdFiles.Length}\n");
        }
        
        // 3. Scripts en Fixes/
        Debug.Log("🔧 SCRIPTS EN FIXES/:");
        string fixesPath = Path.Combine(Application.dataPath, "Scripts", "Fixes");
        if (Directory.Exists(fixesPath))
        {
            string[] csFiles = Directory.GetFiles(fixesPath, "*.cs", SearchOption.TopDirectoryOnly);
            foreach (string file in csFiles)
            {
                Debug.Log($"  - {GetRelativePath(file)}");
                totalFiles++;
            }
            Debug.Log($"Total scripts en Fixes: {csFiles.Length}\n");
        }
        
        Debug.Log($"=== TOTAL DE ARCHIVOS ANALIZADOS: {totalFiles} ===");
        Debug.Log("Usa 'Clone Hero/Limpieza/2. Eliminar Archivos .bak' para limpiar automáticamente los .bak");
    }
    
    [MenuItem("Clone Hero/Limpieza/2. Eliminar Archivos .bak")]
    public static void DeleteBakFiles()
    {
        if (!EditorUtility.DisplayDialog("Eliminar Archivos .bak", 
            "¿Estás seguro de que quieres eliminar todos los archivos .bak del proyecto?\n\nEsta acción NO se puede deshacer.", 
            "Sí, eliminar", "Cancelar"))
        {
            return;
        }
        
        string[] bakFiles = Directory.GetFiles(Application.dataPath, "*.bak", SearchOption.AllDirectories);
        int deletedCount = 0;
        
        foreach (string file in bakFiles)
        {
            try
            {
                File.Delete(file);
                
                // También eliminar el .meta asociado
                string metaFile = file + ".meta";
                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                }
                
                deletedCount++;
                Debug.Log($"✅ Eliminado: {GetRelativePath(file)}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error eliminando {file}: {e.Message}");
            }
        }
        
        AssetDatabase.Refresh();
        
        Debug.Log($"\n=== ✅ LIMPIEZA COMPLETADA ===");
        Debug.Log($"Archivos .bak eliminados: {deletedCount}");
        
        EditorUtility.DisplayDialog("Limpieza Completada", 
            $"Se eliminaron {deletedCount} archivos .bak exitosamente.", "OK");
    }
    
    [MenuItem("Clone Hero/Limpieza/3. Crear Carpeta Documentation")]
    public static void CreateDocumentationFolder()
    {
        string docPath = Path.Combine(Application.dataPath, "Documentation");
        
        if (Directory.Exists(docPath))
        {
            Debug.LogWarning("La carpeta Documentation ya existe.");
            return;
        }
        
        // Crear estructura de carpetas
        Directory.CreateDirectory(docPath);
        Directory.CreateDirectory(Path.Combine(docPath, "Video"));
        Directory.CreateDirectory(Path.Combine(docPath, "Highway"));
        Directory.CreateDirectory(Path.Combine(docPath, "UI"));
        Directory.CreateDirectory(Path.Combine(docPath, "General"));
        
        AssetDatabase.Refresh();
        
        Debug.Log("✅ Carpeta Documentation creada con subcarpetas:");
        Debug.Log("  - Documentation/Video/");
        Debug.Log("  - Documentation/Highway/");
        Debug.Log("  - Documentation/UI/");
        Debug.Log("  - Documentation/General/");
        
        EditorUtility.DisplayDialog("Carpetas Creadas", 
            "Se creó la carpeta Documentation con sus subcarpetas.\n\nAhora puedes mover manualmente los archivos .md a estas carpetas.", "OK");
    }
    
    [MenuItem("Clone Hero/Limpieza/4. Crear Carpeta _Testing")]
    public static void CreateTestingFolder()
    {
        string testingPath = Path.Combine(Application.dataPath, "Scripts", "_Testing");
        
        if (Directory.Exists(testingPath))
        {
            Debug.LogWarning("La carpeta _Testing ya existe.");
            return;
        }
        
        Directory.CreateDirectory(testingPath);
        AssetDatabase.Refresh();
        
        Debug.Log("✅ Carpeta Scripts/_Testing/ creada");
        Debug.Log("Ahora puedes mover los scripts de diagnóstico y testing a esta carpeta.");
        
        EditorUtility.DisplayDialog("Carpeta Creada", 
            "Se creó la carpeta Scripts/_Testing/\n\nPuedes mover los scripts de testing a esta carpeta.", "OK");
    }
    
    [MenuItem("Clone Hero/Build/Verificar Videos en StreamingAssets")]
    public static void VerifyStreamingAssetsVideos()
    {
        Debug.Log("=== 🎬 VERIFICACIÓN DE VIDEOS EN STREAMINGASSETS ===\n");
        
        string songsPath = Path.Combine(Application.streamingAssetsPath, "Songs");
        
        if (!Directory.Exists(songsPath))
        {
            Debug.LogError($"❌ La carpeta Songs no existe en: {songsPath}");
            EditorUtility.DisplayDialog("Error", 
                "No se encontró la carpeta StreamingAssets/Songs/", "OK");
            return;
        }
        
        string[] songFolders = Directory.GetDirectories(songsPath);
        int totalSongs = songFolders.Length;
        int songsWithVideo = 0;
        int songsWithoutVideo = 0;
        
        List<string> songsWithVideoList = new List<string>();
        List<string> songsWithoutVideoList = new List<string>();
        
        foreach (string songFolder in songFolders)
        {
            string songName = Path.GetFileName(songFolder);
            
            // Buscar archivos de video
            string[] videoFiles = new string[0];
            videoFiles = CombineArrays(
                Directory.GetFiles(songFolder, "*.mp4"),
                Directory.GetFiles(songFolder, "*.webm"),
                Directory.GetFiles(songFolder, "*.mov"),
                Directory.GetFiles(songFolder, "*.avi")
            );
            
            if (videoFiles.Length > 0)
            {
                songsWithVideo++;
                songsWithVideoList.Add(songName);
            }
            else
            {
                songsWithoutVideo++;
                songsWithoutVideoList.Add(songName);
            }
        }
        
        // Mostrar resultados
        Debug.Log($"📊 RESUMEN:");
        Debug.Log($"Total de canciones: {totalSongs}");
        Debug.Log($"Canciones con video: {songsWithVideo} ({(float)songsWithVideo/totalSongs*100:F1}%)");
        Debug.Log($"Canciones sin video: {songsWithoutVideo} ({(float)songsWithoutVideo/totalSongs*100:F1}%)\n");
        
        if (songsWithVideo > 0)
        {
            Debug.Log("✅ CANCIONES CON VIDEO:");
            foreach (string song in songsWithVideoList)
            {
                Debug.Log($"  ✓ {song}");
            }
            Debug.Log("");
        }
        
        if (songsWithoutVideo > 0)
        {
            Debug.Log("⚠️ CANCIONES SIN VIDEO:");
            foreach (string song in songsWithoutVideoList)
            {
                Debug.Log($"  × {song}");
            }
            Debug.Log("");
        }
        
        string message = $"Total: {totalSongs} canciones\n" +
                        $"Con video: {songsWithVideo}\n" +
                        $"Sin video: {songsWithoutVideo}\n\n" +
                        $"Revisa la consola para más detalles.";
        
        EditorUtility.DisplayDialog("Verificación Completada", message, "OK");
    }
    
    [MenuItem("Clone Hero/Build/Configurar Build Settings")]
    public static void ConfigureBuildSettings()
    {
        Debug.Log("=== ⚙️ CONFIGURACIÓN DE BUILD SETTINGS ===\n");
        
        // Encontrar todas las escenas
        string[] scenes = new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/CrearUsuario.unity",
            "Assets/Scenes/Gameplay.unity",
            "Assets/Scenes/PostGameplay.unity"
        };
        
        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
        
        foreach (string scenePath in scenes)
        {
            if (File.Exists(scenePath))
            {
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                Debug.Log($"✅ Escena agregada: {scenePath}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Escena no encontrada: {scenePath}");
            }
        }
        
        EditorBuildSettings.scenes = buildScenes.ToArray();
        
        Debug.Log($"\n=== ✅ BUILD SETTINGS CONFIGURADO ===");
        Debug.Log($"Total de escenas: {buildScenes.Count}");
        
        EditorUtility.DisplayDialog("Build Settings Configurado", 
            $"Se configuraron {buildScenes.Count} escenas en Build Settings.\n\nAhora puedes hacer el build desde File > Build Settings.", "OK");
        
        // Abrir Build Settings window
        EditorApplication.ExecuteMenuItem("File/Build Settings...");
    }
    
    [MenuItem("Clone Hero/Ayuda/Abrir Guía de Limpieza")]
    public static void OpenCleanupGuide()
    {
        string guidePath = Path.Combine(Application.dataPath, "..", "GUIA_LIMPIEZA_Y_BUILD.md");
        
        if (File.Exists(guidePath))
        {
            Application.OpenURL("file:///" + Path.GetFullPath(guidePath));
            Debug.Log("✅ Guía abierta: GUIA_LIMPIEZA_Y_BUILD.md");
        }
        else
        {
            Debug.LogError("❌ No se encontró el archivo GUIA_LIMPIEZA_Y_BUILD.md");
            EditorUtility.DisplayDialog("Archivo no encontrado", 
                "No se encontró GUIA_LIMPIEZA_Y_BUILD.md en la raíz del proyecto.", "OK");
        }
    }
    
    // Utilidades
    static string GetRelativePath(string fullPath)
    {
        return fullPath.Replace(Application.dataPath, "Assets");
    }
    
    static string[] CombineArrays(params string[][] arrays)
    {
        List<string> result = new List<string>();
        foreach (string[] array in arrays)
        {
            result.AddRange(array);
        }
        return result.ToArray();
    }
    
    #endif
}
