using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FileDialogManager
{
    /*[DllImport("user32.dll")]
    private static extern void SaveFileDialog(); //in your case : OpenFileDialog

    [DllImport("user32.dll")]
    private static extern void OpenFileDialog(); //in your case : OpenFileDialog

    [DllImport("user32.dll")]
    private static extern void FolderBrowserDialog(); //in your case : OpenFileDialog*/

    public static string OpenFolder()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // En WebGL, pas d'accès au système de fichiers
        // La sauvegarde/chargement passe par le NetworkManager
        Debug.LogWarning("FileDialogManager Non disponible en WebGL.");
        return null;
#else
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);

        return Application.streamingAssetsPath;
#endif
    }

    public static string OpenFile(string p_filename)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.LogWarning("FileDialogManager Non disponible en WebGL.");
        return null;
#else
        string l_filePath = Application.streamingAssetsPath + "/" + p_filename;
        return l_filePath;
#endif
    }
}
