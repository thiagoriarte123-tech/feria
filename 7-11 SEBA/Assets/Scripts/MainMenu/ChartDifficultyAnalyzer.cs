using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Analyzes .chart files to detect available difficulties
/// </summary>
public static class ChartDifficultyAnalyzer
{
    /// <summary>
    /// Represents the available difficulties in a chart file
    /// </summary>
    public class DifficultyInfo
    {
        public bool hasEasy = false;
        public bool hasMedium = false;
        public bool hasHard = false;
        public bool hasExpert = false;
        
        // Spanish equivalents for the UI
        public bool hasFacil => hasEasy;
        public bool hasDificil => hasHard || hasExpert;
        
        public override string ToString()
        {
            var difficulties = new List<string>();
            if (hasEasy) difficulties.Add("Easy");
            if (hasMedium) difficulties.Add("Medium");
            if (hasHard) difficulties.Add("Hard");
            if (hasExpert) difficulties.Add("Expert");
            return string.Join(", ", difficulties);
        }
    }
    
    /// <summary>
    /// Analyzes a .chart file and returns available difficulties
    /// </summary>
    /// <param name="chartPath">Path to the .chart file</param>
    /// <returns>DifficultyInfo containing available difficulties</returns>
    public static DifficultyInfo AnalyzeChart(string chartPath)
    {
        var difficultyInfo = new DifficultyInfo();
        
        if (!File.Exists(chartPath))
        {
            Debug.LogWarning($"⚠️ Chart file not found: {chartPath}");
            return difficultyInfo;
        }
        
        try
        {
            string[] lines = File.ReadAllLines(chartPath);
            
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                
                // Check for difficulty sections
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    string sectionName = trimmedLine.Substring(1, trimmedLine.Length - 2).ToLower();
                    
                    switch (sectionName)
                    {
                        case "easysingle":
                        case "easy":
                            difficultyInfo.hasEasy = true;
                            break;
                            
                        case "mediumsingle":
                        case "medium":
                            difficultyInfo.hasMedium = true;
                            break;
                            
                        case "hardsingle":
                        case "hard":
                            difficultyInfo.hasHard = true;
                            break;
                            
                        case "expertsingle":
                        case "expert":
                            difficultyInfo.hasExpert = true;
                            break;
                    }
                }
            }
            
            Debug.Log($"📊 Chart analysis for {Path.GetFileName(chartPath)}: {difficultyInfo}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error analyzing chart file {chartPath}: {e.Message}");
        }
        
        return difficultyInfo;
    }
    
    /// <summary>
    /// Analyzes a song folder and returns available difficulties
    /// </summary>
    /// <param name="songFolderPath">Path to the song folder</param>
    /// <returns>DifficultyInfo containing available difficulties</returns>
    public static DifficultyInfo AnalyzeSongFolder(string songFolderPath)
    {
        string chartPath = Path.Combine(songFolderPath, "notes.chart");
        return AnalyzeChart(chartPath);
    }
    
    /// <summary>
    /// Gets the full path to a song folder from a song name
    /// </summary>
    /// <param name="songName">Name of the song folder</param>
    /// <returns>Full path to the song folder</returns>
    public static string GetSongFolderPath(string songName)
    {
        return Path.Combine(Application.streamingAssetsPath, "Songs", songName);
    }
    
    /// <summary>
    /// Analyzes a song by name and returns available difficulties
    /// </summary>
    /// <param name="songName">Name of the song folder</param>
    /// <returns>DifficultyInfo containing available difficulties</returns>
    public static DifficultyInfo AnalyzeSongByName(string songName)
    {
        string songFolderPath = GetSongFolderPath(songName);
        return AnalyzeSongFolder(songFolderPath);
    }
}
