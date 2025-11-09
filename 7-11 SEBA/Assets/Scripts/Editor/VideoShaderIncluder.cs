using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// Asegura que los shaders necesarios para el video estén incluidos en el build
/// </summary>
[InitializeOnLoad]
public class VideoShaderIncluder
{
    static VideoShaderIncluder()
    {
        // Ejecutar cuando Unity se inicia o recompila
        EnsureVideoShadersIncluded();
    }

    [MenuItem("Tools/Video System/Include Video Shaders in Build")]
    public static void EnsureVideoShadersIncluded()
    {
        // Obtener Graphics Settings
        var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
        
        // Nombres de shaders necesarios para el video
        string[] requiredShaders = new string[]
        {
            "Unlit/Texture",
            "UI/Default",
            "Standard"
        };

        // Cargar los shaders y agregarlos a Always Included Shaders
        SerializedObject graphicsSettingsObj = new SerializedObject(graphicsSettings);
        SerializedProperty alwaysIncludedShaders = graphicsSettingsObj.FindProperty("m_AlwaysIncludedShaders");

        bool shadersAdded = false;

        foreach (string shaderName in requiredShaders)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                // Verificar si ya está incluido
                bool alreadyIncluded = false;
                for (int i = 0; i < alwaysIncludedShaders.arraySize; i++)
                {
                    if (alwaysIncludedShaders.GetArrayElementAtIndex(i).objectReferenceValue == shader)
                    {
                        alreadyIncluded = true;
                        break;
                    }
                }

                // Agregar si no está incluido
                if (!alreadyIncluded)
                {
                    alwaysIncludedShaders.InsertArrayElementAtIndex(alwaysIncludedShaders.arraySize);
                    alwaysIncludedShaders.GetArrayElementAtIndex(alwaysIncludedShaders.arraySize - 1).objectReferenceValue = shader;
                    Debug.Log($"✅ Shader '{shaderName}' agregado a Always Included Shaders");
                    shadersAdded = true;
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ Shader '{shaderName}' no encontrado");
            }
        }

        if (shadersAdded)
        {
            graphicsSettingsObj.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            Debug.Log("✅ Graphics Settings actualizados - Los shaders ahora estarán disponibles en builds");
        }
        else
        {
            Debug.Log("✅ Todos los shaders necesarios ya están incluidos en builds");
        }
    }

    [MenuItem("Tools/Video System/Verify Video System Setup")]
    public static void VerifyVideoSystemSetup()
    {
        Debug.Log("🔍 Verificando configuración del sistema de video...");

        // Verificar shaders
        string[] requiredShaders = new string[] { "Unlit/Texture", "UI/Default", "Standard" };
        int shadersFound = 0;

        foreach (string shaderName in requiredShaders)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                Debug.Log($"✅ Shader '{shaderName}' disponible");
                shadersFound++;
            }
            else
            {
                Debug.LogWarning($"❌ Shader '{shaderName}' NO disponible");
            }
        }

        // Verificar BackgroundVideoSystem
        BackgroundVideoSystem videoSystem = Object.FindFirstObjectByType<BackgroundVideoSystem>();
        if (videoSystem != null)
        {
            Debug.Log("✅ BackgroundVideoSystem encontrado en la escena");
            Debug.Log($"   - Enable Background Video: {videoSystem.enableBackgroundVideo}");
            Debug.Log($"   - Show Debug Info: {videoSystem.showDebugInfo}");
        }
        else
        {
            Debug.LogWarning("⚠️ BackgroundVideoSystem NO encontrado en la escena Gameplay");
        }

        // Verificar videos en StreamingAssets
        string streamingAssetsPath = Application.dataPath + "/StreamingAssets/Songs";
        if (System.IO.Directory.Exists(streamingAssetsPath))
        {
            int videoCount = 0;
            string[] extensions = new string[] { "*.mp4", "*.webm", "*.mov", "*.avi" };
            
            foreach (string ext in extensions)
            {
                videoCount += System.IO.Directory.GetFiles(streamingAssetsPath, ext, System.IO.SearchOption.AllDirectories).Length;
            }

            if (videoCount > 0)
            {
                Debug.Log($"✅ {videoCount} archivos de video encontrados en StreamingAssets/Songs");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontraron archivos de video en StreamingAssets/Songs");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Carpeta StreamingAssets/Songs no existe");
        }

        // Resumen
        Debug.Log("═══════════════════════════════════");
        if (shadersFound >= 1 && videoSystem != null)
        {
            Debug.Log("✅ SISTEMA DE VIDEO CONFIGURADO CORRECTAMENTE");
        }
        else
        {
            Debug.LogWarning("⚠️ SISTEMA DE VIDEO NECESITA CONFIGURACIÓN");
        }
        Debug.Log("═══════════════════════════════════");
    }
}
