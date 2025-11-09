using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages three text files for record keeping:
/// 1. current_player.txt - Current player name
/// 2. current_game.txt - Current song score and combo (overwritten each song)
/// 3. all_records.txt - Complete history of all records by song
/// </summary>
public static class RecordFileManager
{
    // File paths
    private static readonly string RecordsFolder = Path.Combine(Application.persistentDataPath, "Records");
    private static readonly string CurrentPlayerFile = Path.Combine(RecordsFolder, "current_player.txt");
    private static readonly string CurrentGameFile = Path.Combine(RecordsFolder, "current_game.txt");
    private static readonly string AllRecordsFile = Path.Combine(RecordsFolder, "all_records.txt");

    /// <summary>
    /// Initialize the records system - create folder and files if they don't exist
    /// </summary>
    public static void Initialize()
    {
        try
        {
            // Create records folder if it doesn't exist
            if (!Directory.Exists(RecordsFolder))
            {
                Directory.CreateDirectory(RecordsFolder);
                Debug.Log($"📁 Created Records folder: {RecordsFolder}");
            }

            // Create files if they don't exist
            if (!File.Exists(CurrentPlayerFile))
            {
                File.WriteAllText(CurrentPlayerFile, "Player");
                Debug.Log($"📄 Created current_player.txt");
            }

            if (!File.Exists(CurrentGameFile))
            {
                File.WriteAllText(CurrentGameFile, "Song: None\nScore: 0\nCombo: 0");
                Debug.Log($"📄 Created current_game.txt");
            }

            if (!File.Exists(AllRecordsFile))
            {
                File.WriteAllText(AllRecordsFile, "# Records History\n# Format: [SONG_NAME] Player: PLAYER_NAME | Score: SCORE | Combo: COMBO | Date: DATE\n\n");
                Debug.Log($"📄 Created all_records.txt");
            }

            Debug.Log($"✅ RecordFileManager initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error initializing RecordFileManager: {e.Message}");
        }
    }

    #region Current Player Management

    /// <summary>
    /// Save the current player name
    /// </summary>
    /// <param name="playerName">Name of the player</param>
    public static void SaveCurrentPlayer(string playerName)
    {
        try
        {
            File.WriteAllText(CurrentPlayerFile, playerName);
            Debug.Log($"👤 Saved current player: {playerName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error saving current player: {e.Message}");
        }
    }

