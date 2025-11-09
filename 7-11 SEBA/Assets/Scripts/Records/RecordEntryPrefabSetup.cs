using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper script to set up a record entry prefab for ScrollView
/// Attach this to your record prefab to help with setup
/// </summary>
public class RecordEntryPrefabSetup : MonoBehaviour
{
    [Header("Text Components (Optional - will be auto-found)")]
    public Text rankText;
    public Text playerText;
    public Text scoreText;
    public Text comboText;
    public Text mainText; // Single text for all info

    [Header("Visual Settings")]
    public Image backgroundImage;
    public Color[] rankBackgroundColors = { 
        new Color(1f, 0.84f, 0f, 0.3f), // Gold
        new Color(0.75f, 0.75f, 0.75f, 0.3f), // Silver
        new Color(0.8f, 0.5f, 0.2f, 0.3f) // Bronze
    };

    void Awake()
    {
        // Auto-find components if not assigned
        if (rankText == null)
            rankText = transform.Find("RankText")?.GetComponent<Text>();
        
        if (playerText == null)
            playerText = transform.Find("PlayerText")?.GetComponent<Text>();
        
        if (scoreText == null)
            scoreText = transform.Find("ScoreText")?.GetComponent<Text>();
        
        if (comboText == null)
            comboText = transform.Find("ComboText")?.GetComponent<Text>();
        
        if (mainText == null)
            mainText = GetComponentInChildren<Text>();
        
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
    }

    /// <summary>
    /// Set up the record entry with data
    /// </summary>
    /// <param name="record">Record data</param>
    /// <param name="rank">Rank (1-3)</param>
    public void SetupRecord(RecordEntry record, int rank)
    {
        // Set individual text components
        if (rankText != null)
            rankText.text = $"{rank}.";
        
        if (playerText != null)
            playerText.text = record.PlayerName;
        
        if (scoreText != null)
            scoreText.text = $"{record.Score:N0}";
        
        if (comboText != null)
            comboText.text = $"x{record.Combo}";

        // Set main text if using single text component
        if (mainText != null)
        {
            mainText.text = $"{rank}. {record.PlayerName}: {record.Score:N0} pts (Combo: {record.Combo})";
        }

        // Set background color based on rank
        if (backgroundImage != null && rank <= rankBackgroundColors.Length)
        {
            backgroundImage.color = rankBackgroundColors[rank - 1];
        }

        // Set text colors based on rank
        Color textColor = GetRankColor(rank);
        
        if (rankText != null) rankText.color = textColor;
        if (playerText != null) playerText.color = textColor;
        if (scoreText != null) scoreText.color = textColor;
        if (comboText != null) comboText.color = textColor;
        if (mainText != null) mainText.color = textColor;
    }

    /// <summary>
    /// Get color for rank
    /// </summary>
    /// <param name="rank">Rank number</param>
    /// <returns>Color for the rank</returns>
    private Color GetRankColor(int rank)
    {
        switch (rank)
        {
            case 1: return Color.yellow; // Gold
            case 2: return Color.white;  // Silver
            case 3: return new Color(0.8f, 0.5f, 0.2f); // Bronze
            default: return Color.white;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Setup Prefab Structure")]
    void SetupPrefabStructure()
    {
        // This method helps set up the prefab structure in the editor
        Debug.Log("Setting up record prefab structure...");
        
        // Create child objects if they don't exist
        CreateChildIfNotExists("RankText");
        CreateChildIfNotExists("PlayerText");
        CreateChildIfNotExists("ScoreText");
        CreateChildIfNotExists("ComboText");
        
        Debug.Log("Prefab structure setup complete!");
    }

    void CreateChildIfNotExists(string childName)
    {
        if (transform.Find(childName) == null)
        {
            GameObject child = new GameObject(childName);
            child.transform.SetParent(transform);
            child.AddComponent<Text>();
            Debug.Log($"Created {childName}");
        }
    }
#endif
}
