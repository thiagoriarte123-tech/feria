using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages game records integration with the RecordFileManager
/// This script should be attached to a GameObject in your game scene
/// </summary>
public class GameRecordManager : MonoBehaviour
{
    [Header("Record Settings")]
    [SerializeField] private string currentPlayerName = "Player";
    [SerializeField] private bool autoSaveRecords = true;
    [SerializeField] private bool debugMode = true;

    [Header("Current Game Data")]
    [SerializeField] private string currentSongName = "";
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int currentCombo = 0;

    // Events
    public static System.Action<string, int, int> OnRecordSaved;
    public static System.Action<string> OnPlayerNameChanged;

    // Singleton pattern
    public static GameRecordManager Instance { get; private set; }

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeRecordSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Load current player name
        LoadCurrentPlayer();
    }

    #region Initialization

    /// <summary>
    /// Initialize the record system
    /// </summary>
    private void InitializeRecordSystem()
    {
        RecordFileManager.Initialize();
        
        if (debugMode)
        {
            Debug.Log($"🎮 GameRecordManager initialized");
            Debug.Log($"📁 Records folder: {RecordFileManager.GetRecordsFolderPath()}");
        }
    }

    #endregion

    #region Player Management

    /// <summary>
    /// Set the current player name
    /// </summary>
    /// <param name="playerName">Name of the player</param>
    public void SetPlayerName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player";
        }

        currentPlayerName = playerName;
        RecordFileManager.SaveCurrentPlayer(playerName);
        
        OnPlayerNameChanged?.Invoke(playerName);
        
        if (debugMode)
        {
            Debug.Log($"👤 Player name set to: {playerName}");
        }
    }

    /// <summary>
    /// Load the current player name from file
    /// </summary>
    public void LoadCurrentPlayer()
    {
        currentPlayerName = RecordFileManager.GetCurrentPlayer();
        
        if (debugMode)
        {
            Debug.Log($"👤 Loaded player name: {currentPlayerName}");
        }
    }

    /// <summary>
    /// Get the current player name
    /// </summary>
    /// <returns>Current player name</returns>
    public string GetCurrentPlayerName()
    {
        return currentPlayerName;
    }

    #endregion

    #region Game Data Management

    /// <summary>
    /// Update current game data (called during gameplay)
    /// </summary>
    /// <param name="songName">Name of the current song</param>
    /// <param name="score">Current score</param>
    /// <param name="combo">Current combo</param>
    public void UpdateCurrentGame(string songName, int score, int combo)
    {
        currentSongName = songName;
        currentScore = score;
        currentCombo = combo;

        // Save to current_game.txt (overwrites previous data)
        RecordFileManager.SaveCurrentGame(songName, score, combo);

        if (debugMode)
        {
            Debug.Log($"🎮 Updated current game - Song: {songName}, Score: {score}, Combo: {combo}");
        }
    }

    /// <summary>
    /// Save the current game as a permanent record
    /// </summary>
    public void SaveCurrentGameAsRecord()
    {
        if (string.IsNullOrEmpty(currentSongName))
        {
            Debug.LogWarning("⚠️ Cannot save record: No current song");
            return;
        }

        SaveRecord(currentSongName, currentPlayerName, currentScore, currentCombo);
    }

    /// <summary>
    /// Save a record with specific data
    /// </summary>
    /// <param name="songName">Name of the song</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="score">Score achieved</param>
    /// <param name="combo">Combo achieved</param>
    public void SaveRecord(string songName, string playerName, int score, int combo)
    {
        // Add to all_records.txt
        RecordFileManager.AddRecord(songName, playerName, score, combo);
        
        // Update current game data
        UpdateCurrentGame(songName, score, combo);
        
        OnRecordSaved?.Invoke(songName, score, combo);
        
        if (debugMode)
        {
            Debug.Log($"💾 Saved record - Song: {songName}, Player: {playerName}, Score: {score}, Combo: {combo}");
        }
    }

    #endregion

    #region Record Retrieval

    /// <summary>
    /// Get top 3 records for a specific song
    /// </summary>
    /// <param name="songName">Name of the song</param>
    /// <returns>List of top 3 records</returns>
    public List<RecordEntry> GetTop3Records(string songName)
    {
        return RecordFileManager.GetTop3RecordsForSong(songName);
    }

    /// <summary>
    /// Get all records for a specific song
    /// </summary>
    /// <param name="songName">Name of the song</param>
    /// <returns>List of all records for the song</returns>
    public List<RecordEntry> GetAllRecordsForSong(string songName)
    {
        return RecordFileManager.GetRecordsForSong(songName);
    }

    /// <summary>
    /// Get all records from all songs
    /// </summary>
    /// <returns>Dictionary with song names and their records</returns>
    public Dictionary<string, List<RecordEntry>> GetAllRecords()
    {
        return RecordFileManager.GetAllRecords();
    }

    /// <summary>
    /// Get current game data
    /// </summary>
    /// <returns>Dictionary with current game info</returns>
    public Dictionary<string, string> GetCurrentGameData()
    {
        return RecordFileManager.GetCurrentGame();
    }

    #endregion

    #region Public API Methods

    /// <summary>
    /// Called when a song starts
    /// </summary>
    /// <param name="songName">Name of the song</param>
    public void OnSongStart(string songName)
    {
        currentSongName = songName;
        currentScore = 0;
        currentCombo = 0;
        
        UpdateCurrentGame(songName, 0, 0);
        
        if (debugMode)
        {
            Debug.Log($"🎵 Song started: {songName}");
        }
    }

    /// <summary>
    /// Called when a song ends
    /// </summary>
    /// <param name="finalScore">Final score</param>
    /// <param name="maxCombo">Maximum combo achieved</param>
    public void OnSongEnd(int finalScore, int maxCombo)
    {
        currentScore = finalScore;
        currentCombo = maxCombo;
        
        if (autoSaveRecords)
        {
            SaveCurrentGameAsRecord();
        }
        
        if (debugMode)
        {
            Debug.Log($"🏁 Song ended - Final Score: {finalScore}, Max Combo: {maxCombo}");
        }
    }

    /// <summary>
    /// Called during gameplay to update score and combo
    /// </summary>
    /// <param name="score">Current score</param>
    /// <param name="combo">Current combo</param>
    public void OnScoreUpdate(int score, int combo)
    {
        currentScore = score;
        currentCombo = combo;
        
        // Update current game file less frequently to avoid performance issues
        if (Time.frameCount % 60 == 0) // Update every 60 frames (about once per second at 60 FPS)
        {
            UpdateCurrentGame(currentSongName, score, combo);
        }
    }

    #endregion

    #region Debug Methods

    /// <summary>
    /// Add some test records (for testing purposes)
    /// </summary>
    [ContextMenu("Add Test Records")]
    public void AddTestRecords()
    {
        SaveRecord("Test Song 1", "Player1", 95000, 150);
        SaveRecord("Test Song 1", "Player2", 87000, 120);
        SaveRecord("Test Song 1", "Player3", 92000, 140);
        SaveRecord("Test Song 2", "Player1", 78000, 100);
        SaveRecord("Test Song 2", "Player2", 85000, 110);
        
        Debug.Log("✅ Added test records");
    }

    /// <summary>
    /// Clear all records (for testing purposes)
    /// </summary>
    [ContextMenu("Clear All Records")]
    public void ClearAllRecords()
    {
        RecordFileManager.ClearAllRecords();
        Debug.Log("🗑️ Cleared all records");
    }

    /// <summary>
    /// Print current records to console
    /// </summary>
    [ContextMenu("Print All Records")]
    public void PrintAllRecords()
    {
        var allRecords = GetAllRecords();
        
        Debug.Log("📊 === ALL RECORDS ===");
        foreach (var songRecords in allRecords)
        {
            Debug.Log($"🎵 {songRecords.Key}:");
            for (int i = 0; i < songRecords.Value.Count && i < 3; i++)
            {
                var record = songRecords.Value[i];
                Debug.Log($"  {i + 1}. {record.PlayerName}: {record.Score} pts (Combo: {record.Combo})");
            }
        }
    }

    #endregion

    #region Unity Editor

#if UNITY_EDITOR
    [Header("Debug Info (Read Only)")]
    [SerializeField] private string debugCurrentPlayer;
    [SerializeField] private string debugCurrentSong;
    [SerializeField] private int debugCurrentScore;
    [SerializeField] private int debugCurrentCombo;

    void Update()
    {
        if (debugMode)
        {
            debugCurrentPlayer = currentPlayerName;
            debugCurrentSong = currentSongName;
            debugCurrentScore = currentScore;
            debugCurrentCombo = currentCombo;
        }
    }
#endif

    #endregion
}
