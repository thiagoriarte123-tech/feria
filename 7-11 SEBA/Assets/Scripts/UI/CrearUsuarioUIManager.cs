using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Gestor de UI para la escena CrearUsuario
/// Maneja los TextMeshPro de nombres de usuario
/// </summary>
public class CrearUsuarioUIManager : MonoBehaviour
{
    [Header("TextMeshPro References")]
    public TextMeshProUGUI userNameDisplayText;
    public TextMeshProUGUI userNamePreviewText;
    public TextMeshProUGUI welcomeText;
    
    [Header("Input Field")]
    public TMP_InputField userNameInputField;
    
    [Header("Auto Setup")]
    public bool autoFindComponents = true;
    public bool updateInRealTime = true;
    
    [Header("User Data")]
    public string currentUserName = "";
    public string defaultUserName = "Jugador";
    
    void Start()
    {
        if (autoFindComponents)
        {
            FindUIComponents();
        }
        
        LoadUserData();
        SetupInputField();
        UpdateUI();
    }
    
    void Update()
    {
        if (updateInRealTime)
        {
            UpdateUserNameFromInput();
        }
    }
    
    /// <summary>
    /// Busca automáticamente los componentes UI
    /// </summary>
    void FindUIComponents()
    {
        Debug.Log("[CrearUsuarioUIManager] Buscando componentes UI...");
        
        // Buscar todos los TextMeshPro en la escena
        TextMeshProUGUI[] allTMPTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI tmpComponent in allTMPTexts)
        {
            string name = tmpComponent.name.ToLower();
            string text = tmpComponent.text.ToLower();
            
            Debug.Log($"[CrearUsuarioUIManager] Analizando TMP: {tmpComponent.name} - Texto: '{tmpComponent.text}'");
            
            // Identificar por nombre o contenido
            if (name.Contains("username") || name.Contains("nombre") || name.Contains("user"))
            {
                if (name.Contains("display") || name.Contains("show"))
                {
                    userNameDisplayText = tmpComponent;
                    Debug.Log($"✅ UserName Display encontrado: {tmpComponent.name}");
                }
                else if (name.Contains("preview"))
                {
                    userNamePreviewText = tmpComponent;
                    Debug.Log($"✅ UserName Preview encontrado: {tmpComponent.name}");
                }
                else if (userNameDisplayText == null)
                {
                    userNameDisplayText = tmpComponent;
                    Debug.Log($"✅ UserName Display (genérico) encontrado: {tmpComponent.name}");
                }
            }
            else if (name.Contains("welcome") || name.Contains("bienvenido") || text.Contains("welcome") || text.Contains("bienvenido"))
            {
                welcomeText = tmpComponent;
                Debug.Log($"✅ Welcome Text encontrado: {tmpComponent.name}");
            }
        }
        
        // Buscar InputField
        TMP_InputField[] allInputFields = FindObjectsByType<TMP_InputField>(FindObjectsSortMode.None);
        
