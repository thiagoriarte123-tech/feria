using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script to create a record prefab programmatically
/// Use this in the editor to create the prefab automatically
/// </summary>
public class CreateRecordPrefab : MonoBehaviour
{
    [ContextMenu("Create Record Prefab")]
    public void CreatePrefab()
    {
        // Create main GameObject
        GameObject recordPrefab = new GameObject("RecordPrefab");
        
        // Add RectTransform
        RectTransform rectTransform = recordPrefab.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 60);
        
        // Add background Image
        Image backgroundImage = recordPrefab.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Create main text (single text approach)
        GameObject mainTextObj = new GameObject("MainText");
        mainTextObj.transform.SetParent(recordPrefab.transform);
        
        RectTransform mainTextRect = mainTextObj.AddComponent<RectTransform>();
        mainTextRect.anchorMin = Vector2.zero;
        mainTextRect.anchorMax = Vector2.one;
        mainTextRect.offsetMin = new Vector2(10, 5);
        mainTextRect.offsetMax = new Vector2(-10, -5);
        
        Text mainText = mainTextObj.AddComponent<Text>();
        mainText.text = "1. Player: 95000 pts (Combo: 150)";
        mainText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        mainText.fontSize = 16;
        mainText.color = Color.white;
        mainText.alignment = TextAnchor.MiddleLeft;
        
        // Add the prefab setup script
        recordPrefab.AddComponent<RecordEntryPrefabSetup>();
        
        Debug.Log("✅ Record prefab created! Drag it to your Project window to save as prefab.");
        Debug.Log("📍 GameObject created in scene: " + recordPrefab.name);
        
        // Select the created object
        #if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = recordPrefab;
        #endif
    }
    
    [ContextMenu("Create Detailed Record Prefab")]
    public void CreateDetailedPrefab()
    {
        // Create main GameObject
        GameObject recordPrefab = new GameObject("DetailedRecordPrefab");
        
        // Add RectTransform
        RectTransform rectTransform = recordPrefab.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 60);
        
        // Add background Image
        Image backgroundImage = recordPrefab.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Create rank text
        CreateTextChild(recordPrefab, "RankText", "1.", new Vector2(0, 0.5f), new Vector2(0.15f, 0.5f), 
                       new Vector2(10, -15), new Vector2(-10, 15), TextAnchor.MiddleCenter, 18, Color.yellow);
        
        // Create player text
        CreateTextChild(recordPrefab, "PlayerText", "Player Name", new Vector2(0.15f, 0.5f), new Vector2(0.6f, 0.5f), 
                       new Vector2(5, -15), new Vector2(-5, 15), TextAnchor.MiddleLeft, 16, Color.white);
        
        // Create score text
        CreateTextChild(recordPrefab, "ScoreText", "95000", new Vector2(0.6f, 0.5f), new Vector2(0.85f, 0.5f), 
                       new Vector2(5, -15), new Vector2(-5, 15), TextAnchor.MiddleCenter, 16, Color.white);
        
        // Create combo text
        CreateTextChild(recordPrefab, "ComboText", "x150", new Vector2(0.85f, 0.5f), new Vector2(1f, 0.5f), 
                       new Vector2(5, -15), new Vector2(-10, 15), TextAnchor.MiddleCenter, 14, Color.cyan);
        
        // Add the prefab setup script
        recordPrefab.AddComponent<RecordEntryPrefabSetup>();
        
        Debug.Log("✅ Detailed record prefab created! Drag it to your Project window to save as prefab.");
        Debug.Log("📍 GameObject created in scene: " + recordPrefab.name);
        
        // Select the created object
        #if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = recordPrefab;
        #endif
    }
    
    private void CreateTextChild(GameObject parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, 
                                Vector2 offsetMin, Vector2 offsetMax, TextAnchor alignment, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = anchorMin;
        textRect.anchorMax = anchorMax;
        textRect.offsetMin = offsetMin;
        textRect.offsetMax = offsetMax;
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = alignment;
    }
}
