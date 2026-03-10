/*
 * ReactBridge.jslib
 * ─────────────────────────────────────────────────────────────
 * Plugin JavaScript pour la communication Unity → React.
 *
 * Emplacement : Assets/Plugins/WebGL/ReactBridge.jslib
 *
 * Chaque fonction est appelable depuis C# via [DllImport("__Internal")]
 * et émet un événement capté par react-unity-webgl côté React.
 */

mergeInto(LibraryManager.library, {

  /**
   * Demande à React de sauvegarder une configuration.
   * Côté C# : NetworkManager appelle SendSaveRequest(jsonString)
   * Côté React : écouté via addEventListener("OnSaveRequest", callback)
   */
  SendSaveRequest: function (configurationJsonPtr) {
    var configurationJson = UTF8ToString(configurationJsonPtr);
    try {
      window.dispatchReactUnityEvent("OnSaveRequest", configurationJson);
    } catch (e) {
      console.error("ReactBridge.SendSaveRequest error:", e);
    }
  },

  /**
   * Demande à React de charger une configuration.
   * Côté C# : NetworkManager appelle SendLoadRequest(configId)
   * Côté React : écouté via addEventListener("OnLoadRequest", callback)
   */
  SendLoadRequest: function (configurationIdPtr) {
    var configurationId = UTF8ToString(configurationIdPtr);
    try {
      window.dispatchReactUnityEvent("OnLoadRequest", configurationId);
    } catch (e) {
      console.error("ReactBridge.SendLoadRequest error:", e);
    }
  },

  /**
   * Envoie un PDF généré (en base64) à React pour téléchargement.
   * Côté C# : NetworkManager appelle SendPdfGenerated(base64String)
   * Côté React : écouté via addEventListener("OnPdfGenerated", callback)
   */
  SendPdfGenerated: function (pdfBase64Ptr) {
    var pdfBase64 = UTF8ToString(pdfBase64Ptr);
    try {
      window.dispatchReactUnityEvent("OnPdfGenerated", pdfBase64);
    } catch (e) {
      console.error("ReactBridge.SendPdfGenerated error:", e);
    }
  },
});