        foreach (TMP_InputField inputField in allInputFields)
        {
            string name = inputField.name.ToLower();
            
            if (name.Contains("username") || name.Contains("nombre") || name.Contains("user"))
            {
                userNameInputField = inputField;
                Debug.Log($"✅ UserName InputField encontrado: {inputField.name}");
                break;
            }
        }
    }
    
    /// <summary>
    /// Configura el InputField
    /// </summary>
    void SetupInputField()
    {
        if (userNameInputField != null)
        {
            // Configurar placeholder si existe
            if (userNameInputField.placeholder != null)
            {
                Text placeholderText = userNameInputField.placeholder.GetComponent<Text>();
                if (placeholderText != null)
                {
                    placeholderText.text = "Ingresa tu nombre...";
                }
                
                TextMeshProUGUI placeholderTMP = userNameInputField.placeholder.GetComponent<TextMeshProUGUI>();
                if (placeholderTMP != null)
                {
                    placeholderTMP.text = "Ingresa tu nombre...";
                }
            }
            
            // Configurar texto inicial
            if (string.IsNullOrEmpty(userNameInputField.text))
            {
                userNameInputField.text = currentUserName;
            }
            
            // Agregar listener para cambios
            userNameInputField.onValueChanged.AddListener(OnUserNameChanged);
            
            Debug.Log("[CrearUsuarioUIManager] InputField configurado");
        }
    }
    
    /// <summary>
    /// Actualiza el nombre de usuario desde el input
    /// </summary>
    void UpdateUserNameFromInput()
    {
        if (userNameInputField != null && !string.IsNullOrEmpty(userNameInputField.text))
        {
            if (currentUserName != userNameInputField.text)
            {
                currentUserName = userNameInputField.text;
                UpdateUI();
            }
        }
    }
    
    /// <summary>
    /// Callback cuando cambia el nombre de usuario
    /// </summary>
    void OnUserNameChanged(string newName)
    {
        currentUserName = string.IsNullOrEmpty(newName) ? defaultUserName : newName;
        UpdateUI();
        SaveUserData();
        
        Debug.Log($"[CrearUsuarioUIManager] Nombre de usuario cambiado: {currentUserName}");
    }
    
    /// <summary>
    /// Actualiza todos los elementos UI
    /// </summary>
    void UpdateUI()
    {
        string displayName = string.IsNullOrEmpty(currentUserName) ? defaultUserName : currentUserName;
        
        // Actualizar texto de display
        if (userNameDisplayText != null)
        {
            userNameDisplayText.text = displayName;
        }
        
        // Actualizar texto de preview
        if (userNamePreviewText != null)
        {
            userNamePreviewText.text = $"Jugador: {displayName}";
        }
        
        // Actualizar texto de bienvenida
        if (welcomeText != null)
        {
            welcomeText.text = $"¡Bienvenido, {displayName}!";
        }
    }
    
    /// <summary>
    /// Establece el nombre de usuario manualmente
    /// </summary>
    public void SetUserName(string userName)
    {
        currentUserName = string.IsNullOrEmpty(userName) ? defaultUserName : userName;
        
        if (userNameInputField != null)
        {
            userNameInputField.text = currentUserName;
        }
        
        UpdateUI();
        SaveUserData();
        
        Debug.Log($"[CrearUsuarioUIManager] Nombre establecido manualmente: {currentUserName}");
    }
    
    /// <summary>
    /// Obtiene el nombre de usuario actual
    /// </summary>
    public string GetUserName()
    {
        return currentUserName;
    }
    
    /// <summary>
    /// Valida el nombre de usuario
    /// </summary>
    public bool ValidateUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName) || userName.Trim().Length < 2)
        {
            return false;
        }
        
        // Verificar caracteres válidos
        foreach (char c in userName)
        {
            if (!char.IsLetterOrDigit(c) && c != ' ' && c != '_' && c != '-')
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Confirma la creación del usuario
    /// </summary>
    public void ConfirmUserCreation()
    {
        if (ValidateUserName(currentUserName))
        {
            SaveUserData();
            Debug.Log($"[CrearUsuarioUIManager] Usuario creado: {currentUserName}");
            
            // Aquí puedes agregar lógica adicional como cambiar de escena
            // SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("[CrearUsuarioUIManager] Nombre de usuario inválido");
            ShowValidationError();
        }
    }
    
    /// <summary>
    /// Muestra error de validación
    /// </summary>
    void ShowValidationError()
    {
        if (userNameDisplayText != null)
        {
            string originalText = userNameDisplayText.text;
            userNameDisplayText.text = "¡Nombre inválido!";
            userNameDisplayText.color = Color.red;
            
            // Restaurar después de 2 segundos
            Invoke(nameof(RestoreOriginalText), 2f);
        }
    }
    
    /// <summary>
    /// Restaura el texto original
    /// </summary>
    void RestoreOriginalText()
    {
        if (userNameDisplayText != null)
        {
            userNameDisplayText.color = Color.white;
            UpdateUI();
        }
    }
    
    /// <summary>
    /// Guarda los datos del usuario
    /// </summary>
    void SaveUserData()
    {
        PlayerPrefs.SetString("UserName", currentUserName);
        PlayerPrefs.SetString("LastUserName", currentUserName);
        PlayerPrefs.Save();
        
        Debug.Log($"[CrearUsuarioUIManager] Datos guardados: {currentUserName}");
    }
    
    /// <summary>
    /// Carga los datos del usuario
    /// </summary>
    void LoadUserData()
    {
        currentUserName = PlayerPrefs.GetString("UserName", defaultUserName);
        
        Debug.Log($"[CrearUsuarioUIManager] Datos cargados: {currentUserName}");
    }
    
    /// <summary>
    /// Limpia los datos del usuario
    /// </summary>
    public void ClearUserData()
    {
        currentUserName = defaultUserName;
        
        PlayerPrefs.DeleteKey("UserName");
        PlayerPrefs.DeleteKey("LastUserName");
        PlayerPrefs.Save();
        
        if (userNameInputField != null)
        {
            userNameInputField.text = "";
        }
        
        UpdateUI();
        
        Debug.Log("[CrearUsuarioUIManager] Datos de usuario limpiados");
    }
    
    void OnDestroy()
    {
        // Remover listeners
        if (userNameInputField != null)
        {
            userNameInputField.onValueChanged.RemoveListener(OnUserNameChanged);
        }
    }
}
