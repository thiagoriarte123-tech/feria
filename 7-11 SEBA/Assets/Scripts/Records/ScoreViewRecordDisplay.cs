using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Displays top 3 records in the ScoreView ScrollView UI
/// Attach this to your ScoreView GameObject or Content panel
/// </summary>
public class ScoreViewRecordDisplay : MonoBehaviour
{
    [Header("ScrollView References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform contentParent; // Content panel of the ScrollView
    [SerializeField] private GameObject recordPrefab; // Prefab for each record entry
    
    [Header("UI References (Optional)")]
    [SerializeField] private Text songNameText; // Optional - can be null
    [SerializeField] private Text noRecordsText; // Optional - can be null
    
    [Header("Alternative: Fixed UI Elements (if not using prefabs)")]
    [SerializeField] private Text[] recordTexts = new Text[3]; // Top 3 records
    [SerializeField] private GameObject[] recordPanels = new GameObject[3]; // Panels for each record
    
    [Header("Record Display Settings")]
    [SerializeField] private string recordFormat = "{0}. {1}: {2} pts (Combo: {3})";
    [SerializeField] private Color[] rankColors = { Color.yellow, Color.white, new Color(0.8f, 0.5f, 0.2f) }; // Gold, Silver, Bronze
    [SerializeField] private bool showPlayerName = true;
    [SerializeField] private bool showCombo = true;
    [SerializeField] private bool autoRefresh = true;
    [SerializeField] private float refreshInterval = 2f;

    [Header("Current Song")]
    [SerializeField] private string currentSongName = "";
    
    [Header("ScrollView Settings")]
    [SerializeField] private bool usePrefabSystem = true; // Use prefabs or fixed UI elements
    [SerializeField] private bool scrollToTop = true; // Scroll to top when refreshing
    [SerializeField] private float recordSpacing = 10f; // Spacing between records

    private float lastRefreshTime;
    private List<GameObject> instantiatedRecords = new List<GameObject>();

    void Start()
    {
        // Initialize UI
        InitializeUI();
        
        // Subscribe to events
        if (GameRecordManager.Instance != null)
        {
            GameRecordManager.OnRecordSaved += OnRecordSaved;
        }
    }

    void Update()
    {
        // Auto refresh records periodically
        if (autoRefresh && Time.time - lastRefreshTime > refreshInterval)
        {
            RefreshRecords();
            lastRefreshTime = Time.time;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (GameRecordManager.Instance != null)
        {
            GameRecordManager.OnRecordSaved -= OnRecordSaved;
        }
    }

    #region Initialization

    /// <summary>
    /// Initialize the UI components
    /// </summary>
    private void InitializeUI()
    {
        // Auto-find ScrollView components if not assigned
        if (scrollRect == null)
        {
            scrollRect = GetComponentInParent<ScrollRect>();
        }
        
        if (contentParent == null && scrollRect != null)
        {
            contentParent = scrollRect.content;
        }
        
        // If still no content parent, use this transform
        if (contentParent == null)
        {
            contentParent = transform;
        }

        // Validate UI references for fixed system
        if (!usePrefabSystem)
        {
            if (recordTexts == null || recordTexts.Length < 3)
            {
                Debug.LogWarning("⚠️ ScoreViewRecordDisplay: recordTexts array must have at least 3 elements");
                recordTexts = new Text[3];
            }

            if (recordPanels == null || recordPanels.Length < 3)
            {
                recordPanels = new GameObject[3];
            }
        }
        else
        {
            // Validate prefab system
            if (recordPrefab == null)
            {
                Debug.LogWarning("⚠️ ScoreViewRecordDisplay: recordPrefab is required when using prefab system");
                usePrefabSystem = false;
            }
        }

        // Set initial state
        ShowNoRecords();
        
        Debug.Log($"📊 ScoreViewRecordDisplay initialized - Using prefab system: {usePrefabSystem}");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set the current song and display its records
    /// </summary>
    /// <param name="songName">Name of the song</param>
    public void SetSong(string songName)
    {
        if (string.IsNullOrEmpty(songName))
        {
            Debug.LogWarning("⚠️ ScoreViewRecordDisplay: Song name is empty");
            return;
        }

        currentSongName = songName;
        
        // Update song name display
        if (songNameText != null)
        {
            songNameText.text = $"Records for: {songName}";
        }

        // Refresh records for this song
        RefreshRecords();
        
        Debug.Log($"📊 ScoreViewRecordDisplay: Set song to {songName}");
    }

    /// <summary>
    /// Refresh the displayed records
    /// </summary>
    public void RefreshRecords()
    {
        if (string.IsNullOrEmpty(currentSongName))
        {
            ShowNoRecords();
            return;
        }

        if (GameRecordManager.Instance == null)
        {
            Debug.LogWarning("⚠️ ScoreViewRecordDisplay: GameRecordManager instance not found");
            ShowNoRecords();
            return;
        }

        // Get top 3 records for current song
        List<RecordEntry> top3Records = GameRecordManager.Instance.GetTop3Records(currentSongName);
        
        if (top3Records == null || top3Records.Count == 0)
        {
            ShowNoRecords();
            return;
        }

        // Display records
        DisplayRecords(top3Records);
    }

    /// <summary>
    /// Clear all displayed records
    /// </summary>
    public void ClearRecords()
    {
        currentSongName = "";
        ShowNoRecords();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Display the list of records
    /// </summary>
    /// <param name="records">List of records to display</param>
    private void DisplayRecords(List<RecordEntry> records)
    {
        // Hide no records message (if exists)
        if (noRecordsText != null)
        {
            noRecordsText.gameObject.SetActive(false);
        }

        if (usePrefabSystem)
        {
            DisplayRecordsWithPrefabs(records);
        }
        else
        {
            DisplayRecordsWithFixedUI(records);
        }

        // Scroll to top if enabled
        if (scrollToTop && scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        Debug.Log($"📊 Displayed {records.Count} records for {currentSongName}");
    }

    /// <summary>
    /// Display records using prefab system (for ScrollView)
    /// </summary>
    /// <param name="records">List of records to display</param>
    private void DisplayRecordsWithPrefabs(List<RecordEntry> records)
    {
        // Clear existing records
        ClearInstantiatedRecords();

        // Create record entries (show top 3)
        int recordsToShow = Mathf.Min(records.Count, 3);
        for (int i = 0; i < recordsToShow; i++)
        {
            CreateRecordPrefab(records[i], i + 1);
        }
    }

    /// <summary>
    /// Display records using fixed UI elements
    /// </summary>
    /// <param name="records">List of records to display</param>
    private void DisplayRecordsWithFixedUI(List<RecordEntry> records)
    {
        // Display each record in fixed UI elements
        for (int i = 0; i < 3; i++)
        {
            if (i < records.Count)
            {
                // Show record
                ShowRecord(i, records[i], i + 1);
            }
            else
            {
                // Hide empty slot
                HideRecord(i);
            }
        }
    }

    /// <summary>
    /// Create a record prefab instance
    /// </summary>
    /// <param name="record">Record data</param>
    /// <param name="rank">Rank number</param>
    private void CreateRecordPrefab(RecordEntry record, int rank)
    {
        if (recordPrefab == null || contentParent == null) return;

        // Instantiate prefab
        GameObject recordObj = Instantiate(recordPrefab, contentParent);
        instantiatedRecords.Add(recordObj);

        // Find text components in the prefab
        Text[] texts = recordObj.GetComponentsInChildren<Text>();
        
        if (texts.Length > 0)
        {
            // Set main text (assuming first Text component is the main one)
            string recordText = FormatRecordText(record, rank);
            texts[0].text = recordText;
            
            // Set rank color
            if (rank - 1 < rankColors.Length)
            {
                texts[0].color = rankColors[rank - 1];
            }
        }

        // Try to find specific text components by name
        Transform rankText = recordObj.transform.Find("RankText");
        Transform playerText = recordObj.transform.Find("PlayerText");
        Transform scoreText = recordObj.transform.Find("ScoreText");
        Transform comboText = recordObj.transform.Find("ComboText");

        if (rankText != null)
        {
            Text rankComponent = rankText.GetComponent<Text>();
            if (rankComponent != null)
            {
                rankComponent.text = $"{rank}.";
                if (rank - 1 < rankColors.Length)
                {
                    rankComponent.color = rankColors[rank - 1];
                }
            }
        }

        if (playerText != null)
        {
            Text playerComponent = playerText.GetComponent<Text>();
            if (playerComponent != null)
            {
                playerComponent.text = record.PlayerName;
            }
        }

        if (scoreText != null)
        {
            Text scoreComponent = scoreText.GetComponent<Text>();
            if (scoreComponent != null)
            {
                scoreComponent.text = $"{record.Score} pts";
            }
        }

        if (comboText != null)
        {
            Text comboComponent = comboText.GetComponent<Text>();
            if (comboComponent != null)
            {
                comboComponent.text = $"Combo: {record.Combo}";
            }
        }
    }

    /// <summary>
    /// Clear all instantiated record objects
    /// </summary>
    private void ClearInstantiatedRecords()
    {
        foreach (GameObject record in instantiatedRecords)
        {
            if (record != null)
            {
                DestroyImmediate(record);
            }
        }
        instantiatedRecords.Clear();
    }

    /// <summary>
    /// Show a specific record in the UI
    /// </summary>
    /// <param name="index">Index of the record slot (0-2)</param>
    /// <param name="record">Record to display</param>
    /// <param name="rank">Rank number (1-3)</param>
    private void ShowRecord(int index, RecordEntry record, int rank)
    {
        // Show record panel
        if (recordPanels[index] != null)
        {
            recordPanels[index].SetActive(true);
        }

        // Set record text
        if (recordTexts[index] != null)
        {
            recordTexts[index].gameObject.SetActive(true);
            
            // Format record text
            string recordText = FormatRecordText(record, rank);
            recordTexts[index].text = recordText;
            
            // Set rank color
            if (index < rankColors.Length)
            {
                recordTexts[index].color = rankColors[index];
            }
        }
    }

    /// <summary>
    /// Hide a specific record slot
    /// </summary>
    /// <param name="index">Index of the record slot to hide</param>
    private void HideRecord(int index)
    {
        if (recordPanels[index] != null)
        {
            recordPanels[index].SetActive(false);
        }

        if (recordTexts[index] != null)
        {
            recordTexts[index].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Show "no records" message
    /// </summary>
    private void ShowNoRecords()
    {
        if (usePrefabSystem)
        {
            // Clear instantiated records
            ClearInstantiatedRecords();
        }
        else
        {
            // Hide all fixed record slots
            for (int i = 0; i < 3; i++)
            {
                HideRecord(i);
            }
        }

        // Show no records message (if text component exists)
        if (noRecordsText != null)
        {
            noRecordsText.gameObject.SetActive(true);
            noRecordsText.text = string.IsNullOrEmpty(currentSongName) ? 
                "Select a song to view records" : 
                $"No records found for {currentSongName}";
        }
        else
        {
            // If no noRecordsText, just log the message
            string message = string.IsNullOrEmpty(currentSongName) ? 
                "No song selected" : 
                $"No records found for {currentSongName}";
            Debug.Log($"📊 {message}");
        }

        // Update song name (if text component exists)
        if (songNameText != null)
        {
            songNameText.text = string.IsNullOrEmpty(currentSongName) ? 
                "Song Records" : 
                $"Records for: {currentSongName}";
        }
    }

    /// <summary>
    /// Format a record for display
    /// </summary>
    /// <param name="record">Record to format</param>
    /// <param name="rank">Rank number</param>
    /// <returns>Formatted record string</returns>
    private string FormatRecordText(RecordEntry record, int rank)
    {
        if (showPlayerName && showCombo)
        {
            return string.Format(recordFormat, rank, record.PlayerName, record.Score, record.Combo);
        }
        else if (showPlayerName)
        {
            return $"{rank}. {record.PlayerName}: {record.Score} pts";
        }
        else if (showCombo)
        {
            return $"{rank}. {record.Score} pts (Combo: {record.Combo})";
        }
        else
        {
            return $"{rank}. {record.Score} pts";
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Called when a new record is saved
    /// </summary>
    /// <param name="songName">Name of the song</param>
    /// <param name="score">Score achieved</param>
    /// <param name="combo">Combo achieved</param>
    private void OnRecordSaved(string songName, int score, int combo)
    {
        // Refresh if this is the current song
        if (songName == currentSongName)
        {
            RefreshRecords();
            Debug.Log($"📊 Refreshed records for {songName} after new record saved");
        }
    }

    #endregion

    #region Integration Methods

    /// <summary>
    /// Call this method when a song is selected in the main menu
    /// </summary>
    /// <param name="songData">Selected song data</param>
    public void OnSongSelected(SongData songData)
    {
        if (songData != null)
        {
            SetSong(songData.songName);
        }
    }

    /// <summary>
    /// Call this method when a song is selected by name
    /// </summary>
    /// <param name="songName">Name of the selected song</param>
    public void OnSongSelectedByName(string songName)
    {
        SetSong(songName);
    }

    #endregion

    #region Unity Editor

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    [ContextMenu("Test Display")]
    private void TestDisplay()
    {
        if (GameRecordManager.Instance != null)
        {
            GameRecordManager.Instance.AddTestRecords();
            SetSong("Test Song 1");
        }
    }

    [ContextMenu("Refresh Now")]
    private void ForceRefresh()
    {
        RefreshRecords();
    }

    void OnValidate()
    {
        if (debugMode && Application.isPlaying)
        {
            RefreshRecords();
        }
    }
#endif

    #endregion
}
