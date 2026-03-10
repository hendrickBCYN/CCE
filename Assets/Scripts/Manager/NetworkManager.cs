using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Pont de communication entre React et Unity WebGL.
/// 
/// Ce GameObject doit exister dans la scène avec le nom exact "NetworkManager"
/// car React utilise sendMessage("NetworkManager", ...) pour communiquer.
/// 
/// React → Unity : méthodes publiques appelées par sendMessage()
/// Unity → React : fonctions DllImport du plugin ReactBridge.jslib
/// </summary>
public class NetworkManager : MonoBehaviour
{
    // ─── Singleton ────────────────────────────────────────────────
    public static NetworkManager Instance { get; private set; }

    // ─── État d'authentification ──────────────────────────────────
    private string _authToken;
    private string _userEmail;
    private string _userDisplayName;

    public string AuthToken => _authToken;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_authToken);

    // ─── Référence au SaveConfig pour le chargement ───────────────
    [SerializeField] private SaveConfig _saveConfig;

    // ─── Import des fonctions JavaScript (ReactBridge.jslib) ──────
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendSaveRequest(string configurationJson);

    [DllImport("__Internal")]
    private static extern void SendLoadRequest(string configurationId);

    [DllImport("__Internal")]
    private static extern void SendPdfGenerated(string pdfBase64);
#else
    // Stubs pour l'éditeur Unity (les appels JS ne fonctionnent qu'en WebGL)
    private static void SendSaveRequest(string configurationJson)
        => Debug.Log($"[Editor] SendSaveRequest: {configurationJson.Substring(0, Mathf.Min(100, configurationJson.Length))}...");
    private static void SendLoadRequest(string configurationId)
        => Debug.Log($"[Editor] SendLoadRequest: {configurationId}");
    private static void SendPdfGenerated(string pdfBase64)
        => Debug.Log($"[Editor] SendPdfGenerated: {pdfBase64.Length} chars");
#endif

    // ─── Cycle de vie ─────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── React → Unity ────────────────────────────────────────────

    /// <summary>
    /// Reçoit le JWT depuis React après authentification Google SSO.
    /// Appelé par : sendMessage("NetworkManager", "ReceiveAuthToken", token)
    /// </summary>
    public void ReceiveAuthToken(string token)
    {
        _authToken = token;
        Debug.Log("[NetworkManager] Token JWT reçu.");
    }

    /// <summary>
    /// Reçoit les informations utilisateur depuis React.
    /// Appelé par : sendMessage("NetworkManager", "ReceiveUserInfo", jsonString)
    /// </summary>
    public void ReceiveUserInfo(string userInfoJson)
    {
        var userInfo = JsonUtility.FromJson<UserInfo>(userInfoJson);
        _userEmail = userInfo.email;
        _userDisplayName = userInfo.displayName;
        Debug.Log($"[NetworkManager] Utilisateur: {_userDisplayName} ({_userEmail})");
    }

    /// <summary>
    /// Reçoit une configuration chargée depuis l'API (via React).
    /// Appelé par : sendMessage("NetworkManager", "ReceiveConfiguration", jsonString)
    /// </summary>
    public void ReceiveConfiguration(string configurationJson)
    {
        Debug.Log("[NetworkManager] Configuration reçue depuis React.");
        if (_saveConfig != null)
        {
            _saveConfig.LoadedFromJson(configurationJson);
        }
    }

    /// <summary>
    /// Reçoit le résultat d'une sauvegarde depuis React.
    /// Appelé par : sendMessage("NetworkManager", "ReceiveSaveResult", "success"|"error")
    /// </summary>
    public void ReceiveSaveResult(string result)
    {
        Debug.Log($"[NetworkManager] Résultat sauvegarde: {result}");
    }

    // ─── Unity → React ────────────────────────────────────────────

    /// <summary>
    /// Demande à React de sauvegarder la configuration courante via l'API.
    /// </summary>
    public void RequestSave(string configurationJson)
    {
        if (!IsAuthenticated)
        {
            Debug.LogWarning("[NetworkManager] Sauvegarde impossible : non authentifié.");
            return;
        }
        SendSaveRequest(configurationJson);
    }

    /// <summary>
    /// Demande à React de charger une configuration depuis l'API.
    /// </summary>
    public void RequestLoad(string configurationId)
    {
        if (!IsAuthenticated)
        {
            Debug.LogWarning("[NetworkManager] Chargement impossible : non authentifié.");
            return;
        }
        SendLoadRequest(configurationId);
    }

    /// <summary>
    /// Envoie un PDF généré à React pour téléchargement navigateur.
    /// </summary>
    public void RequestPdfDownload(string pdfBase64)
    {
        SendPdfGenerated(pdfBase64);
    }

    // ─── Classe sérialisable ──────────────────────────────────────

    [System.Serializable]
    private class UserInfo
    {
        public string email;
        public string displayName;
    }
}