    /// <summary>
    /// Get the current player name
    /// </summary>
    /// <returns>Current player name</returns>
    public static string GetCurrentPlayer()
    {
        try
        {
            if (File.Exists(CurrentPlayerFile))
            {
                string playerName = File.ReadAllText(CurrentPlayerFile).Trim();
                return string.IsNullOrEmpty(playerName) ? "Player" : playerName;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error reading current player: {e.Message}");
        }
        return "Player";
    }

    #endregion

    #region Current Game Management

    /// <summary>
    /// Save current game data (overwrites previous data)
    /// </summary>
    /// <param name="songName">Name of the current song</param>
    /// <param name="score">Current score</param>
    /// <param name="combo">Current combo</param>
    public static void SaveCurrentGame(string songName, int score, int combo)
    {
        try
        {
            string gameData = $"Song: {songName}\nScore: {score}\nCombo: {combo}\nUpdated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            File.WriteAllText(CurrentGameFile, gameData);
            Debug.Log($"🎮 Saved current game - Song: {songName}, Score: {score}, Combo: {combo}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error saving current game: {e.Message}");
        }
    }

    /// <summary>
    /// Get current game data
    /// </summary>
    /// <returns>Dictionary with current game info</returns>
    public static Dictionary<string, string> GetCurrentGame()
    {
        var gameData = new Dictionary<string, string>
        {
            ["Song"] = "None",
            ["Score"] = "0",
            ["Combo"] = "0",
            ["Updated"] = "Never"
        };

        try
        {
            if (File.Exists(CurrentGameFile))
            {
                string[] lines = File.ReadAllLines(CurrentGameFile);
                foreach (string line in lines)
                {
                    if (line.Contains(":"))
                    {
                        string[] parts = line.Split(new char[] { ':' }, 2);
                        if (parts.Length == 2)
                        {
                            gameData[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error reading current game: {e.Message}");
        }

        return gameData;
    }

    #endregion

    #region All Records Management

    /// <summary>
    /// Add a new record to the complete history
    /// </summary>
    /// <param name="songName">Name of the song</param>
    /// <param name="playerName">Name of the player</param>
    /// <param name="score">Score achieved</param>
    /// <param name="combo">Combo achieved</param>
    public static void AddRecord(string songName, string playerName, int score, int combo)
    {
        try
        {
            string recordLine = $"[{songName}] Player: {playerName} | Score: {score} | Combo: {combo} | Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            File.AppendAllText(AllRecordsFile, recordLine + "\n");
            Debug.Log($"📊 Added record - {recordLine}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error adding record: {e.Message}");
        }
    }

    /// <summary>
    /// Get all records for a specific song
    /// </summary>
    /// <param name="songName">Name of the song</param>
    /// <returns>List of records for the song</returns>
    public static List<RecordEntry> GetRecordsForSong(string songName)
    {
        var records = new List<RecordEntry>();

        try
        {
            if (File.Exists(AllRecordsFile))
            {
                string[] lines = File.ReadAllLines(AllRecordsFile);
                foreach (string line in lines)
                {
                    if (line.StartsWith($"[{songName}]"))
                    {
                        var record = ParseRecordLine(line);
                        if (record != null)
                        {
                            records.Add(record);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error reading records for song {songName}: {e.Message}");
        }

        // Sort by score (highest first), then by combo (highest first)
        return records.OrderByDescending(r => r.Score).ThenByDescending(r => r.Combo).ToList();
    }

    /// <summary>
    /// Get top 3 records for a specific song
    /// </summary>
    /// <param name="songName">Name of the song</param>
    /// <returns>Top 3 records for the song</returns>
    public static List<RecordEntry> GetTop3RecordsForSong(string songName)
    {
        return GetRecordsForSong(songName).Take(3).ToList();
    }

    /// <summary>
    /// Get all records from all songs
    /// </summary>
    /// <returns>Dictionary with song names as keys and their records as values</returns>
    public static Dictionary<string, List<RecordEntry>> GetAllRecords()
    {
        var allRecords = new Dictionary<string, List<RecordEntry>>();

        try
        {
            if (File.Exists(AllRecordsFile))
            {
                string[] lines = File.ReadAllLines(AllRecordsFile);
                foreach (string line in lines)
                {
                    if (line.StartsWith("[") && line.Contains("]"))
                    {
                        var record = ParseRecordLine(line);
                        if (record != null)
                        {
                            if (!allRecords.ContainsKey(record.SongName))
                            {
                                allRecords[record.SongName] = new List<RecordEntry>();
                            }
                            allRecords[record.SongName].Add(record);
                        }
                    }
                }

                // Sort each song's records
                foreach (var songRecords in allRecords.Values)
                {
                    songRecords.Sort((a, b) => 
                    {
                        int scoreComparison = b.Score.CompareTo(a.Score);
                        return scoreComparison != 0 ? scoreComparison : b.Combo.CompareTo(a.Combo);
                    });
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error reading all records: {e.Message}");
        }

        return allRecords;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Parse a record line from the all_records.txt file
    /// </summary>
    /// <param name="line">Line to parse</param>
    /// <returns>RecordEntry or null if parsing failed</returns>
    private static RecordEntry ParseRecordLine(string line)
    {
        try
        {
            // Format: [SONG_NAME] Player: PLAYER_NAME | Score: SCORE | Combo: COMBO | Date: DATE
            if (!line.StartsWith("[") || !line.Contains("]"))
                return null;

            int songEndIndex = line.IndexOf("]");
            string songName = line.Substring(1, songEndIndex - 1);

            string dataSection = line.Substring(songEndIndex + 1);
            string[] parts = dataSection.Split('|');

            if (parts.Length < 3)
                return null;

            string playerName = ExtractValue(parts[0], "Player:");
            int score = int.Parse(ExtractValue(parts[1], "Score:"));
            int combo = int.Parse(ExtractValue(parts[2], "Combo:"));
            
            string dateStr = parts.Length > 3 ? ExtractValue(parts[3], "Date:") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime.TryParse(dateStr, out DateTime date);

            return new RecordEntry
            {
                SongName = songName,
                PlayerName = playerName,
                Score = score,
                Combo = combo,
                Date = date
            };
        }
        catch (Exception e)
        {
            Debug.LogWarning($"⚠️ Failed to parse record line: {line} - {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Extract value from a key:value pair
    /// </summary>
    /// <param name="part">String containing key:value</param>
    /// <param name="key">Key to look for</param>
    /// <returns>Value part</returns>
    private static string ExtractValue(string part, string key)
    {
        int keyIndex = part.IndexOf(key);
        if (keyIndex >= 0)
        {
            return part.Substring(keyIndex + key.Length).Trim();
        }
        return "";
    }

    /// <summary>
    /// Get the records folder path
    /// </summary>
    /// <returns>Path to records folder</returns>
    public static string GetRecordsFolderPath()
    {
        return RecordsFolder;
    }

    /// <summary>
    /// Clear all records (for testing purposes)
    /// </summary>
    public static void ClearAllRecords()
    {
        try
        {
            if (File.Exists(AllRecordsFile))
            {
                File.WriteAllText(AllRecordsFile, "# Records History\n# Format: [SONG_NAME] Player: PLAYER_NAME | Score: SCORE | Combo: COMBO | Date: DATE\n\n");
                Debug.Log("🗑️ Cleared all records");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error clearing records: {e.Message}");
        }
    }

    #endregion
}

/// <summary>
/// Represents a single record entry
/// </summary>
[System.Serializable]
public class RecordEntry
{
    public string SongName;
    public string PlayerName;
    public int Score;
    public int Combo;
    public DateTime Date;

    public override string ToString()
    {
        return $"{PlayerName}: {Score} pts (Combo: {Combo})";
    }
}
