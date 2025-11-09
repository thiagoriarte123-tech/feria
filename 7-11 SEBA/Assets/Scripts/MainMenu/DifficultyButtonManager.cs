using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the FACIL and DIFICIL difficulty buttons in the main menu
/// Ensures only one can be selected at a time and they start unselected
/// </summary>
public class DifficultyButtonManager : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    public Button facilButton;
    public Button dificilButton;
    
    [Header("Visual Settings")]
    [SerializeField] private Color selectedColor = new Color(0f, 0.8f, 0f, 1f); // Verde brillante
    [SerializeField] private Color unselectedColor = new Color(0.9f, 0.9f, 0.9f, 1f); // Blanco grisáceo
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gris para botones deshabilitados
    [SerializeField] private Color hoverColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Gris claro
    [SerializeField] private Color playButtonEnabledColor = new Color(0f, 0.8f, 0f, 1f); // Verde para JUGAR
    [SerializeField] private Color playButtonDisabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gris para JUGAR
    
    [Header("Play Button")]
    public Button playButton;
    
    private string selectedDifficulty = "";
    private bool isDifficultySelected = false;
    private ChartDifficultyAnalyzer.DifficultyInfo currentSongDifficulties;
    private bool facilAvailable = true;
    private bool dificilAvailable = true;
    
    void Start()
    {
        InitializeButtons();
        SetupButtonListeners();
        
        // Ensure play button starts in correct state (gray, since no difficulty selected initially)
        UpdatePlayButtonState();
        
        Debug.Log("🎯 DifficultyButtonManager initialized - Play button should be gray until both song and difficulty are selected");
    }
    
    void LateUpdate()
    {
        // Continuously enforce correct play button state to override any other scripts
        if (playButton != null)
        {
            bool songSelected = GameManager.Instance != null && GameManager.Instance.songSelected;
            bool difficultySelected = isDifficultySelected;
            bool shouldBeEnabled = songSelected && difficultySelected;
            
            // If the button state doesn't match what it should be, fix it
            if (playButton.interactable != shouldBeEnabled)
            {
                UpdatePlayButtonState();
                Debug.Log($"🔧 Play button state corrected in LateUpdate - Should be enabled: {shouldBeEnabled}");
            }
        }
    }
    
    void InitializeButtons()
    {
        // Set both buttons to unselected state initially
        UpdateButtonState(facilButton, true, false);
        UpdateButtonState(dificilButton, true, false);
        
        Debug.Log("🎯 Difficulty buttons initialized - both unselected and enabled");
    }
    
    void SetupButtonListeners()
    {
        if (facilButton != null)
        {
            facilButton.onClick.AddListener(() => SelectDifficulty("Facil", facilButton));
        }
        
        if (dificilButton != null)
        {
            dificilButton.onClick.AddListener(() => SelectDifficulty("Dificil", dificilButton));
        }
    }
    
    void SelectDifficulty(string difficulty, Button selectedButton)
    {
        // Check if the difficulty is available for the current song
        if (difficulty == "Facil" && !facilAvailable)
        {
            Debug.LogWarning("⚠️ Cannot select Facil - not available for current song");
            return;
        }
        
        if (difficulty == "Dificil" && !dificilAvailable)
        {
            Debug.LogWarning("⚠️ Cannot select Dificil - not available for current song");
            return;
        }
        
        // Update internal state
        selectedDifficulty = difficulty;
        isDifficultySelected = true;
        
        // Update visual state - set selected button to green, other to appropriate state
        if (difficulty == "Facil")
        {
            UpdateButtonState(facilButton, facilAvailable, true);
            UpdateButtonState(dificilButton, dificilAvailable, false);
        }
        else if (difficulty == "Dificil")
        {
            UpdateButtonState(dificilButton, dificilAvailable, true);
            UpdateButtonState(facilButton, facilAvailable, false);
        }
        
        // Update GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectDifficulty(difficulty);
        }
        
        // Update play button state
        UpdatePlayButtonState();
        
        Debug.Log($"🎯 Difficulty selected: {difficulty}");
    }
    
    void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;
        
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, hoverColor, 0.3f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.2f);
        colors.selectedColor = color;
        button.colors = colors;
    }
    
    void UpdatePlayButtonState()
    {
        if (playButton == null) return;
        
        // CRITICAL: Both conditions must be true - song selected AND difficulty selected
        bool songSelected = GameManager.Instance != null && GameManager.Instance.songSelected;
        bool difficultySelected = isDifficultySelected;
        bool canPlay = songSelected && difficultySelected;
        
        playButton.interactable = canPlay;
        
        // Visual feedback for play button
        ColorBlock playColors = playButton.colors;
        if (canPlay)
        {
            // Only green when BOTH song and difficulty are selected
            playColors.normalColor = playButtonEnabledColor;
            playColors.highlightedColor = Color.Lerp(playButtonEnabledColor, Color.white, 0.3f);
            playColors.pressedColor = Color.Lerp(playButtonEnabledColor, Color.black, 0.2f);
            playColors.selectedColor = playButtonEnabledColor;
        }
        else
        {
            // Gray when either song OR difficulty is missing
            playColors.normalColor = playButtonDisabledColor;
            playColors.highlightedColor = playButtonDisabledColor;
            playColors.pressedColor = playButtonDisabledColor;
            playColors.selectedColor = playButtonDisabledColor;
        }
        playButton.colors = playColors;
        
        // Debug information
        Debug.Log($"🎮 Play Button State - Song: {songSelected}, Difficulty: {difficultySelected}, Can Play: {canPlay}");
    }
    
    // Public methods for external access
    public string GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }
    
    public bool IsDifficultySelected()
    {
        return isDifficultySelected;
    }
    
    public void ResetSelection()
    {
        selectedDifficulty = "";
        isDifficultySelected = false;
        
        SetButtonColor(facilButton, unselectedColor);
        SetButtonColor(dificilButton, unselectedColor);
        
        // Force update play button to gray since no difficulty is selected
        UpdatePlayButtonState();
        
        Debug.Log("🎯 Difficulty selection reset - Play button should be gray");
    }
    
    /// <summary>
    /// Force reset all button colors to correct values - call this if colors look wrong
    /// </summary>
    [ContextMenu("Fix Button Colors")]
    public void FixButtonColors()
    {
        // Reset difficulty buttons to unselected
        UpdateButtonState(facilButton, facilAvailable, false);
        UpdateButtonState(dificilButton, dificilAvailable, false);
        
        // Reset selection state
        selectedDifficulty = "";
        isDifficultySelected = false;
        
        // Force correct play button state
        UpdatePlayButtonState();
        
        Debug.Log("🎨 All button colors have been reset to correct values");
    }
    
    // Subscribe to GameManager events
    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSongSelected += OnSongSelected;
            GameManager.Instance.OnPlayButtonStateChanged += OnPlayButtonStateChanged;
        }
    }
    
    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSongSelected -= OnSongSelected;
            GameManager.Instance.OnPlayButtonStateChanged -= OnPlayButtonStateChanged;
        }
    }
    
    void OnSongSelected(string songPath)
    {
        Debug.Log($"🎵 DifficultyButtonManager: OnSongSelected called with path: {songPath}");
        
        // Analyze the selected song's available difficulties
        UpdateAvailableDifficulties(songPath);
        
        // Force update with correct logic when song is selected
        UpdatePlayButtonState();
    }
    
    void OnPlayButtonStateChanged()
    {
        // Override any other play button state changes with our correct logic
        UpdatePlayButtonState();
        Debug.Log("🎮 Play button state overridden by DifficultyButtonManager");
    }
    
    /// <summary>
    /// Updates available difficulties based on the selected song
    /// </summary>
    /// <param name="songPath">Path to the selected song</param>
    void UpdateAvailableDifficulties(string songPath)
    {
        if (string.IsNullOrEmpty(songPath))
        {
            // No song selected, enable all buttons
            facilAvailable = true;
            dificilAvailable = true;
            UpdateButtonState(facilButton, true, false);
            UpdateButtonState(dificilButton, true, false);
            
            // Reset selection if no song is selected
            ResetSelection();
            
            Debug.Log("🎵 No song selected - all difficulty buttons enabled");
            return;
        }
        
        // Analyze the chart file for available difficulties
        currentSongDifficulties = ChartDifficultyAnalyzer.AnalyzeSongByName(songPath);
        
        // Update availability flags
        facilAvailable = currentSongDifficulties.hasFacil;
        dificilAvailable = currentSongDifficulties.hasDificil;
        
        // Update button states and colors
        UpdateButtonState(facilButton, facilAvailable, false);
        UpdateButtonState(dificilButton, dificilAvailable, false);
        
        // Reset current selection if it's no longer available
        if (isDifficultySelected)
        {
            if ((selectedDifficulty == "Facil" && !facilAvailable) ||
                (selectedDifficulty == "Dificil" && !dificilAvailable))
            {
                ResetSelection();
                Debug.Log($"🎯 Previous difficulty selection reset - {selectedDifficulty} not available for {songPath}");
            }
        }
        
        Debug.Log($"🎵 Song {songPath} difficulties - Facil: {facilAvailable}, Dificil: {dificilAvailable}");
    }
    
    /// <summary>
    /// Sets the enabled state of a button and updates its interactability
    /// </summary>
    /// <param name="button">Button to modify</param>
    /// <param name="enabled">Whether the button should be enabled</param>
    void SetButtonEnabled(Button button, bool enabled)
    {
        if (button == null) return;
        
        button.interactable = enabled;
        
        // Update visual feedback for disabled state
        ColorBlock colors = button.colors;
        colors.disabledColor = disabledColor;
        button.colors = colors;
    }
    
    /// <summary>
    /// Updates both the enabled state and color of a button
    /// </summary>
    /// <param name="button">Button to modify</param>
    /// <param name="available">Whether the difficulty is available</param>
    /// <param name="selected">Whether this difficulty is currently selected</param>
    void UpdateButtonState(Button button, bool available, bool selected)
    {
        if (button == null) 
        {
            Debug.LogWarning("⚠️ UpdateButtonState: button is null!");
            return;
        }
        
        Debug.Log($"🔧 UpdateButtonState: {button.name} - Available: {available}, Selected: {selected}");
        
        // Set interactability - this makes it behave like the JUGAR button when disabled
        button.interactable = available;
        
        // Ensure the button is always visible
        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        // Set color based on state
        Color targetColor;
        if (!available)
        {
            // Not available - use disabled color (like JUGAR button when gray)
            targetColor = disabledColor;
        }
        else if (selected)
        {
            // Available and selected - use selected color
            targetColor = selectedColor;
        }
        else
        {
            // Available but not selected - use unselected color
            targetColor = unselectedColor;
        }
        
        // Apply colors - when interactable=false, Unity automatically uses disabledColor
        ColorBlock colors = button.colors;
        colors.normalColor = targetColor;
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Force gray color for disabled state
        colors.highlightedColor = available ? Color.Lerp(targetColor, hoverColor, 0.3f) : colors.disabledColor;
        colors.pressedColor = available ? Color.Lerp(targetColor, Color.black, 0.2f) : colors.disabledColor;
        colors.selectedColor = targetColor;
        button.colors = colors;
        
        Debug.Log($"🎨 Button {button.name} colors set - interactable: {button.interactable}, disabledColor: {colors.disabledColor}");
        
        // FORCE: Also change the Image component color directly if button is disabled
        if (!available)
        {
            var image = button.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                image.color = new Color(0.3f, 0.3f, 0.3f, 0.7f); // Very dark gray with transparency
                Debug.Log($"🎨 FORCED Image color to VERY DARK GRAY for {button.name}");
            }
            
            // Also try to modify all child images
            var childImages = button.GetComponentsInChildren<UnityEngine.UI.Image>();
            foreach (var childImage in childImages)
            {
                childImage.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
                Debug.Log($"🎨 FORCED Child Image color to VERY DARK GRAY for {childImage.name}");
            }
            
            // Try to modify Text components too
            var texts = button.GetComponentsInChildren<UnityEngine.UI.Text>();
            foreach (var text in texts)
            {
                text.color = new Color(0.4f, 0.4f, 0.4f, 0.8f);
                Debug.Log($"🎨 FORCED Text color to gray for {text.name}");
            }
        }
        else
        {
            var image = button.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                image.color = Color.white; // Reset to normal
                Debug.Log($"🎨 RESET Image color to white for {button.name}");
            }
            
            // Reset child images
            var childImages = button.GetComponentsInChildren<UnityEngine.UI.Image>();
            foreach (var childImage in childImages)
            {
                childImage.color = Color.white;
            }
            
            // Reset text colors
            var texts = button.GetComponentsInChildren<UnityEngine.UI.Text>();
            foreach (var text in texts)
            {
                text.color = Color.white;
            }
        }
    }
    
    /// <summary>
    /// Public method to manually update difficulties (useful for external calls)
    /// </summary>
    /// <param name="songPath">Path to the song to analyze</param>
    public void UpdateDifficultiesForSong(string songPath)
    {
        UpdateAvailableDifficulties(songPath);
    }
    
    /// <summary>
    /// Gets information about available difficulties for the current song
    /// </summary>
    /// <returns>DifficultyInfo for the current song, or null if no song selected</returns>
    public ChartDifficultyAnalyzer.DifficultyInfo GetCurrentSongDifficulties()
    {
        return currentSongDifficulties;
    }
    
    /// <summary>
    /// Checks if a specific difficulty is available for the current song
    /// </summary>
    /// <param name="difficulty">Difficulty to check ("Facil" or "Dificil")</param>
    /// <returns>True if the difficulty is available</returns>
    public bool IsDifficultyAvailable(string difficulty)
    {
        switch (difficulty)
        {
            case "Facil":
                return facilAvailable;
            case "Dificil":
                return dificilAvailable;
            default:
                return false;
        }
    }
}
